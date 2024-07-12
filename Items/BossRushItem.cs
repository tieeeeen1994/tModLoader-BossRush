using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush.Items;

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
    }

    public override string Texture => $"Terraria/Images/Item_{ItemID.Acorn}";

    public override bool? UseItem(Player player)
    {
        SoundEngine.PlaySound(SoundID.Roar, player.Center);
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            BossRushSystem.I.ToggleBossRush();
            return true;
        }

        return false;
    }
}
