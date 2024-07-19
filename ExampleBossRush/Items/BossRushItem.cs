using System;
using System.Collections.Generic;
using System.Linq;
using BossRush;
using BossRush.Types;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleBossRush.Items;

public class BossRushItem : ModItem
{
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Acorn);
        Item.maxStack = 1;
        Item.value = 0;
        Item.rare = ItemRarityID.Purple;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.consumable = false;
        Item.useTime = Item.useAnimation = 60;
        Item.UseSound = SoundID.Roar;
        Item.autoReuse = false;
        Item.createTile = -1;
    }

    public override string Texture => $"Terraria/Images/Item_{ItemID.Acorn}";

    public override bool? UseItem(Player player)
    {
        if (Main.netMode != NetmodeID.Server)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
        }
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            AddBossesToSystem();
            BRS.ToggleBossRush();
        }

        return true;
    }

    private BossRushSystem BRS => ModContent.GetInstance<BossRushSystem>();

    private void AddBossesToSystem()
    {
        //BRS.AddBoss(1, new(
        //   [NPCID.KingSlime],
        //   spawnOffset: (_, _) =>
        //   {
        //       int sign = Util.RandomSign();
        //       return new(1000 * sign, -700, 200 * sign, -200);
        //   },
        //   modifiedAttributes: new(lifeMultiplier: 40, damageMultiplier: 2,
        //                           lifeFlatIncrease: 80, damageFlatIncrease: 30),
        //   update: (npc, ai) =>
        //   {
        //       if (npc.type == NPCID.KingSlime && npc.life < npc.lifeMax * .2f && !ai.ContainsKey("DefenseAdded"))
        //       {
        //           ai["DefenseAdded"] = true;
        //           npc.defense *= 20;
        //       }
        //   }
        //));

        //BRS.AddBoss(2, new(
        //   [NPCID.EyeofCthulhu],
        //   spawnOffset: (_, _) =>
        //   {
        //       int sign = Util.RandomSign();
        //       return new(1000 * sign, 1000, 200 * sign, -2000);
        //   },
        //   timeContext: TimeContext.Night,
        //   modifiedAttributes: new(lifeMultiplier: 90, damageMultiplier: 8,
        //                           lifeFlatIncrease: 80, damageFlatIncrease: 4),
        //   update: (npc, ai) =>
        //   {
        //       if (BRS.CurrentBoss.Contains(npc))
        //       {
        //           if (ai.TryGetValue("BossForcedDamage", out object value))
        //           {
        //               npc.damage = (int)value;
        //           }
        //           else
        //           {
        //               ai["BossForcedDamage"] = npc.damage;
        //           }
        //       }
        //   }
        //));

        //brs.addboss(3, new(
        //    [npcid.eaterofworldshead],
        //    subtypes: [npcid.eaterofworldsbody, npcid.eaterofworldstail, npcid.eaterofworldshead],
        //    spawnoffset: (_, _) => new(-1000, 1000, 2000, 500),
        //    placecontexts: [new(player => player.zonecorrupt = true)],
        //    modifiedattributes: new(lifemultiplier: 200, damagemultiplier: 9, defensemultiplier: 50),
        //    update: (npc, ai) =>
        //    {
        //        if (!ai.trygetvalue("segmentdefensetracker", out object value1))
        //        {
        //            value1 = new dictionary<npc, bool>();
        //            ai["segmentdefensetracker"] = value1;
        //        }
        //        if (npc.type == npcid.eaterofworldsbody &&
        //            npc.active && value1 is dictionary<npc, bool> bodytracker)
        //        {
        //            if (!bodytracker.trygetvalue(npc, out bool isdefended) && !isdefended)
        //            {
        //                npc.defense = 0;
        //                bodytracker[npc] = true;
        //            }
        //        }
        //        if (!ai.trygetvalue("headhealthtracker", out object value2))
        //        {
        //            value2 = new dictionary<npc, bool>();
        //            ai["headhealthtracker"] = value2;
        //        }
        //        if ((npc.type == npcid.eaterofworldshead || npc.type == npcid.eaterofworldstail) &&
        //            npc.active && value2 is dictionary<npc, bool> headtracker)
        //        {
        //            if (!headtracker.trygetvalue(npc, out bool istracked) && !istracked)
        //            {
        //                npc.life = npc.lifemax;
        //                headtracker[npc] = true;
        //            }
        //        }
        //        if (npc == brs.currentboss.find(entity => entity.active) && value1 is dictionary<npc, bool> bodies)
        //        {
        //            foreach (var body in bodies)
        //            {
        //                if (!body.key.active)
        //                {
        //                    if (!ai.trygetvalue("corruptortimers", out object timer))
        //                    {
        //                        timer = new dictionary<npc, (vector2, int)>();
        //                        ai["corruptortimers"] = timer;
        //                    }
        //                    if (timer is dictionary<npc, (vector2, int)> timerdata)
        //                    {
        //                        if (!timerdata.trygetvalue(body.key, out (vector2, int) value))
        //                        {
        //                             timerdata[body.key] = (body.key.center, 1.toframes());
        //                        }
        //                        else if (value.item2 <= 0)
        //                        {
        //                            timerdata.remove(body.key);
        //                            bodies.remove(body.key);
        //                            int mobindex = npc.newnpc(body.key.getsource_fromai("partcutoff"),
        //                                                      util.roundoff(value.item1.x),
        //                                                      util.roundoff(value.item1.y),
        //                                                      npcid.corruptor);
        //                            npc mob = main.npc[mobindex];
        //                            mob.lifemax = util.roundoff(body.key.lifemax * .5f);
        //                            mob.life = mob.lifemax;
        //                            mob.defense = 0;
        //                            mob.damage = body.key.damage;
        //                        }
        //                        else
        //                        {
        //                            timerdata[body.key] = (value.item1, value.item2 - 1);
        //                            for (int i = 0; i < 3; i++)
        //                            {
        //                                dust.newdust(value.item1 - new vector2(15, 15), 30, 30, dustid.demonite);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //));

        //BRS.AddBoss(3 , new(
        //    [NPCID.BrainofCthulhu],
        //    spawnOffset: (_, _) =>
        //    {
        //        int sign = Util.RandomSign();
        //        return new(500 * sign, 500, 200 * sign, -1000);
        //    },
        //    placeContexts: [new(player => player.ZoneCrimson = true)],
        //    modifiedAttributes: new(lifeMultiplier: 50, damageMultiplier: 2.5f,
        //                            lifeFlatIncrease: 300, damageFlatIncrease: 70),
        //    update: (npc, ai) =>
        //    {
        //        if (npc.type == NPCID.Creeper)
        //        {
        //            if (!ai.TryGetValue("Tracker", out object value))
        //            {
        //                value = new Dictionary<int, bool>();
        //                ai["Tracker"] = value;
        //            }
        //            if (value is Dictionary<int, bool> tracker1 && npc.active &&
        //                !tracker1.TryGetValue(npc.whoAmI, out bool isModified) && !isModified)
        //            {
        //                npc.knockBackResist = 0f;
        //                tracker1[npc.whoAmI] = true;
        //            }
        //        }
        //        else if (npc.type == NPCID.BrainofCthulhu)
        //        {
        //            npc.knockBackResist = 0f;
        //            if (ai.TryGetValue("Tracker", out object value) &&
        //                 value is Dictionary<int, bool> tracker2)
        //            {
        //                foreach (var pair in tracker2)
        //                {
        //                    if (!Main.npc[pair.Key].active)
        //                    {
        //                        int mob1 = NPC.NewNPC(npc.GetSource_FromAI("CreeperDied"),
        //                                              Main.npc[pair.Key].Center.X.RoundOff(),
        //                                              Main.npc[pair.Key].Center.Y.RoundOff(),
        //                                              NPCID.IchorSticker);
        //                        tracker2.Remove(pair.Key);
        //                        Main.npc[mob1].lifeMax = Util.RoundOff(Main.npc[mob1].lifeMax * .5f);
        //                        Main.npc[mob1].life = Main.npc[mob1].lifeMax;
        //                        Main.npc[mob1].defense = 0;
        //                        Main.npc[mob1].knockBackResist = 0f;
        //                        Main.npc[mob1].damage = BRS.CurrentBoss.First().damage;
        //                    }
        //                }
        //            }
        //        }
        //        else if(npc.type == NPCID.IchorSticker)
        //        {
        //            npc.velocity += npc.DirectionTo(BRS.CurrentBoss.First().Center) * .25f;
        //        }
        //    }
        //));

        //Continue working on Queen Bee.
        BRS.AddBoss(4, new([NPCID.QueenBee],
                           spawnOffset: (_, _) => new(-1000, -1000, 2000, -200),
                           timeContext: TimeContext.Noon,
                           modifiedAttributes: new(lifeFlatIncrease: 70, lifeMultiplier: 70,
                                                   damageMultiplier: 2, damageFlatIncrease: 35)));

        BRS.AddBoss(5, new(
            [NPCID.SkeletronHead],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 500, 200 * sign, -1000);
            },
            timeContext: TimeContext.Night
        ));

        BRS.AddBoss(6, new(
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

        BRS.AddBoss(7, new(
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
        BRS.AddBoss(8, new(
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

        BRS.AddBoss(8, new([NPCID.TheDestroyer],
                           spawnOffset: (_, _) => new(-1000, 1000, 2000, 500),
                           timeContext: TimeContext.Night));

        BRS.AddBoss(8, new([NPCID.SkeletronPrime],
                           spawnOffset: (_, _) => new(-1000, -800, 2000, -200),
                           timeContext: TimeContext.Night));

        BRS.AddBoss(9, new([NPCID.Plantera],
                           spawnOffset: (_, _) => new(-1000, 1500, 2000, 500),
                           timeContext: TimeContext.Noon));

        BRS.AddBoss(10, new(
            [NPCID.Golem],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(500 * sign, 0, -200 * sign, -500);
            },
            placeContexts: [new(player => player.ZoneLihzhardTemple = true)]
        ));

        BRS.AddBoss(11, new(
            [NPCID.DukeFishron],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(300 * sign, 50, -100 * sign, -50);
            },
            placeContexts: [new(player => player.ZoneBeach = true)]
        ));

        BRS.AddBoss(12, new([NPCID.HallowBoss],
                    spawnOffset: (_, _) => new(-50, -50, 100, -50),
                    timeContext: TimeContext.Night));

        BRS.AddBoss(13, new(
            [NPCID.CultistBoss],
            spawnOffset: (_, _) =>
            {
                int sign = Util.RandomSign();
                return new(300 * sign, 0, -100 * sign, -500);
            }
        ));

        BRS.AddBoss(14, new([NPCID.MoonLordCore]));
    }
}
