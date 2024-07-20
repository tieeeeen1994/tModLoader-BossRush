using Terraria;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace BossRush.Projectiles
{
    public class BossProjectiles : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => lateInstantiation;

        public override void SetDefaults(Projectile projectile)
        {
            if (BossRushSystem.I.IsBossRushActive && BossRushSystem.I.CurrentBossData?.ModifiedAttributes is { } attributes &&
                attributes.ProjectilesAffected && !projectile.friendly)
            {
                projectile.damage = attributes.ComputeDamage(projectile.damage);
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (BRS.I.IsBossRushActive && !projectile.friendly && BRS.I.CurrentBoss != null &&
                BRS.I.CurrentBossData is { } bossData)
            {
                bossData.ProjectileUpdate(projectile, bossData.AI);
            }
        }
    }
}
