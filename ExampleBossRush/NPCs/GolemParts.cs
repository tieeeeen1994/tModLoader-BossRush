using BossRushAPI;
using BossRushAPI.NPCs;
using BossRushAPI.Types;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs;

public class GolemParts : BossRushBossAndMinions
{
    public override void Load()
    {
        BossAndMinions.PreSetDefaults += PreSetDefaults;
    }

    public override void Unload()
    {
        BossAndMinions.PreSetDefaults -= PreSetDefaults;
    }

    protected override List<int> ApplicableTypes => [
        NPCID.GolemFistLeft,
        NPCID.GolemFistRight,
        NPCID.GolemHeadFree,
        NPCID.Golem,
        NPCID.GolemHead
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.Golem);

    protected override void Update(NPC npc)
    {
        var realGolemHead = StoreOrFetch<NPC>("RealGolemHead", null);
        var extraHeads = StoreOrFetch("ExtraHeads", new NPC[2] { null, null });
        var fistStateTracker = StoreOrFetch("FistStateTracker", new Dictionary<NPC, float>());
        if (npc.type == NPCID.GolemFistLeft || npc.type == NPCID.GolemFistRight)
        {
            if (!fistStateTracker.ContainsKey(npc))
            {
                fistStateTracker.Add(npc, npc.ai[0]);
            }
            if (fistStateTracker.TryGetValue(npc, out float value) && value == 2f && npc.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var velocity = Main.rand.NextVector2CircularEdge(10, 10);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity,
                                                 ProjectileID.Nail, Util.RoundOff(npc.damage * .15f), 0f);
                    }
                }
            }
            fistStateTracker[npc] = npc.ai[0];
        }
        else if (npc.type == NPCID.GolemHeadFree)
        {
            var golemHeadTracker = StoreOrFetch("GolemHeadTracker", false);
            if (!golemHeadTracker)
            {
                ai["RealGolemHead"] = npc;
                ai["GolemHeadTracker"] = true;
                for (int i = 0; i < 2; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC head = NPC.NewNPCDirect(npc.GetSource_FromAI(),
                                                    npc.Center.X.RoundOff(),
                                                    npc.Center.Y.RoundOff(),
                                                    npc.type);
                        extraHeads[i] = head;
                        head.netUpdate = true;
                    }
                }
            }
            if (extraHeads[0] == npc)
            {
                npc.velocity += npc.DirectionTo(realGolemHead.Center + new Vector2(-400, 0)) * .1f;
            }
            else if (extraHeads[1] == npc)
            {
                npc.velocity += npc.DirectionTo(realGolemHead.Center + new Vector2(400, 0)) * .1f;
            }
        }
        else if (npc.type == NPCID.Golem)
        {
            CleanInactiveData(fistStateTracker);
        }
    }

    public override bool? CanBeHitByItem(NPC npc, Player player, Item item) => CanBeHitLogic(npc);

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile) => CanBeHitLogic(npc);

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        if (StandardChecks && npc.type == NPCID.GolemHeadFree)
        {
            var realGolemHead = StoreOrFetch<NPC>("RealGolemHead", null);
            var extraHeads = StoreOrFetch("ExtraHeads", new NPC[2] { null, null });
            binaryWriter.Write(realGolemHead.whoAmI);
            binaryWriter.Write(extraHeads[0].whoAmI);
            binaryWriter.Write(extraHeads[1].whoAmI);
            bitWriter.WriteBit(StoreOrFetch("GolemHeadTracker", false));
        }
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        if (StandardChecks && npc.type == NPCID.GolemHeadFree)
        {
            ai["RealGolemHead"] = Main.npc[binaryReader.ReadInt32()];
            var extraHeads = StoreOrFetch("ExtraHeads", new NPC[2] { null, null });
            extraHeads[0] = Main.npc[binaryReader.ReadInt32()];
            extraHeads[1] = Main.npc[binaryReader.ReadInt32()];
            ai["ExtraHeads"] = extraHeads;
            ai["GolemHeadTracker"] = bitReader.ReadBit();
        }
    }

    private bool? CanBeHitLogic(NPC npc)
    {
        if (StandardChecks && (npc.type == NPCID.GolemFistLeft || npc.type == NPCID.GolemFistRight))
        {
            return npc.chaseable = false;
        }
        return null;
    }

    private void PreSetDefaults(NPC npc, ModifiedAttributes attributes)
    {
        if (npc.type == NPCID.Golem && !Main.getGoodWorld)
        {
            npc.lifeMax = 25000;
        }
    }
}
