using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public abstract class BossProjectiles : GlobalProjectile
{
    protected abstract Dictionary<string, object> Ai { get; }

    protected abstract List<int> ApplicableTypes { get; }

    protected abstract void Update(Projectile projectile);

    public sealed override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return lateInstantiation && ApplicableTypes.Contains(entity.type);
    }

    public sealed override void PostAI(Projectile projectile)
    {
        if (!projectile.friendly && BRS.I.IsBossRushActive &&
            BRS.I.CurrentBoss != null && BRS.I.CurrentBossData != null)
        {
            Update(projectile);
        }
    }
}
