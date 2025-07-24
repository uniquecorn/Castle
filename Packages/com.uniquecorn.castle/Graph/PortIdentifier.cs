namespace Castle.Graph
{
    [System.Serializable]
    public struct PortIdentifier
    {
        public long nodeID;
        public string portName;

        public PortIdentifier(long nodeID, string portName)
        {
            this.nodeID = nodeID;
            this.portName = portName;
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