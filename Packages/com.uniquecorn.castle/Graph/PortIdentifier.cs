using System;

namespace Castle.Graph
{
    [System.Serializable]
    public struct PortIdentifier : System.IEquatable<PortIdentifier>
    {
        public long nodeID;
        public string portName;

        public PortIdentifier(long nodeID, string portName)
        {
            this.nodeID = nodeID;
            this.portName = portName;
        }

        public bool Equals(PortIdentifier other)
        {
            return nodeID == other.nodeID && portName == other.portName;
        }

        public override bool Equals(object obj)
        {
            return obj is PortIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nodeID, portName);
        }
    }
    [System.Serializable]
    public struct Connection
    {
        public PortIdentifier output;
        public PortIdentifier input;

        public bool TryGetInput(BaseGraph graph, out BasePortData port)
        {
            if (graph.GetNode<BaseNodeData>(input.nodeID, out var node))
            {
                return node.inputs.TryGetValue(input.portName, out port);
            }
            port = null;
            return false;
        }
        public bool TryGetOutput(BaseGraph graph, out BasePortData port)
        {
            if (graph.GetNode<BaseNodeData>(output.nodeID, out var node))
            {
                return node.outputs.TryGetValue(output.portName, out port);
            }
            port = null;
            return false;
        }
    }
}