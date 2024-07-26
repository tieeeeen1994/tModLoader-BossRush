using Terraria;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace BossRush.NPCs;

public class RandomMobs : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation;

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (BRS.I.IsBossRushActive)
        {
            maxSpawns = 0;
        }
    }
}
