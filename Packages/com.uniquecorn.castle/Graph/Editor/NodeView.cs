using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GraphViewBase;

namespace Castle.Graph.Editor
{
    public class NodeView : BaseNode
    {
        public BaseNodeData node;
        public Label TitleLabel { get; }
        //public Port[] ports;
        public NodeView(BaseNodeData node) : base()
        {
            this.node = node;
            name = node.GetType().Name;
            userData = node;
            viewDataKey = node.nodeID.ToString();
            style.width = node.NodeWidth;
            style.position = Position.Absolute;
            // style.left = node.position.x;
            // style.top = node.position.y;
            if(UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(node,out _,out long localId))
            {
                name = node.GetType().ToString() + localId;
            }
            else
            {
                name = node.GetType().ToString();
            }
            ElementAt(0).style.backgroundColor = new Color(0.2196078f, 0.2196078f, 0.2196078f, 1f);
            var inspector = new InspectorElement(node);
            var container = inspector.Q<IMGUIContainer>();
            container.cullingEnabled = true;
            ExtensionContainer.Add(inspector);
            TitleLabel = new(node.GetType().Name) {pickingMode = PickingMode.Ignore};
            TitleLabel.AddToClassList("node-title-label");
            TitleContainer.Add(TitleLabel);
        }

        public override void SetPosition(Vector2 newPos)
        {
            base.SetPosition(newPos);
            node.position = newPos;
        }
        public virtual void OpenContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete",a=>
            {
                if (Graph is CastleGraphView graphView)
                {
                    graphView.RemoveNode(this);
                }
            });
            evt.StopImmediatePropagation();
        }

        protected override void OnAddedToGraphView()
        {
            base.OnAddedToGraphView();
            transform.position = Graph.WorldToLocal(node.position);
            this.AddManipulator(new ContextualMenuManipulator(OpenContextualMenu));
            node.GetPorts(out var inputs,out var outputs);
            if (inputs.IsSafe())
            {
                foreach (var i in inputs)
                {
                    var port = new PortView(Orientation.Horizontal, Direction.Input, PortCapacity.Multi);
                    port.userData = i;
                    port.PortName = i.portName;
                    AddPort(port);
                }
            }

            if (outputs.IsSafe())
            {
                foreach (var o in outputs)
                {
                    var port = new PortView(Orientation.Horizontal, Direction.Output, PortCapacity.Multi);
                    port.userData = o;
                    port.PortName = o.portName;
                    AddPort(port);
                }
            }
        }
    }
}