using Microsoft.Xna.Framework;

namespace ExampleBossRush;

internal static class ExampleBossRushUtils
{
    internal static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(MathHelper.Clamp(value.X, min.X, max.X), MathHelper.Clamp(value.Y, min.Y, max.Y));
    }
}
