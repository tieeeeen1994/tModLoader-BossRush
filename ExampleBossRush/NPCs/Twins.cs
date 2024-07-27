using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.NPCs
{
    public class TheTwins : BossRushBossAndMinions
    {
        protected override List<int> ApplicableTypes => [
            NPCID.Retinazer,
            NPCID.Spazmatism
        ];

        protected override void Update(NPC npc)
        {
            if (ApplicableTypes.Contains(npc.type))
            {
                var twinsTracker = StoreOrFetch("TwinsTracker", new Dictionary<NPC, bool>());
                if (npc.life < npc.lifeMax * 0.5f && !twinsTracker.TryGetValue(npc, out bool tracked) && !tracked)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                    twinsTracker[npc] = true;
                }

            }
        }
    }
}
