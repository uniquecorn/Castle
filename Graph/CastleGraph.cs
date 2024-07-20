using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Castle.Graph
{
    public abstract class BaseGraph<TNode> : BaseGraph,ISerializationCallbackReceiver where TNode : BaseNodeData
    {
        public CastleDictionary<long, TNode> nodeDictionary;
        public override bool GetNode<T>(long id, out T node)
        {
            if (nodeDictionary.TryGetValue(id, out var n) && n is T n2)
            {
                node = n2;
                return true;
            }
            node = null;
            return false;
        }
#if UNITY_EDITOR
        [Button]
        public void Test()
        {
            AssetDatabase.StartAssetEditing();
            for (var i = 0; i < 100; i++)
            {
                AddNode<TNode>(new Vector2(Random.Range(-5000,5000),Random.Range(-5000,5000)),true);
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }
        public override bool IsDirty()
        {
            if (nodeDictionary != null)
            {
                foreach (var n in nodeDictionary)
                {
                    if (EditorUtility.IsDirty(n.Value))
                    {
                        return true;
                    }
                }
            }
            return EditorUtility.IsDirty(this);
        }
        public override IEnumerator<BaseNodeData> GetEnumerator() => nodeDictionary.Values.GetEnumerator();
        public override System.Type[] GetNodeTypes()
        {
            var types = TypeCache.GetTypesDerivedFrom<TNode>().ToList();
            types.Insert(0,typeof(TNode));
            return types.ToArray();
        }
        public void AddNode(Vector2 position) => AddNode<TNode>(position);
        public override void AddNode<T>(Vector2 position,bool batchEdit=false)
        {
            if (nodeDictionary == null) nodeDictionary = new CastleDictionary<long, TNode>();
            try
            {
                if (!batchEdit)
                {
                    AssetDatabase.StartAssetEditing();
                }

                var node = CreateNode<T>();
                node.position = position;
                if (node is TNode strongNode)
                {
                    AssetDatabase.AddObjectToAsset(strongNode, this);
                    EditorUtility.SetDirty(this);
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(strongNode, out _, out long localId))
                    {
                        strongNode.nodeID = localId;
                        nodeDictionary.Add(strongNode.nodeID, strongNode);
                    }
                    else
                    {
                        DestroyImmediate(strongNode);
                        Debug.LogError("Could not create node!");
                    }
                }
                else
                {
                    DestroyImmediate(node);
                    Debug.LogError("Could not create node!");
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (!batchEdit)
                {
                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.SaveAssets();
                }
            }
        }
#endif
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            foreach (var node in this)
            {
                node.graph = this;
                if (node.inputs != null)
                {
                    foreach (var port in node.inputs)
                    {
                        port.Value.node = node;
                    }
                }
                if (node.outputs != null)
                {
                    foreach (var port in node.outputs)
                    {
                        port.Value.node = node;
                    }
                }
            }
        }
    }
    [CreateAssetMenu(menuName = "Castle/Graph", order = 0)]
    public class CastleGraph : BaseGraph<BaseNodeData>
    {

    }
}