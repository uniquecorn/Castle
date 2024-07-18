using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Graph
{
    public abstract class BaseGraph : ScriptableObject, IEnumerable<BaseNode>
    {
        public abstract bool GetNode<T>(long id, out T node) where T : BaseNode;
        public abstract IEnumerator<BaseNode> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#if UNITY_EDITOR
        public abstract System.Type[] GetNodeTypes();
        public T CreateNode<T>() where T : BaseNode
        {
            UnityEditor.Undo.RecordObject(this,"CreateNode");
            var node = CreateInstance(typeof(T)) as T;
            node.name = typeof(T).ToString();
            return node;
        }
        public abstract void AddNode<T>(Vector2 position) where T : BaseNode;
        public abstract bool IsDirty();
        [Button,ShowIf("IsDirty")]
        public void Save()
        {
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}