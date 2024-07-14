using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static BossRush.BossRush;

namespace BossRush
{
    public struct PlaceContext
    {
        public Vector2 initialPosition;
        public Rectangle teleportRange;

        public PlaceContext(Vector2 initialPosition, int radius)
        {
            this.initialPosition = Util.RoundOff(initialPosition);
            int x = (int)this.initialPosition.X - radius;
            int y = (int)this.initialPosition.Y - radius;
            int roundedDiameter = Util.RoundOff(radius * 2);
            teleportRange = new(x, y, roundedDiameter, roundedDiameter);
        }

        public void TeleportPlayers()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (player.active)
                {
                    Vector2 offsetPosition = Util.ChooseRandomPointInRectangle(teleportRange);
                    Vector2 position = Util.RoundOff(initialPosition + offsetPosition);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        player.Teleport(position);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = BossRush.I.GetPacket();
                        packet.Write((byte)PacketType.Teleport);
                        packet.WriteVector2(position);
                        packet.Send(player.whoAmI);
                    }
                }
            }
        }
    }
}
