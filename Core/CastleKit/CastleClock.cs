using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Castle.Core
{
    public static class CastleClock
    {
        public const string Lib = "__Internal";
#if UNITY_IOS
        [DllImport (Lib)]
        private static extern double _GetDate();
#endif
        public static System.DateTime UTC
        {
            get
            {
#if UNITY_EDITOR
                return System.DateTime.UtcNow;
#else
                return System.DateTime.SpecifyKind(System.DateTime.UnixEpoch.AddMilliseconds(_GetDate()),
                    System.DateTimeKind.Utc);
#endif
            }
        }
    }
}