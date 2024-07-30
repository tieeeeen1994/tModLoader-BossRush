using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class DeerclopsProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [
        ProjectileID.DeerclopsIceSpike,
        ProjectileID.DeerclopsRangedProjectile,
        ProjectileID.InsanityShadowHostile
    ];

    protected override void Update(Projectile projectile)
    {
        if (ApplicableTypes.Contains(projectile.type) && BRS.I.ReferenceBoss is NPC boss)
        {
            projectile.damage = Util.RoundOff(boss.damage * .3f);
        }
    }
}
