using BossRush.Types;
using System;
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
            if (BRS.I.CurrentBossData is BossData bossData &&
                bossData.SpawnAttributes is SpawnAttributes spawnData)
            {
                float multiplier = spawnData.RateMultiplier;
                if (multiplier <= 0)
                {
                    spawnRate = 0;
                }
                else
                {
                    spawnRate = Math.Max(Util.RoundOff(spawnRate / multiplier), 0);
                }
                maxSpawns = Util.RoundOff((maxSpawns + spawnData.MaxFlatIncrease) * spawnData.MaxMultiplier);
            }
            else
            {
                maxSpawns = 0;
            }
        }
    }

    public override bool PreAI(NPC npc)
    {
        if (BRS.I.IsBossRushActive && npc.CountsAsACritter)
        {
            NPC inactiveNpc = Main.npc[npc.whoAmI] = new NPC();
            return inactiveNpc.active = false;
        }

        return true;
    }
}
