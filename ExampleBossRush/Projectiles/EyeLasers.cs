using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.Projectiles
{
    public class EyeLasers : SharedBossProjectiles
    {
        protected override List<int> ApplicableTypes => [ProjectileID.EyeLaser];

        protected override void CalculateDamage(List<NPC> bosses, ref int damage, ref int hits, ref float multiplier)
        {
            FetchBoss(bosses, NPCID.WallofFlesh, ref damage, ref hits, ref multiplier, .05f);
            FetchBoss(bosses, NPCID.Retinazer, ref damage, ref hits, ref multiplier, .1f);
        }
    }
}
