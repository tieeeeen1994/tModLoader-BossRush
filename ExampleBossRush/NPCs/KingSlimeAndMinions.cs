using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.NPCs;

public class KingSlimeAndMinions : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.KingSlime, NPCID.BlueSlime, NPCID.SlimeSpiked];

    protected override void Update(NPC npc)
    {
        var bombardSpikes = StoreOrFetch("BombardSpikes", new Dictionary<NPC, int>());
        if (npc.type == NPCID.KingSlime)
        {
            var states = StoreOrFetch("States", new Dictionary<Projectile, (string, int, Vector2)>());
            var spikeTracker = StoreOrFetch("SpikeTracker", new Dictionary<Projectile, bool>());
            if (npc.life < npc.lifeMax * .2f && !ai.ContainsKey("DefenseAdded"))
            {
                ai["DefenseAdded"] = true;
                npc.defense *= 30;
                npc.netUpdate = true;
            }
            CleanInactiveData(states);
            CleanInactiveData(spikeTracker);
            CleanInactiveData(bombardSpikes);
        }
        if (npc.type == NPCID.BlueSlime)
        {
            npc.Transform(NPCID.SlimeSpiked);
            npc.netUpdate = true;
        }
        if (npc.type == NPCID.SlimeSpiked)
        {
            if (BRS.I.ReferenceBoss == null)
            {
                if (bombardSpikes.Count > 0)
                {
                    bombardSpikes.Clear();
                }
            }
            else
            {
                var timer = StoreOrFetch(bombardSpikes, npc, 1.5f.ToFrames());
                if (timer <= 0)
                {
                    bombardSpikes[npc] = 1.5f.ToFrames();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI("BombardSpikes"),
                                                 npc.Center, new Vector2(0, -5),
                                                 ProjectileID.SpikedSlimeSpike, 1, 0f);
                    }
                }
                else
                {
                    bombardSpikes[npc] = timer - 1;
                }
            }
        }
    }
}
