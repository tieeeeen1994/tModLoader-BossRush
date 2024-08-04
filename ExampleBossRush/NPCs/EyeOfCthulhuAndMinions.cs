using BossRushAPI;
using ExampleBossRush.Types;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

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

    protected override bool AbsoluteCheck => IsCurrentBoss(NPCID.EyeofCthulhu);

    protected override void Update(NPC npc)
    {
        var servantTracker = StoreOrFetch("ServantTracker", new Dictionary<NPC, (string, int)>());
        var dashTracker = StoreOrFetch("DashTracker", new Dictionary<NPC, Vector2>());
        if (npc.type == NPCID.EyeofCthulhu)
        {
            npc.damage = StoreOrFetch("BossForcedDamage", npc.damage);
            CleanInactiveData(servantTracker);
            CleanInactiveData(dashTracker);
        }
        else if (npc.type == NPCID.ServantofCthulhu)
        {
            (string state, int timer) = StoreOrFetch(servantTracker, npc, ("Default", 180));
            if (state == "Default" && timer <= 0)
            {
                state = "Dash";
                timer = 1.2f.ToFrames();
                dashTracker[npc] = npc.DirectionTo(Main.player[npc.target].Center);
                SoundEngine.PlaySound(MiniRoar, npc.Center);
            }
            else if (state == "Dash")
            {
                if (timer <= 0)
                {
                    npc.velocity = Vector2.Zero;
                    state = "Default";
                    timer = 3.ToFrames();
                }
                else
                {
                    npc.velocity = dashTracker[npc] * 20f;
                }
            }
            servantTracker[npc] = (state, timer - 1);
            npc.knockBackResist = 0f;
        }
    }
}
