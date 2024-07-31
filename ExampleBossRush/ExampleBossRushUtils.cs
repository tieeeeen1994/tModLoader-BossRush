using Microsoft.Xna.Framework;
using Terraria;

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
}
