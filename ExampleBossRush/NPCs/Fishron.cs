using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs;

public class Fishron : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [
        NPCID.DukeFishron,
        NPCID.Sharkron2,
        NPCID.Sharkron,
        NPCID.DetonatingBubble
    ];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.DukeFishron);

    protected override void Update(NPC npc)
    {
        var ragingFish = StoreOrFetch("RagingFish", false);
        var sharkTimer = StoreOrFetch("SharkTimer", 1f.ToFrames());
        if (npc.type == NPCID.DukeFishron)
        {
            if (npc.ai[0] == 5f && npc.life <= npc.lifeMax * 0.25f)
            {
                npc.ai[0] = 9f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.netUpdate = true;
                ai["RagingFish"] = true;
            }
            else if (npc.ai[0] == 0 && npc.life <= npc.lifeMax * 0.6f)
            {
                npc.ai[0] = 4f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.netUpdate = true;
            }
            if (ragingFish)
            {
                if (--sharkTimer <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int chooseAttack = Main.rand.Next(100);
                    ai["SharkTimer"] = 1f.ToFrames();
                    if (chooseAttack < 10)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI("RandomNado"),
                                                 npc.Center, Vector2.Zero, ProjectileID.SharknadoBolt, 0, 0,
                                                 Main.myPlayer, 1f, npc.target + 1, Main.rand.NextBool() ? 1 : 0);
                    }
                    else
                    {
                        var randomOffset = Main.rand.NextVector2Circular(300, 300);
                        Vector2 position = npc.Center + randomOffset;
                        NPC.NewNPC(npc.GetSource_FromAI("RandomShork"), position.X.RoundOff(),
                                   position.Y.RoundOff(), NPCID.Sharkron);
                    }
                }
                else
                {
                    ai["SharkTimer"] = sharkTimer;
                }
            }
        }
        else if (npc.type == NPCID.Sharkron || npc.type == NPCID.Sharkron2)
        {
            npc.velocity *= 1.015f;
            npc.velocity = npc.velocity.Clamp(30f);
        }
        else if (npc.type == NPCID.DetonatingBubble)
        {
            npc.velocity *= 1.05f;
            npc.velocity = npc.velocity.Clamp(30f);
        }
    }
}
