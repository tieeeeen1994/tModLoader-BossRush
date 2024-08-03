using ExampleBossRush.NPCs;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush.Detours;

public class ShieldStrengthTowerMaxDetour
{
    private static Hook _hook;

    public static void Apply()
    {
        PropertyInfo property = typeof(NPC).GetProperty("ShieldStrengthTowerMax", BindingFlags.Public | BindingFlags.Static);
        _hook = new Hook(
            property.GetGetMethod(),
            typeof(ShieldStrengthTowerMaxDetour).GetMethod("ShieldStrengthTowerMaxGetter")
        );
    }

    public static void Undo()
    {
        _hook?.Dispose();
        _hook = null;
    }

    public static int ShieldStrengthTowerMaxGetter(Func<int> orig)
    {
        if (BRS.I.IsBossRushActive)
        {
            return Pillars.ShieldValue;
        }
        else
        {
            return orig();
        }
    }
}