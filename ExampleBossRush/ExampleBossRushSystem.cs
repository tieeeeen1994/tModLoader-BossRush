using ExampleBossRush.NPCs;
using Terraria.ModLoader;
using BRS = BossRush.BossRushSystem;

namespace ExampleBossRush
{
    public class ExampleBossRushSystem : ModSystem
    {
        public override void OnWorldUnload()
        {
            BossRushBossAndMinions.ai.Clear();
        }

        public override void PreUpdateWorld()
        {
            var ai = BossRushBossAndMinions.ai;
            if (ai.Count > 0 && (BRS.I.CurrentBoss == null || BRS.I.CurrentBossData == null))
            {
                ai.Clear();
            }
        }
    }
}
