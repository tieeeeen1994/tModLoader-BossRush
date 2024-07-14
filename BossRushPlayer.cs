using Terraria;
using Terraria.ID;
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
            Player.respawnTimer = 10 * Main.frameRate;
        }
    }

    /// <summary>
    /// Necessary to make the boss work. It forces the biome to prevent the boss from despawning.
    /// </summary>
    public override void PostUpdate()
    {
        if (BossRushSystem.IsBossRushActive())
        {
            NPC boss = BossRushSystem.I.referenceBoss;
            if (boss != null && boss.active)
            {
                switch (boss.type)
                {
                    case NPCID.EaterofWorldsHead:
                        Player.ZoneCorrupt = true;
                        break;

                    case NPCID.BrainofCthulhu:
                        Player.ZoneCrimson = true;
                        break;

                    case NPCID.QueenBee:
                    case NPCID.Plantera:
                    case NPCID.Golem:
                        Player.ZoneJungle = true;
                        break;
                }
            }
        }
    }
}
