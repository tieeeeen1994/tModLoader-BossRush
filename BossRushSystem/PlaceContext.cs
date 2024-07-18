using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BR = BossRush.BossRush;

namespace BossRush
{
    public struct PlaceContext
    {
        public Vector2? InitialPosition { get; private set; }
        public Rectangle? TeleportRange { get; private set; }
        public Action<Player> ForceBiomeFunction { get; private set; }
        private readonly Func<Player, Rectangle> customImplementation;

        public static PlaceContext LeftUnderworld => new(UnderworldPosition(100), 10);

        public static PlaceContext RightUnderworld => new(UnderworldPosition(Main.maxTilesX - 100), 10);

        private static Vector2 UnderworldPosition(int xTileCoordinate)
        {
            int yTileCoordinate = Util.RoundOff(Main.UnderworldLayer + .34f * (Main.maxTilesY - Main.UnderworldLayer));
            Vector2 worldPosition = new Vector2(xTileCoordinate, yTileCoordinate).ToWorldCoordinates();
            return worldPosition;
        }

        public PlaceContext(Vector2 initialPosition, int radius, Action<Player> forceBiome = null)
        {
            customImplementation = null;
            this.InitialPosition = Util.RoundOff(initialPosition);
            Vector2 valuePosition = this.InitialPosition.Value;
            // Computation below results to a square instead of a circle, and such is intended.
            int x = (int)valuePosition.X - radius;
            int y = (int)valuePosition.Y - radius;
            int roundedDiameter = Util.RoundOff(radius * 2);
            TeleportRange = new(x, y, roundedDiameter, roundedDiameter);
            ForceBiomeFunction = forceBiome;
        }

        public PlaceContext(Func<Player, Rectangle> implementation, Action<Player> forceBiome = null)
        {
            customImplementation = implementation;
            InitialPosition = null;
            TeleportRange = null;
            ForceBiomeFunction = forceBiome;
        }

        public PlaceContext(Action<Player> forceBiome)
        {
            customImplementation = null;
            InitialPosition = null;
            TeleportRange = null;
            ForceBiomeFunction = forceBiome;
        }

        public void TeleportPlayers()
        {
            if (WillPlayersTeleport())
            {
                foreach (var player in Main.ActivePlayers)
                {
                    if (player.active)
                    {
                        Vector2 position = UseImplementation(player).ChooseRandomPoint().RoundOff();
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

        private Rectangle UseImplementation(Player player) => customImplementation == null ?
                                                              TeleportRange.Value :
                                                              customImplementation(player);

        private bool WillPlayersTeleport() => (InitialPosition != null && TeleportRange != null) ||
                                              customImplementation != null;
    }
}
