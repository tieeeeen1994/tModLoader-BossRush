using BossRushAPI;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush.NPCs;

public class Bees : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.Bee, NPCID.BeeSmall, NPCID.QueenBee];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.QueenBee);

    protected override void Update(NPC npc)
    {
        if (npc.type == NPCID.QueenBee)
        {
            StoreOrFetch("OriginalDamage", npc.damage);
        }
        else if (npc.type == NPCID.Bee)
        {
            npc.knockBackResist = 0f;
            npc.damage = Damage(1);
            npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * .2f;
            npc.velocity = npc.velocity.Clamp(10f);
        }
        else if (npc.type == NPCID.BeeSmall)
        {
            npc.damage = Damage(.75f);
            npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * .5f;
            npc.velocity = npc.velocity.Clamp(20f);
        }
    }
}
