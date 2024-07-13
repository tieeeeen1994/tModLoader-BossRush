using Terraria;
using Terraria.ID;

namespace BossRush;

public partial class BossRushSystem
{
    public struct TimeContext
    {
        public double time;
        public bool dayTime;

        public static TimeContext Day => new TimeContext(0, true);

        public static TimeContext Night => new TimeContext(0, false);

        public TimeContext(double time, bool dayTime)
        {
            this.time = time;
            this.dayTime = dayTime;
        }

        public void ChangeTime()
        {
            Main.dayTime = dayTime;
            Main.time = time;
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
}
