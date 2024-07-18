using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace BossRush.NPCs;

public class BossAndSlaves : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation;

    public override void SetDefaults(NPC npc)
    {
        if (BRS.IsBossRushActive() && BRS.I.CurrentBossData?.ModifiedAttributes is { } attributes)
        {
            npc.lifeMax = attributes.ComputeLife(npc.lifeMax);
            npc.damage = attributes.ComputeDamage(npc.damage);
        }
    }

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        if (BRS.IsBossRushActive() && BRS.I.CurrentBoss != null &&
            BRS.I.CurrentBossData?.SubTypes?.Contains(npc.type) == true)
        {
            BRS.I.CurrentBoss.Add(npc);
            BRS.I.BossDefeated.Add(npc, false);
        }
    }

    public override void OnKill(NPC npc)
    {
        if (BRS.IsBossRushActive() && BRS.I.State == BRS.States.Run && BRS.I.CurrentBoss.Contains(npc))
        {
            BRS.I.BossDefeated[npc] = true;
            BRS.I.CheckBossCondition();
        }
    }

    public override void AI(NPC npc)
    {
        if (BRS.IsBossRushActive() && BRS.I.CurrentBoss?.Contains(npc) == true && BRS.I.CurrentBossData is { } bossData)
        {
            bossData.Update.Invoke(npc, bossData.AI);
        }
    }
}
