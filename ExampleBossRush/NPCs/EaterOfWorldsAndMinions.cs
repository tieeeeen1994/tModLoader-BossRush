using BossRush;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

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
        var spitTracker = (Dictionary<NPC, bool>)StoreOrFetch("SpitTracker", new Dictionary<NPC, bool>()).Value;
        var segmentTracker = (Dictionary<NPC, bool>)StoreOrFetch("SegmentTracker", new Dictionary<NPC, bool>()).Value;
        var headTracker = (Dictionary<NPC, bool>)StoreOrFetch("HeadHealthTracker", new Dictionary<NPC, bool>()).Value;
        var corruptTimers = (Dictionary<NPC, (Vector2, int)>)StoreOrFetch(
            "CorruptorTimers",
            new Dictionary<NPC, (Vector2, int)>()
        ).Value;
        if (npc == BRS.I.ReferenceBoss)
        {
            CleanInactiveData("SpitTracker");
            CleanInactiveData("HeadHealthTracker");
            foreach (var body in segmentTracker)
            {
                NPC bodyEntity = body.Key;
                if (!bodyEntity.active)
                {
                    if (!corruptTimers.TryGetValue(bodyEntity, out (Vector2, int) value))
                    {
                        segmentTracker.Remove(bodyEntity);
                        corruptTimers[bodyEntity] = (bodyEntity.Center, 1.5f.ToFrames());
                    }
                    else if (value.Item2 <= 0)
                    {
                        corruptTimers.Remove(bodyEntity);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC mob = NPC.NewNPCDirect(bodyEntity.GetSource_FromAI("PartCutOff"),
                                                       value.Item1.X.RoundOff(),
                                                       value.Item1.Y.RoundOff(),
                                                       NPCID.Corruptor);
                            mob.lifeMax = Util.RoundOff(bodyEntity.lifeMax * .2f);
                            mob.life = mob.lifeMax;
                            mob.defense = 0;
                            mob.damage = Util.RoundOff(bodyEntity.damage * .5f);
                            mob.knockBackResist = 0f;
                            mob.netUpdate = true;
                        }
                    }
                    else
                    {
                        corruptTimers[bodyEntity] = (value.Item1, value.Item2 - 1);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            ModPacket packet = ExampleBossRush.Instance.GetPacket();
                            packet.Write((byte)ExampleBossRush.PacketTypes.CorruptorDust);
                            packet.WriteVector2(value.Item1);
                            packet.Send();
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Dust.NewDust(value.Item1 - new Vector2(15, 15), 30, 30, DustID.Demonite);
                            }
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
            }
            NPC currentHead = BRS.I.CurrentBoss.Find(boss => boss.type == NPCID.EaterofWorldsHead);
            if (currentHead != null)
            {
                npc.damage = Util.RoundOff(currentHead.damage * .5f);
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
    }
}
