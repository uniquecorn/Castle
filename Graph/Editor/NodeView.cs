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
            //style.backgroundColor = new Color(0.1f,0.1f,0.1f,1f);
            style.width = node.NodeWidth;
            style.position = Position.Absolute;
            style.left = node.position.x;
            style.top = node.position.y;

            if(UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(node,out _,out long localId))
            {
                name = node.GetType().ToString() + localId;
            }
            else
            {
                name = node.GetType().ToString();
            }

            foreach (var i in node.inputs)
            {
                var port = new BasePort(Orientation.Horizontal, Direction.Input, PortCapacity.Multi);
                port.userData = i.Value;
                port.PortName = i.Key;
                AddPort(port);
            }
            foreach (var o in node.outputs)
            {
                var port = new BasePort(Orientation.Horizontal, Direction.Output, PortCapacity.Multi);
                port.userData = o.Value;
                port.PortName = o.Key;
                AddPort(port);

            }


            ElementAt(0).style.backgroundColor = new Color(0.2196078f, 0.2196078f, 0.2196078f, 1f);
            var inspector = new InspectorElement(node);
            var container = inspector.Q<IMGUIContainer>();
            container.cullingEnabled = true;
            ExtensionContainer.Add(inspector);
            // using (var tree = PropertyTree.Create(node))
            // {
            //     var container = new IMGUIContainer(() =>
            //     {
            //         Sirenix.Utilities.Editor.GUIHelper.PushLabelWidth(100);
            //         tree.BeginDraw(true);
            //         foreach (var property in tree.EnumerateTree(true, true))
            //         {
            //             property.Draw();
            //         }
            //         tree.EndDraw();
            //         Sirenix.Utilities.Editor.GUIHelper.PopLabelWidth();
            //     }) {name = "OdinTree"};
            //     container.style.marginBottom = container.style.marginLeft =
            //         container.style.marginRight = container.style.marginTop = 4;
            //     extensionContainer.Add(container);
            // }
        }

        public override void SetPosition(Vector2 newPos)
        {
            base.SetPosition(newPos);
            node.position = newPos;
        }
    }
}