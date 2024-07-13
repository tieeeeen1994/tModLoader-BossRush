using System.Collections.Generic;
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
    public NPC currentBoss = null;
    public bool bossDefeated = false;
    private Queue<BossData> bossQueue = [];
    private bool allDead = false;
    private int allDeadEndTimer = 0;
    private int prepareTimer = 0;

    public override void PostUpdateWorld()
    {
        switch (state)
        {
            case States.On:
                InitializeSystem();
                state = States.Prepare;
                break;

            case States.Prepare:
                if (++prepareTimer >= .3f * Main.frameRate)
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
                if (allDead)
                {
                    if (IsBossGone() || ++allDeadEndTimer >= 10 * Main.frameRate)
                    {
                        state = States.End;
                    }
                }
                else if (IsBossDespawned())
                {
                    state = States.Prepare;
                }
                else if (IsBossDefeated())
                {
                    Util.SpawnDeadPlayers();
                    allDead = false;
                    bossQueue.Dequeue();
                    state = States.Prepare;
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

        bossQueue.Enqueue(new(NPCID.BrainofCthulhu,
                              spawnOffsets: [new(500, 500, 200, -1000),
                                             new(-500, 500, -200,-1000)],
                              timeContext: TimeContext.Day));

        bossQueue.Enqueue(new(NPCID.QueenBee,
                              spawnOffsets: [new(1000, -700, -200, -200),
                                             new(-1000, -700, 200, -200)]));

        bossQueue.Enqueue(new(NPCID.SkeletronHead,
                              spawnOffsets: [new(500, 500, 200, -1000),
                                             new(-500, 500, -200,-1000)],
                              timeContext: TimeContext.Night));

        bossQueue.Enqueue(new(NPCID.QueenSlimeBoss,
                              spawnOffsets: [new(1000, -500, -100, -100),
                                             new(-1000, -500, 100, -100)],
                              timeContext: TimeContext.Day));
    }

    private void ResetSystem()
    {
        state = States.Off;
        currentBoss = null;
        bossQueue.Clear();
        allDeadEndTimer = 0;
        allDead = false;
        prepareTimer = 0;
        bossDefeated = false;
    }

    private void SpawnNextBoss()
    {
        bossDefeated = false;
        BossData nextBoss = bossQueue.Peek();
        int npcIndex = Util.SpawnBoss(nextBoss);
        currentBoss = Main.npc[npcIndex];
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
        return bossDefeated;
    }

    private bool IsBossDespawned()
    {
        return currentBoss == null || !bossDefeated && !currentBoss.active;
    }

    public enum States { Off, On, Prepare, Run, End }
}
