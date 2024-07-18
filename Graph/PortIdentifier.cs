namespace Castle.Graph
{
    [System.Serializable]
    public struct PortIdentifier
    {
        public string nodeID;
        public string portID;
    }
    [System.Serializable]
    public struct Connection
    {
        public PortIdentifier output;
        public PortIdentifier input;
    }
}