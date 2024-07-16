using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BR = BossRush.BossRush;

namespace BossRush;

/// <summary>
/// Utility static class for the mod.
/// </summary>
public static class Util
{
    /// <summary>
    /// Spawns all dead players in the game.
    /// </summary>
    public static void SpawnDeadPlayers()
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (player.dead)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    player.Spawn(PlayerSpawnContext.ReviveFromDeath);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = BR.I.GetPacket();
                    packet.Write((byte)BR.PacketType.SpawnPlayer);
                    packet.Send(player.whoAmI);
                }
            }
        }
    }

    /// <summary>
    /// Removes all enemies according to the parameter.
    /// If the parameter is not given or null, it will use Main.npc.
    /// </summary>
    /// <param name="npcs">Group of enemies to remove</param>
    public static void CleanAllEnemies(IEnumerable<NPC> npcs = null)
    {
        npcs ??= Main.npc;
        foreach (var npc in npcs)
        {
            if (!npc.friendly)
            {
                npc.active = false;
            }
        }
    }

    /// <summary>
    /// Rounds off a float value to the nearest integer.
    /// </summary>
    /// <param name="value">Value to round off</param>
    /// <returns>Rounded off value</returns>
    public static int RoundOff(float value)
    {
        return (int)MathF.Round(value, 0, MidpointRounding.ToEven);
    }

    /// <summary>
    /// Rounds off Vector2 coordinates to the nearest integers.
    /// </summary>
    /// <param name="value">Coordinates to round off</param>
    /// <returns>Vector2 containing the rounded coordinates</returns>
    public static Vector2 RoundOff(Vector2 value)
    {
        return new(RoundOff(value.X), RoundOff(value.Y));
    }

    /// <summary>
    /// Sends a message to all players in the game.
    /// It automatically uses the proper methods for singleplayer and multiplayer.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public static void NewText(string message, Color? color = null)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            Main.NewText(message, color ?? Color.White);
        }
        else if (Main.netMode == NetmodeID.Server)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(message), color ?? Color.White);
        }
    }

    /// <summary>
    /// Converts seconds to frames. Useful for checking against code timers.
    /// </summary>
    /// <param name="seconds">Time in seconds</param>
    /// <returns>Computed frames</returns>
    public static int SecondsInFrames(float seconds)
    {
        return RoundOff(seconds * Main.frameRate);
    }

    /// <summary>
    /// Converts seconds to frames. Useful for checking against code timers.
    /// </summary>
    /// <param name="seconds">Time in seconds</param>
    /// <returns>Computed frames</returns>
    public static int SecondsInFrames(int seconds)
    {
        return seconds * Main.frameRate;
    }

    /// <summary>
    /// Chooses a random point in a rectangle.
    /// </summary>
    /// <param name="rectangle">Rectangle to choose a rnadom point from</param>
    /// <returns>A random point</returns>
    public static Vector2 ChooseRandomPointInRectangle(Rectangle rectangle)
    {
        int signWidth = Math.Sign(rectangle.Width);
        int signHeight = Math.Sign(rectangle.Height);
        int offsetX = rectangle.X + Main.rand.Next(Math.Abs(rectangle.Width) + 1) * signWidth;
        int offsetY = rectangle.Y + Main.rand.Next(Math.Abs(rectangle.Height) + 1) * signHeight;
        return new(offsetX, offsetY);
    }

    /// <summary>
    /// Returns -1 or 1 randomly.
    /// </summary>
    /// <returns>-1 or 1</returns>
    public static int RandomSign()
    {
        return Main.rand.NextBool() ? 1 : -1;
    }
}
