﻿using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class SkeletronProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [
        ProjectileID.Skull,
        ProjectileID.InfernoHostileBlast,
        ProjectileID.LostSoulHostile
    ];

    protected override void Update(Projectile projectile)
    {
        if (projectile.type == ProjectileID.Skull)
        {
            var skullTracker = StoreOrFetch("SkullTracker", new Dictionary<int, bool>());
            if (!skullTracker.TryGetValue(projectile.whoAmI, out bool tracked) && !tracked)
            {
                if (BRS.I.ReferenceBoss is NPC boss)
                {
                    projectile.damage = Util.RoundOff(boss.damage * .05f);
                }
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
}
