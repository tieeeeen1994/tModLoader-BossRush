using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush.Types;

public abstract class BossRushBossAndMinions : GlobalNPC
{
    internal static readonly Dictionary<string, object> ai = [];

    protected abstract List<int> ApplicableTypes { get; }

    protected abstract bool AbsoluteCheck { get; }

    protected abstract void Update(NPC npc);

    public sealed override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation && ApplicableTypes.Contains(entity.type);
    }

    public sealed override void PostAI(NPC npc)
    {
        if (StandardChecks)
        {
            Update(npc);
        }
    }

    public T StoreOrFetch<T>(string key, T value) => (T)StoreOrFetch(ai, key, value);

    public U StoreOrFetch<T, U>(Dictionary<T, U> storage, T key, U value)
    {
        if (!storage.ContainsKey(key))
        {
            storage[key] = value;
        }
        return storage[key];
    }

    public void CleanInactiveData<T, U>(Dictionary<T, U> storage) where T : Entity
    {
        foreach (var entry in storage)
        {
            if (!entry.Key.active)
            {
                storage.Remove(entry.Key);
            }
        }
    }

    public void CleanInactiveData<T>(Dictionary<int, T> storage)
    {
        foreach (var entry in storage)
        {
            if (!Main.npc[entry.Key].active)
            {
                storage.Remove(entry.Key);
            }
        }
    }

    protected bool StandardChecks => BRS.I.IsBossRushActive && BRS.I.CurrentBoss != null &&
                                     BRS.I.CurrentBossData != null && AbsoluteCheck;
}
