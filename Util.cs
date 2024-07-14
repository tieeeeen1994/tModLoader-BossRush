using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using static BossRush.BossRushSystem;

namespace BossRush;

public static class Util
{
    public static int SpawnBoss(BossData data)
    {
        data.timeContext?.ChangeTime();

        Vector2 offsetValue = data.RandomSpawnLocation();
        List<Player> potentialTargetPlayers = [];
        float highestAggro = float.MinValue;

        foreach (var player in Main.ActivePlayers)
        {
            if (highestAggro == player.aggro)
            {
                potentialTargetPlayers.Add(player);
            }
            else if (player.aggro > highestAggro)
            {
                potentialTargetPlayers.Clear();
                potentialTargetPlayers.Add(player);
                highestAggro = player.aggro;
            }
        }

        Player target = Main.rand.Next(potentialTargetPlayers);
        int spawnX = RoundOff(target.Center.X + offsetValue.X);
        int spawnY = RoundOff(target.Center.Y + offsetValue.Y);

        // Start at index 1 to avoid encountering the nasty vanilla bug for certain bosses.
        return NPC.NewNPC(new EntitySource_BossSpawn(target), spawnX, spawnY, data.type, 1);
    }

    public static void SpawnDeadPlayers()
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (player.dead)
            {
                player.Spawn(PlayerSpawnContext.ReviveFromDeath);
            }
        }
    }

    public static void CleanAllEnemies()
    {
        foreach (var npc in Main.npc)
        {
            if (!npc.friendly)
            {
                npc.active = false;
            }
        }
    }

    public static int RoundOff(float value)
    {
        return (int)MathF.Round(value, 0, MidpointRounding.AwayFromZero);
    }

    public static Vector2 RoundOff(Vector2 value)
    {
        return new(RoundOff(value.X), RoundOff(value.Y));
    }

    public static void NewText(string message, Color? color = null)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            Main.NewText(message, color ?? Color.White);
        }
        else if (Main.netMode == NetmodeID.Server)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color ?? Color.White);
        }
    }
}
