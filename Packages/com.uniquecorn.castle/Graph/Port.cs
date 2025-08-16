using System;
using UnityEngine;

namespace Castle.Graph
{
    [System.Serializable]
    public abstract class BasePortData
    {
        [HideInInspector,System.NonSerialized]
        public BaseNodeData node;
        public string name;
        public BasePortData(string name) => this.name = name;
        public abstract bool IsCompatible(BasePortData port);
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

    // public class InputPort : BasePortData
    // {
    //     public InputPort(string name) : base(name) { }
    //     public override bool IsCompatible(BasePortData port) => port is OutputPort outputPort &&
    //                                                             IsOutputCompatible(outputPort) &&
    //                                                             outputPort.IsInputCompatible(this);
    //     public virtual bool IsOutputCompatible(OutputPort port)
    //     {
    //         return true;
    //     }
    // }
    //
    // public class OutputPort : BasePortData
    // {
    //     public OutputPort(string name) : base(name) { }
    //     public override bool IsCompatible(BasePortData port) => port is InputPort inputPort &&
    //                                                             IsInputCompatible(inputPort) &&
    //                                                             inputPort.IsOutputCompatible(this);
    //     public virtual bool IsInputCompatible(InputPort port) => true;
    // }
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class PortAttribute : System.Attribute
    {
        public string[] ports;
        public PortAttribute(params string[] ports) => this.ports = ports;
    }

    public class InputPortAttribute : PortAttribute
    {
        public InputPortAttribute(params string[] ports) : base(ports){}
    }

    public class OutputPortAttribute : PortAttribute
    {
        public OutputPortAttribute(params string[] ports) : base(ports){}
    }
}