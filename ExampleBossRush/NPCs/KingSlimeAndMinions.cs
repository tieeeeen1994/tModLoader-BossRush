using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.NPCs
{
    public class KingSlimeAndMinions : GlobalNPC
    {
        internal static readonly Dictionary<string, object> ai = [];
        private static readonly List<int> applicableTypes = [NPCID.KingSlime, NPCID.BlueSlime, NPCID.SlimeSpiked];

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return lateInstantiation && applicableTypes.Contains(entity.type);
        }

        public override void PostAI(NPC npc)
        {
            if (BRS.I.IsBossRushActive && BRS.I.CurrentBoss != null && BRS.I.CurrentBossData != null)
            {
                if (npc.type == NPCID.KingSlime)
                {
                    if (npc.life < npc.lifeMax * .2f && !ai.ContainsKey("DefenseAdded"))
                    {
                        ai["DefenseAdded"] = true;
                        npc.defense *= 20;
                        npc.netUpdate = true;
                    }
                    if (ai.TryGetValue("States", out object statesObject) &&
                        statesObject is Dictionary<Projectile, Tuple<string, int, Vector2>> value1)
                    {
                        foreach (var pair in value1)
                        {
                            if (!pair.Key.active)
                            {
                                value1.Remove(pair.Key);
                            }
                        }
                    }
                    if (ai.TryGetValue("SpikeTracker", out object spikesTrackerObject) &&
                        spikesTrackerObject is Dictionary<Projectile, bool> value2)
                    {
                        foreach (var pair in value2)
                        {
                            if (!pair.Key.active)
                            {
                                value2.Remove(pair.Key);
                            }
                        }
                    }
                }
                if (npc.type == NPCID.BlueSlime)
                {
                    npc.Transform(NPCID.SlimeSpiked);
                    npc.netUpdate = true;
                }
                if (npc.type == NPCID.SlimeSpiked)
                {
                    if (!ai.TryGetValue("BombardSpikes", out object value))
                    {
                        value = new Dictionary<NPC, int>();
                        ai["BombardSpikes"] = value;
                    }
                    if (value is Dictionary<NPC, int> tracker)
                    {
                        if (!tracker.TryGetValue(npc, out int timer))
                        {
                            tracker[npc] = timer = 90;
                        }
                        if (timer <= 0)
                        {
                            tracker[npc] = 90;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI("BombardSpikes"),
                                                         npc.Center, new Vector2(0, -5),
                                                         ProjectileID.SpikedSlimeSpike, 1, 0f);
                            }
                        }
                        else
                        {
                            tracker[npc] = timer - 1;
                        }
                    }
                }
            }
        }
    }
}
