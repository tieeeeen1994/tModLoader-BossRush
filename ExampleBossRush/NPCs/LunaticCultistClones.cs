using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRushAPI.BossRushSystem;

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

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item) => CanBeHitLogic(npc);

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) => CanBeHitLogic(npc);

    private bool? CanBeHitLogic(NPC npc)
    {
        if (StandardChecks && npc.type == NPCID.AncientLight || npc.type == NPCID.AncientDoom)
        {
            return npc.chaseable = false;
        }
        return null;
    }
}
