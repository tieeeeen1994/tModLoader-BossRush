using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using BRS = BossRushAPI.BossRushSystem;

namespace ExampleBossRush;

internal static class ExampleBossRushUtils
{
    internal static Vector2 Clamp(this Vector2 value, float maxDistance)
    {
        if (Vector2.Distance(Vector2.Zero, value) > maxDistance)
        {
            return Vector2.Normalize(value) * maxDistance;
        }
        else
        {
            return value;
        }
    }

    internal static bool IsCurrentBoss(List<int> types)
    {
        return BRS.I.CurrentBoss is List<NPC> bosses && bosses.Exists(npc => types.Contains(npc.type));
    }

    internal static bool IsCurrentBoss(int type) => IsCurrentBoss([type]);
}
