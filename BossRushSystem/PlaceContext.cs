using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static BossRush.BossRush;

namespace BossRush
{
    /// <summary>
    /// Struct for containing information about the spawn requirements of a boss related to place.
    /// This is used in BossData struct.
    /// </summary>
    public struct PlaceContext
    {
        /// <summary>
        /// The initial position where players will be teleported.
        /// </summary>
        public Vector2 initialPosition;

        /// <summary>
        /// The range where players will be teleported based on the initial position.
        /// The initial position will be the center of this rectangle.
        /// A random location will be chosen in the rectangle so that players will not be cramped in one spot.
        /// </summary>
        public Rectangle teleportRange;

        /// <summary>
        /// Constructor for PlaceContext.
        /// </summary>
        /// <param name="initialPosition">Position where players are teleported</param>
        /// <param name="radius">
        /// Length of the rectangle from the center to the edge
        /// This is not a circle
        /// </param>
        public PlaceContext(Vector2 initialPosition, int radius)
        {
            this.initialPosition = Util.RoundOff(initialPosition);
            int x = (int)this.initialPosition.X - radius;
            int y = (int)this.initialPosition.Y - radius;
            int roundedDiameter = Util.RoundOff(radius * 2);
            teleportRange = new(x, y, roundedDiameter, roundedDiameter);
        }

        /// <summary>
        /// Teleports all active players to a random location in the teleport range.
        /// </summary>
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
