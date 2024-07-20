using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Graph
{
    public abstract class BaseGraph : ScriptableObject, IEnumerable<BaseNodeData>
    {
        public Connection[] connections;
        public abstract bool GetNode<T>(long id, out T node) where T : BaseNodeData;
        public abstract IEnumerator<BaseNodeData> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#if UNITY_EDITOR
        public abstract System.Type[] GetNodeTypes();
        public T CreateNode<T>() where T : BaseNodeData
        {
            UnityEditor.Undo.RecordObject(this,"CreateNode");
            var node = CreateInstance(typeof(T)) as T;
            node.graph = this;
            node.name = typeof(T).ToString();
            return node;
        }
        public abstract void AddNode<T>(Vector2 position,bool batchEdit=false) where T : BaseNodeData;
        public abstract bool IsDirty();
        [Button,ShowIf("IsDirty")]
        public void Save()
        {
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}