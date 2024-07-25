using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace ExampleBossRush.NPCs;

public class EyeOfCthulhuAndMinions : BossRushBossAndMinions
{
    public static SoundStyle MiniRoar => new("Terraria/Sounds/Roar_0")
    {
        MaxInstances = 20,
        Pitch = 2f,
        Volume = 1f
    };

    protected override List<int> ApplicableTypes => [NPCID.EyeofCthulhu, NPCID.ServantofCthulhu];

    protected override void Update(NPC npc)
    {
        if (npc.type == NPCID.EyeofCthulhu)
        {
            if (ai.TryGetValue("BossForcedDamage", out object value))
            {
                npc.damage = (int)value;
            }
            else
            {
                ai["BossForcedDamage"] = npc.damage;
            }
        }
        else if (npc.type == NPCID.ServantofCthulhu)
        {
            npc.knockBackResist = 0f;
            if (!ai.TryGetValue("ServantTracker", out object value1))
            {
                value1 = new Dictionary<NPC, Tuple<string, int>>();
                ai["ServantTracker"] = value1;
            }
            if (!ai.TryGetValue("DashTracker", out object value2))
            {
                value2 = new Dictionary<NPC, Vector2>();
                ai["DashTracker"] = value2;
            }
            if (value1 is Dictionary<NPC, Tuple<string, int>> servantTracker && npc.active)
            {
                if (!servantTracker.TryGetValue(npc, out var data))
                {
                    data = ("Default", 180).ToTuple();
                }
                if (data.Item1 == "Default" && data.Item2 <= 0)
                {
                    data = ("Dash", 30).ToTuple();
                    if (value2 is Dictionary<NPC, Vector2> dashTracker)
                    {
                        dashTracker[npc] = npc.DirectionTo(Main.player[npc.target].Center);
                    }
                    SoundEngine.PlaySound(MiniRoar, npc.Center);
                }
                else if (data.Item1 == "Dash")
                {
                    if (data.Item2 <= 0)
                    {
                        data = ("Default", 180).ToTuple();
                    }
                    if (value2 is Dictionary<NPC, Vector2> dashTracker)
                    {
                        npc.velocity = dashTracker[npc] * 10f;
                    }
                }
                servantTracker[npc] = (data.Item1, data.Item2 - 1).ToTuple();
            }
        }
    }
}
