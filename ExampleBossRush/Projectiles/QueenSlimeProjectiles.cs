using BossRush.Types;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class QueenSlimeProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [
        ProjectileID.QueenSlimeMinionBlueSpike,
        ProjectileID.QueenSlimeMinionPinkBall,
        ProjectileID.QueenSlimeGelAttack,
        ProjectileID.QueenSlimeSmash
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.QueenSlimeBoss);

    protected override void Update(Projectile projectile)
    {
        if (ApplicableTypes.Contains(projectile.type) && BRS.I.CurrentBossData is BossData bossData)
        {
            var tracker = StoreOrFetch("Tracker", new Dictionary<Projectile, bool>());
            if (!tracker.TryGetValue(projectile, out var tracked) && !tracked)
            {
                projectile.damage = bossData.ModifiedAttributes.ComputeDamage(projectile.damage);
                tracker[projectile] = true;
            }
        }
    }
}
