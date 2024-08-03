using BossRushAPI.Interfaces;
using Microsoft.Xna.Framework;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRushAPI.BossRushSystem;

namespace BossRushAPI;

public class BossRushAPI : Mod, IInstanceable<BossRushAPI>
{
    public static BossRushAPI I => ModContent.GetInstance<BossRushAPI>();
    public static BossRushAPI Instance => I;

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketType packetType = (PacketType)reader.ReadByte();
        switch (packetType)
        {
            case PacketType.Teleport:
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    Vector2 position = reader.ReadVector2();
                    if (Main.LocalPlayer.active)
                    {
                        Main.LocalPlayer.Teleport(position);
                        NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0,
                                            Main.myPlayer, position.X, position.Y);
                    }
                }
                break;

            case PacketType.SpawnPlayer:
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer.dead)
                {
                    Main.LocalPlayer.Spawn(PlayerSpawnContext.ReviveFromDeath);
                }
                break;

            case PacketType.CleanStage:
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    Util.CleanStage();
                }
                break;
        }
    }

    public override void Load()
    {
        On_NPC.NPCLoot_DropHeals += NPCLoot_DropHeals;
        On_NPC.NPCLoot_DropItems += NPCLoot_DropItems;
        On_NPC.NPCLoot_DropMoney += NPCLoot_DropMoney;
        On_NPC.DoDeathEvents_DropBossPotionsAndHearts += DropBossPotionsAndHearts;
        On_NPC.NewNPC += NewNPC;
        On_NPC.CreateBrickBoxForWallOfFlesh += CreateBrickBoxForWallOfFlesh;
        On_WorldGen.TriggerLunarApocalypse += TriggerLunarApocalypse;
    }

    public override void Unload()
    {
        On_NPC.NPCLoot_DropHeals -= NPCLoot_DropHeals;
        On_NPC.NPCLoot_DropItems -= NPCLoot_DropItems;
        On_NPC.NPCLoot_DropMoney -= NPCLoot_DropMoney;
        On_NPC.DoDeathEvents_DropBossPotionsAndHearts -= DropBossPotionsAndHearts;
        On_NPC.NewNPC -= NewNPC;
        On_NPC.CreateBrickBoxForWallOfFlesh -= CreateBrickBoxForWallOfFlesh;
        On_WorldGen.TriggerLunarApocalypse -= TriggerLunarApocalypse;
    }

    private int NewNPC(On_NPC.orig_NewNPC orig, IEntitySource source, int X, int Y, int Type,
                       int Start, float ai0, float ai1, float ai2, float ai3, int Target)
    {
        if (BRS.I.IsBossRushActive)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo method = typeof(NPC).GetMethod("GetAvailableNPCSlot", flags);
            int trueStart = Start == 0 ? 1 : Start; // Do not use index 0 in Boss Rush as Worm AI have bugs.
            int availableNPCSlot = (int)method.Invoke(null, [Type, trueStart]);
            if (availableNPCSlot >= 0)
            {
                NPC selected = Main.npc[availableNPCSlot] = new NPC();
                selected.SetDefaults(Type, default);
                selected.whoAmI = availableNPCSlot;
                method = typeof(NPC).GetMethod("GiveTownUniqueDataToNPCsThatNeedIt", flags);
                method.Invoke(null, [Type, availableNPCSlot]);
                selected.position.X = X - selected.width / 2;
                selected.position.Y = Y - selected.height;
                selected.active = true;
                selected.timeLeft = Util.RoundOff(NPC.activeTime * 1.25f);
                selected.wet = Collision.WetCollision(selected.position, selected.width, selected.height);
                selected.ai[0] = ai0;
                selected.ai[1] = ai1;
                selected.ai[2] = ai2;
                selected.ai[3] = ai3;
                selected.target = Target;
                flags = BindingFlags.Static | BindingFlags.NonPublic;
                method = typeof(NPCLoader).GetMethod("OnSpawn", flags);
                method.Invoke(null, [Main.npc[availableNPCSlot], source]);
                return availableNPCSlot;
            }
            // Code is the same except King Slime spawn message is removed here.
            return 200;
        }
        else
        {
            return orig(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
        }
    }

    private void NPCLoot_DropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
    {
        if (BRS.I.IsBossRushOff)
        {
            orig(self, closestPlayer);
        }
    }

    private void NPCLoot_DropItems(On_NPC.orig_NPCLoot_DropItems orig, NPC self, Player closestPlayer)
    {
        if (BRS.I.IsBossRushOff)
        {
            orig(self, closestPlayer);
        }
    }

    private void NPCLoot_DropHeals(On_NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer)
    {
        if (BRS.I.IsBossRushOff)
        {
            orig(self, closestPlayer);
        }
    }

    private void DropBossPotionsAndHearts(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig,
                                          NPC self, ref string typeName)
    {
        if (BRS.I.IsBossRushOff)
        {
            orig(self, ref typeName);
        }
    }

    private void CreateBrickBoxForWallOfFlesh(On_NPC.orig_CreateBrickBoxForWallOfFlesh orig, NPC self)
    {
        if (BRS.I.IsBossRushOff)
        {
            orig(self);
        }
    }

    private void TriggerLunarApocalypse(On_WorldGen.orig_TriggerLunarApocalypse orig)
    {
        if (BRS.I.IsBossRushOff)
        {
            orig();
        }
    }

    public enum PacketType : byte { Teleport, SpawnPlayer, CleanStage }
}
