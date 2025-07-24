using UnityEngine;

namespace Castle.Graph
{
    [System.Serializable]
    public class BasePortData
    {
        [HideInInspector,System.NonSerialized]
        public BaseNodeData node;
        public string name;

        public BasePortData(string name)
        {
            this.name = name;
        }
        public virtual System.Type portType => typeof(BaseNodeData);
        // public bool TryGetIdentifier(out PortIdentifier identifier)
        // {
        //     if (node.inputs != null)
        //     {
        //         for (var i = 0; i < node.inputs.Length; i++)
        //         {
        //             if (node.inputs[i] == this)
        //             {
        //                 identifier = new PortIdentifier(node.nodeID, -(i + 1));
        //                 return true;
        //             }
        //         }
        //     }
        //     if (node.outputs != null)
        //     {
        //         for (var i = 0; i < node.outputs.Length; i++)
        //         {
        //             if (node.outputs[i] == this)
        //             {
        //                 identifier = new PortIdentifier(node.nodeID, i);
        //                 return true;
        //             }
        //         }
        //     }
        //
        //     identifier = default;
        //     return false;
        // }
    }
}