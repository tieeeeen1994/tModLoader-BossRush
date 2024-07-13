using Terraria;
using Terraria.ModLoader;

namespace BossRush;

public class Boss : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation;
    }

    public override void SetDefaults(NPC entity)
    {
        if (BossRushSystem.IsBossRushActive() && entity.boss)
        {
            // entity.lifeMax = Util.RoundOff(entity.lifeMax * 0.1f);
        }
    }
}
