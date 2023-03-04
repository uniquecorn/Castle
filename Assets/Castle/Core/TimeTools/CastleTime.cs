using System;

namespace Castle.Core.TimeTools
{
    public static class CastleTime
    {
        public static DateTime Now => simulatedTime ? simulatedCastleTime : DateTime.Now;
        public static bool simulatedTime;
        public static DateTime simulatedCastleTime;
        public static int Today => (int)Now.ToOADate();
        public static int Day => Now.TimeOfDay.TotalHours < 8 ? (int)Now.ToOADate() - 1 : (int)Now.ToOADate();
        public static void SetSimulatedTime(DateTime simTime)
        {
            simulatedCastleTime = simTime;
            simulatedTime = true;
        }
    }
}
