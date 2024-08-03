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
        NPCID.TheHungryII,
        NPCID.LeechHead,
        NPCID.LeechBody,
        NPCID.LeechTail,
        NPCID.WallofFlesh
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.WallofFlesh);

    protected override void Update(NPC npc)
    {
        var leechTracker = StoreOrFetch("LeechTracker", new Dictionary<NPC, bool>());
        if (npc.type == NPCID.TheHungry || npc.type == NPCID.TheHungryII)
        {
            npc.knockBackResist = 0f;
        }
        else if (npc.type == NPCID.WallofFlesh)
        {
            CleanInactiveData(leechTracker);
        }
        else if (npc.type == NPCID.LeechHead || npc.type == NPCID.LeechBody || npc.type == NPCID.LeechTail)
        {
            if (!leechTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                npc.damage = Util.RoundOff(npc.damage * .5f);
                leechTracker[npc] = true;
            }
        }
    }
}
