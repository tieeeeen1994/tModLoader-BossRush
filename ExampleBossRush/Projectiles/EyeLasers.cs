using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles
{
    public class EyeLasers : BossRushProjectiles
    {
        protected override List<int> ApplicableTypes => [ProjectileID.EyeLaser];

        protected override void Update(Projectile projectile)
        {
            if (projectile.type == ProjectileID.EyeLaser && BRS.I.ReferenceBoss is NPC boss)
            {
                projectile.damage = Util.RoundOff(boss.damage * .05f);
            }
        }
    }
}
