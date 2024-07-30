using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria.ID;
using Terraria;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class DestroyerProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [ProjectileID.PinkLaser];

    protected override void Update(Projectile projectile)
    {
        if (ApplicableTypes.Contains(projectile.type) && BRS.I.ReferenceBoss is NPC boss)
        {
            projectile.damage = Util.RoundOff(boss.damage * .035f);
        }
    }
}
