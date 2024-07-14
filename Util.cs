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
    public static List<int> SpawnBoss(BossData data)
    {
        data.timeContext?.ChangeTime();

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
        List<int> spawnedBossIndex = [];

        foreach (var type in data.type)
        {
            Vector2 offsetValue = data.RandomSpawnLocation();
            int spawnX = RoundOff(target.Center.X + offsetValue.X);
            int spawnY = RoundOff(target.Center.Y + offsetValue.Y);

            // Start at index 1 to avoid encountering the nasty vanilla bug for certain bosses.
            spawnedBossIndex.Add(NPC.NewNPC(new EntitySource_BossSpawn(target), spawnX, spawnY, type, 1));
        }

        return spawnedBossIndex;
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

    public static int SecondsInFrames(float seconds)
    {
        return RoundOff(seconds * Main.frameRate);
    }

    public static int SecondsInFrames(int seconds)
    {
        return seconds * Main.frameRate;
    }
}
