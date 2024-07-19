using Terraria;
using Terraria.ModLoader;

namespace BossRush.Projectiles
{
    public class BossProjectiles : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => lateInstantiation;

        public override void SetDefaults(Projectile projectile)
        {
            if (BossRushSystem.I.IsBossRushActive && BossRushSystem.I.CurrentBossData?.ModifiedAttributes is { } attributes)
            {
                if (attributes.ProjectilesAffected)
                {
                    projectile.damage = attributes.ComputeDamage(projectile.damage);
                }
            }
        }
    }
}
