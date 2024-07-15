using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BossRush;

public partial class BossRushSystem
{
    /// <summary>
    /// Contains information about the boss in Boss Rush mode.
    /// </summary>
    public struct BossData
    {
        /// <summary>
        /// List of types of entities to be spawned.
        /// </summary>
        public List<int> type;

        /// <summary>
        /// Offset to be used for spawning. The center here will be the target player's Center.
        /// Use offsets to spawn the boss in a random location in the specified rectangle.
        /// The field is a Func so that it runs the logic when the check is needed.
        /// This will allow flexible checks instead of being hard-coded and static.
        /// The integer parameter is the type of the boss.
        /// </summary>
        public Func<int, Rectangle> spawnOffset;

        /// <summary>
        /// Multiplier for the boss's life.
        /// Computed after the flat increase.
        /// This also affects its minions.
        /// </summary>
        public float lifeMultiplier;

        /// <summary>
        /// Multiplier for the boss's damage.
        /// Computed after the flat increase.
        /// This also affects its minions.
        /// </summary>
        public float damageMultiplier;

        /// <summary>
        /// Flat increase for the boss's life.
        /// Computed before the multiplier.
        /// This also affects its minions.
        /// </summary>
        public int lifeFlatIncrease;

        /// <summary>
        /// Flat increase for the boss's damage.
        /// Computed before the multiplier.
        /// This also affects its minions.
        /// </summary>
        public int damageFlatIncrease;

        /// <summary>
        /// Time requirement for the boss to work properly.
        /// Time will change according to the context when the boss is spawned.
        /// If no context is provided, time will not change.
        /// </summary>
        public TimeContext? timeContext;

        /// <summary>
        /// Place requirement for the boss to work properly.
        /// Players will be teleported to the specified location before the boss is spawned.
        /// If no context is provided, players will not be teleported.
        /// This contains a list so that there can be multiple places the boss fight can happen.
        /// A good example of this is Wall of Flesh. It can be fought on either side of the world.
        /// </summary>
        public PlaceContext? placeContext;

        /// <summary>
        /// Constructor for BossData.
        /// </summary>
        /// <param name="type">Types of bosses</param>
        /// <param name="spawnOffset">Spawn offset for bosses to spawn (integer parameter is boss type)</param>
        /// <param name="lifeMultiplier">Life multiplied when the boss spawns</param>
        /// <param name="damageMultiplier">Damage multipled when the boss spawns</param>
        /// <param name="lifeFlatIncrease">Life added when the boss spawns</param>
        /// <param name="damageFlatIncrease">Damage added when the boss spawns</param>
        /// <param name="timeContext">Refer to TimeContext struct for more details</param>
        /// <param name="placeContext">Refer to PlaceContext struct for more details</param>
        public BossData(List<int> type, Func<int, Rectangle> spawnOffset = null,
                        float lifeMultiplier = 1f, float damageMultiplier = 1f,
                        int lifeFlatIncrease = 0, int damageFlatIncrease = 0,
                        TimeContext? timeContext = null, PlaceContext? placeContext = null)
        {
            this.type = type;
            this.spawnOffset = spawnOffset ?? ((_) => new(0, 0, 0, 0));
            this.lifeMultiplier = lifeMultiplier;
            this.damageMultiplier = damageMultiplier;
            this.lifeFlatIncrease = lifeFlatIncrease;
            this.damageFlatIncrease = damageFlatIncrease;
            this.timeContext = timeContext;
            this.placeContext = placeContext;
        }

        /// <summary>
        /// Used to determine and pinpoint a random location from the given spawnOffsets.
        /// The parameter is mainly used for spawnOffset Func object for flexibility.
        /// </summary>
        /// <param name="type">Type of the boss</param>
        /// <returns>A random location</returns>
        public Vector2 RandomSpawnLocation(int type)
        {
            Vector2 chosenOffset = Util.ChooseRandomPointInRectangle(spawnOffset(type));
            return Util.RoundOff(chosenOffset);
        }
    }
}
