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
            Player.respawnTimer = 600;
        }
    }
}
