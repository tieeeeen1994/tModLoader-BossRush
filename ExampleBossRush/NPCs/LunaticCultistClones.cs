using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.NPCs;

public class LunaticCultistClone : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.CultistBossClone];

    protected override void Update(NPC npc)
    {
        if (npc.type == NPCID.CultistBossClone && BRS.I.ReferenceBoss is NPC boss)
        {
            npc.damage = boss.damage;
        }
    }
}
