// using System;
// using System.Collections.Generic;
// using System.Linq;
// using BossRush;
// using BossRush.Types;
// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.Audio;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace ExampleBossRush.Items.Old;

// public class BossRushItem : ModItem
// {
//     public override void SetDefaults()
//     {
//         Item.CloneDefaults(ItemID.Acorn);
//         Item.maxStack = 1;
//         Item.value = 0;
//         Item.rare = ItemRarityID.Purple;
//         Item.useStyle = ItemUseStyleID.HoldUp;
//         Item.consumable = false;
//         Item.useTime = Item.useAnimation = 60;
//         Item.UseSound = SoundID.Roar;
//         Item.autoReuse = false;
//         Item.createTile = -1;
//     }

//     public override string Texture => $"Terraria/Images/Item_{ItemID.Acorn}";

//     public override bool? UseItem(Player player)
//     {
//         if (Main.netMode != NetmodeID.Server)
//         {
//             SoundEngine.PlaySound(SoundID.Roar, player.Center);
//         }
//         if (Main.netMode != NetmodeID.MultiplayerClient)
//         {
//             AddBossesToSystem();
//             BRS.ToggleBossRush();
//         }

//         return true;
//     }

//     private BossRushSystem BRS => ModContent.GetInstance<BossRushSystem>();

//     private void AddBossesToSystem()
//     {
//         # region Brain of Cthulhu
//         BRS.AddBoss(3, new(
//             [NPCID.BrainofCthulhu],
//             spawnOffset: (_, _) =>
//             {
//                 int sign = Util.RandomSign();
//                 return new(500 * sign, 500, 200 * sign, -1000);
//             },
//             placeContexts: [new(player => player.ZoneCrimson = true)],
//             modifiedAttributes: new(lifeMultiplier: 45, damageMultiplier: 2,
//                                     lifeFlatIncrease: 300, damageFlatIncrease: 70),
//             bossUpdate: (npc, ai) =>
//             {
//                 if (npc.type == NPCID.Creeper)
//                 {
//                     if (!ai.TryGetValue("Tracker", out object value))
//                     {
//                         value = new Dictionary<int, bool>();
//                         ai["Tracker"] = value;
//                     }
//                     if (value is Dictionary<int, bool> tracker1 && npc.active &&
//                         !tracker1.TryGetValue(npc.whoAmI, out bool isModified) && !isModified)
//                     {
//                         npc.knockBackResist = 0f;
//                         tracker1[npc.whoAmI] = true;
//                     }
//                 }
//                 else if (npc.type == NPCID.BrainofCthulhu)
//                 {
//                     npc.knockBackResist = 0f;
//                     if (ai.TryGetValue("Tracker", out object value) &&
//                         value is Dictionary<int, bool> tracker2)
//                     {
//                         foreach (var pair in tracker2)
//                         {
//                             if (!Main.npc[pair.Key].active)
//                             {
//                                 int mobIndex = NPC.NewNPC(npc.GetSource_FromAI("CreeperDied"),
//                                                         Main.npc[pair.Key].Center.X.RoundOff(),
//                                                         Main.npc[pair.Key].Center.Y.RoundOff(),
//                                                         NPCID.IchorSticker);

//                                 tracker2.Remove(pair.Key);
//                                 Main.npc[mobIndex].lifeMax = Util.RoundOff(Main.npc[mobIndex].lifeMax * .5f);
//                                 Main.npc[mobIndex].life = Main.npc[mobIndex].lifeMax;
//                                 Main.npc[mobIndex].defense = 0;
//                                 Main.npc[mobIndex].damage = BRS.ReferenceBoss.damage;
//                             }
//                         }
//                     }
//                 }
//                 else if (npc.type == NPCID.IchorSticker)
//                 {
//                     npc.velocity += npc.DirectionTo(BRS.CurrentBoss.First().Center) * .25f;
//                 }
//             },
//             projectileUpdate: (projectile, ai) =>
//             {
//                 if (projectile.type == ProjectileID.GoldenShowerHostile)
//                 {
//                     projectile.velocity.Y = projectile.oldVelocity.Y;
//                 }
//             }
//         ));
//         # endregion

//         # region Queen Bee
//         BRS.AddBoss(4, new(
//             [NPCID.QueenBee],
//             spawnOffset: (_, _) => new(-1000, -1000, 2000, -200),
//             timeContext: TimeContext.Noon,
//             modifiedAttributes: new(lifeFlatIncrease: 200, lifeMultiplier: 30,
//                                     damageFlatIncrease: 50, damageMultiplier: 1.5f),
//             bossUpdate: (npc, ai) =>
//             {
//                 if (npc.type == NPCID.Bee)
//                 {
//                     npc.knockBackResist = 0f;
//                     npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * .2f;
//                 }
//                 else if (npc.type == NPCID.BeeSmall)
//                 {
//                     npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * .5f;
//                 }
//             },
//             projectileUpdate: (projectile, ai) =>
//             {
//                 if (projectile.type == ProjectileID.QueenBeeStinger)
//                 {
//                     projectile.damage = Util.RoundOff(BRS.ReferenceBoss.damage * .1f);
//                 }
//             },
//             placeContexts: [new(player => player.ZoneJungle = true)]
//         ));
//         # endregion

//         # region Skeletron
//         BRS.AddBoss(5, new(
//             [NPCID.SkeletronHead],
//             spawnOffset: (_, _) =>
//             {
//                 int sign = Util.RandomSign();
//                 return new(500 * sign, 500, 200 * sign, -1000);
//             },
//             timeContext: TimeContext.Night,
//             modifiedAttributes: new(lifeFlatIncrease: 500, lifeMultiplier: 40,
//                                     damageMultiplier: 2, damageFlatIncrease: 40),
//             bossUpdate: (npc, ai) =>
//             {
//                 if (!ai.TryGetValue("HandTracker", out object value))
//                 {
//                     value = new List<NPC>();
//                     ai["HandTracker"] = value;
//                 }
//                 if (!ai.TryGetValue("TotalHands", out object number))
//                 {
//                     ai["TotalHands"] = number = 0;
//                 }
//                 List<NPC> handTracker = (List<NPC>)value;
//                 int handCount = (int)number;
//                 if (npc.type == NPCID.SkeletronHand)
//                 {
//                     if (npc.active && !handTracker.Exists(hand => hand == npc))
//                     {
//                         handTracker.Add(npc);
//                         ai["TotalHands"] = handCount + 1;
//                     }
//                 }
//                 if (npc.type == NPCID.SkeletronHead)
//                 {
//                     if (!ai.TryGetValue("HeadDefense", out object defense))
//                     {
//                         ai["HeadDefense"] = defense = npc.defense;
//                     }
//                     if (!ai.TryGetValue("InfernoAttack", out object attack1))
//                     {
//                         attack1 = new Tuple<int, int>(0, 94);
//                         ai["InfernoAttack"] = attack1;
//                     }
//                     if (!ai.TryGetValue("SpectreAttack", out object attack2))
//                     {
//                         attack2 = new Tuple<int, int>(0, 61);
//                         ai["SpectreAttack"] = attack2;
//                     }
//                     handTracker.RemoveAll(hand => !hand.active);
//                     if (handTracker.Count > 0)
//                     {
//                         npc.defense = 10000;
//                     }
//                     else
//                     {
//                         npc.defense = (int)defense;
//                     }
//                     if (handTracker.Count <= Util.RoundOff(handCount * .5f) &&
//                         attack1 is Tuple<int, int> infernoTimer)
//                     {
//                         int timer = infernoTimer.Item1 + 1;
//                         if (timer >= infernoTimer.Item2)
//                         {
//                             ai["InfernoAttack"] = new Tuple<int, int>(0, infernoTimer.Item2);
//                             Vector2 velocity = npc.DirectionTo(Main.player[npc.target].Center) * 10f;
//                             Projectile.NewProjectile(npc.GetSource_FromAI("Inferno"), npc.Center, velocity,
//                                                      ProjectileID.InfernoHostileBlast,
//                                                      Util.RoundOff(npc.damage * .08f), 0f);
//                         }
//                         else
//                         {
//                             ai["InfernoAttack"] = new Tuple<int, int>(timer, infernoTimer.Item2);
//                         }
//                     }
//                     if (handTracker.Count <= 0 &&
//                         attack2 is Tuple<int, int> spectreTimer)
//                     {
//                         int timer = spectreTimer.Item1 + 1;
//                         if (timer >= spectreTimer.Item2)
//                         {
//                             ai["SpectreAttack"] = new Tuple<int, int>(0, spectreTimer.Item2);
//                             Vector2 velocity = npc.DirectionTo(Main.player[npc.target].Center) * 5f;
//                             Projectile.NewProjectile(npc.GetSource_FromAI("Spectre"), npc.Center, velocity,
//                                                      ProjectileID.LostSoulHostile,
//                                                      Util.RoundOff(npc.damage * .08f), 0f);
//                         }
//                         else
//                         {
//                             ai["SpectreAttack"] = new Tuple<int, int>(timer, spectreTimer.Item2);
//                         }
//                     }
//                 }
//             },
//             projectileUpdate: (projectile, ai) =>
//             {
//                 if (projectile.type == ProjectileID.Skull)
//                 {
//                     projectile.damage = Util.RoundOff(BRS.ReferenceBoss.damage * .08f);
//                 }
//                 if (projectile.type == ProjectileID.InfernoHostileBlast)
//                 {
//                     projectile.velocity -= Vector2.Zero.DirectionTo(projectile.velocity) * .2f;
//                 }
//                 if (projectile.type == ProjectileID.LostSoulHostile)
//                 {
//                     Player target = null;
//                     float distance = float.MaxValue;
//                     foreach (var player in Main.ActivePlayers)
//                     {
//                         float newDistance = projectile.Distance(player.Center);
//                         if (newDistance <= 12 * 16 && (target == null || newDistance < distance))
//                         {
//                             target = player;
//                             distance = newDistance;
//                         }
//                     }
//                     if (target != null)
//                     {
//                         projectile.velocity += projectile.DirectionTo(target.Center) * .1f;
//                     }
//                 }
//             }
//         ));
//         # endregion

//         # region Wall of Flesh
//         BRS.AddBoss(6, new(
//             [NPCID.WallofFlesh],
//             placeContexts: [PlaceContext.LeftUnderworld, PlaceContext.RightUnderworld],
//             spawnOffset: (_, bossData) =>
//             {
//                 if (bossData.PlaceContext.Value.InitialPosition.Value.X < Main.maxTilesX / 2)
//                 {
//                     return new(-1000, 0, 0, 0);
//                 }
//                 else
//                 {
//                     return new(1000, 0, 0, 0);
//                 }
//             },
//             modifiedAttributes: new(lifeMultiplier: 150, damageMultiplier: 1.5f, damageFlatIncrease: 50),
//             bossUpdate: (npc, ai) =>
//             {
//                 if (npc.type == NPCID.TheHungry || npc.type == NPCID.TheHungryII)
//                 {
//                     npc.knockBackResist = 0f;
//                 }
//             },
//             projectileUpdate: (projectile, ai) =>
//             {
//                 if (projectile.type == ProjectileID.EyeLaser)
//                 {
//                     projectile.damage = Util.RoundOff(BRS.ReferenceBoss.damage * .05f);
//                 }
//             }
//         ));
//         # endregion

//         # region Queen Slime
//         BRS.AddBoss(7, new(
//             [NPCID.QueenSlimeBoss],
//             spawnOffset: (_, _) =>
//             {
//                 int sign = Util.RandomSign();
//                 return new(1000 * sign, -500, -100 * sign, -100);
//             },
//             timeContext: TimeContext.Noon,
//             placeContexts: [new((player) =>
//             {
//                 player.FindSpawn();
//                 int spawnX = player.SpawnX != -1 ? player.SpawnX : Main.spawnTileX;
//                 int spawnY = player.SpawnY != -1 ? player.SpawnY : Main.spawnTileY;
//                 Vector2 worldCoordinates = new Vector2(spawnX, spawnY).ToWorldCoordinates();
//                 worldCoordinates -= new Vector2(player.width / 2, player.height);
//                 worldCoordinates = Util.RoundOff(worldCoordinates);
//                 return new((int)worldCoordinates.X, (int)worldCoordinates.Y, 0, 0);
//             })]
//         ));
//         # endregion

//         int twinsSign = Util.RandomSign();
//         # region The Twins
//         BRS.AddBoss(8, new(
//             [NPCID.Retinazer, NPCID.Spazmatism],
//             spawnOffset: (type, _) =>
//                 {
//                     if (type == NPCID.Retinazer)
//                     {
//                         return new(1000 * twinsSign, 1000, 200 * twinsSign, -2000);
//                     }
//                     else
//                     {
//                         return new(-1000 * twinsSign, 1000, -200 * twinsSign, -2000);
//                     }
//                 },
//                 timeContext: TimeContext.Night
//         ));
//         # endregion

//         # region The Destroyer
//         BRS.AddBoss(8, new([NPCID.TheDestroyer],
//                            spawnOffset: (_, _) => new(-1000, 1000, 2000, 500),
//                            timeContext: TimeContext.Night));
//         # endregion

//         # region Skeletron Prime
//         BRS.AddBoss(8, new([NPCID.SkeletronPrime],
//                            spawnOffset: (_, _) => new(-1000, -800, 2000, -200),
//                            timeContext: TimeContext.Night));
//         # endregion

//         # region Plantera
//         BRS.AddBoss(9, new([NPCID.Plantera],
//                            spawnOffset: (_, _) => new(-1000, 1500, 2000, 500),
//                            timeContext: TimeContext.Noon));
//         # endregion

//         # region Golem
//         BRS.AddBoss(10, new(
//             [NPCID.Golem],
//             spawnOffset: (_, _) =>
//             {
//                 int sign = Util.RandomSign();
//                 return new(500 * sign, 0, -200 * sign, -500);
//             },
//             placeContexts: [new(player => player.ZoneLihzhardTemple = true)]
//         ));
//         # endregion

//         # region Duke Fishron
//         BRS.AddBoss(11, new(
//             [NPCID.DukeFishron],
//             spawnOffset: (_, _) =>
//             {
//                 int sign = Util.RandomSign();
//                 return new(300 * sign, 50, -100 * sign, -50);
//             },
//             placeContexts: [new(player => player.ZoneBeach = true)]
//         ));
//         # endregion

//         # region Empress of Light
//         BRS.AddBoss(12, new([NPCID.HallowBoss],
//                     spawnOffset: (_, _) => new(-50, -50, 100, -50),
//                     timeContext: TimeContext.Night));
//         # endregion

//         # region Lunatic Cultist
//         BRS.AddBoss(13, new(
//             [NPCID.CultistBoss],
//             spawnOffset: (_, _) =>
//             {
//                 int sign = Util.RandomSign();
//                 return new(300 * sign, 0, -100 * sign, -500);
//             }
//         ));
//         # endregion

//         # region Moon Lord
//         BRS.AddBoss(14, new([NPCID.MoonLordCore]));
//         # endregion
//     }
// }
