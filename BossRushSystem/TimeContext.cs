using Terraria;
using Terraria.ID;

namespace BossRush;

public partial class BossRushSystem
{
    /// <summary>
    /// Struct for containing information about the spawn requirements of a boss related to time.
    /// This is used in BossData struct.
    /// </summary>
    public struct TimeContext
    {
        /// <summary>
        /// The time of day to be set.
        /// Values should be similar as to how Terraria handles time.
        /// </summary>
        public double time;
        /// <summary>
        /// Whether it is day time or night time.
        /// Values should be similar as to how Terraria handles time.
        /// </summary>
        public bool dayTime;

        /// <summary>
        /// Returns the TimeContext for the beginning of a day.
        /// </summary>
        public static TimeContext Day => new(0, true);

        /// <summary>
        /// Returns the TimeContext for .
        /// </summary>
        public static TimeContext Noon => new(27000, true);

        /// <summary>
        /// Returns the TimeContext for the beginning of a night.
        /// </summary>
        public static TimeContext Night => new(0, false);

        /// <summary>
        /// Constructor for TimeContext.
        /// </summary>
        /// <param name="time">Similar to Main.time</param>
        /// <param name="dayTime">Similar to Main.dayTime></param>
        public TimeContext(double time, bool dayTime)
        {
            this.time = time;
            this.dayTime = dayTime;
        }

        /// <summary>
        /// Changes the time of day according to this struct.
        /// </summary>
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
