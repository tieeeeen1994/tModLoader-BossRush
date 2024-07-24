using BossRush;
using ExampleBossRush.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles
{
    public class KingSlimeProjectiles : GlobalProjectile
    {
        internal static Dictionary<string, object> Ai => KingSlimeAndMinions.ai;
        private static readonly List<int> applicableTypes = [ProjectileID.SpikedSlimeSpike];

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return lateInstantiation && applicableTypes.Contains(entity.type);
        }

        public override void PostAI(Projectile projectile)
        {
            if (BRS.I.IsBossRushActive && BRS.I.CurrentBoss != null && BRS.I.CurrentBossData != null)
            {
                if (projectile.type == ProjectileID.SpikedSlimeSpike)
                {
                    if (!Ai.TryGetValue("SpikeTracker", out object value1))
                    {
                        value1 = new Dictionary<Projectile, bool>();
                        Ai["SpikeTracker"] = value1;
                    }
                    if (BRS.I.ReferenceBoss is NPC referenceBoss && value1 is Dictionary<Projectile, bool> spikeTracker &&
                        !spikeTracker.TryGetValue(projectile, out bool tracked) && !tracked)
                    {
                        spikeTracker[projectile] = true;
                        projectile.damage = Util.RoundOff(referenceBoss.damage * .1f);
                    }
                    if (!Ai.TryGetValue("States", out object value2))
                    {
                        value2 = new Dictionary<Projectile, Tuple<string, int, Vector2>>();
                        Ai["States"] = value2;
                    }
                    if (value2 is Dictionary<Projectile, Tuple<string, int, Vector2>> stateTracker)
                    {
                        if (!stateTracker.TryGetValue(projectile, out Tuple<string, int, Vector2> state))
                        {
                            state = ("Default", 30, Vector2.Zero).ToTuple();
                        }
                        if (state.Item1 == "Default" && state.Item2 <= 0)
                        {
                            List<Player> players = [];
                            foreach (var player in Main.ActivePlayers)
                            {
                                if (!player.dead)
                                {
                                    players.Add(player);
                                }
                            }
                            Player target = players[Main.rand.Next(players.Count)];
                            Vector2 direction = projectile.DirectionTo(target.Center);
                            stateTracker[projectile] = ("Target", 0, direction).ToTuple();
                        }
                        else if (state.Item1 == "Default")
                        {
                            stateTracker[projectile] = (state.Item1, state.Item2 - 1, state.Item3).ToTuple();
                        }
                        else if (state.Item1 == "Target")
                        {
                            projectile.velocity = state.Item3 * 5f;
                        }
                    }
                }
            }
        }
    }
}
