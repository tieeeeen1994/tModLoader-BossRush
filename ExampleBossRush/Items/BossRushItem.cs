using BossRush;
using BossRush.Types;
using Microsoft.Xna.Framework;
using System.Linq;
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
        int sign = Util.RandomSign();
        # region King Slime
        BRS.AddBoss(0, new(
            [NPCID.KingSlime],
            spawnOffset: new(1000 * sign, -700, 200 * sign, -200),
            modifiedAttributes: new(lifeMultiplier: 40, damageMultiplier: 2,
                                    lifeFlatIncrease: 80, damageFlatIncrease: 30)
        ));
        # endregion

        sign = Util.RandomSign();
        # region Deerclops
        BRS.AddBoss(1, new(
            [NPCID.Deerclops],
            spawnOffset: new(500 * sign, 0, -200 * sign, -500),
            modifiedAttributes: new(lifeMultiplier: 20, damageMultiplier: 2)
        ));
        # endregion

        sign = Util.RandomSign();
        # region Eye of Cthulhu
        BRS.AddBoss(2, new(
            [NPCID.EyeofCthulhu],
            spawnOffset: new(1000 * sign, 1000, 200 * sign, -2000),
            timeContext: TimeContext.Night,
            modifiedAttributes: new(lifeMultiplier: 90, damageMultiplier: 8,
                                    lifeFlatIncrease: 80, damageFlatIncrease: 4)
        ));
        #endregion

        # region Eater of Worlds
        BRS.AddBoss(3, new(
            [NPCID.EaterofWorldsHead],
            subTypes: [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail],
            spawnOffset: new(-1000, 1000, 2000, 500),
            modifiedAttributes: new(damageMultiplier: 2, damageFlatIncrease: 70,
                                    lifeMultiplier: 200, defenseMultiplier: 50)
        ));
        # endregion

        sign = Util.RandomSign();
        # region Brain of Cthulhu
        BRS.AddBoss(3, new(
            [NPCID.BrainofCthulhu],
            spawnOffset: new(500 * sign, 500, 200 * sign, -1000),
            modifiedAttributes: new(lifeMultiplier: 45, damageMultiplier: 2,
                                    lifeFlatIncrease: 300, damageFlatIncrease: 70)
        ));
        # endregion

        # region Queen Bee
        BRS.AddBoss(4, new(
            [NPCID.QueenBee],
            spawnOffset: new(-1000, -1000, 2000, -200),
            timeContext: TimeContext.Noon,
            modifiedAttributes: new(lifeFlatIncrease: 200, lifeMultiplier: 30,
                                    damageFlatIncrease: 50, damageMultiplier: 1.5f)
        ));
        # endregion

        sign = Util.RandomSign();
        # region Skeletron
        BRS.AddBoss(-5, new(
            [NPCID.SkeletronHead],
            spawnOffset: new(500 * sign, 500, 200 * sign, -1000),
            timeContext: TimeContext.Night,
            modifiedAttributes: new(lifeFlatIncrease: 500, lifeMultiplier: 40,
                                    damageMultiplier: 2, damageFlatIncrease: 40)
        ));
        # endregion

        bool result = Main.rand.NextBool();
        PlaceContext chooseSide = result ? PlaceContext.LeftUnderworld : PlaceContext.RightUnderworld;
        Rectangle offsetSide = result ? new(-1000, 0, 0, 0) : new(1000, 0, 0, 0);
        # region Wall of Flesh
        BRS.AddBoss(6, new(
            [NPCID.WallofFlesh],
            placeContext: chooseSide,
            spawnOffset: offsetSide,
            modifiedAttributes: new(lifeMultiplier: 150, damageMultiplier: 1.5f, damageFlatIncrease: 50)
        ));
        # endregion

        sign = Util.RandomSign();
        Vector2 worldCoordinates = new Vector2(Main.spawnTileX, Main.spawnTileY).ToWorldCoordinates();
        Player player = Main.player.Where(p => p.active).First();
        worldCoordinates -= new Vector2(player.width / 2, player.height);
        # region Queen Slime
        BRS.AddBoss(7, new(
            [NPCID.QueenSlimeBoss],
            spawnOffset: new(1000 * sign, -500, -100 * sign, -100),
            timeContext: TimeContext.Noon,
            placeContext: new(worldCoordinates, 0, 0)
        ));
        # endregion

        sign = Util.RandomSign();
        # region The Twins
        BRS.AddBoss(8, new(
            [NPCID.Retinazer, NPCID.Spazmatism],
            spawnOffset: new(1000 * sign, 1000, 200 * sign, -2000),
            timeContext: TimeContext.Night
        ));
        # endregion

        # region The Destroyer
        BRS.AddBoss(8, new([NPCID.TheDestroyer],
                           spawnOffset: new(-1000, 1000, 2000, 500),
                           timeContext: TimeContext.Night));
        # endregion

        # region Skeletron Prime
        BRS.AddBoss(8, new([NPCID.SkeletronPrime],
                           spawnOffset: new(-1000, -700, 2000, -300),
                           timeContext: TimeContext.Night));
        # endregion

        # region Plantera
        BRS.AddBoss(9, new([NPCID.Plantera],
                           spawnOffset: new(-1000, 1500, 2000, 500),
                           timeContext: TimeContext.Noon));
        # endregion

        sign = Util.RandomSign();
        #region Golem
        BRS.AddBoss(10, new(
            [NPCID.Golem],
            spawnOffset: new(500 * sign, 0, -200 * sign, -500)
        ));
        # endregion

        sign = Util.RandomSign();
        # region Duke Fishron
        BRS.AddBoss(11, new(
            [NPCID.DukeFishron],
            spawnOffset: new(300 * sign, 50, -100 * sign, -50)
        ));
        # endregion

        # region Empress of Light
        BRS.AddBoss(12, new([NPCID.HallowBoss],
                    spawnOffset: new(0, -100, 0, 0),
                    timeContext: TimeContext.Night));
        # endregion

        sign = Util.RandomSign();
        # region Lunatic Cultist
        BRS.AddBoss(13, new(
            [NPCID.CultistBoss],
            spawnOffset: new(300 * sign, 0, -100 * sign, -500)
        ));
        # endregion

        # region Moon Lord
        BRS.AddBoss(14, new([NPCID.MoonLordCore]));
        # endregion
    }
}
