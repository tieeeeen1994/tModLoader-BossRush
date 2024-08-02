using BossRush;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush.NPCs;

public class BrainOfCthulhuAndMinions : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.Creeper, NPCID.BrainofCthulhu, NPCID.IchorSticker];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.BrainofCthulhu);

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
        else if (npc.type == NPCID.BrainofCthulhu)
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
        else if (npc.type == NPCID.IchorSticker && BRS.I.ReferenceBoss is NPC boss)
        {
            if (!stickerTracker.TryGetValue(npc.whoAmI, out bool tracked) && !tracked)
            {
                npc.lifeMax = Util.RoundOff(npc.lifeMax * .4f);
                npc.life = npc.lifeMax;
                npc.defense = 0;
                npc.damage = boss.damage;
                stickerTracker[npc.whoAmI] = true;
            }
            npc.velocity += npc.DirectionTo(boss.Center) * .25f;
            npc.velocity = npc.velocity.Clamp(20f);
        }
    }
}
