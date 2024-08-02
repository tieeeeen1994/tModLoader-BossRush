using ExampleBossRush.Types;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static ExampleBossRush.ExampleBossRushUtils;

namespace ExampleBossRush.NPCs
{
    public class Pillars : BossRushBossAndMinions
    {
        internal const int ShieldValue = 150;

        protected override List<int> ApplicableTypes => [
            NPCID.LunarTowerSolar,
            NPCID.LunarTowerVortex,
            NPCID.LunarTowerNebula,
            NPCID.LunarTowerStardust
        ];

        protected override bool AbsoluteCheck => IsCurrentBoss(ApplicableTypes);

        protected override void Update(NPC npc)
        {
            if (ApplicableTypes.Contains(npc.type))
            {
                var shieldTracker = StoreOrFetch("ShieldTracker", new Dictionary<NPC, bool>());
                if (!shieldTracker.TryGetValue(npc, out bool shielded) && !shielded)
                {
                    switch (npc.type)
                    {
                        case NPCID.LunarTowerSolar:
                            NPC.ShieldStrengthTowerSolar = ShieldValue;
                            break;

                        case NPCID.LunarTowerVortex:
                            NPC.ShieldStrengthTowerVortex = ShieldValue;
                            break;

                        case NPCID.LunarTowerNebula:
                            NPC.ShieldStrengthTowerNebula = ShieldValue;
                            break;

                        case NPCID.LunarTowerStardust:
                            NPC.ShieldStrengthTowerStardust = ShieldValue;
                            break;
                    }
                    shieldTracker[npc] = shielded;
                    NetMessage.SendData(MessageID.UpdateTowerShieldStrengths, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                }
            }
        }
    }
}
