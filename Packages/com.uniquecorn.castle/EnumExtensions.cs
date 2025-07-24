using System.Collections.Generic;

namespace Castle
{
    public static class EnumExtensions
    {
        public static T[] GetFlags<T>(this T @enum) where T : System.Enum
        {
            var enums = (T[])System.Enum.GetValues(typeof(T));
            var flags = new List<T>();
            for (var i = 0; i < enums.Length; i++)
            {
                if (@enum.FlagExists(enums[i]))
                {
                    flags.Add(enums[i]);
                }
            }
            return flags.ToArray();
        }
        public static bool FlagExists(this System.Enum @enum, System.Enum flag) =>
            (System.Convert.ToInt32(@enum) & System.Convert.ToInt32(flag)) != 0;
        public static bool FlagsOverlap<T>(this T @enum, T enum2) where T : System.Enum
        {
            var enums = GetFlags(enum2);
            for (var i = 0; i < enums.Length; i++)
            {
                if (@enum.FlagExists(enums[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}