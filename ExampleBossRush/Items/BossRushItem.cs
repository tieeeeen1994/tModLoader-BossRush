using BossRushAPI;
using BossRushAPI.Types;
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
        Item.value = 0;
        Item.rare = ItemRarityID.Purple;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = Item.useAnimation = 60;
        Item.UseSound = SoundID.Roar;
        Item.autoReuse = false;
        Item.createTile = -1;
    }

    public override string Texture => nameof(ExampleBossRush) + "/Textures/" + nameof(BossRushItem);

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

    public override void AddRecipes()
    {
        var recipe = CreateRecipe(2);
        recipe.AddIngredient(ItemID.Acorn, 20);
        recipe.AddIngredient(ItemID.CelestialSigil);
        recipe.AddTile(TileID.LunarCraftingStation);
        recipe.Register();
    }

    private BossRushSystem BRS => ModContent.GetInstance<BossRushSystem>();

    private void AddBossesToSystem()
    {
        #region King Slime
        int sign = Util.RandomSign();
        BRS.AddBoss(0, new(
            [NPCID.KingSlime],
            spawnOffset: new(1000 * sign, -700, 200 * sign, -200),
            modifiedAttributes: new(lifeMultiplier: 35, damageMultiplier: 1.5f,
                                    lifeFlatIncrease: 75, damageFlatIncrease: 30),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion King Slime

        #region Eye of Cthulhu
        sign = Util.RandomSign();
        BRS.AddBoss(1, new(
            [NPCID.EyeofCthulhu],
            spawnOffset: new(1000 * sign, 1000, 200 * sign, -2000),
            timeContext: TimeContext.Night,
            modifiedAttributes: new(lifeMultiplier: 70, damageMultiplier: 6,
                                    lifeFlatIncrease: 80, damageFlatIncrease: 4),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion Eye of Cthulhu

        #region Deerclops
        sign = Util.RandomSign();
        BRS.AddBoss(2, new(
            [NPCID.Deerclops],
            spawnOffset: new(500 * sign, 0, -200 * sign, -500),
            modifiedAttributes: new(lifeMultiplier: 15, damageFlatIncrease: 50,
                                    damageMultiplier: 1.2f, projectilesAffected: true),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion Deerclops

        # region Eater of Worlds
        BRS.AddBoss(3, new(
            [NPCID.EaterofWorldsHead],
            subTypes: [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail],
            spawnOffset: new(-1000, 1000, 2000, 500),
            modifiedAttributes: new(damageMultiplier: 1.3f, damageFlatIncrease: 50,
                                    lifeMultiplier: 100, defenseMultiplier: 2,
                                    defenseFlatIncrease: 30),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Brain of Cthulhu
        sign = Util.RandomSign();
        BRS.AddBoss(3, new(
            [NPCID.BrainofCthulhu],
            spawnOffset: new(500 * sign, 500, 200 * sign, -1000),
            modifiedAttributes: new(lifeMultiplier: 30, lifeFlatIncrease: 300, damageFlatIncrease: 50,
                                    damageMultiplier: 1.3f),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        # endregion

        #region Queen Bee
        BRS.AddBoss(4, new(
            [NPCID.QueenBee],
            spawnOffset: new(-1000, -1000, 2000, -200),
            timeContext: TimeContext.Noon,
            modifiedAttributes: new(lifeFlatIncrease: 100, lifeMultiplier: 27,
                                    damageFlatIncrease: 50),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Skeletron
        BRS.AddBoss(5, new(
            [NPCID.SkeletronHead],
            spawnOffset: new(-1000, -700, 2000, -300),
            timeContext: TimeContext.Night,
            modifiedAttributes: new(lifeFlatIncrease: 500, lifeMultiplier: 15,
                                    damageFlatIncrease: 50, damageMultiplier: 1.1f),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Wall of Flesh
        bool result = Main.rand.NextBool();
        PlaceContext chooseSide = result ? PlaceContext.LeftUnderworld : PlaceContext.RightUnderworld;
        Rectangle offsetSide = result ? new(-1000, 0, 0, 0) : new(1000, 0, 0, 0);
        BRS.AddBoss(6, new(
            [NPCID.WallofFlesh],
            placeContext: chooseSide,
            spawnOffset: offsetSide,
            modifiedAttributes: new(lifeMultiplier: 30, lifeFlatIncrease: 70, damageFlatIncrease: 30),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Queen Slime
        sign = Util.RandomSign();
        BRS.AddBoss(7, new(
            [NPCID.QueenSlimeBoss],
            spawnOffset: new(1000 * sign, -500, -100 * sign, -100),
            timeContext: TimeContext.Noon,
            modifiedAttributes: new(lifeMultiplier: 7, damageFlatIncrease: 20, damageMultiplier: 1.2f,
                                    lifeFlatIncrease: 800, projectilesAffected: true),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region The Twins
        BRS.AddBoss(8, new(
            [NPCID.Retinazer, NPCID.Spazmatism],
            spawnOffset: new(1000, -700, -2000, -300),
            timeContext: TimeContext.Night,
            modifiedAttributes: new(lifeMultiplier: 5, damageMultiplier: 1.1f, damageFlatIncrease: 50),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        # region The Destroyer
        BRS.AddBoss(8, new(
            [NPCID.TheDestroyer],
            spawnOffset: new(-1000, 1000, 2000, 500),
            timeContext: TimeContext.Night,
            modifiedAttributes: new(lifeMultiplier: 8, damageFlatIncrease: 30,
                                    lifeFlatIncrease: 10),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        # endregion

        # region Skeletron Prime
        BRS.AddBoss(8, new(
            [NPCID.SkeletronPrime],
            spawnOffset: new(-1000, -700, 2000, -300),
            timeContext: TimeContext.Night,
            modifiedAttributes: new(lifeMultiplier: 4, defenseFlatIncrease: 5,
                                    damageFlatIncrease: 25, damageMultiplier: 1.2f),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        # endregion

        # region Plantera
        BRS.AddBoss(9, new(
            [NPCID.Plantera],
            spawnOffset: new(-1000, 1500, 2000, 500),
            timeContext: TimeContext.Noon,
            modifiedAttributes: new(lifeMultiplier: 3f, damageMultiplier: .95f, projectilesAffected: true),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Golem
        sign = Util.RandomSign();
        ModifiedAttributes golemModifiedAttributes =
            Main.getGoodWorld ? new(lifeMultiplier: 1.5f, damageFlatIncrease: 15) :
                                new(lifeMultiplier: 4, damageFlatIncrease: 50); ;
        BRS.AddBoss(10, new(
            [NPCID.Golem],
            spawnOffset: new(700 * sign, -400, -200 * sign, -10),
            modifiedAttributes: golemModifiedAttributes,
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Duke Fishron
        sign = Util.RandomSign();
        chooseSide = Main.rand.NextBool() ? PlaceContext.LeftOcean : PlaceContext.RightOcean;
        BRS.AddBoss(11, new(
            [NPCID.DukeFishron],
            spawnOffset: new(900 * sign, 0, -200 * sign, -10),
            placeContext: chooseSide,
            modifiedAttributes: new(lifeMultiplier: 3, damageMultiplier: 1.05f, lifeFlatIncrease: 130),
            spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Empress of Light
        BRS.AddBoss(12, new([NPCID.HallowBoss],
                    spawnOffset: new(0, -300, 0, 0),
                    timeContext: TimeContext.Night,
                    modifiedAttributes: new(lifeMultiplier: 4, damageMultiplier: 1.05f),
                    spawnAttributes: SpawnAttributes.NoSpawns
        ));
        #endregion

        #region Lunatic Cultist
        BRS.AddBoss(13, new(
            [NPCID.CultistBoss],
            spawnOffset: new(75, 20, 0, 0),
            spawnAttributes: SpawnAttributes.NoSpawns,
            modifiedAttributes: new(lifeMultiplier: 3, damageFlatIncrease: 50,
                                    lifeFlatIncrease: 10000)
        ));
        #endregion

        #region Pillars
        BRS.AddBoss(14, new(
            [NPCID.LunarTowerSolar],
            spawnOffset: new(0, -500, 0, 0),
            spawnAttributes: SpawnAttributes.DoubleSpawns,
            modifiedAttributes: new(lifeMultiplier: 2, damageMultiplier: .95f),
            defeatMessage: Message.Vanilla("Mods.ExampleBossRush.DeathMessages.SolarPillar")
        ));

        BRS.AddBoss(14, new(
            [NPCID.LunarTowerNebula],
            spawnOffset: new(0, -500, 0, 0),
            spawnAttributes: SpawnAttributes.DoubleSpawns,
            modifiedAttributes: new(lifeMultiplier: 2, damageMultiplier: .95f),
            defeatMessage: Message.Vanilla("Mods.ExampleBossRush.DeathMessages.NebulaPillar")
        ));

        BRS.AddBoss(14, new(
            [NPCID.LunarTowerStardust],
            spawnOffset: new(0, -500, 0, 0),
            spawnAttributes: SpawnAttributes.DoubleSpawns,
            modifiedAttributes: new(lifeMultiplier: 2, damageMultiplier: .95f),
            defeatMessage: Message.Vanilla("Mods.ExampleBossRush.DeathMessages.StardustPillar")
        ));

        BRS.AddBoss(14, new(
            [NPCID.LunarTowerVortex],
            spawnOffset: new(0, -500, 0, 0),
            spawnAttributes: SpawnAttributes.DoubleSpawns,
            modifiedAttributes: new(lifeMultiplier: 2, damageMultiplier: .95f),
            defeatMessage: Message.Vanilla("Mods.ExampleBossRush.DeathMessages.VortexPillar")
        ));
        #endregion

        #region Moon Lord
        BRS.AddBoss(15, new(
            [NPCID.MoonLordCore],
            spawnAttributes: SpawnAttributes.NoSpawns,
            modifiedAttributes: new(lifeMultiplier: 5f, damageMultiplier: 1.5f, projectilesAffected: true)
        ));
        #endregion
    }
}
