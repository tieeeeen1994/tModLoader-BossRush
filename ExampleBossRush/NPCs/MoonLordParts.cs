using BossRushAPI;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush.NPCs
{
    public class MoonLordParts : BossRushBossAndMinions
    {
        private const int moonBoulderDamage = 30;

        protected override List<int> ApplicableTypes => [
            NPCID.MoonLordHand,
            NPCID.MoonLordCore,
            NPCID.MoonLordHead
        ];

        protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.MoonLordCore);

        protected override void Update(NPC npc)
        {
            if (BRS.I.ReferenceBoss is NPC boss)
            {
                var heartOpen = StoreOrFetch("HeartOpen", false);
                var moonBoulderAttack = StoreOrFetch("MoonBoulderAttack", new Dictionary<NPC, int>());
                if (npc.type == NPCID.MoonLordHead)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var headTracker = StoreOrFetch("HeadTracker", false);
                        if (!headTracker)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI("HeadSpawn"), npc.Center.X.RoundOff(), npc.Center.Y.RoundOff(),
                                       NPCID.MoonLordFreeEye, 0, 0, 0, 0, boss.whoAmI);
                            ai["HeadTracker"] = true;
                        }
                    }
                }
                if (npc.type == NPCID.MoonLordCore)
                {
                    if (!npc.dontTakeDamage && !heartOpen)
                    {
                        ai["HeartOpen"] = true;
                        NPC.NewNPC(npc.GetSource_FromAI("HeadSpawn"), npc.Center.X.RoundOff(), npc.Center.Y.RoundOff(),
                                   NPCID.MoonLordFreeEye, 0, 0, 0, 0, boss.whoAmI);
                        npc.defense *= 4;
                    }
                    CleanInactiveData(moonBoulderAttack);
                    if (boss.ai[0] == 2f)
                    {
                        var boulderDrama = StoreOrFetch("BoulderDrama", 5);
                        if (boulderDrama > 0)
                        {
                            ai["BoulderDrama"] = boulderDrama - 1;
                        }
                        else if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.netUpdate = true;
                            ai["BoulderDrama"] = 5;
                            var offset = Main.rand.NextVector2Circular(15, 30);
                            var velocity = Main.rand.NextVector2Circular(10, 10);
                            Projectile.NewProjectile(npc.GetSource_FromAI("BoulderDrama"), npc.Center + offset,
                                                     velocity, ProjectileID.MoonBoulder, moonBoulderDamage, 100f);
                        }
                    }
                }
                if ((npc.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead) &&
                    heartOpen && boss.ai[0] != 2f)
                {
                    if (!moonBoulderAttack.TryGetValue(npc, out int timer))
                    {
                        moonBoulderAttack[npc] = timer = 3.ToFrames();
                    }
                    if (timer <= 0)
                    {
                        moonBoulderAttack[npc] = 3.ToFrames();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI("MoonBoulders"), npc.Center,
                                                     npc.DirectionTo(Main.player[boss.target].Center) * 10f,
                                                     ProjectileID.MoonBoulder, moonBoulderDamage, 100f);
                        }
                    }
                    else
                    {
                        moonBoulderAttack[npc] = timer - 1;
                    }
                }
            }
        }
    }
}
