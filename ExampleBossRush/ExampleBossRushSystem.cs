using ExampleBossRush.Types;
using Terraria.ModLoader;
using BRS = BossRushAPI.BossRushSystem;

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
            if (ai.Count > 0 && (BRS.I.State != BRS.States.Run))
            {
                ai.Clear();
            }
        }
    }
}
