using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.Projectiles
{
    public class SkeletronPrimeProjectiles : BossRushProjectiles
    {
        protected override List<int> ApplicableTypes => [
            ProjectileID.BombSkeletronPrime,
            ProjectileID.RocketSkeleton,
            ProjectileID.Boulder,
            ProjectileID.SaucerScrap
        ];

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
                if (Ai.TryGetValue("OriginalDamage", out object originalDamage))
                {
                    projectile.damage = Util.RoundOff((int)originalDamage * .12f);
                }
                StoreOrFetch(rocketTracker, projectile, 3.ToFrames());
                if (--rocketTracker[projectile] <= 0)
                {
                    projectile.Kill();
                }
                foreach (var player in Main.player)
                {
                    if (player.active && !player.dead)
                    {
                        Rectangle pBox = player.Hitbox;
                        Rectangle iBox = new(pBox.X + 4, pBox.Y + 4, pBox.Width - 8, pBox.Height - 8);
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
            else if (projectile.type == ProjectileID.Boulder)
            {
                if (Ai.TryGetValue("OriginalDamage", out object originalDamage))
                {
                    projectile.damage = Util.RoundOff((int)originalDamage * .2f);
                }
            }
        }
    }
}
