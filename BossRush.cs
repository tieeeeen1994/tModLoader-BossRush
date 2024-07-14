using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush;

/// <summary>
/// Mod class for Boss Rush.
/// </summary>
public class BossRush : Mod
{
    /// <summary>
    /// Contains hooks and detours for the mod.
    /// Repsonsible for disabling loot in Boss Rush mode and tracking boss defeat.
    /// It also removes the pesky King Slime has awoken message included in NPC.NewNPC when Boss Rush is active.
    /// </summary>
    public override void Load()
    {
        On_NPC.NPCLoot_DropHeals += NPCLoot_DropHeals;
        On_NPC.NPCLoot_DropItems += NPCLoot_DropItems;
        On_NPC.NPCLoot_DropMoney += NPCLoot_DropMoney;
        On_NPC.DoDeathEvents_DropBossPotionsAndHearts += DropBossPotionsAndHearts;
        On_NPC.NewNPC += NewNPC;
    }

    /// <summary>
    /// Unloads the hooks and detours for the mod.
    /// </summary>
    public override void Unload()
    {
        On_NPC.NPCLoot_DropHeals -= NPCLoot_DropHeals;
        On_NPC.NPCLoot_DropItems -= NPCLoot_DropItems;
        On_NPC.NPCLoot_DropMoney -= NPCLoot_DropMoney;
        On_NPC.DoDeathEvents_DropBossPotionsAndHearts -= DropBossPotionsAndHearts;
        On_NPC.NewNPC -= NewNPC;
    }

    /// <summary>
    /// Detour for NPC.NewNPC.
    /// It only disables the King Slime has awoken message when Boss Rush is active.
    /// </summary>
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

    /// <summary>
    /// Detour for NPC.NPCLoot_DropMoney.
    /// Removes coin drops when Boss Rush is active.
    /// </summary>
    private void NPCLoot_DropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, closestPlayer);
        }
    }

    /// <summary>
    /// Detour for NPC.NPCLoot_DropItems.
    /// Removes item drops when Boss Rush is active.
    /// It is instead used as a tracker for boss defeat when Boss Rush is active.
    /// </summary>
    private void NPCLoot_DropItems(On_NPC.orig_NPCLoot_DropItems orig, NPC self, Player closestPlayer)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, closestPlayer);
        }
        else if (BossRushSystem.IsBossRushActive() &&
                 BossRushSystem.I.currentBoss.Exists(boss => boss == self))
        {
            BossRushSystem.I.bossDefeated[self] = true;
        }
    }

    /// <summary>
    /// Detour for NPC.NPCLoot_DropHeals.
    /// Disables mana and heart drops when Boss Rush is active.
    /// Statues should still work.
    /// </summary>
    private void NPCLoot_DropHeals(On_NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, closestPlayer);
        }
    }

    /// <summary>
    /// Detour for NPC.DoDeathEvents_DropBossPotionsAndHearts.
    /// Disables the drops from the boss that drops many hearts and potions when Boss Rush is active.
    /// </summary>
    private void DropBossPotionsAndHearts(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig,
                                          NPC self, ref string typeName)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self, ref typeName);
        }
    }
}
