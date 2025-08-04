using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GraphViewBase;

namespace Castle.Graph.Editor
{
    public class NodeView : BaseNode
    {
        public BaseNodeData node;
        //public Port[] ports;
        public NodeView(BaseNodeData node) : base()
        {
            this.node = node;
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
                Graph.RemoveElement(this);
                node.graph.RemoveNode(node);
                //Debug.Log(node.bigText);
            });
            evt.StopImmediatePropagation();
        }

        protected override void OnAddedToGraphView()
        {
            base.OnAddedToGraphView();
            transform.position = Graph.WorldToLocal(node.position);
            this.AddManipulator(new ContextualMenuManipulator(OpenContextualMenu));
            if (node.inputs.IsSafe())
            {
                foreach (var i in node.inputs)
                {
                    var port = new BasePort(Orientation.Horizontal, Direction.Input, PortCapacity.Multi);
                    port.userData = i.Value;
                    port.PortName = i.Key;
                    AddPort(port);
                }
            }

            if (node.outputs.IsSafe())
            {
                foreach (var o in node.outputs)
                {
                    var port = new BasePort(Orientation.Horizontal, Direction.Output, PortCapacity.Multi);
                    port.userData = o.Value;
                    port.PortName = o.Key;
                    AddPort(port);
                }
            }
        }
    }
}