using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush
{
    public class BossRushSystem : ModSystem
    {
        public static BossRushSystem I => ModContent.GetInstance<BossRushSystem>();

        public States state = States.Off;
        public NPC currentBoss = null;

        public override void PostUpdateWorld()
        {
            switch (state)
            {
                case States.Prepare:
                    int npcIndex = NPC.NewNPC(new EntitySource_BossSpawn(Main.LocalPlayer), (int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, NPCID.KingSlime);
                    currentBoss = Main.npc[npcIndex];
                    state = States.Run;
                    break;

                case States.Run:
                    if (currentBoss == null || !currentBoss.active || currentBoss.life <= 0)
                    {
                        state = States.Prepare;
                    }
                    break;

                case States.End:
                    foreach (var npc in Main.npc)
                    {
                        if (!npc.friendly)
                        {
                            npc.active = false;
                        }
                    }
                    state = States.Off;
                    break;
            }
        }

        public override void OnWorldUnload()
        {
            state = States.Off;
            currentBoss = null;
        }

        public void ToggleBossRush()
        {
            switch (state)
            {
                case States.Off:
                    state = States.Prepare;
                    break;
                case States.Prepare:
                case States.Run:
                    state = States.End;
                    break;
            }
        }

        public enum States { Off, Prepare, Run, End }
    }
}
