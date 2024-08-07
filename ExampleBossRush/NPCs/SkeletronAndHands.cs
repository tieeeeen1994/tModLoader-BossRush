using BossRushAPI;
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
        if (npc.type == NPCID.SkeletronHand && !handTracker.Contains(npc))
        {
            handTracker.Add(npc);
        }
        if (npc.type == NPCID.SkeletronHead)
        {
            StoreOrFetch("OriginalDamage", npc.damage);
            var defense = StoreOrFetch("HeadDefense", npc.defense);
            var infernoAttack = StoreOrFetch("InfernoAttack", (0, 94));
            var spectreAttack = StoreOrFetch("SpectreAttack", (0, 61));
            var skullTracker = StoreOrFetch("SkullTracker", new Dictionary<int, bool>());
            CleanInactiveData(skullTracker);
            handTracker.RemoveAll(hand => !hand.active);
            if (handTracker.Count > 0)
            {
                npc.defense = 10000;
            }
            else
            {
                npc.defense = defense;
            }
            if (handTracker.Count <= 1)
            {
                (int timer, int maxTimer) = infernoAttack;
                if (++timer >= maxTimer)
                {
                    ai["InfernoAttack"] = (0, maxTimer);
                    Vector2 velocity = npc.DirectionTo(Main.player[npc.target].Center) * 10f;
                    Projectile.NewProjectile(npc.GetSource_FromAI("Inferno"), npc.Center, velocity,
                                             ProjectileID.InfernoHostileBlast,
                                             Damage(.11f), 8f);
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
                                             Damage(.09f), 0f);
                }
                else
                {
                    ai["SpectreAttack"] = (timer, maxTimer);
                }
            }
        }
    }
}
