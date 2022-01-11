using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Core.Range;
using UnityEngine;
using UnityEngine.UI;

namespace Castle.Tools
{
    public static class CastleExtensions
    {
        public static bool Check(this CastleValueRange.ConditionCheck check, bool param) => check == CastleValueRange.ConditionCheck.Less ? !param : param;

        public static bool Check(this CastleValueRange.ConditionCheck check, int param,int value) =>
            check switch
            {
                CastleValueRange.ConditionCheck.MoreOrEqual => param >= value,
                CastleValueRange.ConditionCheck.Equal => param == value,
                CastleValueRange.ConditionCheck.Less => param < value,
                _ => false
            };

        public static T GetInterface<T>(this GameObject inObj) where T : class
        {
            if (typeof(T).IsInterface) return inObj.GetComponents<Component>().OfType<T>().FirstOrDefault();
            Debug.LogError(typeof(T) + ": is not an actual interface!");
            return null;

        }
        public static IEnumerable<T> GetInterfaces<T>(this GameObject inObj) where T : class
        {
            if (typeof(T).IsInterface) return inObj.GetComponents<Component>().OfType<T>();
            Debug.LogError(typeof(T) + ": is not an actual interface!");
            return Enumerable.Empty<T>();
        }
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            var type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Instance | BindingFlags.Default | 
                                       BindingFlags.DeclaredOnly;
            var pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos) {
                if (pinfo.CanWrite) {
                    try {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            var finfos = type.GetFields(flags);
            foreach (var finfo in finfos) {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component => go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
    public static class VectorExtensions
    {
        public static Vector2 ClosestVector(this Vector2 vec, params Vector2[] args)
        {
            var dist = (vec - args[0]).sqrMagnitude;
            var num = 0;
            for (var i = 1; i < args.Length; i++)
            {
                var dist2 = (vec - args[i]).sqrMagnitude;
                if (dist <= dist2) continue;
                dist = dist2;
                num = i;
            }
            return args[num];
        }
        public static void Move2D(this Transform t, Vector3 pos) => t.position = pos.RepZ(t);
        public static void Move2D(this Transform t, Transform t2) => Move2D(t, t2.position);
        public static void Move2DLerp(this Transform t, Vector3 pos, float snappiness = 10) => t.position = Vector3.Lerp(t.position, pos.RepZ(t), Time.deltaTime * snappiness);
        public static Vector3 ZeroZ(this Vector3 vec) => RepZ(vec, 0);
        public static Vector3 RepZ(this Vector3 vec,float z) => new(vec.x, vec.y, z);
        public static Vector3 RepZ(this Vector3 vec,Transform t) => RepZ(vec,t.position.z);
        public static Vector3 RepZ(this Vector2 vec, float z) => new(vec.x, vec.y, z);
    }
    public static class ColorExtensions
    {
        public static void Alpha(this UnityEngine.UI.Graphic g, float percent) => g.color = g.color.Alpha(percent);
        public static void Clear(this UnityEngine.UI.Graphic g) => g.color = g.color.Clear();
        public static void Full(this UnityEngine.UI.Graphic g) => g.color = g.color.Full();
        public static Color Alpha(this Color _color, float percent) => new Color(_color.r, _color.g, _color.b, percent);
        public static Color Clear(this Color _color) => new Color(_color.r, _color.g, _color.b, 0);
        public static Color Full(this Color _color) => new Color(_color.r, _color.g, _color.b, 1);
    }

    public static class ColliderExtensions
    {
        public static float Area(this Collider2D coll)
        {
            switch (coll)
            {
                case PolygonCollider2D poly:
                    var result = 0f;
                    for (int p = poly.points.Length - 1, q = 0; q < poly.points.Length; p = q++)
                    {
                        result += Vector3.Cross(poly.points[q], poly.points[p]).magnitude;
                    }
                    return result/2;
                case CircleCollider2D circ:
                    return Mathf.PI * Mathf.Pow(circ.radius,2);
                case BoxCollider2D box:
                    return box.size.x * box.size.y;
                case CapsuleCollider2D cap:
                    var sizeY = (cap.size.y - cap.size.x) * cap.size.x;
                    var circle = Mathf.PI * Mathf.Pow(cap.size.x / 2,2);
                    return sizeY + circle;
                default:
                    return 1;
            }
        }
    }
    
    public static class SpriteRendererExtensions
    {
        
        public static void SetSorting(this SpriteRenderer spriteRenderer, int sortingLayerID, int sortingOrder)
        {
            spriteRenderer.sortingLayerID = sortingLayerID;
            spriteRenderer.sortingOrder = sortingOrder;
        }
        public static void SetSorting(this SpriteRenderer[] renderers, int sortingLayerID, int sortingOrder)
        {
            if (renderers == null) return;
            foreach (var spriteRenderer in renderers)
            {
                SetSorting(spriteRenderer,sortingLayerID,sortingOrder);
            }
        }
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/SpriteRenderer/Set Pivot", false, 1)]
        public static void PivotPoint(UnityEditor.MenuCommand command)
        {
            SpriteRenderer SR = ((SpriteRenderer)command.context);
            SR.SetPivot();
        }
        public static void SetPivot(this SpriteRenderer SR)
        {
            if (SR.transform.localPosition.magnitude < 0.00001f)
            {
                return;
            }
            var assetPath = UnityEditor.AssetDatabase.GetAssetOrScenePath(SR.sprite);
            var assetMetaPath = Application.dataPath + assetPath.Remove(0, 6) + ".meta";
            var yaml = CastleTools.ReadTextFile(assetMetaPath);
            var yamlSplit = yaml.Split(new string[] { "spriteSheet:", "spritePackingTag" }, System.StringSplitOptions.None);
            var yamlSplit2 = yamlSplit[1].Split(new string[] { "outline:", "physicsShape:" }, System.StringSplitOptions.None);
            var outlineCoords = new List<string>();
            for (var i = 0; i < yamlSplit2.Length; i++)
            {
                if (i % 2 > 0)
                {
                    outlineCoords.Add(yamlSplit2[i]);
                }
            }

            var texImport = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(assetPath);
            var texSettings = new UnityEditor.TextureImporterSettings();

            texImport.ReadTextureSettings(texSettings);
            var zPos = SR.transform.localPosition.z;
            var flip = new Vector2(SR.flipX ? -1 : 1, SR.flipY ? -1 : 1);
            var localPosToPixelPivot = SR.transform.localPosition * texImport.spritePixelsPerUnit * (Vector2)SR.transform.localScale * flip;
            var pixelPivotToNormalPivot = localPosToPixelPivot;
            if (texImport.spritesheet.IsSafe())
            {
                var spritesMetaData = texImport.spritesheet;
                for (var i = 0; i < spritesMetaData.Length; i++)
                {
                    if (spritesMetaData[i].name == SR.sprite.name)
                    {
                        var metaData = spritesMetaData[i];
                        if (metaData.alignment != 9)
                        {
                            metaData.alignment = 9;
                            metaData.pivot = Vector2.one / 2;
                        }
                        pixelPivotToNormalPivot = new Vector2(localPosToPixelPivot.x / texImport.spritesheet[i].rect.width, localPosToPixelPivot.y / texImport.spritesheet[i].rect.height);
                        metaData.pivot -= pixelPivotToNormalPivot;

                        spritesMetaData[i] = metaData;
                        break;
                    }
                }
                texImport.spritesheet = spritesMetaData;
                texImport.SetTextureSettings(texSettings);
                UnityEditor.EditorUtility.SetDirty(texImport);
                texImport.SaveAndReimport();
                SR.transform.localPosition = new Vector3(0, 0, zPos);
            }
            else
            {
                texSettings.spriteAlignment = 9;
                texImport.SetTextureSettings(texSettings);
                pixelPivotToNormalPivot = new Vector2(localPosToPixelPivot.x / SR.sprite.rect.width, localPosToPixelPivot.y / SR.sprite.rect.height);
                texImport.spritePivot -= pixelPivotToNormalPivot;
                UnityEditor.EditorUtility.SetDirty(texImport);
                texImport.SaveAndReimport();
                SR.transform.localPosition = new Vector3(0, 0, zPos);
            }
            yaml = CastleTools.ReadTextFile(assetMetaPath);
            yamlSplit = yaml.Split(new string[] { "spriteSheet:", "spritePackingTag" }, System.StringSplitOptions.None);
            yamlSplit2 = yamlSplit[1].Split(new string[] { "outline:", "physicsShape:" }, System.StringSplitOptions.None);
            var j = 0;
            for (var i = 0; i < yamlSplit2.Length; i++)
            {
                if (i % 2 > 0)
                {
                    yamlSplit2[i] = "outline:" + outlineCoords[j] + "physicsShape:";
                    j++;
                }
            }
            yamlSplit[1] = "";
            for (var i = 0; i < yamlSplit2.Length; i++)
            {
                yamlSplit[1] += yamlSplit2[i];
            }
            yaml = yamlSplit[0] + "spriteSheet:" + yamlSplit[1] + "spritePackingTag" + yamlSplit[2];
            System.IO.File.SetAttributes(assetMetaPath, System.IO.FileAttributes.Normal);
            CastleTools.WriteTextFile(assetMetaPath, yaml);
            texImport.SaveAndReimport();
        }
        #endif
    }

    public static class ArrayExtensions
    {
        public static T RandomValue<T>(this IEnumerable<T> list) => list.ElementAt(Random.Range(0, list.Count()));
        public static T RandomValue<T>(this IEnumerable<T> list, System.Func<T, bool> filter) => list.Where(filter).RandomValue();
        public static T LoopFrom<T>(this IList<T> list, int startingPos, int i) => list[(startingPos + i) % list.Count];
        public static bool IsSafe(this IList list)
        {
            if (list == null) return false;
            return list.Count != 0;
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
        public static void Shuffle(this IList list,bool seeded= false,int seed = 0)
        {
            var rng = seeded ? new System.Random(seed) : new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source,bool seeded= false,int seed = 0)
        {
            var rng = seeded ? new System.Random(seed) : new System.Random();
            return source.Select(x => new {Number = rng.Next(), Item = x}).OrderBy(x => x.Number).Select(x => x.Item);
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
                if (arr[i] == null)
                {
                    arr.RemoveAt(i);
                }
            }
        }
        public static T[] ClearNullEntries<T>(this T[] array) where T : class
        {
            var arr = array.ToList();
            for (var i = arr.Count-1; i >= 0; i--)
            {
                if (arr[i] == null)
                {
                    arr.RemoveAt(i);
                }
            }
            return arr.ToArray();
        }
    }

    public static class StringExtensions
    {
        public static string Shorten(this string value, int delete) => value[..^delete];

        public static string NameNoExtension(this FileInfo file) => file.Name.Shorten(file.Extension.Length);
    }

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

    public static class UIExtensions
    {
        
        [UnityEditor.MenuItem("CONTEXT/LayoutGroup/Set Size", false, 1)]
        public static void SetSize(UnityEditor.MenuCommand command)
        {
            LayoutGroup RT = ((LayoutGroup)command.context);
            RT.SetSize();
        }
        public static void SetSize(this LayoutGroup layoutGroup)
        {
            if (layoutGroup.TryGetComponent<RectTransform>(out var rt))
            {
                rt.sizeDelta = new Vector2(layoutGroup.preferredWidth, layoutGroup.preferredHeight);
            }
        }

        public static int CountCornersVisible(this RectTransform rectTransform, Camera cam)
        {
            var screenBounds = CastleTools.ScreenBounds;
            var objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
            var visibleCorners = 0;
            for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
            {
                if (screenBounds.Contains(cam.WorldToScreenPoint(objectCorners[i]))) // If the corner is inside the screen
                {
                    visibleCorners++;
                }
            }
            return visibleCorners;
        }
        public static bool IsFullyVisibleFrom(this RectTransform rectTransform,Camera cam) => CountCornersVisible(rectTransform,cam) == 4;
        public static bool IsVisibleFrom(this RectTransform rectTransform,Camera cam) => CountCornersVisible(rectTransform,cam) > 0;
    }
}
