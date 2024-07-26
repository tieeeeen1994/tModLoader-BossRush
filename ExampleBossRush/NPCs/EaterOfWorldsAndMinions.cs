using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;
using EBR = ExampleBossRush.ExampleBossRush;

namespace ExampleBossRush.NPCs;

public class EaterOfWorldsAndMinions : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [
        NPCID.EaterofWorldsHead,
        NPCID.EaterofWorldsBody,
        NPCID.EaterofWorldsTail,
        NPCID.Corruptor,
        NPCID.EaterofSouls,
        NPCID.VileSpit,
        NPCID.VileSpitEaterOfWorlds
    ];

    protected override void Update(NPC npc)
    {
        var spitTracker = StoreOrFetch("SpitTracker", new Dictionary<NPC, bool>());
        var segmentTracker = StoreOrFetch("SegmentTracker", new Dictionary<NPC, bool>());
        var headTracker = StoreOrFetch("HeadHealthTracker", new Dictionary<NPC, bool>());
        var corruptTimers = StoreOrFetch("CorruptorTimers", new List<(Vector2, int)>());
        var corruptTracker = StoreOrFetch("CorruptorTracker", new Dictionary<NPC, bool>());
        var spitTimer = StoreOrFetch("SpitTimer", new Dictionary<NPC, int>());
        if (npc == BRS.I.ReferenceBoss)
        {
            CleanInactiveData(spitTracker);
            CleanInactiveData(headTracker);
            CleanInactiveData(corruptTracker);
            CleanInactiveData(spitTimer);
            foreach (var body in segmentTracker)
            {
                NPC bodyEntity = body.Key;
                if (!bodyEntity.active)
                {
                    corruptTimers.Add((bodyEntity.Center, 1.5f.ToFrames()));
                    segmentTracker.Remove(bodyEntity);
                }
            }
            for (int i = 0; i < corruptTimers.Count; i++)
            {
                (Vector2 position, int timer) = corruptTimers[i];
                if (timer <= 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC(BRS.I.ReferenceBoss.GetSource_FromAI("PartCutOff"),
                                   position.X.RoundOff(), position.Y.RoundOff(),
                                   NPCID.Corruptor);
                    }
                    corruptTimers.RemoveAt(i--);
                }
                else
                {
                    corruptTimers[i] = (position, timer - 1);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = EBR.Instance.GetPacket();
                        packet.Write((byte)EBR.PacketTypes.CorruptorDust);
                        packet.WriteVector2(position);
                        packet.Send();
                    }
                    else
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Dust.NewDust(position - new Vector2(15, 15), 30, 30, DustID.Demonite);
                        }
                    }
                }
            }
        }
        if (npc.type == NPCID.VileSpit || npc.type == NPCID.VileSpitEaterOfWorlds)
        {
            if (!spitTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                npc.life = npc.lifeMax = 10;
                npc.defense = 10000;
                npc.knockBackResist = 0f;
                spitTracker[npc] = true;
                spitTimer[npc] = 5.ToFrames();
            }
            NPC currentHead = BRS.I.CurrentBoss.Find(boss => boss.type == NPCID.EaterofWorldsHead);
            if (currentHead != null)
            {
                npc.damage = Util.RoundOff(currentHead.damage * .5f);
            }
            if (spitTimer.TryGetValue(npc, out int timer))
            {
                if (timer <= 0)
                {
                    npc.active = false;
                }
                else
                {
                    spitTimer[npc] = timer - 1;
                }
            }
        }
        if (npc.type == NPCID.EaterofWorldsBody && npc.active)
        {
            if (!segmentTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                npc.defense = 0;
                segmentTracker[npc] = true;
            }
        }
        if ((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail) && npc.active)
        {
            if (!headTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                npc.life = npc.lifeMax;
                npc.netUpdate = true;
                headTracker[npc] = true;
            }
        }
        if (npc.type == NPCID.Corruptor && npc.active)
        {
            if (!corruptTracker.TryGetValue(npc, out bool tracked) && !tracked)
            {
                NPC currentBody = BRS.I.CurrentBoss.Find(boss => boss.type == NPCID.EaterofWorldsBody);
                if (currentBody != null)
                {
                    npc.lifeMax = Util.RoundOff(currentBody.lifeMax * .3f);
                    npc.life = npc.lifeMax;
                    npc.defense = 0;
                    npc.damage = Util.RoundOff(currentBody.damage * .5f);
                    npc.knockBackResist = 0f;
                    corruptTracker[npc] = true;
                }
            }
        }
    }
}
