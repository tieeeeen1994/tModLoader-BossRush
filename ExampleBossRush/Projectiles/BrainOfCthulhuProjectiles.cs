using BossRushAPI;
using ExampleBossRush.Types;
using System.Collections.Generic;
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
            var peeTracker = StoreOrFetch("PeeTracker", new Dictionary<int, bool>());
            if (!peeTracker.TryGetValue(projectile.whoAmI, out bool isModified) && !isModified)
            {
                projectile.damage = Util.RoundOff(projectile.damage * .5f);
                peeTracker[projectile.whoAmI] = true;
            }
            projectile.velocity.Y = projectile.oldVelocity.Y;
        }
    }
}
