using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.Projectiles
{
    public class DeathLasers : SharedBossProjectiles
    {
        protected override List<int> ApplicableTypes => [ProjectileID.DeathLaser];

        override protected void CalculateDamage(List<NPC> bosses, ref int damage, ref int hits, ref float multiplier)
        {
            FetchBoss(bosses, NPCID.Retinazer, ref damage, ref hits, ref multiplier, .08f);
            FetchBoss(bosses, NPCID.TheDestroyer, ref damage, ref hits, ref multiplier, .04f);
            FetchBoss(bosses, NPCID.SkeletronPrime, ref damage, ref hits, ref multiplier, .09f);
        }
    }
}
