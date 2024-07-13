using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush;
public partial class BossRushSystem : ModSystem
{
    public static BossRushSystem I => ModContent.GetInstance<BossRushSystem>();

    public static bool IsBossRushActive() => I.state != States.Off;

    public static bool IsBossRushOff() => I.state == States.Off;

    private States state = States.Off;
    private NPC currentBoss = null;
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
                if (++prepareTimer >= 1 * Main.frameRate)
                {
                    prepareTimer = 0;
                    if (bossQueue.Count <= 0)
                    {
                        Main.NewText("You win.");
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
                    if (Util.IsNPCGone(currentBoss) || ++allDeadEndTimer >= 10 * Main.frameRate)
                    {
                        state = States.End;
                    }
                }
                else if (Util.IsNPCDefeated(currentBoss))
                {
                    Util.SpawnDeadPlayers();
                    allDead = false;
                    bossQueue.Dequeue();
                    state = States.Prepare;
                }
                else if (Util.IsNPCDespawned(currentBoss))
                {
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
                Main.NewText("Boss Rush commencing.");
                Util.CleanAllEnemies();
                state = States.On;
                break;
            case States.Prepare:
            case States.Run:
                Main.NewText("Why pussy out?");
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
                          timeContext: new(0, false)));
        bossQueue.Enqueue(new(NPCID.QueenSlimeBoss,
                          spawnOffsets: [new(1000, -500, -100, -100),
                                         new(-1000, -500, 100, -100)]));
    }

    private void ResetSystem()
    {
        state = States.Off;
        currentBoss = null;
        bossQueue.Clear();
        allDeadEndTimer = 0;
        allDead = false;
        prepareTimer = 0;
    }

    private void SpawnNextBoss()
    {
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
            Main.NewText("An issue of skill, it is.");
        }
    }

    private enum States { Off, On, Prepare, Run, End }
}
