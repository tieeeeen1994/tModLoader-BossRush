using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace BossRush;

public static class Util
{
    public static int SpawnBoss(int type, Vector2? offset = null)
    {
        Vector2 offsetValue = offset ?? Vector2.Zero;
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

        return NPC.NewNPC(new EntitySource_BossSpawn(target), spawnX, spawnY, type, 0, 0, 0, 0, 0, target.whoAmI);
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
}
