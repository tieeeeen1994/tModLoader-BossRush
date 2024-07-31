using BossRush;
using BossRush.Types;
using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EBR = ExampleBossRush.ExampleBossRush;

namespace ExampleBossRush.NPCs;

public class EmpressOfLight : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.HallowBoss];

    protected override void Update(NPC npc)
    {
        if (npc.type == NPCID.HallowBoss)
        {
            var dayTracker = StoreOrFetch("DayTracker", false);
            if (!dayTracker && npc.life <= npc.lifeMax * .05f)
            {
                npc.ai[0] = 10f;
                npc.ai[1] = 0f;
                npc.ai[2] += 1f;
                npc.netUpdate = true;
                ai["DayTracker"] = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    DestroyAllProjectiles(npc.whoAmI);
                    TimeContext.Noon.ChangeTime();
                }
            }
        }
    }

    internal void DestroyAllProjectiles(int empress)
    {
        foreach (var projectile in Main.projectile)
        {
            if (projectile.active && projectile.hostile && !projectile.friendly && projectile.damage > 0)
            {
                projectile.Kill();
            }
        }
        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = EBR.Instance.GetPacket();
            packet.Write((byte)EBR.PacketTypes.EmpressClean);
            packet.Write(empress);
            packet.Send();
        }
    }
}
