using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

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
        public List<short> type;

        /// <summary>
        /// List of offsets to be used for spawning.
        /// The center here will be the target player's Center.
        /// Use offsets to spawn the boss in a random location in the specified rectangle.
        /// Only on offset will be chosen from the list by random.
        /// e.g. Use two offsets to spawn the boss in either the left or the right side of the player.
        /// </summary>
        public List<Rectangle> spawnOffsets;

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
        /// </summary>
        public PlaceContext? placeContext;

        /// <summary>
        /// Constructor for BossData.
        /// </summary>
        /// <param name="type">Types of bosses</param>
        /// <param name="spawnOffsets">Spawn offsets for bosses to spawn</param>
        /// <param name="lifeMultiplier">Life multiplied when the boss spawns</param>
        /// <param name="damageMultiplier">Damage multipled when the boss spawns</param>
        /// <param name="lifeFlatIncrease">Life added when the boss spawns</param>
        /// <param name="damageFlatIncrease">Damage added when the boss spawns</param>
        /// <param name="timeContext">Refer to TimeContext struct for more details</param>
        /// <param name="placeContext">Refer to PlaceContext struct for more details</param>
        public BossData(List<short> type, List<Rectangle> spawnOffsets = null,
                        float lifeMultiplier = 1f, float damageMultiplier = 1f,
                        int lifeFlatIncrease = 0, int damageFlatIncrease = 0,
                        TimeContext? timeContext = null, PlaceContext? placeContext = null)
        {
            this.type = type;
            this.spawnOffsets = spawnOffsets ?? [new(0, 0, 0, 0)];
            this.lifeMultiplier = lifeMultiplier;
            this.damageMultiplier = damageMultiplier;
            this.lifeFlatIncrease = lifeFlatIncrease;
            this.damageFlatIncrease = damageFlatIncrease;
            this.timeContext = timeContext;
            this.placeContext = placeContext;
        }

        /// <summary>
        /// Constructor for BossData.
        /// </summary>
        /// <param name="type">Types of bosses</param>
        /// <param name="spawnOffsets">Spawn offsets for bosses to spawn</param>
        /// <param name="lifeMultiplier">Life multiplied when the boss spawns</param>
        /// <param name="damageMultiplier">Damage multipled when the boss spawns</param>
        /// <param name="lifeFlatIncrease">Life added when the boss spawns</param>
        /// <param name="damageFlatIncrease">Damage added when the boss spawns</param>
        /// <param name="timeContext">Refer to TimeContext struct for more details</param>
        /// <param name="placeContext">Refer to PlaceContext struct for more details</param>
        public BossData(short type, List<Rectangle> spawnOffsets = null,
                        float lifeMultiplier = 1f, float damageMultiplier = 1f,
                        int lifeFlatIncrease = 0, int damageFlatIncrease = 0,
                        TimeContext? timeContext = null, PlaceContext? placeContext = null)
               : this([type], spawnOffsets, lifeMultiplier, damageMultiplier,
                      lifeFlatIncrease, damageFlatIncrease, timeContext, placeContext)
        {
        }

        /// <summary>
        /// Used to determine and pinpoint a random location from the given spawnOffsets.
        /// </summary>
        /// <returns>A random location</returns>
        public Vector2 RandomSpawnLocation()
        {
            Rectangle spawnOffset = Main.rand.Next(spawnOffsets);
            Vector2 chosenOffset = Util.ChooseRandomPointInRectangle(spawnOffset);
            return Util.RoundOff(chosenOffset);
        }
    }
}
