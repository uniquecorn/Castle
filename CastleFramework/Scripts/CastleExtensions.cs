using UnityEngine;
using System.Collections.Generic;

public static class CastleExtensions
{
    public static List<T> Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        // exit if possitions are equal or outside array
        if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= list.Count) || (0 > newIndex) ||
            (newIndex >= list.Count)) return list;
        // local variables
        var i = 0;
        T tmp = list[oldIndex];
        // move element down and shift other elements up
        if (oldIndex < newIndex)
        {
            for (i = oldIndex; i < newIndex; i++)
            {
                list[i] = list[i + 1];
            }
        }
        // move element up and shift other elements down
        else
        {
            for (i = oldIndex; i > newIndex; i--)
            {
                list[i] = list[i - 1];
            }
        }
        // put element from position 1 to destination
        list[newIndex] = tmp;
        return list;
    }
    public static Color Percent(this Color _color,float percent)
    {
        return new Color(_color.r, _color.g, _color.b, percent);
    }
    public static Color Clear(this Color _color)
    {
        return new Color(_color.r, _color.g, _color.b, 0);
    }
    public static Color Full(this Color _color)
    {
        return new Color(_color.r, _color.g, _color.b, 1);
    }
}
