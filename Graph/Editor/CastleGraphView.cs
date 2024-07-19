using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Castle.Graph.Editor
{
    public class CastleGraphView : GraphView
    {
        new class UxmlFactory : UxmlFactory<CastleGraphView, UxmlTraits> { }
        private BaseGraph graph;
        private MiniMap miniMap;
        public CastleGraphView()
        {
            //styleSheets.Add(Resources.Load<StyleSheet>("Graph"));
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Setup Grid
            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
            miniMap = new MiniMap {anchored = false};
            miniMap.SetPosition(new Rect(10, 30, 300, 300));
            Add(miniMap);
        }

        public void Inspect(BaseGraph graph)
        {
            graphViewChanged -= OnGraphViewChanged;
            this.graph = graph;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            if (graph != null)
            {
                foreach (var node in graph)
                {
                    var view = new NodeView(node);
                    AddElement(view);
                }

                foreach (var connection in graph.connections)
                {
                    if (connection.TryGetInput(graph, out var input) && connection.TryGetOutput(graph, out var output))
                    {
                        var n = graphElements.FirstOrDefault(x => x is NodeView node && node.node == input.node);
                        var c = graphElements.FirstOrDefault(x => x is NodeView node && node.node == output.node);
                        AddElement(n.Q<Port>(input.name).ConnectTo(c.Q<Port>(output.name)));
                        // n.MarkDirtyRepaint();
                        // c.MarkDirtyRepaint();
                    }
                }
            }
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    var inputNode = edge.input.userData as BasePort;
                    var outputNode = edge.output.userData as BasePort;
                    var connection = new Connection()
                    {
                        input = new PortIdentifier(inputNode.node.nodeID, inputNode.name),
                        output = new PortIdentifier(outputNode.node.nodeID, outputNode.name)
                    };
                    graph.connections = graph.connections.AddToArray(connection);
                }
            }
            return change;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if(!Application.isPlaying && graph != null)
            {
                Vector2 mousePosition = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
                var types = graph.GetNodeTypes();
                foreach (var t in types)
                {
                    evt.menu.AppendAction($"Create/{t.Name}", a => AddState(mousePosition));
                }
                base.BuildContextualMenu(evt);
            }
        }

        void AddState(Vector2 mousePosition)
        {

            graph.AddNode<BaseNode>(mousePosition);
            Inspect(graph);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()!.Where(endPort =>
                    endPort.direction != startPort.direction &&
                    endPort.node != startPort.node &&
                    endPort.portType == startPort.portType)
                .ToList();
        }

    }
}

// public class CastleGraphNode : Node
// {
//     public CastleNode node;
//
//     public void BindToNode(CastleNode node)
//     {
//         this.node = node;
//         SetPosition(new Rect(node.position,new Vector2(500,500)));
//     }
//     // public VisualElement CreateEditorFromNodeData()
//     // {
//     //     var propertyField = new PropertyField(i
//     // }
// }