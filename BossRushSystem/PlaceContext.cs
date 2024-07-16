using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BR = BossRush.BossRush;

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
        public Vector2? initialPosition;

        /// <summary>
        /// The range where players will be teleported based on the initial position.
        /// The initial position will be the center of this rectangle.
        /// A random location will be chosen in the rectangle so that players will not be cramped in one spot.
        /// </summary>
        public Rectangle? teleportRange;

        /// <summary>
        /// Function generally used to force the biome around the player.
        /// Do note that due to Terraria's biome system, this can be finicky.
        /// It wil not always work and it's better to teleport players into the real biome.
        /// This always works for corruption and crimson, however.
        /// </summary>
        public Action<Player> forceBiomeFunction;

        /// <summary>
        /// Custom implementation for the place context.
        /// This is used for more dynamic and flexible implementation where static data will not do.
        /// </summary>
        private readonly Func<Player, Rectangle> customImplementation;

        /// <summary>
        /// Shortcut for the left side of the Underworld.
        /// </summary>
        public static PlaceContext LeftUnderworld
        {
            get
            {
                int yInTile = Util.RoundOff(Main.UnderworldLayer + 0.34f * (Main.maxTilesY - Main.UnderworldLayer));
                Vector2 worldPosition = new Vector2(100, yInTile).ToWorldCoordinates();
                return new(worldPosition, 10);
            }
        }

        /// <summary>
        /// Shortcut for the right side of the Underworld.
        /// </summary>
        public static PlaceContext RightUnderworld
        {
            get
            {
                int yInTile = Util.RoundOff(Main.UnderworldLayer + 0.34f * (Main.maxTilesY - Main.UnderworldLayer));
                Vector2 worldPosition = new Vector2(Main.maxTilesX - 100, yInTile).ToWorldCoordinates();
                return new(worldPosition, 10);
            }
        }

        /// <summary>
        /// Constructor for PlaceContext.
        /// The rectangle here is only statically created.
        /// Use the other constructor for a dynamic approach.
        /// </summary>
        /// <param name="initialPosition">Position where players are teleported</param>
        /// <param name="radius">
        /// Length of the rectangle from the center to the edge
        /// (This is not a circle)
        /// </param>
        /// <param name="forceBiome">
        /// Implementation for forcing the biome
        /// (Player parameter is the affected one of the forcing)
        /// </param>
        public PlaceContext(Vector2 initialPosition, int radius, Action<Player> forceBiome = null)
        {
            customImplementation = null;
            this.initialPosition = Util.RoundOff(initialPosition);
            Vector2 valuePosition = this.initialPosition.Value;
            int x = (int)valuePosition.X - radius;
            int y = (int)valuePosition.Y - radius;
            int roundedDiameter = Util.RoundOff(radius * 2);
            teleportRange = new(x, y, roundedDiameter, roundedDiameter);
            forceBiomeFunction = forceBiome;
        }

        /// <summary>
        /// Constructor for PlaceContext.
        /// This constructor is used for a more dynamic approach.
        /// </summary>
        /// <param name="implementation">
        /// Custom implementation
        /// (Player parameter is the player being teleported)
        /// </param>
        /// <param name="forceBiome">
        /// Implementation for forcing the biome
        /// (Player parameter is the affected one of the forcing)
        /// </param>
        public PlaceContext(Func<Player, Rectangle> implementation, Action<Player> forceBiome = null)
        {
            customImplementation = implementation;
            initialPosition = null;
            teleportRange = null;
            forceBiomeFunction = forceBiome;
        }

        /// <summary>
        /// Constructor for PlaceContext.
        /// This constructor is for only setting the forcing of a biome into players.
        /// No teleportation will happen here.
        /// </summary>
        /// <param name="forceBiome">
        /// Implementation for forcing the biome
        /// (Player parameter is the affected one of the forcing)
        /// </param>
        public PlaceContext(Action<Player> forceBiome)
        {
            customImplementation = null;
            initialPosition = null;
            teleportRange = null;
            forceBiomeFunction = forceBiome;
        }

        /// <summary>
        /// Teleports all active players to a random location in the teleport range.
        /// </summary>
        public void TeleportPlayers()
        {
            if (WillPlayersTeleport())
            {
                foreach (var player in Main.ActivePlayers)
                {
                    if (player.active)
                    {
                        Vector2 position = Util.RoundOff(Util.ChooseRandomPointInRectangle(UseImplementation(player)));
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            player.Teleport(position);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            ModPacket packet = BR.I.GetPacket();
                            packet.Write((byte)BR.PacketType.Teleport);
                            packet.WriteVector2(position);
                            packet.Send(player.whoAmI);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects the implementation to use.
        /// </summary>
        /// <param name="player">Player being teleported</param>
        /// <returns>Rectangle for spawning the player</returns>
        private Rectangle UseImplementation(Player player)
        {
            if (customImplementation == null)
            {
                return teleportRange.Value;
            }
            else
            {
                return customImplementation(player);
            }
        }

        /// <summary>
        /// Check whether the PlaceContext object will teleport players.
        /// </summary>
        /// <returns>True or False</returns>
        private bool WillPlayersTeleport()
        {
            return (initialPosition != null && teleportRange != null) || customImplementation != null;
        }
    }
}
