using Terraria;
using Terraria.ID;

namespace BossRush.Types;

public struct TimeContext(double time, bool dayTime)
{
    public double Time { get; private set; } = time;
    public bool DayTime { get; private set; } = dayTime;
    public static TimeContext Day => new(0, true);
    public static TimeContext Noon => new(27000, true);
    public static TimeContext Night => new(0, false);

    public readonly void ChangeTime()
    {
        Main.dayTime = DayTime;
        Main.time = Time;
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(MessageID.WorldData);
        }
    }
}
