using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles
{
    public class GolemProjectiles : BossRushProjectiles
    {
        protected override List<int> ApplicableTypes => [
            ProjectileID.Fireball,
            ProjectileID.EyeBeam
        ];

        protected override void Update(Projectile projectile)
        {
            if (BRS.I.ReferenceBoss is NPC boss)
            {
                switch (projectile.type)
                {
                    case ProjectileID.Fireball:
                        projectile.damage = Util.RoundOff(boss.damage * .2f);
                        break;

                    case ProjectileID.EyeBeam:
                        projectile.damage = Util.RoundOff(boss.damage * .1f);
                        break;
                }
            }
        }
    }
}
