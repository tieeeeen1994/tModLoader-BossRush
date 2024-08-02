using BossRush;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs;

public class SkeletronAndHands : BossRushBossAndMinions
{
    protected override List<int> ApplicableTypes => [NPCID.SkeletronHead, NPCID.SkeletronHand];

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.SkeletronHead);

    protected override void Update(NPC npc)
    {
        var handTracker = StoreOrFetch("HandTracker", new List<NPC>());
        var handCount = StoreOrFetch("TotalHands", 0);
        if (npc.type == NPCID.SkeletronHand)
        {
            if (npc.active && !handTracker.Exists(hand => hand == npc))
            {
                handTracker.Add(npc);
                ai["TotalHands"] = ++handCount;
            }
        }
        if (npc.type == NPCID.SkeletronHead)
        {
            var defense = StoreOrFetch("HeadDefense", npc.defense);
            var infernoAttack = StoreOrFetch("InfernoAttack", (0, 94));
            var spectreAttack = StoreOrFetch("SpectreAttack", (0, 61));
            handTracker.RemoveAll(hand => !hand.active);
            if (handTracker.Count > 0)
            {
                npc.defense = 10000;
            }
            else
            {
                npc.defense = defense;
            }
            if (handTracker.Count <= Util.RoundOff(handCount * .5f))
            {
                (int timer, int maxTimer) = infernoAttack;
                if (++timer >= maxTimer)
                {
                    ai["InfernoAttack"] = (0, maxTimer);
                    Vector2 velocity = npc.DirectionTo(Main.player[npc.target].Center) * 10f;
                    Projectile.NewProjectile(npc.GetSource_FromAI("Inferno"), npc.Center, velocity,
                                             ProjectileID.InfernoHostileBlast,
                                             Util.RoundOff(npc.damage * .09f), 8f);
                }
                else
                {
                    ai["InfernoAttack"] = (timer, maxTimer);
                }
            }
            if (handTracker.Count <= 0)
            {
                (int timer, int maxTimer) = spectreAttack;
                if (++timer >= maxTimer)
                {
                    ai["SpectreAttack"] = (0, maxTimer);
                    Vector2 velocity = npc.DirectionTo(Main.player[npc.target].Center) * 5f;
                    Projectile.NewProjectile(npc.GetSource_FromAI("Spectre"), npc.Center, velocity,
                                             ProjectileID.LostSoulHostile,
                                             Util.RoundOff(npc.damage * .07f), 0f);
                }
                else
                {
                    ai["SpectreAttack"] = (timer, maxTimer);
                }
            }
            if (npc.ai[1] == 0)
            {
                ai.Remove("SkullAttack");
            }
            else if (handTracker.Count > 0)
            {
                (int timer, int maxTimer) = StoreOrFetch("SkullAttack", (0, 41));
                if (++timer >= maxTimer)
                {
                    ai["SkullAttack"] = (0, maxTimer);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 velocity = (npc.DirectionTo(Main.player[npc.target].Center) + npc.velocity);
                        velocity.Normalize();
                        velocity *= 5f;
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI("Skull"), npc.Center, velocity,
                                                       ProjectileID.Skull, 1, 0f, -1, -1);
                    }
                }
                else
                {
                    ai["SkullAttack"] = (timer, maxTimer);
                }
            }
            var skullTracker = StoreOrFetch("SkullTracker", new Dictionary<int, bool>());
            CleanInactiveData(skullTracker);
        }
    }
}
