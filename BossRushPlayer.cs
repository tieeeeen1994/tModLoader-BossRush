using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BossRush;

public class BossRushPlayer : ModPlayer
{
    public override void UpdateDead()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            Player.respawnTimer = 10.ToFrames();
        }
        // For debugging and quick respawns.
        // Player.respawnTimer = 0;
    }

    public override void PostUpdate()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            BossRushSystem.I.CurrentBossData?.PlaceContext?.ForceBiomeFunction?.Invoke(Player);
        }
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (BossRushSystem.IsBossRushActive())
        {
            BossRushSystem.I.TrackPlayerDeaths();
        }
    }

    public override void PlayerDisconnect()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            Player.active = false;
            BossRushSystem.I.TrackPlayerDeaths();
        }
    }
}
