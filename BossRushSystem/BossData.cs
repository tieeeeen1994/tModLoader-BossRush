using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace BossRush;

public partial class BossRushSystem
{
    public struct BossData
    {
        public short type;
        public Rectangle spawnOffset;
        public bool mirroredSpawnOffset;
        public float lifeMultiplier;
        public float damageMultiplier;
        public int lifeFlatIncrease;
        public int damageFlatIncrease;
        private Vector2? _determinedSpawnLocation;

        public BossData(short type, Rectangle? spawnOffset = null, bool mirroredSpawnOffset = false,
                        float lifeMultiplier = 1f, float damageMultiplier = 1f,
                        int lifeFlatIncrease = 0, int damageFlatIncrease = 0)
        {
            this.type = type;
            this.spawnOffset = spawnOffset ?? new(0, 0, 0, 0);
            this.mirroredSpawnOffset = mirroredSpawnOffset;
            this.lifeMultiplier = lifeMultiplier;
            this.damageMultiplier = damageMultiplier;
            this.lifeFlatIncrease = lifeFlatIncrease;
            this.damageFlatIncrease = damageFlatIncrease;
            _determinedSpawnLocation = null;
        }

        public Vector2 RandomSpawnLocation(Vector2? currentPosition = null)
        {
            if (!_determinedSpawnLocation.HasValue)
            {
                Vector2 trueCurrentPosition = currentPosition ?? Vector2.Zero;
                int mirror = mirroredSpawnOffset && Main.rand.NextBool() ? -1 : 1;
                int signWidth = Math.Sign(spawnOffset.Width);
                int signHeight = Math.Sign(spawnOffset.Height);
                int offsetX = spawnOffset.X + Main.rand.Next(Math.Abs(spawnOffset.Width) + 1) * signWidth * mirror;
                int offsetY = spawnOffset.Y + Main.rand.Next(Math.Abs(spawnOffset.Height) + 1) * signHeight * mirror;
                _determinedSpawnLocation =
                    Util.RoundOff(new Vector2(trueCurrentPosition.X + offsetX, trueCurrentPosition.Y + offsetY));
            }

            return _determinedSpawnLocation.Value;
        }
    }
}
