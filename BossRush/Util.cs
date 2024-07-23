using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace BossRush;

public static class Util
{
    public static int RoundOff(this float value) => (int)MathF.Round(value, 0, MidpointRounding.ToEven);

    public static Vector2 RoundOff(this Vector2 value) => new(RoundOff(value.X), RoundOff(value.Y));

    public static void NewText(string message, Color? color = null)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            Main.NewText(Language.GetTextValue(message), color ?? Color.White);
        }
        else if (Main.netMode == NetmodeID.Server)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(message), color ?? Color.White);
        }
    }

    // public static int ToFrames(this float seconds) => RoundOff(seconds * Main.frameRate);

    // public static int ToFrames(this int seconds) => seconds * Main.frameRate;

    public static int ToFrames(this float seconds) => RoundOff(ToFrames(seconds));

    public static int ToFrames(this int seconds) => seconds * 60;

    public static Vector2 ChooseRandomPoint(this Rectangle rectangle)
    {
        int signWidth = Math.Sign(rectangle.Width);
        int signHeight = Math.Sign(rectangle.Height);
        int offsetX = rectangle.X + Main.rand.Next(Math.Abs(rectangle.Width) + 1) * signWidth;
        int offsetY = rectangle.Y + Main.rand.Next(Math.Abs(rectangle.Height) + 1) * signHeight;
        return new(offsetX, offsetY);
    }

    public static int RandomSign(int number = 1) => Main.rand.NextBool() ? number : -number;
}
