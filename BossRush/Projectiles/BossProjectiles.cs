﻿using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace BossRush.Projectiles;

public class BossProjectiles : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => lateInstantiation;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (BRS.I.IsBossRushActive && !projectile.friendly &&
            BRS.I.CurrentBossData?.ModifiedAttributes is { } attributes &&
            attributes.ProjectilesAffected)
        {
            projectile.damage = attributes.ComputeDamage(projectile.damage);
        }
    }
}
