using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles
{
    public class SpazmatismProjectiles : BossRushProjectiles
    {
        protected override List<int> ApplicableTypes => [ProjectileID.EyeFire, ProjectileID.CursedFlameHostile];

        protected override void Update(Projectile projectile)
        {
            if (ApplicableTypes.Contains(projectile.type) && BRS.I.CurrentBoss is List<NPC> bosses)
            {
                NPC spazmatism = bosses.Find(boss => boss.type == NPCID.Spazmatism);
                if (spazmatism != null)
                {
                    if (projectile.type == ProjectileID.EyeFire)
                    {
                        projectile.damage = Util.RoundOff(spazmatism.damage * .09f);
                    }
                    else if (projectile.type == ProjectileID.CursedFlameHostile)
                    {
                        projectile.damage = Util.RoundOff(spazmatism.damage * .13f);
                    }
                }
            }
        }
    }
}
