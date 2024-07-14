using Terraria;
using Terraria.ModLoader;

namespace BossRush.NPCs;

/// <summary>
/// NPCs that refers to the bosses and their minions when Boss Rush is active.
/// </summary>
public class BossAndSlaves : GlobalNPC
{
    /// <summary>
    /// Late instantiation for the NPC.
    /// </summary>
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return lateInstantiation;
    }

    /// <summary>
    /// Sets the defaults for the NPC when it spawns during Boss Rush.
    /// </summary>
    public override void SetDefaults(NPC entity)
    {
        if (BossRushSystem.IsBossRushActive())
        {
            // entity.lifeMax *= 30;
        }
    }
}
