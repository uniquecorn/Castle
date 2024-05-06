using UnityEngine;

namespace Castle.Core
{
    public static class Moon
    {
        private const double JulianConstant = 2415018.5;
        private const double TotalLengthOfCycle = 29.53;
        public enum MoonPhase
        {
            NewMoon,
            WaxingCrescent,
            FirstQuarter,
            WaxingGibbous,
            FullMoon,
            WaningGibbous,
            ThirdQuarter,
            WaningCrescent
        }
        public static MoonPhase GetPhase()
        {
            var period = TotalLengthOfCycle / 8;
            var julianDate = System.DateTime.UtcNow.ToOADate() + JulianConstant;
            var daysSinceLastNewMoon = new System.DateTime(1920, 1, 21, 5, 25, 00, System.DateTimeKind.Utc).ToOADate() + JulianConstant;
            var newMoons = (julianDate - daysSinceLastNewMoon) / TotalLengthOfCycle;
            var intoCycle = (newMoons - (int) newMoons) * TotalLengthOfCycle;
            var phase = MoonPhase.NewMoon;
            for (var i = 0; i < 8; i++)
            {
                if(intoCycle < i * period)continue;
                if(intoCycle > period * (i + 1))continue;
                phase = (MoonPhase) i;
            }
            return phase;
        }
    }
}