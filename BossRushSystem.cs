using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BossRush;

public partial class BossRushSystem : ModSystem
{
    public static BossRushSystem I => ModContent.GetInstance<BossRushSystem>();

    public static bool IsBossRushActive() => I.state != States.Off;

    public static bool IsBossRushOff() => I.state == States.Off;

    public States state = States.Off;
    public List<NPC> currentBoss = null;
    public NPC referenceBoss = null;
    public Dictionary<NPC, bool> bossDefeated = null;
    private Queue<BossData> bossQueue = [];
    private bool allDead = false;
    private int allDeadEndTimer = 0;
    private int prepareTimer = 0;
    private int performanceTimer = 0;

    public override void PostUpdateWorld()
    {
        switch (state)
        {
            case States.On:
                InitializeSystem();
                state = States.Prepare;
                break;

            case States.Prepare:
                if (++prepareTimer >= Util.SecondsInFrames(.3f))
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
                TrackPlayerDeaths();
                if (++performanceTimer >= Util.SecondsInFrames(.05f))
                {
                    performanceTimer = 0;
                    CheckBossAndPlayerCondition();
                }
                break;

            case States.End:
                ResetSystem();
                break;
        }
    }

    public override void OnWorldUnload()
    {
        ResetSystem();
    }

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

    private void InitializeSystem()
    {
        ResetSystem();

        bossQueue.Enqueue(new(NPCID.KingSlime,
                              spawnOffsets: [new(1000, -700, -200, -200),
                                             new(-1000, -700, 200, -200)]));

        bossQueue.Enqueue(new(NPCID.EyeofCthulhu,
                              spawnOffsets: [new(1000, 1000, 200, -2000),
                                             new(-1000, 1000, -200,-2000)],
                              timeContext: TimeContext.Night));

        // bossQueue.Enqueue(new(NPCID.EaterofWorldsHead,
        //                       spawnOffsets: [new(-1000, 1000, 2000, 500)]));

        bossQueue.Enqueue(new(NPCID.BrainofCthulhu,
                              spawnOffsets: [new(500, 500, 200, -1000),
                                             new(-500, 500, -200,-1000)]));

        bossQueue.Enqueue(new(NPCID.QueenBee,
                              spawnOffsets: [new(-1000, -1000, 2000, -200)],
                              timeContext: TimeContext.Day));

        bossQueue.Enqueue(new(NPCID.SkeletronHead,
                              spawnOffsets: [new(500, 500, 200, -1000),
                                             new(-500, 500, -200,-1000)],
                              timeContext: TimeContext.Night));

        // bossQueue.Enqueue(new(NPCID.WallofFlesh));

        bossQueue.Enqueue(new(NPCID.QueenSlimeBoss,
                              spawnOffsets: [new(1000, -500, -100, -100),
                                             new(-1000, -500, 100, -100)],
                              timeContext: TimeContext.Day));

        bossQueue.Enqueue(new([NPCID.Retinazer, NPCID.Spazmatism],
                              spawnOffsets: [new(1000, 1000, 200, -2000),
                                             new(-1000, 1000, -200,-2000)],
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new(NPCID.TheDestroyer,
                              spawnOffsets: [new(-1000, 1000, 2000, 500)],
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new(NPCID.SkeletronPrime,
                              spawnOffsets: [new(-1000, -1200, 2000, -300)],
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new(NPCID.Plantera,
                              spawnOffsets: [new(-1000, 1500, 2000, 500)],
                              timeContext: TimeContext.Day));

        bossQueue.Enqueue(new(NPCID.Golem,
                              spawnOffsets: [new(500, 0, -200, -500),
                                             new(-500, 0, 200, -500)]));

        bossQueue.Enqueue(new(NPCID.DukeFishron,
                              spawnOffsets: [new(300, 50, -100, -50),
                                             new(-300, 50, 100, -50)]));

        bossQueue.Enqueue(new(NPCID.HallowBoss,
                              spawnOffsets: [new(-50, -50, 100, -50)],
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new(NPCID.CultistBoss,
                              spawnOffsets: [new(300, 0, -100, -500),
                                             new(-300, 0, 100, -500)],
                              timeContext: TimeContext.Day));

        bossQueue.Enqueue(new(NPCID.MoonLordCore));
    }

    private void ResetSystem()
    {
        state = States.Off;
        currentBoss = null;
        referenceBoss = null;
        bossQueue.Clear();
        allDeadEndTimer = 0;
        allDead = false;
        prepareTimer = 0;
        bossDefeated = null;
        performanceTimer = 0;
    }

    private void SpawnNextBoss()
    {
        BossData nextBoss = bossQueue.Peek();
        List<int> npcIndex = Util.SpawnBoss(nextBoss);
        currentBoss = npcIndex.ConvertAll(element => Main.npc[element]);
        referenceBoss = currentBoss.First();
        bossDefeated = currentBoss.ToDictionary(boss => boss, _ => false);
    }

    private void TrackPlayerDeaths()
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

    private bool IsBossGone()
    {
        return IsBossDespawned() || IsBossDefeated();
    }

    private bool IsBossDefeated()
    {
        return !bossDefeated.ContainsValue(false);
    }

    private bool IsBossDespawned()
    {
        return currentBoss == null || bossDefeated == null ||
               currentBoss.Any(boss => !bossDefeated[boss] && !boss.active);
    }

    private void CheckBossAndPlayerCondition()
    {
        if (allDead)
        {
            if (IsBossGone() || ++allDeadEndTimer >= Util.SecondsInFrames(10))
            {
                state = States.End;
            }
        }
        else if (IsBossDespawned())
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

    public enum States { Off, On, Prepare, Run, End }
}
