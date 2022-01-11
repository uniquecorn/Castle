using System.Linq;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Castle.Core
{
    [CreateAssetMenu(fileName = "CastleScriptableObjectManager", menuName = "Castle/CastleScriptableObjectManager", order = 0)]
    public class CastleScriptableObjectManager : ScriptableObject
    {
        private static CastleScriptableObjectManager _instance;
        public static CastleScriptableObjectManager Instance => _instance ? _instance : FindInstance();
        #if ODIN_INSPECTOR
        [ReadOnly]
        #endif
        public CastleScriptableObject[] scriptableObjects;
#if ODIN_INSPECTOR
        [Button]
#else
        [ContextMenu("GetScriptableObjects")]
#endif
        public void GetAllScriptableObjects()
        {
            scriptableObjects = Resources.FindObjectsOfTypeAll<CastleScriptableObject>();
        }
        public bool GetScriptableObject<T>(out T obj) where T : CastleScriptableObject
        {
            Debug.Log(typeof(T));
            foreach (var so in scriptableObjects)
            {
                if (so is not T s) continue;
                obj = s;
                return true;
            }
            obj = null;
            return false;
        }
        private static CastleScriptableObjectManager FindInstance()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                _instance = Resources.FindObjectsOfTypeAll<CastleScriptableObjectManager>().FirstOrDefault();
                return _instance;
            }
#endif
            _instance = Resources.FindObjectsOfTypeAll<CastleScriptableObjectManager>().FirstOrDefault();
            return _instance;
        }
    }
}