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

    public override void PostUpdateWorld()
    {
        switch (state)
        {
            case States.On:
                InitializeSystem();
                state = States.Prepare;
                break;
            case States.Prepare:
                Util.SpawnDeadPlayers();
                if (bossQueue.Count <= 0)
                {
                    state = States.End;
                }
                else
                {
                    SpawnNextBoss();
                    state = States.Run;
                }
                break;

            case States.Run:
                bool allDead = true;
                foreach (var player in Main.ActivePlayers)
                {
                    if (!player.dead)
                    {
                        allDead = false;
                        break;
                    }
                }
                if (allDead)
                {
                    state = States.End;
                }
                else if (currentBoss == null || !currentBoss.active || currentBoss.life <= 0)
                {
                    state = States.Prepare;
                }
                break;

            case States.End:
                // Util.SpawnDeadPlayers();
                // Util.CleanAllEnemies();
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
                Util.CleanAllEnemies();
                state = States.On;
                break;
            case States.Prepare:
            case States.Run:
                state = States.End;
                break;
        }
    }

    private void InitializeSystem()
    {
        bossQueue.Enqueue(new(NPCID.KingSlime, spawnOffset: new(30, 70, 0, 0)));
        bossQueue.Enqueue(new(NPCID.QueenSlimeBoss, spawnOffset: new(-100, 100, 0, 0)));
    }

    private void ResetSystem()
    {
        state = States.Off;
        currentBoss = null;
        bossQueue.Clear();
    }

    private void SpawnNextBoss()
    {
        BossData nextBoss = bossQueue.Dequeue();

        int npcIndex = Util.SpawnBoss(nextBoss.type, nextBoss.RandomSpawnLocation());
        currentBoss = Main.npc[npcIndex];
    }

    private enum States { Off, On, Prepare, Run, End }
}
