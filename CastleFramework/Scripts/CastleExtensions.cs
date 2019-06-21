using UnityEngine;
using System.Collections.Generic;

public static class CastleExtensions
{
    public static Vector3 ZeroZ(this Vector3 vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }
    public static List<T> Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        // exit if possitions are equal or outside array
        if ((oldIndex == newIndex) || (oldIndex < 0) || (oldIndex >= list.Count) || (newIndex < 0) ||
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
	public static void Shuffle<T>(this IList<T> list)
	{
		System.Random rng = new System.Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
    public static bool IsSafe<T>(this IList<T> list)
    {
        if (list == null)
            return false;
        if (list.Count == 0)
            return false;
        return true;
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
