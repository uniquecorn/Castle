using GraphViewBase;
using UnityEngine;

namespace Castle.Graph.Editor
{
    public class PortView : BasePort
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public PortView(Orientation orientation, Direction direction, PortCapacity capacity) : base(orientation, direction, capacity)
        {
        }

        public override bool CanConnectTo(BasePort other, bool ignoreCandidateEdges = true)
        {
            if (other.ParentNode.userData is BaseNodeData otherNode && ParentNode.userData is BaseNodeData thisNode)
            {
                if (Direction == Direction.Output)
                {
                    if (!otherNode.CanConnectFrom(other.PortName, (PortIdentifier) userData)) return false;
                    if(!thisNode.CanConnectTo(PortName, (PortIdentifier)other.userData)) return false;
                }
                if (Direction == Direction.Input)
                {
                    if (!thisNode.CanConnectFrom(PortName, (PortIdentifier)other.userData)) return false;
                    if(!otherNode.CanConnectTo(other.PortName, (PortIdentifier)userData)) return false;
                }
            }
            return base.CanConnectTo(other, ignoreCandidateEdges);
        }
    }
}