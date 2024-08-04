using BossRushAPI;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs;

public class WallOfFleshAndMinions : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [
        NPCID.TheHungry,
        NPCID.TheHungryII
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.WallofFlesh);

    protected override void Update(NPC npc)
    {
        if (npc.type == NPCID.TheHungry || npc.type == NPCID.TheHungryII)
        {
            npc.knockBackResist = 0f;
        }
    }
}
