using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleBossRush;

public class ExampleBossRush : Mod
{
    public static ExampleBossRush Instance => ModContent.GetInstance<ExampleBossRush>();

    public static SoundStyle MiniRoar => new("Terraria/Sounds/Roar_0")
    {
        MaxInstances = 20,
        Pitch = 2f,
        Volume = 1f
    };

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

            case PacketTypes.ServantRoar:
                short npcIndex = reader.ReadInt16();
                NPC npc = Main.npc[npcIndex];
                if (npc.active)
                {
                    SoundEngine.PlaySound(MiniRoar, Main.npc[npcIndex].Center);
                }
                break;
        }
    }

    public enum PacketTypes : byte { CorruptorDust, ServantRoar }
}
