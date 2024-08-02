using ExampleBossRush.Types;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs;

public class Twins : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [
        NPCID.Retinazer,
        NPCID.Spazmatism
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(ApplicableTypes);

    protected override void Update(NPC npc)
    {
        if (ApplicableTypes.Contains(npc.type))
        {
            var twinsTracker = StoreOrFetch("TwinsTracker", new Dictionary<NPC, bool>());
            if (npc.life < npc.lifeMax * 0.6f && !twinsTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                npc.ai[0] = 1f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                npc.defense = npc.defDefense = 100;
                npc.netUpdate = true;
                twinsTracker[npc] = true;
            }

        }
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        binaryWriter.Write(npc.defense);
        binaryWriter.Write(npc.defDefense);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        npc.defense = binaryReader.ReadInt32();
        npc.defDefense = binaryReader.ReadInt32();
    }
}
