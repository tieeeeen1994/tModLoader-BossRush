using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;   
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class DeathLasers : SharedBossProjectiles
{
    protected override List<int> ApplicableTypes => [ProjectileID.DeathLaser];

    protected override bool AbsoluteCheck => IsCurrentBoss(ApplicableBosses);

    protected override void Update(Projectile projectile)
    {
        base.Update(projectile);
        if (BRS.I.ReferenceBoss is NPC boss && boss.type == NPCID.SkeletronPrime && projectile.ai[2] != 69)
        {
            var laserTracker = StoreOrFetch("LaserTracker", new Dictionary<Projectile, bool>());
            if (!laserTracker.TryGetValue(projectile, out bool tracked) && !tracked)
            {
                var laserQueue = StoreOrFetch("LaserQueue", new List<(int, Vector2)>());
                laserQueue.Add((10, projectile.oldVelocity));
                laserQueue.Add((21, projectile.oldVelocity));
                laserTracker[projectile] = true;
            }
        }
    }

    protected override void CalculateDamage(List<NPC> bosses, ref int damage, ref int hits, ref float multiplier)
    {
        FetchBoss(bosses, NPCID.Retinazer, ref damage, ref hits, ref multiplier, .09f);
        FetchBoss(bosses, NPCID.TheDestroyer, ref damage, ref hits, ref multiplier, .05f);
        FetchBoss(bosses, NPCID.SkeletronPrime, ref damage, ref hits, ref multiplier, .13f);
    }

    private List<int> ApplicableBosses => [NPCID.Retinazer, NPCID.TheDestroyer, NPCID.SkeletronPrime];
}
