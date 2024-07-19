using System;
using BossRush;
using BossRush.Types;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ExampleBossRush;

public class ExampleBossRushSystem : BossRushSystem
{
    protected override void InitializeSystem()
    {
        // AddBoss(new(
        //    [NPCID.KingSlime],
        //    spawnOffset: (_, _) =>
        //    {
        //        int sign = Util.RandomSign();
        //        return new(1000 * sign, -700, 200 * sign, -200);
        //    },
        //    modifiedAttributes: new(lifeMultiplier: 50, damageMultiplier: 2,
        //                            lifeFlatIncrease: 80, damageFlatIncrease: 30),
        //    update: (npc, ai) =>
        //    {
        //        if (npc.type == NPCID.KingSlime && npc.life < npc.lifeMax * .2f && !ai.ContainsKey("DefenseAdded"))
        //        {
        //            ai["DefenseAdded"] = true;
        //            npc.defense *= 20;
        //        }
        //    }
        // ));

        // AddBoss(new(
        //    [NPCID.EyeofCthulhu],
        //    spawnOffset: (_, _) =>
        //    {
        //        int sign = Util.RandomSign();
        //        return new(1000 * sign, 1000, 200 * sign, -2000);
        //    },
        //    timeContext: TimeContext.Night,
        //    modifiedAttributes: new(lifeMultiplier: 100, damageMultiplier: 8,
        //                            lifeFlatIncrease: 80, damageFlatIncrease: 4),
        //    update: (npc, ai) =>
        //    {
        //        if (CurrentBoss.Contains(npc))
        //        {
        //            if (ai.TryGetValue("BossForcedDamage", out object value))
        //            {
        //                npc.damage = (int)value;
        //            }
        //            else
        //            {
        //                ai["BossForcedDamage"] = npc.damage;
        //            }
        //        }
        //    }
        // ));

        // AddBoss(new(
        //     [NPCID.EaterofWorldsHead],
        //     subTypes: [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.EaterofWorldsHead],
        //     spawnOffset: (_, _) => new(-1000, 1000, 2000, 500),
        //     placeContexts: [new(player => player.ZoneCorrupt = true)],
        //     modifiedAttributes: new(lifeMultiplier: 200, damageMultiplier: 9, defenseMultiplier: 50),
        //     update: (npc, ai) =>
        //     {
        //         if (!ai.TryGetValue("SegmentDefenseTracker", out object value1))
        //         {
        //             value1 = new Dictionary<int, bool>();
        //             ai["SegmentDefenseTracker"] = value1;
        //         }
        //         if (npc.type == NPCID.EaterofWorldsBody &&
        //             npc.active && value1 is Dictionary<int, bool> bodyTracker)
        //         {
        //             if (!bodyTracker.TryGetValue(npc.whoAmI, out bool isDefended) && !isDefended)
        //             {
        //                 npc.defense = 0;
        //                 bodyTracker[npc.whoAmI] = true;
        //             }
        //         }
        //         if (!ai.TryGetValue("HeadHealthTracker", out object value2))
        //         {
        //             value2 = new Dictionary<int, bool>();
        //             ai["HeadHealthTracker"] = value2;
        //         }
        //         if ((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsTail) &&
        //             npc.active && value2 is Dictionary<int, bool> headTracker)
        //         {
        //             if (!headTracker.TryGetValue(npc.whoAmI, out bool isTracked) && !isTracked)
        //             {
        //                 npc.life = npc.lifeMax;
        //                 headTracker[npc.whoAmI] = true;
        //             }
        //         }
        //     }
        // ));

        AddBoss(new(
            [NPCID.BrainofCthulhu],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 500, 200 * sign, -1000);
            },
            placeContexts: [new(player => player.ZoneCrimson = true)],
            modifiedAttributes: new(lifeMultiplier: 100, damageMultiplier: 30, lifeFlatIncrease: 100)
        ));

        Action<Player> forceJungle = (player) =>
        {
            player.ZoneJungle = true;
            player.ZoneDirtLayerHeight = true;
            player.ZoneRockLayerHeight = true;
        };
        AddBoss(new([NPCID.QueenBee],
                              spawnOffset: (_, _) => new(-1000, -1000, 2000, -200),
                              timeContext: TimeContext.Noon,
                              placeContexts: [new(forceJungle)]));

        AddBoss(new(
            [NPCID.SkeletronHead],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 500, 200 * sign, -1000);
            },
            timeContext: TimeContext.Night
        ));

        AddBoss(new(
            [NPCID.WallofFlesh],
            placeContexts: [PlaceContext.LeftUnderworld, PlaceContext.RightUnderworld],
            spawnOffset: (_, bossData) =>
            {
                if (bossData.PlaceContext.Value.InitialPosition.Value.X < Main.maxTilesX / 2)
                {
                    return new(-500, 0, 0, 0);
                }
                else
                {
                    return new(500, 0, 0, 0);
                }
            }
        ));

        AddBoss(new(
            [NPCID.QueenSlimeBoss],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(1000 * sign, -500, -100 * sign, -100);
            },
            timeContext: TimeContext.Noon,
            placeContexts: [new((player) =>
            {
                Vector2 worldCoordinates = new Vector2(Main.spawnTileX, Main.spawnTileY).ToWorldCoordinates();
                worldCoordinates -= new Vector2(player.width / 2, player.height);
                worldCoordinates = Util.RoundOff(worldCoordinates);
                return new((int)worldCoordinates.X, (int)worldCoordinates.Y, 0, 0);
            })]
        ));

        int twinsSign = Util.RandomSign();
        AddBoss(new(
            [NPCID.Retinazer, NPCID.Spazmatism],
            spawnOffset: (type, _) =>
                {
                    if (type == NPCID.Retinazer)
                    {
                        return new(1000 * twinsSign, 1000, 200 * twinsSign, -2000);
                    }
                    else
                    {
                        return new(-1000 * twinsSign, 1000, -200 * twinsSign, -2000);
                    }
                },
                timeContext: TimeContext.Night
            ));

        AddBoss(new([NPCID.TheDestroyer],
                              spawnOffset: (_, _) => new(-1000, 1000, 2000, 500),
                              timeContext: TimeContext.Night));

        AddBoss(new([NPCID.SkeletronPrime],
                              spawnOffset: (_, _) => new(-1000, -800, 2000, -200),
                              timeContext: TimeContext.Night));

        AddBoss(new([NPCID.Plantera],
                              spawnOffset: (_, _) => new(-1000, 1500, 2000, 500),
                              timeContext: TimeContext.Noon,
                              placeContexts: [new(forceJungle)]));

        AddBoss(new(
            [NPCID.Golem],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 0, -200 * sign, -500);
            },
            placeContexts: [new(player => player.ZoneLihzhardTemple = true)]
        ));

        AddBoss(new(
            [NPCID.DukeFishron],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(300 * sign, 50, -100 * sign, -50);
            },
            placeContexts: [new(player => player.ZoneBeach = true)]
        ));

        AddBoss(new([NPCID.HallowBoss],
                              spawnOffset: (_, _) => new(-50, -50, 100, -50),
                              timeContext: TimeContext.Night));

        AddBoss(new(
            [NPCID.CultistBoss],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(300 * sign, 0, -100 * sign, -500);
            }
        ));

        AddBoss(new([NPCID.MoonLordCore])); ;
    }
}
