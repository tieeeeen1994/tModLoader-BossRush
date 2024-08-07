using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using BRC = BossRushAPI.BossRushConfig;
using BRS = BossRushAPI.BossRushSystem;

namespace BossRushAPI;

public class BossRushPlayer : ModPlayer
{
    public override void UpdateDead()
    {
        if (BRS.I.IsBossRushActive && !BRC.I.respawnPlayers)
        {
            AssignRespawnTimer();
        }
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (BRS.I.IsBossRushActive)
        {
            BRS.I.TrackPlayerDeaths();
            if (BRC.I.respawnPlayers)
            {
                AssignRespawnTimer();
            }
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

    private void AssignRespawnTimer() => Player.respawnTimer = BRC.I.respawnTimer.ToFrames();
}
