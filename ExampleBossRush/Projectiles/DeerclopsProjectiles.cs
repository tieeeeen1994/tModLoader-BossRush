using BossRushAPI;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.Projectiles;

public class DeerclopsProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [
        ProjectileID.DeerclopsIceSpike,
        ProjectileID.DeerclopsRangedProjectile,
        ProjectileID.InsanityShadowHostile
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.Deerclops);

    protected override void Update(Projectile projectile)
    {
        if (ApplicableTypes.Contains(projectile.type))
        {
            var tracker = StoreOrFetch("Tracker", new Dictionary<int, bool>());
            if (!tracker.TryGetValue(projectile.whoAmI, out bool tracked) && !tracked)
            {
                tracker[projectile.whoAmI] = true;
                projectile.damage = Util.RoundOff(projectile.damage * .5f);
            }
            CleanInactiveData(tracker);
        }
    }
}
