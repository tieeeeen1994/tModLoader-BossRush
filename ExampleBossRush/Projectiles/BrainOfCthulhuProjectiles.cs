using System.Collections.Generic;
using ExampleBossRush.Types;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.Projectiles;

public class BrainOfCthulhuProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [ProjectileID.GoldenShowerHostile];

    protected override void Update(Projectile projectile)
    {
        if (projectile.type == ProjectileID.GoldenShowerHostile)
        {
            projectile.velocity.Y = projectile.oldVelocity.Y;
        }
    }
}
