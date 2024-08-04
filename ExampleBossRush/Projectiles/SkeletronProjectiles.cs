using BossRushAPI;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class SkeletronProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [
        ProjectileID.Skull,
        ProjectileID.InfernoHostileBlast,
        ProjectileID.LostSoulHostile
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.SkeletronHead);

    protected override void Update(Projectile projectile)
    {
        if (projectile.type == ProjectileID.Skull)
        {
            var skullTracker = StoreOrFetch("SkullTracker", new Dictionary<int, bool>());
            if (!skullTracker.TryGetValue(projectile.whoAmI, out bool tracked) && !tracked)
            {
                projectile.damage = Damage(.1f);
                projectile.timeLeft = 300;
                skullTracker[projectile.whoAmI] = true;
            }
        }
        if (projectile.type == ProjectileID.InfernoHostileBlast)
        {
            projectile.velocity -= Vector2.Zero.DirectionTo(projectile.velocity) * .2f;
        }
        if (projectile.type == ProjectileID.LostSoulHostile)
        {
            Player target = null;
            float distance = float.MaxValue;
            foreach (var player in Main.ActivePlayers)
            {
                float newDistance = projectile.Distance(player.Center);
                if (newDistance <= 12 * 16 && (target == null || newDistance < distance))
                {
                    target = player;
                    distance = newDistance;
                }
            }
            if (target != null)
            {
                projectile.velocity += projectile.DirectionTo(target.Center) * .1f;
                projectile.velocity = projectile.velocity.Clamp(12f);
            }
        }
    }

    private int Damage(float multiplier) => Util.RoundOff(((int?)Ai["OriginalDamage"] ?? 0) * multiplier);
}
