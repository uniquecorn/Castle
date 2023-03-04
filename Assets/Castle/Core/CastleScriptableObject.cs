using Sirenix.OdinInspector;

namespace Castle.Core
{

    public abstract class CastleScriptableObject : SerializedScriptableObject
    {
        public virtual void OnValidate() { }
#if UNITY_EDITOR
        public virtual bool TryToValidate()
        {
            OnValidate();
            return true;
        }
#endif
    }
    
    public abstract class CastleScriptableObject<T> : CastleScriptableObject where T : CastleScriptableObject
    {
        private static T _instance;
        public static CastleScriptableObjectManager Manager => CastleScriptableObjectManager.Instance;
        public static T Instance
        {
            get
            {
                if (!_instance && Manager.GetScriptableObject(out _instance))
                {
                    return _instance;
                }
                return null;
            }
        }
    }
}