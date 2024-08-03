using ExampleBossRush.Detours;
using ExampleBossRush.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush;

public class ExampleBossRush : Mod
{
    public static ExampleBossRush Instance => ModContent.GetInstance<ExampleBossRush>();

    public override void Load()
    {
        On_NebulaPillarBigProgressBar.GetMaxShieldValue += NebulaShield;
        On_SolarFlarePillarBigProgressBar.GetMaxShieldValue += SolarShield;
        On_VortexPillarBigProgressBar.GetMaxShieldValue += VortexShield;
        On_StardustPillarBigProgressBar.GetMaxShieldValue += StardustShield;
        ShieldStrengthTowerMaxDetour.Apply();
        On_Projectile.CanExplodeTile += On_Projectile_CanExplodeTile;
    }

    public override void Unload()
    {
        On_NebulaPillarBigProgressBar.GetMaxShieldValue -= NebulaShield;
        On_SolarFlarePillarBigProgressBar.GetMaxShieldValue -= SolarShield;
        On_VortexPillarBigProgressBar.GetMaxShieldValue -= VortexShield;
        On_StardustPillarBigProgressBar.GetMaxShieldValue -= StardustShield;
        ShieldStrengthTowerMaxDetour.Undo();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketTypes packetType = (PacketTypes)reader.ReadByte();
        switch (packetType)
        {
            case PacketTypes.CorruptorDust:
                {
                    Vector2 position = reader.ReadVector2();
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDust(position - new Vector2(15, 15), 30, 30, DustID.Demonite);
                    }
                }
                break;

            case PacketTypes.PrimeScrap:
                {
                    int npcIndex = reader.ReadInt32();
                    NPC npc = Main.npc[npcIndex];
                    if (npc.type == NPCID.PrimeSaw && npc.active &&
                        npc.TryGetGlobalNPC<SkeletronPrimeAndArms>(out var global))
                    {
                        global.SawScrap(npc);
                    }
                }
                break;

            case PacketTypes.EmpressClean:
                {
                    int npcIndex = reader.ReadInt32();
                    NPC npc = Main.npc[npcIndex];
                    if (npc.type == NPCID.HallowBoss && npc.active &&
                        npc.TryGetGlobalNPC<EmpressOfLight>(out var global))
                    {
                        global.DestroyAllProjectiles(npcIndex);
                    }
                }
                break;
        }
    }

    private float SolarShield(On_SolarFlarePillarBigProgressBar.orig_GetMaxShieldValue orig,
                              SolarFlarePillarBigProgressBar self)
    {
        if (BRS.I.IsBossRushActive)
        {
            return Pillars.ShieldValue;
        }
        else
        {
            return orig(self);
        }
    }

    private float VortexShield(On_VortexPillarBigProgressBar.orig_GetMaxShieldValue orig,
                               VortexPillarBigProgressBar self)
    {
        if (BRS.I.IsBossRushActive)
        {
            return Pillars.ShieldValue;
        }
        else
        {
            return orig(self);
        }
    }

    private float NebulaShield(On_NebulaPillarBigProgressBar.orig_GetMaxShieldValue orig,
                               NebulaPillarBigProgressBar self)
    {
        if (BRS.I.IsBossRushActive)
        {
            return Pillars.ShieldValue;
        }
        else
        {
            return orig(self);
        }
    }

    private float StardustShield(On_StardustPillarBigProgressBar.orig_GetMaxShieldValue orig,
                                 StardustPillarBigProgressBar self)
    {
        if (BRS.I.IsBossRushActive)
        {
            return Pillars.ShieldValue;
        }
        else
        {
            return orig(self);
        }
    }

    private bool On_Projectile_CanExplodeTile(On_Projectile.orig_CanExplodeTile orig, Projectile self, int x, int y)
    {
        if (BRS.I.IsBossRushActive && self.type == ProjectileID.BombSkeletronPrime && Main.getGoodWorld)
        {
            return false;
        }
        else
        {
            return orig(self, x, y);
        }
    }

    public enum PacketTypes : byte
    {
        CorruptorDust,
        PrimeScrap,
        EmpressClean
    }
}
