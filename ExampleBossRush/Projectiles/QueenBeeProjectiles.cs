using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles
{
    public class QueenBeeProjectiles : BossRushProjectiles
    {
        protected override List<int> ApplicableTypes => [ProjectileID.QueenBeeStinger];

        protected override void Update(Projectile projectile)
        {
            if (projectile.type == ProjectileID.QueenBeeStinger && BRS.I.ReferenceBoss != null)
            {
                projectile.damage = Util.RoundOff(BRS.I.ReferenceBoss.damage * .12f);
            }
        }
    }
}
