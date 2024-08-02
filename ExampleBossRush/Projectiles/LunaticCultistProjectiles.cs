using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles
{
    public class LunaticCultistProjectiles : BossRushProjectiles
    {
        protected override List<int> ApplicableTypes => [
            ProjectileID.CultistBossIceMist,
            ProjectileID.CultistBossLightningOrb,
            ProjectileID.CultistBossLightningOrbArc,
            ProjectileID.CultistBossFireBall,
            ProjectileID.CultistBossFireBallClone,
        ];

        protected override void Update(Projectile projectile)
        {
            if (ApplicableTypes.Contains(projectile.type) && BRS.I.ReferenceBoss is NPC boss)
            {
                projectile.damage = Util.RoundOff(boss.damage * .15f);
            }
        }
    }
}
