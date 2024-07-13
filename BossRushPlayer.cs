using Terraria;
using Terraria.ModLoader;

namespace BossRush;

public class BossRushPlayer : ModPlayer
{
    public override void UpdateDead()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            Player.respawnTimer = 10 * Main.frameRate;
        }
    }
}
