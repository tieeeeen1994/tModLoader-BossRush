using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

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
        if (BRS.IsBossRushActive())
        {
            entity.lifeMax = Util.RoundOff(entity.lifeMax * .3f);
        }
    }

    /// <summary>
    /// This hook is used for adding BossData.subTypes into the BossRushSystem.currentBoss list.
    /// </summary>
    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        if (BRS.IsBossRushActive() && BRS.I.currentBoss != null &&
            BRS.I.currentBossData?.subTypes?.Contains(npc.type) == true)
        {
            BRS.I.currentBoss.Add(npc);
            BRS.I.bossDefeated.Add(npc, false);
        }
    }

    /// <summary>
    /// Triggers the boss-related main code for States.Run in the system.
    /// In this context, it checks whether all bosses are defeated.
    /// </summary>
    public override void OnKill(NPC npc)
    {
        if (BRS.IsBossRushActive() && BRS.I.state == BRS.States.Run && BRS.I.currentBoss.Contains(npc))
        {
            BRS.I.bossDefeated[npc] = true;
            BRS.I.CheckBossCondition();
        }
    }
}
