using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace BossRush;

public class BossRushPlayer : ModPlayer
{
    public override void UpdateDead()
    {
        if (BRS.I.IsBossRushActive)
        {
            Player.respawnTimer = 10.ToFrames();
        }
        // For debugging and quick respawns.
        // Player.respawnTimer = 0;
    }

    public override void PostUpdate()
    {
        if (BRS.I.IsBossRushActive)
        {
            BRS.I.CurrentBossData?.PlaceContext?.ForceBiomeFunction?.Invoke(Player);
        }
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (BRS.I.IsBossRushActive)
        {
            BRS.I.TrackPlayerDeaths();
        }
    }

    public override void PlayerDisconnect()
    {
        if (BRS.I.IsBossRushActive)
        {
            Player.active = false;
            BRS.I.TrackPlayerDeaths();
        }
    }
}
