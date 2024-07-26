using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Projectiles;

public class KingSlimeProjectiles : BossRushProjectiles
{
    protected override List<int> ApplicableTypes => [ProjectileID.SpikedSlimeSpike];

    protected override void Update(Projectile projectile)
    {
        if (projectile.type == ProjectileID.SpikedSlimeSpike)
        {
            var states = StoreOrFetch("States", new Dictionary<Projectile, (string, int, Vector2)>());
            var spikeTracker = StoreOrFetch("SpikeTracker", new Dictionary<Projectile, bool>());
            if (BRS.I.ReferenceBoss is NPC referenceBoss &&
                !spikeTracker.TryGetValue(projectile, out bool tracked) && !tracked)
            {
                spikeTracker[projectile] = true;
                projectile.damage = Util.RoundOff(referenceBoss.damage * .1f);
            }
            (string state, int timer, Vector2 direction) = StoreOrFetch(states, projectile,
                                                                        ("Default", .5f.ToFrames(), Vector2.Zero));
            if (state == "Default" && timer <= 0)
            {
                List<Player> players = [];
                foreach (var player in Main.ActivePlayers)
                {
                    if (!player.dead)
                    {
                        players.Add(player);
                    }
                }
                if (players.Count > 0)
                {
                    Player target = players[Main.rand.Next(players.Count)];
                    direction = projectile.DirectionTo(target.Center);
                    states[projectile] = ("Target", 0, direction);
                }
            }
            else if (state == "Default")
            {
                states[projectile] = (state, timer - 1, direction);
            }
            else if (state == "Target")
            {
                projectile.velocity = direction * 5f;
            }
        }
    }
}
