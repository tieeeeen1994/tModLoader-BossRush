using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BossRushAPI.Types;

public struct BossData(List<int> types, List<int> subTypes = null, Rectangle? spawnOffset = null,
                       ModifiedAttributes? modifiedAttributes = null, SpawnAttributes? spawnAttributes = null,
                       TimeContext? timeContext = null, PlaceContext? placeContext = null,
                       Message? startMessage = null, Message? defeatMessage = null)
{
    public readonly List<int> Types => [.. _types];
    public readonly List<int> SubTypes => [.. _subTypes];
    public Rectangle SpawnOffset { get; private set; } = spawnOffset ?? new(0, 0, 0, 0);
    public ModifiedAttributes ModifiedAttributes { get; private set; } = modifiedAttributes ?? new();
    public TimeContext? TimeContext { get; private set; } = timeContext;
    public PlaceContext? PlaceContext { get; private set; } = placeContext;
    public SpawnAttributes SpawnAttributes { get; private set; } = spawnAttributes ?? new();
    public Message? StartMessage { get; private set; } = startMessage;
    public Message? DefeatMessage { get; private set; } = defeatMessage;
    private readonly List<int> _types = types;
    private readonly List<int> _subTypes = subTypes ?? [];

    public readonly Vector2 RandomSpawnLocation()
    {
        Vector2 chosenOffset = SpawnOffset.ChooseRandomPoint();
        return Util.RoundOff(chosenOffset);
    }
}
