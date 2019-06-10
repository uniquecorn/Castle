using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public abstract class CastleScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
	static T _instance = null;
	public static T Instance
	{
		get
		{
			if (!_instance)
				_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
			return _instance;
		}
	}
#if UNITY_EDITOR
    public static T EditorInstance
    {
        get
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:"+ typeof(T).Name);
            T data = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
            UnityEditor.EditorUtility.SetDirty(data);
            return data;
        }
    }
#endif
}