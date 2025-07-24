using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Castle
{
    public static class ArrayExtensions
    {
        public static T RandomValue<T>(this IEnumerable<T> list) => list.ElementAt(Random.Range(0, list.Count()));
        public static T RandomValue<T>(this IEnumerable<T> list, System.Func<T, bool> filter) => list.Where(filter).RandomValue();
        public static T LoopFrom<T>(this IList<T> list, int startingPos, int i) => list[(startingPos + i) % list.Count];
        public static bool IsSafe<T>(this IList<T> list) => list != null && list.Count != 0;
        public static bool IsSafe<T>(this ICollection<T> collection) => collection != null && collection.Count != 0;
        public static bool IsSafe<T>(this IList<T> list,int count) => list != null && list.Count == count;
        public static bool IsSafe<T>(this ICollection<T> collection,int count) => collection != null && collection.Count == count;
        public static bool IsSafe<T>(this IEnumerable<T> enumerable) => enumerable != null && enumerable.Any();
        public static bool InRange<T>(this ICollection<T> collection, int index) => collection != null && index >= 0 && collection.Count > index;
        public static bool Get<T>(this ICollection<T> collection, int index,out T item)
        {
            if(InRange(collection,index))
            {
                item = collection.ElementAt(index);
                return true;
            }
            item = default;
            return false;
        }
        public static int SafeLength(this IList list) => list?.Count ?? 0;
        public static IList Swap(this IList list, int oldIndex, int newIndex)
        {
            (list[oldIndex], list[newIndex]) = (list[newIndex], list[oldIndex]);
            return list;
        }
        public static void Shift<T>(this List<T> list, int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex || oldIndex < 0 || oldIndex >= list.Count || newIndex < 0 ||
                newIndex >= list.Count) return;
            var tmp = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex,tmp);
        }
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            // exit if positions are equal or outside array
            if ((oldIndex == newIndex) || (oldIndex < 0) || (oldIndex >= list.Count) || (newIndex < 0) ||
                (newIndex >= list.Count)) return;
            // local variables
            int i;
            var tmp = list[oldIndex];
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
        }
        public static IList<T> Shuffle<T>(this IList<T> list,System.Random rng)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }
        public static IList Shuffle(this IList list,bool seeded= false,int seed = 0)
        {
            var rng = seeded ? new System.Random(seed) : new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }
        public static T[] ShuffleArray<T>(this IList<T> list,bool seeded= false,int seed = 0)
        {
            var arr = new T[list.Count];
            list.CopyTo(arr,0);
            var rng = seeded ? new System.Random(seed) : new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (arr[k], arr[n]) = (arr[n], arr[k]);
            }
            return arr;
        }
        public static T[] ShuffleSelect<T>(this IList<T> list, int length, bool seeded = false, int seed = 0)
        {
            var arr = new T[list.Count];
            list.CopyTo(arr,0);
            var rng = seeded ? new System.Random(seed) : new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (arr[k], arr[n]) = (arr[n], arr[k]);
            }

            return arr[..length];
        }
        public static T[] AddToArray<T>(this T[] array, T variable)
        {
            var arr = array.ToList();
            arr.Add(variable);
            return arr.ToArray();
        }
        public static T[] RemoveFromArray<T>(this T[] array, T variable)
        {
            var arr = array.ToList();
            arr.Remove(variable);
            return arr.ToArray();
        }
        public static void ClearNullEntries<T>(this List<T> arr) where T : class
        {
            for (var i = arr.Count-1; i >= 0; i--)
            {
                if (arr[i] != null) continue;
                arr.RemoveAt(i);
            }
        }
        public static T[] ClearNullEntries<T>(this T[] array) where T : class
        {
            var arr = array.ToList();
            for (var i = arr.Count-1; i >= 0; i--)
            {
                if (arr[i] != null) continue;
                arr.RemoveAt(i);
            }
            return arr.ToArray();
        }
        public static bool TryGetPooledObject<T>(this IList<T> array, out T obj) where T : Component
        {
            foreach (var o in array)
            {
                if(o.gameObject.activeSelf)continue;
                obj = o;
                obj.gameObject.SetActive(true);
                return true;
            }
            obj = null;
            return false;
        }
    }
}