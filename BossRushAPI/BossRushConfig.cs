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

    [Header("PlaceContext")]
    [DefaultValue(1)]
    [Range(0, 10)]
    public int placeContextTeleportationDelay;
}
