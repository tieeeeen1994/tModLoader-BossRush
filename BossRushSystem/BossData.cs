using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace BossRush;

public partial class BossRushSystem
{
    public struct BossData
    {
        public List<short> type;
        public List<Rectangle> spawnOffsets;
        public float lifeMultiplier;
        public float damageMultiplier;
        public int lifeFlatIncrease;
        public int damageFlatIncrease;
        public TimeContext? timeContext;

        public BossData(List<short> type, List<Rectangle> spawnOffsets = null,
                        float lifeMultiplier = 1f, float damageMultiplier = 1f,
                        int lifeFlatIncrease = 0, int damageFlatIncrease = 0,
                        TimeContext? timeContext = null)
        {
            this.type = type;
            this.spawnOffsets = spawnOffsets ?? [new(0, 0, 0, 0)];
            this.lifeMultiplier = lifeMultiplier;
            this.damageMultiplier = damageMultiplier;
            this.lifeFlatIncrease = lifeFlatIncrease;
            this.damageFlatIncrease = damageFlatIncrease;
            this.timeContext = timeContext;
        }

        public BossData(short type, List<Rectangle> spawnOffsets = null,
                        float lifeMultiplier = 1f, float damageMultiplier = 1f,
                        int lifeFlatIncrease = 0, int damageFlatIncrease = 0,
                        TimeContext? timeContext = null)
               : this([type], spawnOffsets, lifeMultiplier, damageMultiplier,
                      lifeFlatIncrease, damageFlatIncrease, timeContext)
        {
        }

        public Vector2 RandomSpawnLocation(Vector2? currentPosition = null)
        {
            Vector2 trueCurrentPosition = currentPosition ?? Vector2.Zero;
            Rectangle spawnOffset = Main.rand.Next(spawnOffsets);
            int signWidth = Math.Sign(spawnOffset.Width);
            int signHeight = Math.Sign(spawnOffset.Height);
            int offsetX = spawnOffset.X + Main.rand.Next(Math.Abs(spawnOffset.Width) + 1) * signWidth;
            int offsetY = spawnOffset.Y + Main.rand.Next(Math.Abs(spawnOffset.Height) + 1) * signHeight;
            return Util.RoundOff(new Vector2(trueCurrentPosition.X + offsetX, trueCurrentPosition.Y + offsetY));
        }
    }
}
