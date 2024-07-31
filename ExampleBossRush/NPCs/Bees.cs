using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.NPCs;

public class Bees : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.Bee, NPCID.BeeSmall];

    protected override void Update(NPC npc)
    {
        if (npc.type == NPCID.Bee)
        {
            npc.knockBackResist = 0f;
            npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * .2f;
            npc.velocity = npc.velocity.Clamp(10f);
        }
        else if (npc.type == NPCID.BeeSmall)
        {
            npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * .5f;
            npc.velocity = npc.velocity.Clamp(20f);
        }
    }
}
