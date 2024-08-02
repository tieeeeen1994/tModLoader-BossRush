using System.Collections.Generic;
using ExampleBossRush.Types;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.Projectiles;

public class BrainOfCthulhuProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [ProjectileID.GoldenShowerHostile];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.BrainofCthulhu);

    protected override void Update(Projectile projectile)
    {
        if (projectile.type == ProjectileID.GoldenShowerHostile)
        {
            projectile.velocity.Y = projectile.oldVelocity.Y;
        }
    }
}
