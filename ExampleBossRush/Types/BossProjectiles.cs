using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.Types;

public abstract class BossRushProjectiles : GlobalProjectile
{
    protected Dictionary<string, object> Ai => BossRushBossAndMinions.ai;

    protected abstract List<int> ApplicableTypes { get; }

    protected abstract void Update(Projectile projectile);

    public sealed override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return lateInstantiation && ApplicableTypes.Contains(entity.type);
    }

    public sealed override void PostAI(Projectile projectile)
    {
        if (!projectile.friendly && BRS.I.IsBossRushActive &&
            BRS.I.CurrentBoss != null && BRS.I.CurrentBossData != null)
        {
            Update(projectile);
        }
    }

    public T StoreOrFetch<T>(string key, T value) => (T)StoreOrFetch(Ai, key, value);

    public U StoreOrFetch<T, U>(Dictionary<T, U> storage, T key, U value)
    {
        if (!storage.ContainsKey(key))
        {
            storage[key] = value;
        }
        return storage[key];
    }

    public void CleanInactiveData<T, U>(Dictionary<T, U> storage) where T : Projectile
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
            if (!Main.projectile[entry.Key].active)
            {
                storage.Remove(entry.Key);
            }
        }
    }
}
