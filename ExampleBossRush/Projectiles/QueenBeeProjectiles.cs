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
            if (projectile.type == ProjectileID.QueenBeeStinger && BRS.I.ReferenceBoss is NPC boss)
            {
                projectile.damage = Util.RoundOff(boss.damage * .12f);
            }
        }
    }
}
