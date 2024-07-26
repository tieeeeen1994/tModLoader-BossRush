using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.NPCs
{
    public class TheHungries : BossRushBossAndMinions
    {
        protected override List<int> ApplicableTypes => [NPCID.TheHungry, NPCID.TheHungryII];

        protected override void Update(NPC npc)
        {
            if (npc.type == NPCID.TheHungry || npc.type == NPCID.TheHungryII)
            {
                npc.knockBackResist = 0f;
            }
        }
    }
}
