using Terraria;
using Terraria.ModLoader;

namespace BossRush.NPCs
{
    public class BossSlaves : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            entity.damage *= 2;
        }
    }
}
