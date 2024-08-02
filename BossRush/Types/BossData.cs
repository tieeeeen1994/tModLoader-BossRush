using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BossRush.Types;

/// <summary>
/// Data Structure for storing information about a boss rush stage.
/// Once the information is set, use BossRushSystem.AddBoss to add the stage into a boss rush session.
/// </summary>
/// <param name="types">
/// Types of the main bosses to initially spawn in the boss rush stage.
/// For example, [NPCID.KingSlime] or [ModContent.NPCType<CustomBoss>()]
/// Multiple entities in a boss rush stage is supported.
/// For example, [NPCID.Retinazer, NPCID.Spazmatism].
/// </param>
/// <param name="subTypes">
/// Sub-types are part of the main bosses which are spawned in their AI or states.
/// Include types here if they are required to be detected as defeated in order for a boss rush stage to be completed.
/// For example, Eater of Worlds would need [NPCID.EaterofWorldsHead] for types parameter.
/// Then, [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail] would be included in subTypes parameter.
/// It is because Eater of Worlds is spawned using the Head, and the Head AI spawns the rest of the body,
/// and the rest of the body is needed to be detected as defeated to progress the boss rush stage properly.
/// </param>
/// <param name="spawnOffset">
/// Spawns the boss at a random location within the offset. The base loocation would be a random target player.
/// For example, new Rectangle(-100, -100, 200, 200) would spawn the boss within a 200x200 square around a player.
/// It will choose a random point from inside the rectangle. Player with higher aggro stat will be selected as target.
/// A random player will be chosen if they have equal aggro stat.
/// </param>
/// <param name="modifiedAttributes">
/// Used to modify the attributes of the bosses and their minions within the stage.
/// The calculation is as follows: finalValue = (baseValue + addValue) * multiplyValue.
/// When any NPC spawns, their attributes will be modified by this data.
/// Only supported stats are: life, defense and damage.
/// Projectile damage behaves differently and may differ in implementation, so a flag is provided to handle it.
/// </param>
/// <param name="spawnAttributes">
/// This is used to modify the spawning mechanics in the boss rush stage. By default, the spawns will be normal.
/// This can be used to double the spawn rate, or disable it. This will be useful if the stage relies on spawn rates.
/// For example, the lunar pillars rely on spawn rates to progress the stage.
/// </param>
/// <param name="timeContext">
/// Used to modify the time of the boss rush stage. By default, the time will be normal and unchanged if not set.
/// However, if set, the time will be modified by the given value when the boss rush stage starts.
/// Useful for several bosses that are contrainted by time, such as Eye of Cthulhu.
/// </param>
/// <param name="placeContext">
/// Used to teleport all players to a specific location when the boss rush stage starts.
/// The rectangle is an absolute position in the world. Players will be teleported to a random point in the rectangle.
/// Useful for stages that require players to be in specific locations of the world, such as Wall of Flesh.
/// </param>
/// <param name="startMessage">
/// In a boss rush stage, no message will appear when the boss spawns to allow flexibility.
/// Messages can be manually set instead. This will be seen when the boss spawns in the boss rush stage.
/// </param>
/// <param name="defeatMessage">
/// Bosses will still display their defeat messages when they are defeated in a boss rush stage.
/// A custom message will not replace that message, and instead add to it as another message.
/// </param>
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
