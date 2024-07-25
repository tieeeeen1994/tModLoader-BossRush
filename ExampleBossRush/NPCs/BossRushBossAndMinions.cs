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
        else if (ai.Count > 0 && BRS.I.IsBossRushActive && (BRS.I.CurrentBoss == null || BRS.I.CurrentBossData == null))
        {
            ai.Clear();
        }
    }

    protected KeyValuePair<string, object> StoreOrFetch(string key, object value)
    {
        if (!ai.TryGetValue(key, out object existingValue))
        {
            ai[key] = existingValue = value;
        }
        return new KeyValuePair<string, object>(key, existingValue);
    }

    protected Dictionary<Entity, object> CleanInactiveData(string key)
    {
        if (ai.TryGetValue(key, out object value) && value is Dictionary<Entity, object> storage)
        {
            foreach (var entry in storage)
            {
                if (!entry.Key.active)
                {
                    storage.Remove(entry.Key);
                }
            }
            return storage;
        }
        else
        {
            return null;
        }
    }
}
