using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BossRush;

/// <summary>
/// Player class that handles the player's behavior during Boss Rush.
/// </summary>
public class BossRushPlayer : ModPlayer
{
    /// <summary>
    /// When a player dies in Boss Rush mode, pause the respawn timer.
    /// They can only respawn again when the boss of that stage is defeated.
    /// This prevents respawn cheesing.
    /// </summary>
    public override void UpdateDead()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            Player.respawnTimer = Util.SecondsInFrames(10);
        }
    }

    /// <summary>
    /// Necessary to make the boss work. It forces the biome to prevent the boss from despawning.
    /// </summary>
    public override void PostUpdate()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            BossRushSystem.I.currentBossData?.placeContext?.forceBiomeFunction?.Invoke(Player);
        }
    }

    /// <summary>
    /// When a player dies in Boss Rush mode, track the death.
    /// </summary>
    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (BossRushSystem.IsBossRushActive())
        {
            BossRushSystem.I.TrackPlayerDeaths();
        }
    }

    /// <summary>
    /// When a player disconnects in Boss Rush mode, check for other players if they are still alive.
    /// </summary>
    public override void PlayerDisconnect()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            Player.active = false;
            BossRushSystem.I.TrackPlayerDeaths();
        }
    }
}
