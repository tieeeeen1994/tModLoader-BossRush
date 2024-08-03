using BossRushAPI;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.Projectiles;

public class SkeletronPrimeProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [
        ProjectileID.BombSkeletronPrime,
        ProjectileID.RocketSkeleton,
        ProjectileID.SaucerScrap,
        ProjectileID.Spike
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.SkeletronPrime);

    protected override void Update(Projectile projectile)
    {
        if (projectile.type == ProjectileID.BombSkeletronPrime)
        {
            var bombTracker = StoreOrFetch("BombTracker", new Dictionary<Projectile, bool>());
            if (!bombTracker.TryGetValue(projectile, out bool tracked) && !tracked)
            {
                if (Ai.TryGetValue("OriginalDamage", out object originalDamage))
                {
                    projectile.damage = Util.RoundOff((int)originalDamage * .18f);
                }
                var rocketQueue = StoreOrFetch("RocketQueue", new List<int>());
                rocketQueue.Add(15);
                bombTracker[projectile] = true;
            }
        }
        else if (projectile.type == ProjectileID.RocketSkeleton)
        {
            var rocketTracker = StoreOrFetch("RocketTracker", new Dictionary<Projectile, int>());
            StoreOrFetch(rocketTracker, projectile, 3.ToFrames());
            if (--rocketTracker[projectile] <= 0)
            {
                projectile.Kill();
            }
            foreach (var player in Main.player)
            {
                if (player.active && !player.dead)
                {
                    Point point = new(player.Center.X.RoundOff(), player.Center.Y.RoundOff());
                    Rectangle iBox = new(point.X - 4, point.Y - 4, 8, 8);
                    if (projectile.Hitbox.Intersects(iBox))
                    {
                        projectile.Kill();
                        break;
                    }
                }
            }
            if (projectile.active)
            {
                projectile.velocity += projectile.DirectionTo(Main.player[(int)projectile.ai[2]].Center) * .8f;
                projectile.velocity.Normalize();
                projectile.velocity *= 20f;
            }
        }
        else if (projectile.type == ProjectileID.Spike)
        {
            projectile.light = .5f;
        }
    }
}
