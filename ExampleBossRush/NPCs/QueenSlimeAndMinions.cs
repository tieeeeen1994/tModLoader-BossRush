using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs;

public class QueenSlimeAndMinions : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [
        NPCID.QueenSlimeBoss,
        NPCID.QueenSlimeMinionBlue,
        NPCID.QueenSlimeMinionPink,
        NPCID.QueenSlimeMinionPurple
    ];

    override protected bool AbsoluteCheck => IsCurrentBoss(NPCID.QueenSlimeBoss);

    protected override void Update(NPC npc)
    {
        if (npc.type == NPCID.QueenSlimeBoss)
        {
            var tracker = StoreOrFetch("Tracker", new Dictionary<Projectile, bool>());
            CleanInactiveData(tracker);
        }
        else if (npc.type == NPCID.QueenSlimeMinionPurple)
        {
            npc.knockBackResist = 0.05f;
        }
        else if (ApplicableTypes.Contains(npc.type))
        {
            npc.knockBackResist = 0;
        }
    }
}
