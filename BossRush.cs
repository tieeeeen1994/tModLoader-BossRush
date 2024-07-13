using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush;

public class BossRush : Mod
{
    public override void Load()
    {
        On_NPC.NPCLoot_DropHeals += NPCLoot_DropHeals;
        On_NPC.NPCLoot_DropItems += NPCLoot_DropItems;
        On_NPC.NPCLoot_DropMoney += NPCLoot_DropMoney;
        On_NPC.DoDeathEvents_DropBossPotionsAndHearts += DropBossPotionsAndHearts;
        On_NPC.NewNPC += NewNPC;
    }

    public override void Unload()
    {
        On_NPC.NPCLoot_DropHeals -= NPCLoot_DropHeals;
        On_NPC.NPCLoot_DropItems -= NPCLoot_DropItems;
        On_NPC.NPCLoot_DropMoney -= NPCLoot_DropMoney;
        On_NPC.DoDeathEvents_DropBossPotionsAndHearts -= DropBossPotionsAndHearts;
        On_NPC.NewNPC -= NewNPC;
    }

    private int NewNPC(On_NPC.orig_NewNPC orig, IEntitySource source, int X, int Y, int Type,
                       int Start, float ai0, float ai1, float ai2, float ai3, int Target)
    {
        if (BossRushSystem.IsBossRushActive() && Type == NPCID.KingSlime)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            MethodInfo method = typeof(NPC).GetMethod("GetAvailableNPCSlot", flags);
            int availableNPCSlot = (int)method.Invoke(null, [Type, Start]);
            if (availableNPCSlot >= 0)
            {
                Main.npc[availableNPCSlot] = new NPC();
                Main.npc[availableNPCSlot].SetDefaults(Type, default);
                Main.npc[availableNPCSlot].whoAmI = availableNPCSlot;
                method = typeof(NPC).GetMethod("GiveTownUniqueDataToNPCsThatNeedIt", flags);
                method.Invoke(null, [Type, availableNPCSlot]);
                Main.npc[availableNPCSlot].position.X = X - Main.npc[availableNPCSlot].width / 2;
                Main.npc[availableNPCSlot].position.Y = Y - Main.npc[availableNPCSlot].height;
                Main.npc[availableNPCSlot].active = true;
                Main.npc[availableNPCSlot].timeLeft = (int)(NPC.activeTime * 1.25);
                Main.npc[availableNPCSlot].wet = Collision.WetCollision(Main.npc[availableNPCSlot].position,
                                                                        Main.npc[availableNPCSlot].width,
                                                                        Main.npc[availableNPCSlot].height);
                Main.npc[availableNPCSlot].ai[0] = ai0;
                Main.npc[availableNPCSlot].ai[1] = ai1;
                Main.npc[availableNPCSlot].ai[2] = ai2;
                Main.npc[availableNPCSlot].ai[3] = ai3;
                Main.npc[availableNPCSlot].target = Target;
                return availableNPCSlot;
            }
            return 200;
        }
        else
        {
            return orig(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
        }
    }

    private void NPCLoot_DropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, closestPlayer);
        }
    }

    private void NPCLoot_DropItems(On_NPC.orig_NPCLoot_DropItems orig, NPC self, Player closestPlayer)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, closestPlayer);
        }
    }

    private void NPCLoot_DropHeals(On_NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, closestPlayer);
        }
    }

    private void DropBossPotionsAndHearts(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig,
                                          NPC self, ref string typeName)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, ref typeName);
        }
    }
}
