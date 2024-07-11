using Terraria;
using Terraria.ModLoader;

namespace BossRush.NPCs
{
    public class RandomMobs : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (BossRushSystem.I.state != BossRushSystem.States.Off)
            {
                maxSpawns = 0;
                spawnRate = int.MaxValue;
            }
        }
    }
}
