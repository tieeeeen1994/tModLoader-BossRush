using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleBossRush;

public class ExampleBossRush : Mod
{
    public static ExampleBossRush Instance => ModContent.GetInstance<ExampleBossRush>();

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketTypes packetType = (PacketTypes)reader.ReadByte();
        switch (packetType)
        {
            case PacketTypes.CorruptorDust:
                Vector2 position = reader.ReadVector2();
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(position - new Vector2(15, 15), 30, 30, DustID.Demonite);
                }
                break;

            case PacketTypes.BoulderProperties:
                int boulderIndex = reader.ReadInt32();
                Projectile projectile = Main.projectile[boulderIndex];
                projectile.friendly = false;
                projectile.hostile = true;
                projectile.trap = false;
                break;
        }
    }

    public enum PacketTypes : byte { CorruptorDust, BoulderProperties }
}
