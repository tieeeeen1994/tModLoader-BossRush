using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BR = BossRush.BossRush;

namespace BossRush;

public partial class BossRushSystem : ModSystem
{
    public static BossRushSystem I => ModContent.GetInstance<BossRushSystem>();
    public static bool IsBossRushActive() => I.State != States.Off;
    public static bool IsBossRushOff() => I.State == States.Off;
    public States State { get; private set; } = States.Off;
    public List<NPC> CurrentBoss => _currentBoss?.ToList();
    public BossData? CurrentBossData { get; private set; } = null;
    private readonly Queue<BossData> bossQueue = [];
    private List<NPC> _currentBoss = null;
    private Dictionary<NPC, bool> bossDefeated = null;
    private bool allDead = false;
    private int allDeadEndTimer = 0;
    private int prepareTimer = 0;

    public override void PostUpdateWorld()
    {
        switch (State)
        {
            case States.On:
                InitializeSystem();
                State = States.Prepare;
                break;

            case States.Prepare:
                if (++prepareTimer >= .5f.ToFrames())
                {
                    prepareTimer = 0;
                    if (bossQueue.Count <= 0)
                    {
                        Util.NewText(Language.GetTextValue("Mods.BossRush.Messages.Win"));
                        State = States.End;
                    }
                    else
                    {
                        SpawnNextBoss();
                        State = States.Run;
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

    public override void OnWorldUnload() => ResetSystem();

    public void ToggleBossRush()
    {
        switch (State)
        {
            case States.Off:
                Util.NewText(Language.GetTextValue("Mods.BossRush.Messages.BossRushActive"));
                CleanStage();
                State = States.On;
                break;
            case States.Prepare:
            case States.Run:
                Util.NewText(Language.GetTextValue("Mods.BossRush.Messages.PussyOut"));
                CleanStage();
                State = States.End;
                break;
        }
    }

    public void CheckPlayerCondition()
    {
        if (allDead)
        {
            if (IsBossGone() || ++allDeadEndTimer >= 10.ToFrames())
            {
                allDeadEndTimer = 0;
                State = States.End;
            }
        }
    }

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

    public void DynamicAddBoss(NPC boss)
    {
        _currentBoss.Add(boss);
        bossDefeated.Add(boss, false);
    }

    public void MarkBossDefeat(NPC boss)
    {
        bossDefeated[boss] = true;
        if (!allDead)
        {
            if (IsBossDespawned())
            {
                CleanStage(_currentBoss);
                State = States.Prepare;
            }
            else if (IsBossDefeated())
            {
                ResurrectPlayers();
                allDead = false;
                bossQueue.Dequeue();
                State = States.Prepare;
            }
        }
    }

    private void ResurrectPlayers()
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (player.dead)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    player.Spawn(PlayerSpawnContext.ReviveFromDeath);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = BR.I.GetPacket();
                    packet.Write((byte)BR.PacketType.SpawnPlayer);
                    packet.Send(player.whoAmI);
                }
            }
        }
    }

    private void InitializeSystem()
    {
        // bossQueue.Enqueue(new(
        //    [NPCID.KingSlime],
        //    spawnOffset: (_, _) =>
        //    {
        //        int sign = Util.RandomSign();
        //        return new(1000 * sign, -700, 200 * sign, -200);
        //    },
        //    modifiedAttributes: new(lifeMultiplier: 50, damageMultiplier: 2,
        //                            lifeFlatIncrease: 80, damageFlatIncrease: 30),
        //    update: (npc, ai) =>
        //    {
        //        if (npc.type == NPCID.KingSlime && npc.life < npc.lifeMax * .2f && !ai.ContainsKey("DefenseAdded"))
        //        {
        //            ai["DefenseAdded"] = true;
        //            npc.defense *= 20;
        //        }
        //    }
        // ));

        // bossQueue.Enqueue(new(
        //    [NPCID.EyeofCthulhu],
        //    spawnOffset: (_, _) =>
        //    {
        //        int sign = Util.RandomSign();
        //        return new(1000 * sign, 1000, 200 * sign, -2000);
        //    },
        //    timeContext: TimeContext.Night,
        //    modifiedAttributes: new(lifeMultiplier: 100, damageMultiplier: 8,
        //                            lifeFlatIncrease: 80, damageFlatIncrease: 4),
        //    update: (npc, ai) =>
        //    {
        //        if (CurrentBoss.Contains(npc))
        //        {
        //            if (ai.TryGetValue("BossForcedDamage", out object value))
        //            {
        //                npc.damage = (int)value;
        //            }
        //            else
        //            {
        //                ai["BossForcedDamage"] = npc.damage;
        //            }
        //        }
        //    }
        // ));

        // bossQueue.Enqueue(new(
        //     [NPCID.EaterofWorldsHead],
        //     subTypes: [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.EaterofWorldsHead],
        //     spawnOffset: (_, _) => new(-1000, 1000, 2000, 500),
        //     placeContexts: [new(player => player.ZoneCorrupt = true)],
        //     modifiedAttributes: new(lifeMultiplier: 200, damageMultiplier: 9, defenseMultiplier: 50),
        //     update: (npc, ai) =>
        //     {
        //         if (!ai.TryGetValue("SegmentDefenseTracker", out object value1))
        //         {
        //             value1 = new Dictionary<int, bool>();
        //             ai["SegmentDefenseTracker"] = value1;
        //         }
        //         if (npc.type == NPCID.EaterofWorldsBody &&
        //             npc.active && value1 is Dictionary<int, bool> bodyTracker)
        //         {
        //             if (!bodyTracker.TryGetValue(npc.whoAmI, out bool isDefended) && !isDefended)
        //             {
        //                 npc.defense = 0;
        //                 bodyTracker[npc.whoAmI] = true;
        //             }
        //         }
        //         if (!ai.TryGetValue("HeadHealthTracker", out object value2))
        //         {
        //             value2 = new Dictionary<int, bool>();
        //             ai["HeadHealthTracker"] = value2;
        //         }
        //         if ((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail) &&
        //             npc.active && value2 is Dictionary<int, bool> headTracker)
        //         {
        //             if (!headTracker.TryGetValue(npc.whoAmI, out bool isTracked) && !isTracked)
        //             {
        //                 npc.life = npc.lifeMax;
        //                 headTracker[npc.whoAmI] = true;
        //             }
        //         }
        //     }
        // ));

        bossQueue.Enqueue(new(
            [NPCID.BrainofCthulhu],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 500, 200 * sign, -1000);
            },
            placeContexts: [new(player => player.ZoneCrimson = true)],
            modifiedAttributes: new(lifeMultiplier: 100, damageMultiplier: 30, lifeFlatIncrease: 100)
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
                if (bossData.PlaceContext.Value.InitialPosition.Value.X < Main.maxTilesX / 2)
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

    private void ResetSystem()
    {
        State = States.Off;
        _currentBoss = null;
        CurrentBossData = null;
        bossQueue.Clear();
        allDeadEndTimer = 0;
        allDead = false;
        prepareTimer = 0;
        bossDefeated = null;
    }

    private void SpawnNextBoss()
    {
        CurrentBossData = bossQueue.Peek();
        _currentBoss = Spawn(CurrentBossData.Value);
        bossDefeated = _currentBoss.ToDictionary(boss => boss, _ => false);
    }

    private void CleanStage(IEnumerable<NPC> npcs = null)
    {
        npcs ??= Main.npc;
        foreach (var npc in npcs)
        {
            if (!npc.friendly)
            {
                npc.active = false;
            }
        }
    }

    private bool IsBossGone() => IsBossDespawned() || IsBossDefeated();

    private bool IsBossDefeated() => !bossDefeated.ContainsValue(false);

    private bool IsBossDespawned() => _currentBoss == null || bossDefeated == null ||
                                      _currentBoss.Any(boss => !bossDefeated[boss] && !boss.active);

    private List<NPC> Spawn(BossData data)
    {
        data.TimeContext?.ChangeTime();
        data.PlaceContext?.TeleportPlayers();

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
        List<NPC> spawnedBosses = [];

        foreach (var type in data.Types)
        {
            Vector2 offsetValue = data.RandomSpawnLocation(type);
            int spawnX = Util.RoundOff(target.Center.X + offsetValue.X);
            int spawnY = Util.RoundOff(target.Center.Y + offsetValue.Y);

            // Start at index 1 to avoid encountering the nasty vanilla bug for certain bosses.
            int npcIndex = NPC.NewNPC(new EntitySource_BossSpawn(target), spawnX, spawnY, type, 1);
            spawnedBosses.Add(Main.npc[npcIndex]);
        }

        return spawnedBosses;
    }

    public enum States : byte { Off, On, Prepare, Run, End }
}
