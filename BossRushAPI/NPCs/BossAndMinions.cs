using BossRushAPI.Types;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using BRS = BossRushAPI.BossRushSystem;

namespace BossRushAPI.NPCs;

public class BossAndMinions : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && !entity.CountsAsACritter;
    }

    public override void SetDefaults(NPC npc)
    {
        if (BRS.I.IsBossRushActive && BRS.I.CurrentBossData?.ModifiedAttributes is { } attributes)
        {
            PreSetDefaults(npc, attributes);
            npc.lifeMax = attributes.ComputeLife(npc.lifeMax);
            npc.damage = attributes.ComputeDamage(npc.damage);
            npc.defense = attributes.ComputeDefense(npc.defense);
            PostSetDefaults(npc, attributes);
        }
    }

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        if (BRS.I.IsBossRushActive && BRS.I.CurrentBoss != null &&
            BRS.I.CurrentBossData?.SubTypes?.Contains(npc.type) == true)
        {
            BRS.I.DynamicAddBoss(npc);
        }
    }

    public override void OnKill(NPC npc)
    {
        if (BRS.I.IsBossRushActive && BRS.I.State == BRS.States.Run && BRS.I.CurrentBoss.Contains(npc))
        {
            BRS.I.MarkBossDefeat(npc);
        }
    }

    public static event Action<NPC, ModifiedAttributes> PreSetDefaults = delegate { };
    public static event Action<NPC, ModifiedAttributes> PostSetDefaults = delegate { };
}
