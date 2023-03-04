using System.Linq;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

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
        [Button]
        public void GetAllScriptableObjects() => scriptableObjects = Resources.FindObjectsOfTypeAll<CastleScriptableObject>();
        public bool GetScriptableObject<T>(out T obj) where T : CastleScriptableObject
        {
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
            if (!EditorApplication.isPlaying)
            {
                _instance = Resources.FindObjectsOfTypeAll<CastleScriptableObjectManager>().FirstOrDefault();
                if (!_instance)
                {
                    _instance = EditorInstance;
                }
                return _instance;
            }
#endif
            _instance = Resources.FindObjectsOfTypeAll<CastleScriptableObjectManager>().FirstOrDefault();
            return _instance;
        }
#if UNITY_EDITOR
        public static CastleScriptableObjectManager EditorInstance
        {
            get
            {
                var data = AssetDatabase.LoadAssetAtPath<CastleScriptableObjectManager>("Assets/CastleScriptableObjectManager.asset")??DeepSearchInstance;
                if (data == null)
                {
                    return null;
                }
                EditorUtility.SetDirty(data);
                return data;
            }
        }
        protected static CastleScriptableObjectManager DeepSearchInstance
        {
            get
            {
                var guids = AssetDatabase.FindAssets("t:CastleScriptableObjectManager");
                return guids.IsSafe() ? AssetDatabase.LoadAssetAtPath<CastleScriptableObjectManager>(AssetDatabase.GUIDToAssetPath(guids[0])) : null;
            }
        }
        public bool ValidateAllScriptableObjects()
        {
            foreach (var so in scriptableObjects)
            {
                if (!so.TryToValidate())
                {
                    Debug.LogError(so.name +" is not valid!");
                    return false;
                }
            }
            return true;
        }
#endif

    }
}