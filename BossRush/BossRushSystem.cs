using System.Collections.Generic;
using System.Linq;
using BossRush.Types;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BR = BossRush.BossRush;

namespace BossRush;

public class BossRushSystem : ModSystem
{
    internal static BossRushSystem I => ModContent.GetInstance<BossRushSystem>();
    public bool IsBossRushActive => State != States.Off;
    public bool IsBossRushOff => State == States.Off;
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

    public void AddBoss(BossData bossData) => bossQueue.Enqueue(bossData);

    protected virtual void InitializeSystem() { }

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
