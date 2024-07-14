using Terraria;
using Terraria.ModLoader;

namespace BossRush.NPCs;

/// <summary>
/// NPCs that refers to the random mobs that spawn in the world.
/// </summary>
public class RandomMobs : GlobalNPC
{
    /// <summary>
    /// Late instantiation for the NPC.
    /// </summary>
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation;
    }

    /// <summary>
    /// Removes the spawn rate of random mobs when Boss Rush is active.
    /// </summary>
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (BossRushSystem.IsBossRushActive())
        {
            maxSpawns = 0;
            spawnRate = int.MaxValue;
        }
    }
}
