using UnityEngine;
using UnityEngine.UI;

namespace Castle
{
    public static class ColorExtensions
    {
        public static void Alpha(this Graphic g, float percent) => g.color = g.color.Alpha(percent);
        public static void Clear(this Graphic g) => g.color = g.color.Clear();
        public static void Full(this Graphic g) => g.color = g.color.Full();
        public static Color Alpha(this Color _color, float percent) => new Color(_color.r, _color.g, _color.b, percent);
        public static Color Clear(this Color _color) => new Color(_color.r, _color.g, _color.b, 0);
        public static Color Full(this Color _color) => new Color(_color.r, _color.g, _color.b, 1);
    }
}