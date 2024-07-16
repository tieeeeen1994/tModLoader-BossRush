using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
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
    /// Instance of the Boss Rush Mod.
    /// </summary>
    public static BossRush I => ModContent.GetInstance<BossRush>();

    /// <summary>
    /// Handles the packet sent from the server or client.
    /// </summary>
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
                    }
                }
                break;

            case PacketType.SpawnPlayer:
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (Main.LocalPlayer.dead)
                    {
                        Main.LocalPlayer.Spawn(PlayerSpawnContext.ReviveFromDeath);
                    }
                }
                break;
        }
    }

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
        On_NPC.CreateBrickBoxForWallOfFlesh += CreateBrickBoxForWallOfFlesh;
        On_WorldGen.TriggerLunarApocalypse += TriggerLunarApocalypse;
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
                NPC selected = Main.npc[availableNPCSlot] = new NPC();
                selected.SetDefaults(Type, default);
                selected.whoAmI = availableNPCSlot;
                method = typeof(NPC).GetMethod("GiveTownUniqueDataToNPCsThatNeedIt", flags);
                method.Invoke(null, [Type, availableNPCSlot]);
                selected.position.X = X - selected.width / 2;
                selected.position.Y = Y - selected.height;
                selected.active = true;
                selected.timeLeft = (int)(NPC.activeTime * 1.25);
                selected.wet = Collision.WetCollision(selected.position, selected.width, selected.height);
                selected.ai[0] = ai0;
                selected.ai[1] = ai1;
                selected.ai[2] = ai2;
                selected.ai[3] = ai3;
                selected.target = Target;
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

    /// <summary>
    /// Detour for NPC.CreateBrickBoxForWallOfFlesh.
    /// Disables the creation of bricks when Wall of Flesh is defeated in Boss Rush mode.
    /// </summary>
    private void CreateBrickBoxForWallOfFlesh(On_NPC.orig_CreateBrickBoxForWallOfFlesh orig, NPC self)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig(self);
        }
    }

    /// <summary>
    /// Detour for WorldGen.TriggerLunarApocalypse.
    /// Prevents the pillars from spawning when Boss Rush is active.
    /// </summary>
    private void TriggerLunarApocalypse(On_WorldGen.orig_TriggerLunarApocalypse orig)
    {
        if (BossRushSystem.IsBossRushOff())
        {
            orig();
        }
    }

    /// <summary>
    /// Packet types used in Boss Rush.
    /// </summary>
    public enum PacketType : byte
    {
        /// <summary>
        /// Teleports a player to a specified position.
        /// </summary>
        Teleport,
        /// <summary>
        /// Revives a player from death.
        /// </summary>
        SpawnPlayer
    }
}
