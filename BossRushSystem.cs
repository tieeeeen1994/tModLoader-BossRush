using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BossRush;

/// <summary>
/// Main class for the Boss Rush System.
/// </summary>
public partial class BossRushSystem : ModSystem
{
    /// <summary>
    /// Instance of the Boss Rush System.
    /// </summary>
    public static BossRushSystem I => ModContent.GetInstance<BossRushSystem>();

    /// <summary>
    /// Checks if Boss Rush is active.
    /// </summary>
    /// <returns>True or False</returns>
    public static bool IsBossRushActive() => I.state != States.Off;

    /// <summary>
    /// Checks if Boss Rush is inactive.
    /// </summary>
    /// <returns>True or False</returns>
    public static bool IsBossRushOff() => I.state == States.Off;

    /// <summary>
    /// Current state of the Boss Rush System.
    /// </summary>
    public States state { get; private set; } = States.Off;

    /// <summary>
    /// Pertains to the current boss of a stage. This contains all the entities of the boss.
    /// e.g. The Twins will contain Retinazer and Spazmatism.
    /// </summary>
    public List<NPC> currentBoss { get; private set; } = null;

    /// <summary>
    /// Pertains to the BossData object provided for the current boss fromn the queue.
    /// </summary>
    public BossData? currentBossData { get; private set; } = null;

    /// <summary>
    /// Just a reference to the boss of the current stage.
    /// This field is equal to currentBoss.First().
    /// This field exists for performance reasons.
    /// Use this if checks are needed for the boss, but only one entity is needed to confirm the current boss.
    /// e.g.
    /// </summary>
    public NPC referenceBoss { get; private set; } = null;

    /// <summary>
    /// Contains trackers for the bosses if they were truly defeated or not.
    /// Bosses are considered defeated if they are killed by the player and supposed loot is to be dropped.
    /// Boss Rush mod disables loot drops for enemies and instead uses that as a tracker.
    /// </summary>
    public Dictionary<NPC, bool> bossDefeated { get; private set; } = null;

    public Dictionary<string, object> currentBossAi { get; private set; } = null;

    /// <summary>
    /// Contains the queue of bosses to be spawned.
    /// Each entry is one boss rush stage, and a boss rush stage can contain multiple entities.
    /// </summary>
    private readonly Queue<BossData> bossQueue = [];

    /// <summary>
    /// Flag for determining of all players are dead within a boss rush stage.
    /// </summary>
    private bool allDead = false;

    /// <summary>
    /// A timer for fallback scenarios if all players are dead.
    /// Boss Rush System only officially ends when all players are dead and all bosses have despawned.
    /// This timer is a fallback in case the boss refuses to despawn.
    /// </summary>
    private int allDeadEndTimer = 0;

    /// <summary>
    /// A timer for preparing the next boss.
    /// It just adds pauses in between bosses upon being defeated and spawning a new one.
    /// </summary>
    private int prepareTimer = 0;

    /// <summary>
    /// Main code for the Boss Rush System. Runs every in-game frame.
    /// </summary>
    public override void PostUpdateWorld()
    {
        switch (state)
        {
            case States.On:
                InitializeSystem();
                state = States.Prepare;
                break;

            case States.Prepare:
                if (++prepareTimer >= Util.SecondsInFrames(.5f))
                {
                    prepareTimer = 0;
                    if (bossQueue.Count <= 0)
                    {
                        Util.NewText(Language.GetTextValue("Mods.BossRush.Messages.Win"));
                        state = States.End;
                    }
                    else
                    {
                        SpawnNextBoss();
                        state = States.Run;
                    }
                }
                break;

            case States.Run:
                CheckPlayerCondition();
                break;

            case States.End:
                ResetSystem();
                break;
        }
    }

    /// <summary>
    /// When the world shuts down, the Boss Rush System will reset.
    /// This will ensure that loading into the world again resets everything.
    /// </summary>
    public override void OnWorldUnload()
    {
        ResetSystem();
    }

    /// <summary>
    /// Method mainly used for the item that activates Boss Rush mode.
    /// </summary>
    public void ToggleBossRush()
    {
        switch (state)
        {
            case States.Off:
                Util.NewText(Language.GetTextValue("Mods.BossRush.Messages.BossRushActive"));
                Util.CleanAllEnemies();
                state = States.On;
                break;
            case States.Prepare:
            case States.Run:
                Util.NewText(Language.GetTextValue("Mods.BossRush.Messages.PussyOut"));
                Util.CleanAllEnemies();
                state = States.End;
                break;
        }
    }

    /// <summary>
    /// Main code for States.Run.
    /// The code lives in BossAndSlaves.OnKill for performance reasons.
    /// The code triggers when an enemy in the currentBoss list is killed.
    /// </summary>
    public void CheckBossCondition()
    {
        if (!allDead)
        {
            if (IsBossDespawned())
            {
                Util.CleanAllEnemies(currentBoss);
                state = States.Prepare;
            }
            else if (IsBossDefeated())
            {
                Util.SpawnDeadPlayers();
                allDead = false;
                bossQueue.Dequeue();
                state = States.Prepare;
            }
        }
    }

    /// <summary>
    /// Main code for States.Run.
    /// The code lives in BossRushPlayer.Kill and BossRushPlayer.PlayerDisconnect for performance reasons.
    /// The code triggers when a player is killed or disconnects.
    /// </summary>
    public void CheckPlayerCondition()
    {
        if (allDead)
        {
            if (IsBossGone() || ++allDeadEndTimer >= Util.SecondsInFrames(10))
            {
                state = States.End;
            }
        }
    }

    /// <summary>
    /// Tracks the deaths of all players. Does not return anything.
    /// It automatically assigns allDead variable upon running the method.
    /// </summary>
    public void TrackPlayerDeaths()
    {
        if (!allDead)
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (!player.dead)
                {
                    return;
                }
            }
            allDead = true;
            Util.NewText(Language.GetTextValue("Mods.BossRush.Messages.Failure"));
        }
    }

    /// <summary>
    /// Initializes the Boss Rush System. This is where bosses are queued using BossData struct.
    /// Refer to BossData struct for more details.
    /// </summary>
    private void InitializeSystem()
    {
        ResetSystem();

        // bossQueue.Enqueue(new(
        //     [NPCID.KingSlime],
        //     spawnOffset: (_, _) =>
        //     {
        //         int sign = Util.RandomSign();
        //         return new(1000 * sign, -700, 200 * sign, -200);
        //     },
        //     modifiedAttributes: new(100, 2, 100, 24)
        // ));

        bossQueue.Enqueue(new(
            [NPCID.EyeofCthulhu],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(1000 * sign, 1000, 200 * sign, -2000);
            },
            timeContext: TimeContext.Night,
            modifiedAttributes: new(100, 8, 80, 0),
            update: npc =>
            {
                if (currentBossAi.TryGetValue("bossForcedDamage", out object value))
                {
                    npc.damage = Util.RoundOff((int)value);
                }
                else
                {
                    currentBossAi["bossForcedDamage"] = npc.damage;
                }
            }
        ));

        bossQueue.Enqueue(new(
            [NPCID.EaterofWorldsHead],
            subTypes: [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.EaterofWorldsHead],
            spawnOffset: (_, _) => new(-1000, 1000, 2000, 500),
            placeContexts: [new(player => player.ZoneCorrupt = true)]
        ));

        bossQueue.Enqueue(new(
            [NPCID.BrainofCthulhu],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 500, 200 * sign, -1000);
            },
            placeContexts: [new(player => player.ZoneCrimson = true)]
        ));

        Action<Player> forceJungle = (player) =>
        {
            player.ZoneJungle = true;
            player.ZoneDirtLayerHeight = true;
            player.ZoneRockLayerHeight = true;
        };
        bossQueue.Enqueue(new([NPCID.QueenBee],
                              spawnOffset: (_, _) => new(-1000, -1000, 2000, -200),
                              timeContext: TimeContext.Noon,
                              placeContexts: [new(forceJungle)]));

        bossQueue.Enqueue(new(
            [NPCID.SkeletronHead],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 500, 200 * sign, -1000);
            },
            timeContext: TimeContext.Night
        ));

        bossQueue.Enqueue(new(
            [NPCID.WallofFlesh],
            placeContexts: [PlaceContext.LeftUnderworld, PlaceContext.RightUnderworld],
            spawnOffset: (_, bossData) =>
            {
                if (bossData.placeContext.Value.initialPosition.Value.X < Main.maxTilesX / 2)
                {
                    return new(-500, 0, 0, 0);
                }
                else
                {
                    return new(500, 0, 0, 0);
                }
            }
        ));

        bossQueue.Enqueue(new(
            [NPCID.QueenSlimeBoss],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(1000 * sign, -500, -100 * sign, -100);
            },
            timeContext: TimeContext.Noon,
            placeContexts: [new((player) =>
            {
                Vector2 worldCoordinates = new Vector2(Main.spawnTileX, Main.spawnTileY).ToWorldCoordinates();
                worldCoordinates -= new Vector2(player.width / 2, player.height);
                worldCoordinates = Util.RoundOff(worldCoordinates);
                return new((int)worldCoordinates.X, (int)worldCoordinates.Y, 0, 0);
            })]
        ));

        int twinsSign = Util.RandomSign();
        bossQueue.Enqueue(new(

            [NPCID.Retinazer, NPCID.Spazmatism],
            spawnOffset: (type, _) =>
                {
                    if (type == NPCID.Retinazer)
                    {
                        return new(1000 * twinsSign, 1000, 200 * twinsSign, -2000);
                    }
                    else
                    {
                        return new(-1000 * twinsSign, 1000, -200 * twinsSign, -2000);
                    }
                },
                timeContext: TimeContext.Night
            ));

        bossQueue.Enqueue(new([NPCID.TheDestroyer],
                              spawnOffset: (_, _) => new(-1000, 1000, 2000, 500),
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new([NPCID.SkeletronPrime],
                              spawnOffset: (_, _) => new(-1000, -800, 2000, -200),
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new([NPCID.Plantera],
                              spawnOffset: (_, _) => new(-1000, 1500, 2000, 500),
                              timeContext: TimeContext.Noon,
                              placeContexts: [new(forceJungle)]));

        bossQueue.Enqueue(new(
            [NPCID.Golem],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 0, -200 * sign, -500);
            },
            placeContexts: [new(player => player.ZoneLihzhardTemple = true)]
        ));

        bossQueue.Enqueue(new(
            [NPCID.DukeFishron],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(300 * sign, 50, -100 * sign, -50);
            },
            placeContexts: [new(player => player.ZoneBeach = true)]
        ));

        bossQueue.Enqueue(new([NPCID.HallowBoss],
                              spawnOffset: (_, _) => new(-50, -50, 100, -50),
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new(
            [NPCID.CultistBoss],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(300 * sign, 0, -100 * sign, -500);
            }
        ));

        bossQueue.Enqueue(new([NPCID.MoonLordCore]));
    }

    /// <summary>
    /// Resets the system to default values.
    /// </summary>
    private void ResetSystem()
    {
        state = States.Off;
        currentBoss = null;
        currentBossData = null;
        referenceBoss = null;
        bossQueue.Clear();
        allDeadEndTimer = 0;
        allDead = false;
        prepareTimer = 0;
        bossDefeated = null;
        currentBossAi = null;
    }

    /// <summary>
    /// Spawns the next boss in the queue.
    /// </summary>
    private void SpawnNextBoss()
    {
        currentBossData = bossQueue.Peek();
        List<int> npcIndex = SpawnBoss(currentBossData.Value);
        currentBoss = npcIndex.ConvertAll(element => Main.npc[element]);
        referenceBoss = currentBoss.First();
        bossDefeated = currentBoss.ToDictionary(boss => boss, _ => false);
        currentBossAi = [];
    }

    /// <summary>
    /// Checks if the boss is gone.
    /// The boss is considered gone if it is despawned or defeated.
    /// </summary>
    /// <returns>True or False</returns>
    private bool IsBossGone()
    {
        return IsBossDespawned() || IsBossDefeated();
    }

    /// <summary>
    /// Checks if the boss is defeated.
    /// The boss is considered defeated if all entities are defeated.
    /// </summary>
    /// <returns>True or False</returns>
    private bool IsBossDefeated()
    {
        return !bossDefeated.ContainsValue(false);
    }

    /// <summary>
    /// Checks if the boss is despawned.
    /// The boss is considered despawned if it is becomes inactive while the tracker has not marked it as defeated.
    /// </summary>
    /// <returns>True or False</returns>
    private bool IsBossDespawned()
    {
        return currentBoss == null || bossDefeated == null ||
               currentBoss.Any(boss => !bossDefeated[boss] && !boss.active);
    }

    /// <summary>
    /// Method of spawning in Boss Rush mode.
    /// </summary>
    /// <param name="data">Refer to BossData struct for more details</param>
    /// <returns>List of indices to be used in Main.npc</returns>
    private List<int> SpawnBoss(BossData data)
    {
        data.timeContext?.ChangeTime();
        data.placeContext?.TeleportPlayers();

        List<Player> potentialTargetPlayers = [];
        float highestAggro = float.MinValue;

        foreach (var player in Main.ActivePlayers)
        {
            if (highestAggro == player.aggro)
            {
                potentialTargetPlayers.Add(player);
            }
            else if (player.aggro > highestAggro)
            {
                potentialTargetPlayers.Clear();
                potentialTargetPlayers.Add(player);
                highestAggro = player.aggro;
            }
        }

        Player target = Main.rand.Next(potentialTargetPlayers);
        List<int> spawnedBossIndex = [];

        foreach (var type in data.types)
        {
            Vector2 offsetValue = data.RandomSpawnLocation(type);
            int spawnX = Util.RoundOff(target.Center.X + offsetValue.X);
            int spawnY = Util.RoundOff(target.Center.Y + offsetValue.Y);

            // Start at index 1 to avoid encountering the nasty vanilla bug for certain bosses.
            spawnedBossIndex.Add(NPC.NewNPC(new EntitySource_BossSpawn(target), spawnX, spawnY, type, 1));
        }

        return spawnedBossIndex;
    }

    /// <summary>
    /// States of the Boss Rush System.
    /// </summary>
    public enum States
    {
        /// <summary>
        /// Boss Rush is inactive.
        /// </summary>
        Off,
        /// <summary>
        /// Boss Rush is initializing.
        /// </summary>
        On,
        /// <summary>
        /// Boss Rush is preparing the next boss.
        /// </summary>
        Prepare,
        /// <summary>
        /// Boss Rush is checking for player and boss conditions.
        /// </summary>
        Run,
        /// <summary>
        /// Boss Rush is ending.
        /// </summary>
        End
    }
}
