using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace BossRush.Projectiles
{
    public class BossProjectiles : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => lateInstantiation;

        public override void SetDefaults(Projectile projectile)
        {
            if (BRS.I.IsBossRushActive && BRS.I.CurrentBossData?.ModifiedAttributes is { } attributes &&
                attributes.ProjectilesAffected && !projectile.friendly)
            {
                projectile.damage = attributes.ComputeDamage(projectile.damage);
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && BRS.I.IsBossRushActive &&
                !projectile.friendly && BRS.I.CurrentBoss != null &&
                BRS.I.ReferenceBoss != null && BRS.I.CurrentBossData is { } bossData)
            {
                bossData.ProjectileUpdate(projectile, bossData.AI);
            }
        }
    }
}
