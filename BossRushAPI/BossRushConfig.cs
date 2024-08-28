using BossRushAPI.Interfaces;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace BossRushAPI;

public class BossRushConfig : ModConfig, IInstanceable<BossRushConfig>
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public static BossRushConfig Instance => ModContent.GetInstance<BossRushConfig>();

    public static BossRushConfig I => Instance;

    [Header("Respawn")]
    [DefaultValue(false)]
    public bool respawnPlayers;

    [DefaultValue(10)]
    public int respawnTimer;

    [Header("Multiplayer")]
    [DefaultValue(true)]
    public bool periodicSynchronization;

    [DefaultValue(10)]
    public int synchronizationTime;
}