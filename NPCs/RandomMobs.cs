using Terraria;
using Terraria.ModLoader;

namespace BossRush.NPCs;

public class RandomMobs : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation;
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (BossRushSystem.IsBossRushActive())
        {
            maxSpawns = 0;
            spawnRate = int.MaxValue;
        }
    }
}
