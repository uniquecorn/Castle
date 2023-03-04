using Sirenix.Utilities;
using UnityEngine;

namespace Castle
{
    [GlobalConfig("Assets/Castle/"),CreateAssetMenu]
    public class Settings : GlobalConfig<Settings>
    {
        public float QuickTapTimerThreshold = 0.2f;
        public float QuickTapDistanceThreshold = 3.5f;
        public bool useAltProductName;
        public string altProductName;
    }
}