using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush;

public class BrainOfCthulhuAndMinions : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.Creeper, NPCID.BrainofCthulhu, NPCID.IchorSticker];

    protected override void Update(NPC npc)
    {
        var tracker = StoreOrFetch("Tracker", new Dictionary<int, bool>());
        var stickerTracker = StoreOrFetch("StickerTracker", new Dictionary<int, bool>());
        if (npc.type == NPCID.Creeper)
        {
            if (!tracker.TryGetValue(npc.whoAmI, out bool isModified) && !isModified)
            {
                npc.knockBackResist = 0f;
                tracker[npc.whoAmI] = true;
            }
        }
        else if (npc.type == NPCID.BrainofCthulhu && BRS.I.ReferenceBoss != null)
        {
            npc.knockBackResist = 0f;
            foreach (var pair in tracker)
            {
                NPC trackedNpc = Main.npc[pair.Key];
                if (!trackedNpc.active)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC(npc.GetSource_FromAI("CreeperDied"),
                                   trackedNpc.Center.X.RoundOff(),
                                   trackedNpc.Center.Y.RoundOff(),
                                   NPCID.IchorSticker);
                    }
                    tracker.Remove(pair.Key);
                }
            }
            CleanInactiveData(stickerTracker);
        }
        else if (npc.type == NPCID.IchorSticker && BRS.I.ReferenceBoss != null)
        {
            if (!stickerTracker.TryGetValue(npc.whoAmI, out bool tracked) && !tracked)
            {
                npc.lifeMax = Util.RoundOff(npc.lifeMax * .5f);
                npc.life = npc.lifeMax;
                npc.defense = 0;
                npc.damage = BRS.I.ReferenceBoss.damage;
                stickerTracker[npc.whoAmI] = true;
            }
            npc.velocity += npc.DirectionTo(BRS.I.ReferenceBoss.Center) * .25f;
            npc.velocity = npc.velocity.Clamp(new(-10f), new(10f));
        }
    }
}
