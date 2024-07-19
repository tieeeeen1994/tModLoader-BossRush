using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace BossRush.Types;

public struct BossData
{
    public readonly List<int> Types => [.. _types];
    public readonly List<int> SubTypes => [.. _subTypes];
    public Func<int, BossData, Rectangle> SpawnOffset { get; private set; }
    public ModifiedAttributes ModifiedAttributes { get; private set; }
    public TimeContext? TimeContext { get; private set; }
    public PlaceContext? PlaceContext { get; private set; }
    public Action<NPC, Dictionary<string, object>> Update { get; private set; }
    public Dictionary<string, object> AI { get; private set; }
    private readonly List<int> _types;
    private readonly List<int> _subTypes;

    public BossData(List<int> types, List<int> subTypes = null,
                    Func<int, BossData, Rectangle> spawnOffset = null,
                    ModifiedAttributes? modifiedAttributes = null,
                    TimeContext? timeContext = null, List<PlaceContext> placeContexts = null,
                    Action<NPC, Dictionary<string, object>> update = null)
    {
        _types = types;
        _subTypes = subTypes ?? [];
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
