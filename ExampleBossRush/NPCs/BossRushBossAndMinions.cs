using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.NPCs;

public abstract class BossRushBossAndMinions : GlobalNPC
{
    internal static readonly Dictionary<string, object> ai = [];

    protected abstract List<int> ApplicableTypes { get; }

    protected abstract void Update(NPC npc);

    public sealed override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && ApplicableTypes.Contains(entity.type);
    }

    public sealed override void PostAI(NPC npc)
    {
        if (BRS.I.IsBossRushActive && BRS.I.CurrentBoss != null && BRS.I.CurrentBossData != null)
        {
            Update(npc);
        }
    }
}
