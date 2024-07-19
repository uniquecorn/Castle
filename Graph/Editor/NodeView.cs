using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace Castle.Graph.Editor
{
    public class NodeView : Node
    {
        public BaseNode node;
        //public Port[] ports;
        public NodeView(BaseNode node) : base()
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
                title = node.GetType().ToString() + localId;
            }
            else
            {
                title = node.GetType().ToString();
            }

            foreach (var i in node.inputs)
            {
                var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, i.Value.portType);
                port.userData = i.Value;
                port.name = port.portName = i.Value.name;
                inputContainer.Add(port);
            }
            foreach (var o in node.outputs)
            {
                var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, o.Value.portType);
                port.userData = o.Value;
                port.name = port.portName = o.Value.name;
                outputContainer.Add(port);
            }
            // input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Node));
            // inputContainer.Add(input);
            // output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Node));
            // outputContainer.Add(output);

            ElementAt(0).style.backgroundColor = new Color(0.2196078f, 0.2196078f, 0.2196078f, 1f);
            var inspector = new InspectorElement(node);
            var container = inspector.Q<IMGUIContainer>();
            container.cullingEnabled = true;
            extensionContainer.Add(inspector);
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
            RefreshExpandedState();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.position = newPos.position;
        }
    }
}