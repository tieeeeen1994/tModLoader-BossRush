using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace BossRush;

public partial class BossRushSystem
{
    public struct BossData
    {
        public List<int> Types { get; private set; }
        public List<int> SubTypes { get; private set; }
        public Func<int, BossData, Rectangle> SpawnOffset { get; private set; }
        public ModifiedAttributes ModifiedAttributes { get; private set; }
        public TimeContext? TimeContext { get; private set; }
        public PlaceContext? PlaceContext { get; private set; }
        public Action<NPC, Dictionary<string, object>> Update { get; private set; }
        public Dictionary<string, object> AI { get; private set; }

        public BossData(List<int> types, List<int> subTypes = null,
                        Func<int, BossData, Rectangle> spawnOffset = null,
                        ModifiedAttributes? modifiedAttributes = null,
                        TimeContext? timeContext = null, List<PlaceContext> placeContexts = null,
                        Action<NPC, Dictionary<string, object>> update = null)
        {
            Types = types;
            SubTypes = subTypes ?? [];
            SpawnOffset = spawnOffset ?? ((_, _) => new(0, 0, 0, 0));
            ModifiedAttributes = modifiedAttributes ?? new();
            TimeContext = timeContext;
            PlaceContext = placeContexts?[Main.rand.Next(placeContexts.Count)];
            Update = update ?? ((_, _) => { });
            AI = [];
        }

        public readonly Vector2 RandomSpawnLocation(int type)
        {
            Vector2 chosenOffset = SpawnOffset(type, this).ChooseRandomPoint();
            return Util.RoundOff(chosenOffset);
        }
    }
}
