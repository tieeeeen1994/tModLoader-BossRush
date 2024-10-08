using BossRushAPI.Interfaces;
using BossRushAPI.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using BR = BossRushAPI.BossRushAPI;
using BRC = BossRushAPI.BossRushConfig;

namespace BossRushAPI;

public class BossRushSystem : ModSystem, IInstanceable<BossRushSystem>
{
    #region Fields & Properties

    public static BossRushSystem I => Instance;
    public static BossRushSystem Instance => ModContent.GetInstance<BossRushSystem>();
    public bool IsBossRushActive => State != States.Off;
    public bool IsBossRushOff => State == States.Off;
    public States State { get; private set; } = States.Off;
    public List<NPC> CurrentBoss => _currentBoss?.ToList();
    public NPC ReferenceBoss => _currentBoss?.Find(boss => boss.active);
    public BossData? CurrentBossData { get; private set; } = null;
    internal List<int> TeleportReceipts { get; private set; } = null;
    private readonly Dictionary<float, List<BossData>> candidates = [];
    private readonly Queue<BossData> bossQueue = [];
    private List<NPC> _currentBoss = null;
    private Dictionary<NPC, bool> bossDefeated = null;
    private bool allDead = false;
    private int allDeadEndTimer = 0;
    private int prepareTimer = 0;
    private int teleportTimer = 0;
    private int updateTimer = 0;

    #endregion Fields & Properties

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((byte)State);
        if (_currentBoss == null)
        {
            writer.Write(false);
        }
        else
        {
            writer.Write(true);
            writer.Write(_currentBoss.Count);
            foreach (var boss in _currentBoss)
            {
                writer.Write(boss.whoAmI);
            }
        }
        if (CurrentBossData == null)
        {
            writer.Write(false);
        }
        else
        {
            writer.Write(true);
            BossData bossData = CurrentBossData.Value;
            writer.Write(bossData.Types.Count);
            foreach (int type in bossData.Types)
            {
                writer.Write(type);
            }
            writer.Write(bossData.SubTypes.Count);
            foreach (int subType in bossData.SubTypes)
            {
                writer.Write(subType);
            }
            ModifiedAttributes attributes = bossData.ModifiedAttributes;
            writer.Write(attributes.LifeFlatIncrease);
            writer.Write(attributes.LifeMultiplier);
            writer.Write(attributes.DamageFlatIncrease);
            writer.Write(attributes.DamageMultiplier);
            writer.Write(attributes.DefenseFlatIncrease);
            writer.Write(attributes.DefenseMultiplier);
            writer.Write(attributes.ProjectilesAffected);
            if (bossData.TimeContext == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                TimeContext timeContext = bossData.TimeContext.Value;
                writer.Write(timeContext.Time);
                writer.Write(timeContext.DayTime);
            }
            if (bossData.PlaceContext == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                PlaceContext placeContext = bossData.PlaceContext.Value;
                writer.Write(placeContext.TeleportRange.X);
                writer.Write(placeContext.TeleportRange.Y);
                writer.Write(placeContext.TeleportRange.Width);
                writer.Write(placeContext.TeleportRange.Height);
            }
            SpawnAttributes spawns = bossData.SpawnAttributes;
            writer.Write(spawns.RateMultiplier);
            writer.Write(spawns.MaxMultiplier);
            writer.Write(spawns.MaxFlatIncrease);
            if (bossData.StartMessage == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                Message startMessage = bossData.StartMessage.Value;
                writer.Write(startMessage.Text);
                writer.Write(startMessage.Literal);
                writer.Write(startMessage.Color.PackedValue);
            }
            if (bossData.DefeatMessage == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                Message defeatMessage = bossData.DefeatMessage.Value;
                writer.Write(defeatMessage.Text);
                writer.Write(defeatMessage.Literal);
                writer.Write(defeatMessage.Color.PackedValue);
            }
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        State = (States)reader.ReadByte();
        bool currentBossExists = reader.ReadBoolean();
        if (currentBossExists)
        {
            int currentBossCount = reader.ReadInt32();
            _currentBoss = [];
            for (int i = 0; i < currentBossCount; i++)
            {
                int npcIndex = reader.ReadInt32();
                _currentBoss.Add(Main.npc[npcIndex]);
            }
        }
        else
        {
            _currentBoss = null;
        }
        bool currentBossDataExists = reader.ReadBoolean();
        if (currentBossDataExists)
        {
            int typesCount = reader.ReadInt32();
            List<int> types = [];
            for (int i = 0; i < typesCount; i++)
            {
                types.Add(reader.ReadInt32());
            }
            int subTypesCount = reader.ReadInt32();
            List<int> subTypes = [];
            for (int i = 0; i < subTypesCount; i++)
            {
                subTypes.Add(reader.ReadInt32());
            }
            int lifeFlatIncrease = reader.ReadInt32();
            float lifeMultiplier = reader.ReadSingle();
            int damageFlatIncrease = reader.ReadInt32();
            float damageMultiplier = reader.ReadSingle();
            int defenseFlatIncrease = reader.ReadInt32();
            float defenseMultiplier = reader.ReadSingle();
            bool projectilesAffected = reader.ReadBoolean();
            bool timeContextExists = reader.ReadBoolean();
            TimeContext? timeContext = null;
            if (timeContextExists)
            {
                double time = reader.ReadDouble();
                bool dayTime = reader.ReadBoolean();
                timeContext = new(time, dayTime);
            }
            bool placeContextExists = reader.ReadBoolean();
            PlaceContext? placeContext = null;
            if (placeContextExists)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                placeContext = new(x, y, width, height);
            }
            float rateMultiplier = reader.ReadSingle();
            float maxMultiplier = reader.ReadSingle();
            int maxFlatIncrease = reader.ReadInt32();
            bool startMessageExists = reader.ReadBoolean();
            Message? startMessage = null;
            if (startMessageExists)
            {
                string text = reader.ReadString();
                bool literal = reader.ReadBoolean();
                Color color = new() { PackedValue = reader.ReadUInt32() };
                startMessage = new(text, literal, color);
            }
            bool defeatMessageExists = reader.ReadBoolean();
            Message? defeatMessage = null;
            if (defeatMessageExists)
            {
                string text = reader.ReadString();
                bool literal = reader.ReadBoolean();
                Color color = new() { PackedValue = reader.ReadUInt32() };
                defeatMessage = new(text, literal, color);
            }
            CurrentBossData = new BossData(
                types: types, subTypes: subTypes,
                modifiedAttributes: new(lifeMultiplier, damageMultiplier, defenseMultiplier,
                                        lifeFlatIncrease, damageFlatIncrease, defenseFlatIncrease,
                                        projectilesAffected),
                spawnAttributes: new(maxMultiplier, maxFlatIncrease, rateMultiplier),
                timeContext: timeContext, placeContext: placeContext,
                startMessage: startMessage, defeatMessage: defeatMessage
            );
        }
        else
        {
            CurrentBossData = null;
        }
    }

    public override void PostUpdateWorld()
    {
        switch (State)
        {
            case States.On:
                NewText("Mods.BossRushAPI.Messages.Commence");
                InitializeSystem();
                ChangeState(States.Prepare);
                break;

            case States.Prepare:
                if (++prepareTimer >= 1.ToFrames())
                {
                    if (bossQueue.Count <= 0)
                    {
                        prepareTimer = 0;
                        NewText("Mods.BossRushAPI.Messages.Win");
                        ChangeState(States.End);
                    }
                    else
                    {
                        if (SpawnNextBossOrWaitingForTeleport())
                        {
                            prepareTimer = 0;
                            ChangeState(States.Run);
                        }
                    }
                }
                PeriodicSync();
                break;

            case States.Run:
                CheckPlayerCondition();
                CheckBossCondition();
                PeriodicSync();
                break;

            case States.End:
                NewText("Mods.BossRushAPI.Messages.End");
                Util.CleanStage();
                ResetSystem();
                break;
        }
    }

    public override void OnWorldUnload() => ResetSystem();

    public void AddBoss(float position, BossData data)
    {
        if (candidates.TryGetValue(position, out List<BossData> list))
        {
            list.TryAdd(data);
        }
        else
        {
            candidates.TryAdd(position, [data]);
        }
    }

    public void ToggleBossRush()
    {
        switch (State)
        {
            case States.Off:
                NewText("Mods.BossRushAPI.Messages.Active");
                Util.CleanStage();
                ChangeState(States.On);
                break;

            case States.Prepare:
            case States.Run:
                NewText("Mods.BossRushAPI.Messages.Disable");
                Util.CleanStage();
                ChangeState(States.End);
                break;
        }
    }

    internal void TrackPlayerDeaths()
    {
        if (!allDead)
        {
            if (Main.ActivePlayers.GetEnumerator().MoveNext())
            {
                foreach (var player in Main.ActivePlayers)
                {
                    if (!player.dead)
                    {
                        return;
                    }
                }
                allDead = true;
                NewText("Mods.BossRushAPI.Messages.Failure");
            }
            else
            {
                Util.CleanStage();
                ResetSystem();
            }
        }
    }

    internal void DynamicAddBoss(NPC boss)
    {
        _currentBoss.TryAdd(boss);
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            bossDefeated.TryAdd(boss, false);
        }
    }

    internal void MarkBossDefeat(NPC boss)
    {
        bossDefeated[boss] = true;
        if (!allDead && IsBossDefeated())
        {
            CurrentBossData?.DefeatMessage?.Display();
            Util.CleanStage();
            ResurrectPlayers();
            allDead = false;
            bossQueue.Dequeue();
            ChangeState(States.Prepare);
        }
    }

    private void ChangeState(States state)
    {
        State = state;
        NetMessage.SendData(MessageID.WorldData);
    }

    private void CheckPlayerCondition()
    {
        if (allDead && (IsBossGone() || ++allDeadEndTimer >= 10.ToFrames()))
        {
            allDeadEndTimer = 0;
            ChangeState(States.End);
        }
    }

    private void CheckBossCondition()
    {
        if (!allDead && IsBossDespawned())
        {
            Util.CleanStage();
            ChangeState(States.Prepare);
            NewText("Mods.BossRushAPI.Messages.Despawn");
        }
    }

    private void InitializeSystem()
    {
        foreach (var entry in candidates.OrderBy(entry => entry.Key))
        {
            foreach (var data in entry.Value.OrderBy(_ => Main.rand.Next()))
            {
                bossQueue.Enqueue(data);
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

    private void ResetSystem()
    {
        _currentBoss = null;
        CurrentBossData = null;
        bossQueue.Clear();
        allDeadEndTimer = 0;
        allDead = false;
        prepareTimer = 0;
        bossDefeated = null;
        candidates.Clear();
        TeleportReceipts = null;
        teleportTimer = 0;
        ChangeState(States.Off);
    }

    private bool SpawnNextBossOrWaitingForTeleport()
    {
        if (TeleportReceipts == null)
        {
            var nextBossData = bossQueue.Peek();
            if (nextBossData.PlaceContext == null)
            {
                if (CurrentBossData?.PlaceContext is PlaceContext context)
                {
                    TeleportReceipts = [];
                    context.BackToSpawn();
                }
            }
            else
            {
                if (nextBossData.PlaceContext is PlaceContext context)
                {
                    TeleportReceipts = [];
                    context.TeleportPlayers();
                }
            }
        }
        if ((TeleportReceipts == null || TeleportReceipts.Count <= 0) && teleportTimer >= 5)
        {
            TeleportReceipts = null;
            teleportTimer = 0;
            return SpawnLogic();
        }
        else
        {
            teleportTimer++;
            return false;
        }
    }

    private bool SpawnLogic()
    {
        var nextBossData = bossQueue.Peek();
        CurrentBossData = nextBossData;
        nextBossData.TimeContext?.ChangeTime();
        List<Player> targets = PickTargets();
        if (targets.Count > 0)
        {
            _currentBoss = [];
            bossDefeated = [];
            Player target = Main.rand.Next(targets);

            foreach (var type in nextBossData.Types)
            {
                Vector2 offsetValue = nextBossData.RandomSpawnLocation();
                int spawnX = Util.RoundOff(target.Center.X + offsetValue.X);
                int spawnY = Util.RoundOff(target.Center.Y + offsetValue.Y);

                // Start at index 1 to avoid encountering the nasty vanilla bug for certain bosses.
                int npcIndex = NPC.NewNPC(new EntitySource_BossSpawn(target), spawnX, spawnY, type,
                                          1, 0, 0, 0, 0, target.whoAmI);
                _currentBoss.TryAdd(Main.npc[npcIndex]);
            }
            bossDefeated = _currentBoss.ToDictionary(boss => boss, _ => false);
            nextBossData.StartMessage?.Display();
            return true;
        }
        else
        {
            NewText("Mods.BossRushAPI.Messages.NoTarget");
            ChangeState(States.End);
            return false;
        }
    }

    private List<Player> PickTargets()
    {
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
        return potentialTargetPlayers;
    }

    private void PeriodicSync()
    {
        if (BRC.I.periodicSynchronization && ++updateTimer >= BRC.I.synchronizationTime.ToFrames())
        {
            NetMessage.SendData(MessageID.WorldData);
            updateTimer = 0;
        }
    }

    private bool IsBossGone() => IsBossDespawned() || IsBossDefeated();

    private bool IsBossDefeated() => !bossDefeated.ContainsValue(false);

    private bool IsBossDespawned() => _currentBoss == null || bossDefeated == null ||
                                      _currentBoss.Any(boss => !bossDefeated[boss] && !boss.active);

    private void NewText(string text, bool literal = false) => Util.NewText(text, new(102, 255, 255), literal);

    public enum States : byte
    { Off, On, Prepare, Run, End }
}