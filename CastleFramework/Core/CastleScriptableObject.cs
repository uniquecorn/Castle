using UnityEngine;

namespace Castle.Core
{
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    public abstract class CastleScriptableObject : SerializedScriptableObject
    {
#if UNITY_EDITOR
        public virtual bool TryToValidate()
        {
            return true;
        }
#endif
    }
#else
public abstract class CastleScriptableObject : ScriptableObject
{
#if UNITY_EDITOR
    public virtual bool TryToValidate()
    {
        return true;
    }
#endif
}
#endif
    public abstract class CastleScriptableObject<T> : CastleScriptableObject where T : CastleScriptableObject
    {
        private static T _instance;
        public static CastleScriptableObjectManager Manager => CastleScriptableObjectManager.Instance;
        public static T Instance
        {
            get => Manager.GetScriptableObject(out _instance) ? _instance : null;
            set => _instance = value;
        }
    }
}