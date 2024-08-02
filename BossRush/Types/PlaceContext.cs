using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BR = BossRush.BossRush;

namespace BossRush.Types;

public struct PlaceContext
{
    public Vector2 InitialPosition { get; private set; }
    public Rectangle TeleportRange { get; private set; }

    public static PlaceContext LeftUnderworld => new(UnderworldPosition(100), 10, 10);

    public static PlaceContext RightUnderworld => new(UnderworldPosition(Main.maxTilesX - 100), 10, 10);

    public static PlaceContext LeftOcean => new(2000, 3000, -500, 500);

    public static PlaceContext RightOcean => new(Main.maxTilesX.ToWorldCoordinate() - 2000,
                                                 3000, 500, 500);

    public PlaceContext(Vector2 initialPosition, int width, int height)
    {
        Vector2 valuePosition = InitialPosition = initialPosition.RoundOff();
        int x = (int)valuePosition.X - Util.RoundOff(width / 2);
        int y = (int)valuePosition.Y - Util.RoundOff(height / 2);
        TeleportRange = new(x, y, width, height);
    }

    public PlaceContext(int x, int y, int width, int height) : this(new(x, y), width, height) { }

    public readonly void TeleportPlayers()
    {
        TeleportLogic((self, _) => self.TeleportRange.ChooseRandomPoint().RoundOff());
    }

    public readonly void BackToSpawn()
    {
        Vector2 worldCoordinates = new Vector2(Main.spawnTileX, Main.spawnTileY).ToWorldCoordinates();
        TeleportLogic((self, player) => worldCoordinates - new Vector2(player.width / 2, player.height));
    }

    private static Vector2 UnderworldPosition(int xTileCoordinate)
    {
        int yTileCoordinate = Util.RoundOff(Main.UnderworldLayer + .34f * (Main.maxTilesY - Main.UnderworldLayer));
        Vector2 worldPosition = new Vector2(xTileCoordinate, yTileCoordinate).ToWorldCoordinates();
        return worldPosition;
    }

    private readonly void TeleportLogic(Func<PlaceContext, Player, Vector2> positionFunction)
    {
        foreach (var player in Main.ActivePlayers)
        {
            if (player.active)
            {
                Vector2 position = positionFunction(this, player);
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
