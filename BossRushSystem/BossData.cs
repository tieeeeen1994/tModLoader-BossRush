using System;
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
        public List<int> types;

        /// <summary>
        /// Offset to be used for spawning. The center here will be the target player's Center.
        /// Use offsets to spawn the boss in a random location in the specified rectangle.
        /// The field is a Func so that it runs the logic when the check is needed.
        /// This will allow flexible checks instead of being hard-coded and static.
        /// The integer parameter is the type of the boss.
        /// </summary>
        public Func<int, BossData, Rectangle> spawnOffset;

        /// <summary>
        /// Data structure containing the supposedly attributes modifications for the boss.
        /// This will be used to adjust the stats of the boss when it spawns.
        /// </summary>
        public ModifiedAttributes modifiedAttributes;

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
        /// <param name="types">Types of bosses</param>
        /// <param name="spawnOffset">Spawn offset for bosses to spawn (integer parameter is boss type)</param>
        /// <param name="modifiedAttributes">Modifications to the stats of the boss and its minions</param>
        /// <param name="timeContext">Refer to TimeContext struct for more details</param>
        /// <param name="placeContexts">Refer to PlaceContext struct for more details</param>
        public BossData(List<int> types, Func<int, BossData, Rectangle> spawnOffset = null,
                        ModifiedAttributes modifiedAttributes = new(),
                        TimeContext? timeContext = null, List<PlaceContext> placeContexts = null)
        {
            this.types = types;
            this.spawnOffset = spawnOffset ?? ((_, _) => new(0, 0, 0, 0));
            this.modifiedAttributes = modifiedAttributes;
            this.timeContext = timeContext;
            placeContext = placeContexts?[Main.rand.Next(placeContexts.Count)];
        }

        /// <summary>
        /// Used to determine and pinpoint a random location from the given spawnOffsets.
        /// The parameter is mainly used for spawnOffset Func object for flexibility.
        /// </summary>
        /// <param name="type">Type of the boss</param>
        /// <returns>A random location</returns>
        public Vector2 RandomSpawnLocation(int type)
        {
            Vector2 chosenOffset = Util.ChooseRandomPointInRectangle(spawnOffset(type, this));
            return Util.RoundOff(chosenOffset);
        }
    }
}
