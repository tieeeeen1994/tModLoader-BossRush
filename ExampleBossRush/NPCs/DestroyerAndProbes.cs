using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush.NPCs;

public class DestroyerAndProbes : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [
        NPCID.Probe,
        NPCID.DeadlySphere,
        NPCID.TheDestroyer
    ];

    protected override void Update(NPC npc)
    {
        var probeDamage = StoreOrFetch<int?>("ProbeDamage", null);
        var probeDefense = StoreOrFetch<int?>("ProbeDefense", null);
        var sphereTracker = StoreOrFetch("SphereTracker", new Dictionary<NPC, bool>());
        if (npc.type == NPCID.Probe)
        {
            if (!probeDamage.HasValue || !probeDefense.HasValue)
            {
                ai["ProbeDamage"] = npc.damage;
                ai["ProbeDefense"] = npc.defense;
            }
            npc.knockBackResist = 1.2f;
        }
        else if (npc.type == NPCID.DeadlySphere && probeDamage.HasValue && probeDefense.HasValue)
        {
            if (!sphereTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                npc.damage = npc.defDamage = probeDamage.Value;
                npc.defense = npc.defDefense = probeDefense.Value;
                sphereTracker[npc] = true;
            }
        }
        else if (npc.type == NPCID.TheDestroyer)
        {
            CleanInactiveData(sphereTracker);
        }
    }

    public override void OnKill(NPC npc)
    {
        if (npc.type == NPCID.Probe)
        {
            NPC.NewNPC(npc.GetSource_FromAI("ProbeDied"),
                       npc.Center.X.RoundOff(), npc.Center.Y.RoundOff(),
                       NPCID.DeadlySphere);
        }
    }
}
