using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EBR = ExampleBossRush.ExampleBossRush;

namespace ExampleBossRush.NPCs
{
    public class SkeletronPrimeAndArms : BossRushBossAndMinions
    {
        protected override List<int> ApplicableTypes => [
            NPCID.SkeletronPrime,
            NPCID.PrimeCannon,
            NPCID.PrimeSaw,
            NPCID.PrimeVice,
            NPCID.PrimeLaser
        ];

        protected override void Update(NPC npc)
        {
            if (npc.type == NPCID.SkeletronPrime)
            {
                var bombTracker = StoreOrFetch("BombTracker", new Dictionary<Projectile, bool>());
                var rocketTracker = StoreOrFetch("RocketTracker", new Dictionary<Projectile, int>());
                var laserTracker = StoreOrFetch("LaserTracker", new Dictionary<Projectile, bool>());
                StoreOrFetch("OriginalDamage", npc.damage);
                CleanInactiveData(bombTracker);
                CleanInactiveData(rocketTracker);
                CleanInactiveData(laserTracker);
            }
            else if (npc.type == NPCID.PrimeCannon)
            {
                var rocketQueue = StoreOrFetch("RocketQueue", new List<int>());
                for (int i = 0; i < rocketQueue.Count; i++)
                {
                    if (rocketQueue[i] <= 0)
                    {
                        rocketQueue.RemoveAt(i--);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var rotation = (npc.rotation + MathHelper.PiOver2).ToRotationVector2();
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center,
                                                     rotation * 24f, ProjectileID.RocketSkeleton,
                                                     1, 8f, -1, 0, 0, npc.target);
                        }
                    }
                    else
                    {
                        rocketQueue[i] = rocketQueue[i] - 1;
                    }
                }
            }
            else if (npc.type == NPCID.PrimeVice)
            {
                var boulderCooldown = StoreOrFetch("BoulderCooldown", 0);
                if (npc.ai[2] == 2f || (npc.ai[2] == 5f))
                {
                    if (boulderCooldown <= 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center,
                                                                      npc.velocity * 1.5f, ProjectileID.Boulder,
                                                                      1, 20f, -1, 0, 0, npc.target);
                            proj.trap = false;
                            proj.friendly = false;
                            proj.hostile = true;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                ModPacket packet = EBR.Instance.GetPacket();
                                packet.Write((byte)EBR.PacketTypes.BoulderProperties);
                                packet.Write(proj.whoAmI);
                                packet.Send();
                            }
                            proj.netUpdate = true;
                        }
                        ai["BoulderCooldown"] = boulderCooldown = 2.ToFrames();
                    }
                }
                ai["BoulderCooldown"] = Math.Max(0, --boulderCooldown);
            }
            else if (npc.type == NPCID.PrimeSaw)
            {
                var scrapCooldown = StoreOrFetch("ScrapCooldown", 0);
                ai["ScrapCooldown"] = Math.Max(0, --scrapCooldown);
            }
            else if (npc.type == NPCID.PrimeLaser)
            {
                var laserQueue = StoreOrFetch("LaserQueue", new List<(int, Vector2)>());
                for (int i = 0; i < laserQueue.Count; i++)
                {
                    (int timer, Vector2 velocity) = laserQueue[i];
                    if (timer <= 0)
                    {
                        laserQueue.RemoveAt(i--);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            var offset = velocity;
                            offset.Normalize();
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + (offset * 48),
                                                     velocity, ProjectileID.DeathLaser,
                                                     1, 25, -1, 0, 0, 69);
                        }
                    }
                    else
                    {
                        laserQueue[i] = (timer - 1, velocity);
                    }
                }
            }
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            SawScrap(npc);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            SawScrap(npc);
        }

        internal void SawScrap(NPC npc)
        {
            if (npc.type == NPCID.PrimeSaw)
            {
                var scrapCooldown = StoreOrFetch("ScrapCooldown", 0);
                if (scrapCooldown <= 0)
                {
                    ai["ScrapCooldown"] = 15;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var velocity = Main.rand.NextVector2CircularEdge(10f, 10f);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity,
                                                 ProjectileID.SaucerScrap, 1, 15f);
                    }
                    else
                    {
                        ModPacket packet = EBR.Instance.GetPacket();
                        packet.Write((byte)EBR.PacketTypes.PrimeScrap);
                        packet.Write(npc.whoAmI);
                        packet.Send();
                    }
                }
            }
        }
    }
}
