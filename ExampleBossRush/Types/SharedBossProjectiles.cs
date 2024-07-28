using BossRush;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Types
{
    public abstract class SharedBossProjectiles : BossRushProjectiles
    {
        protected override void Update(Projectile projectile)
        {
            if (ApplicableTypes.Contains(projectile.type) && BRS.I.CurrentBoss is List<NPC> bosses)
            {
                int damage = 0;
                float multiplier = 0f;
                int hits = 0;
                CalculateDamage(bosses, ref damage, ref hits, ref multiplier);
                projectile.damage = Util.RoundOff(damage * multiplier / (hits * hits));
            }
        }

        protected abstract void CalculateDamage(List<NPC> bosses, ref int damage, ref int hits, ref float multiplier);

        protected void FetchBoss(List<NPC> bosses, int type, ref int damage, ref int hits,
                                 ref float multiplier, float addedMultiplier)
        {
            NPC boss = bosses.Find(boss => boss.type == type);
            if (boss != null)
            {
                damage += boss.damage;
                multiplier += addedMultiplier;
                hits += 1;
            }
        }
    }
}
