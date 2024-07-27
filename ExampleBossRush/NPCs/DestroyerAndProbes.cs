using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.NPCs
{
    public class DestroyerAndProbes : BossRushBossAndMinions
    {
        protected override List<int> ApplicableTypes => [
            NPCID.Probe,
        ];

        protected override void Update(NPC npc)
        {
            if (npc.type == NPCID.Probe)
            {
                npc.knockBackResist = 1.2f;
            }
        }
    }
}
