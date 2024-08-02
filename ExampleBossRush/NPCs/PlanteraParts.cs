using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs;

public class PlanteraParts : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.Spore, NPCID.PlanterasTentacle, NPCID.Plantera];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.Plantera);

    protected override void Update(NPC npc)
    {
        var tentacleTracker = StoreOrFetch("TentacleTracker", new Dictionary<NPC, bool>());
        if (npc.type == NPCID.Spore)
        {
            npc.knockBackResist = 0f;
        }
        else if (npc.type == NPCID.PlanterasTentacle)
        {
            if (!tentacleTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                npc.knockBackResist = 0f;
                tentacleTracker[npc] = true;
            }
        }
        else if (npc.type == NPCID.Plantera)
        {
            var halfLife3Timer = StoreOrFetch("HalfLife3", 30);
            CleanInactiveData(tentacleTracker);
            if (npc.life < npc.lifeMax / 2)
            {
                if (tentacleTracker.Count > 0)
                {
                    npc.defense = 500;
                }
                else
                {
                    npc.defense = 0;
                }
                if (--halfLife3Timer <= 0)
                {
                    halfLife3Timer = 30;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int projectileType = Main.rand.Next(projectiles);
                        int damage;
                        var velocity = npc.DirectionTo(Main.player[npc.target].Center) * 24;
                        if (projectileType == ProjectileID.SeedPlantera)
                        {
                            damage = 44;
                        }
                        else if (projectileType == ProjectileID.PoisonSeedPlantera)
                        {
                            damage = 54;
                        }
                        else
                        {
                            damage = 62;
                        }
                        damage = npc.GetAttackDamage_ForProjectiles(damage, damage * .9f);
                        Projectile.NewProjectile(npc.GetSource_FromAI("HalfLife3"), npc.Center,
                                                 velocity, projectileType, damage, 0f);
                    }
                }
                ai["HalfLife3"] = halfLife3Timer;
            }
        }
    }

    private static readonly short[] projectiles = [
        ProjectileID.SeedPlantera,
        ProjectileID.PoisonSeedPlantera,
        ProjectileID.ThornBall
    ];
}
