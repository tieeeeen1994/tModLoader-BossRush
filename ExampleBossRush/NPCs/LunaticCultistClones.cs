using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.NPCs;

public class LunaticCultistClone : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [
        NPCID.CultistBossClone,
        NPCID.AncientDoom,
        NPCID.AncientLight
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.CultistBoss);

    protected override void Update(NPC npc)
    {
        if (ApplicableTypes.Contains(npc.type) && BRS.I.ReferenceBoss is NPC boss)
        {
            npc.damage = boss.damage;
        }
    }

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
    {
        return npc.type != NPCID.AncientLight && npc.type != NPCID.AncientDoom;
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        return npc.type != NPCID.AncientLight && npc.type != NPCID.AncientDoom;
    }
}
