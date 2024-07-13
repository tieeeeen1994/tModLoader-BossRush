using System.Security.Policy;
using Terraria;
using Terraria.ID;
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

    public override void PostUpdate()
    {
        if (BossRushSystem.IsBossRushActive() && BossRushSystem.I.currentBoss != null)
        {
            switch (BossRushSystem.I.currentBoss.type)
            {
                case NPCID.BrainofCthulhu:
                    Player.ZoneCrimson = true;
                    break;

                case NPCID.QueenBee:
                    Player.ZoneJungle = true;
                    break;
            }
        }
    }
}
