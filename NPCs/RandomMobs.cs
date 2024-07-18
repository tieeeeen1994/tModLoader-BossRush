using Terraria;
using Terraria.ModLoader;

namespace BossRush.NPCs;

public class RandomMobs : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation;

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (BossRushSystem.IsBossRushActive())
        {
            // Using this might have side-effects.
            // maxSpawns = 0;
            spawnRate = int.MaxValue;
        }
    }
}
