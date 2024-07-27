using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BR = BossRush.BossRush;

namespace BossRush;

public static class Util
{
    public static int RoundOff(this float value) => (int)MathF.Round(value, 0, MidpointRounding.ToEven);

    public static Vector2 RoundOff(this Vector2 value) => new(RoundOff(value.X), RoundOff(value.Y));

    public static void NewText(string message, Color? color = null, bool literal = false)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            if (literal)
            {
                Main.NewText(message, color ?? Color.White);
            }
            else
            {
                Main.NewText(Language.GetTextValue(message), color ?? Color.White);
            }
        }
        else if (Main.netMode == NetmodeID.Server)
        {
            if (literal)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color ?? Color.White);
            }
            else
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(message), color ?? Color.White);
            }
        }
    }

    public static int ToFrames(this float seconds) => RoundOff(seconds * 60);

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

    public static void CleanStage()
    {
        foreach (var npc in Main.npc)
        {
            if (!npc.friendly)
            {
                npc.active = false;
            }
        }
        foreach (var projectile in Main.projectile)
        {
            if (!projectile.friendly)
            {
                projectile.active = false;
            }
        }
        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = BR.I.GetPacket();
            packet.Write((byte)BR.PacketType.CleanStage);
            packet.Send();
        }
    }
}
