using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Castle.Core
{
    static class MonoPCallback
    {
        delegate void MonoPCallbackDelegate(IntPtr actionPtr, string data);
        [AOT.MonoPInvokeCallback(typeof(MonoPCallbackDelegate))]
        static void MonoPCallbackInvoke(IntPtr actionPtr, string data)
        {
            if (IntPtr.Zero.Equals(actionPtr)) return;

            var action = IntPtrToObject(actionPtr, false);
            if (action == null)
            {
                Debug.LogError("Callback not found");
                return;
            }

            try
            {
                var paramTypes = action.GetType().GetGenericArguments();
                var arg = paramTypes.Length == 0 ? null : ConvertObject(data, paramTypes[0]);

                var invokeMethod = action.GetType().GetMethod("Invoke", paramTypes.Length == 0 ? new Type[0] : new[] { paramTypes[0] });
                if (invokeMethod != null)
                    invokeMethod.Invoke(action, paramTypes.Length == 0 ? new object[] { } : new[] { arg });
                else
                    Debug.LogError("Failed to invoke callback " + action + " with arg " + data + ": invoke method not found");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to invoke callback " + action + " with arg " + data + ": " + e.Message);
            }
        }

        static object ConvertObject(string value, Type objectType)
        {
            if (value == null || objectType == typeof(string)) 
                return value;

            if (objectType == typeof(bool)) return Convert.ToBoolean(value);

            if (objectType == typeof(int)) return Convert.ToInt32(value);

            if (objectType == typeof(long)) return Convert.ToInt64(value);

            if (objectType == typeof(ulong)) return Convert.ToUInt64(value);

            if (objectType == typeof(float)) return Convert.ToSingle(value);

            return JsonUtility.FromJson(value, objectType);
        }

        public static IntPtr ObjectToIntPtr(object obj)
        {
            if (obj == null) return IntPtr.Zero;

            var handle = GCHandle.Alloc(obj);
            return GCHandle.ToIntPtr(handle);
        }

        public static IntPtr ActionToIntPtr<T>(Action<T> action)
        {
            return ObjectToIntPtr(action);
        }

        public static IntPtr ActionToIntPtr(Action action)
        {
            return ObjectToIntPtr(action);
        }

        public static object IntPtrToObject(IntPtr handle, bool unpinHandle)
        {
            if (IntPtr.Zero.Equals(handle)) return null;

            var gcHandle = GCHandle.FromIntPtr(handle);
            var result = gcHandle.Target;
            if (unpinHandle) gcHandle.Free();

            return result;
        }
    }
    [Serializable]
    public class CastleResult
    {
        [SerializeField]
        protected CastleError m_error = null;
        [SerializeField]
        protected string m_requestId = string.Empty;

        [SerializeField]
        protected string m_stringData = string.Empty;
        public bool HasError
        {
            get
            {
                if (m_error == null || string.IsNullOrEmpty(m_error.Message) && m_error.Code == default(int)) return false;
                return true;
            }
        }
        public bool IsSucceeded => !HasError;
        public bool IsFailed => HasError;
    }
    [Serializable]
    public class CastleError
    {
        [SerializeField]
        int m_code;
        [SerializeField]
        string m_message = string.Empty;
        public int Code => m_code;
        public string Message => m_message;
        public CastleError(int code, string message = "")
        {
            m_code = code;
            m_message = message;
        }
    }
}
