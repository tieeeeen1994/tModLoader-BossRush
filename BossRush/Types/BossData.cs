using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace BossRush.Types;

public struct BossData
{
    public readonly List<int> Types => [.. _types];
    public readonly List<int> SubTypes => [.. _subTypes];
    public Rectangle SpawnOffset { get; private set; }
    public ModifiedAttributes ModifiedAttributes { get; private set; }
    public TimeContext? TimeContext { get; private set; }
    public PlaceContext? PlaceContext { get; private set; }
    private readonly List<int> _types;
    private readonly List<int> _subTypes;

    public BossData(List<int> types, List<int> subTypes = null, Rectangle? spawnOffset = null,
                    ModifiedAttributes? modifiedAttributes = null,
                    TimeContext? timeContext = null, PlaceContext? placeContext = null)
    {
        _types = types;
        _subTypes = subTypes ?? [];
        SpawnOffset = spawnOffset ?? new(0, 0, 0, 0);
        ModifiedAttributes = modifiedAttributes ?? new();
        TimeContext = timeContext;
        PlaceContext = placeContext;
    }

    public readonly Vector2 RandomSpawnLocation(int type)
    {
        Vector2 chosenOffset = SpawnOffset.ChooseRandomPoint();
        return Util.RoundOff(chosenOffset);
    }
}
