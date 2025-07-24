using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using UnityEngine;

namespace Castle
{
    public static class Extensions
    {
        public static bool GetParentWith(this Transform transform, System.Func<Transform, bool> trait,out Transform parent)
        {
            var p = transform.parent;
            while (p != null)
            {
                if (trait(p))
                {
                    parent = p;
                    return true;
                }
                p = p.parent;
            }
            parent = null;
            return false;
        }
        public static bool GetParentComponent<T>(this Transform transform, out T component) where T : Object
        {
            var p = transform.parent;
            while (p != null)
            {
                if (p.TryGetComponent<T>(out var x))
                {
                    component = x;
                    return true;
                }
                p = p.parent;
            }
            component = null;
            return false;
        }
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
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component => go.AddComponent<T>().GetCopyOf(toAdd);
    }
}