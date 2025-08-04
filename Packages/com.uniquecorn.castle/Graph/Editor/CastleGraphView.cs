using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using GraphViewBase;

namespace Castle.Graph.Editor
{
    [UxmlElement]
    public partial class CastleGraphView : GraphView
    {
        private BaseGraph graph;
        public CastleGraphView() : base()
        {
            this.AddManipulator(new ContextualMenuManipulator(OpenContextualMenu));
        }
        public void Inspect(BaseGraph graph)
        {
            this.graph = graph;
            ClearView();
            if (graph != null)
            {
                foreach (var node in graph)
                {
                    var view = new NodeView(node);
                    AddElement(view);
                }
                if (graph.connections.IsSafe())
                {
                    foreach (var connection in graph.connections)
                    {
                        if (connection.TryGetInput(graph, out var input) && connection.TryGetOutput(graph, out var output))
                        {
                            ContentContainer.Q<BasePort>(input.name).ConnectTo(ContentContainer.Q<BasePort>(output.name));
                            // n.MarkDirtyRepaint();
                            // c.MarkDirtyRepaint();
                        }
                    }
                }
            }
        }

        public override void OnActionExecuted(Actions actionType, object data = null)
        {
            if (actionType == Actions.EdgeCreate && data is Edge edge)
            {
                var inputNode = edge.Input.userData as BasePortData;
                var outputNode = edge.Output.userData as BasePortData;
                var connection = new Connection()
                {
                    input = new PortIdentifier(inputNode.node.nodeID, inputNode.name),
                    output = new PortIdentifier(outputNode.node.nodeID, outputNode.name)
                };
                graph.connections = graph.connections.AddToArray(connection);
            }
        }

        public virtual void OpenContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = graph.GetNodeTypes();
            foreach (var t in types)
            {
                evt.menu.AppendAction($"Create/{t.Name}", a =>
                {
                    if (graph.AddNode(t, this.WorldToLocal(mousePosition), out var node))
                    {
                        var nodeView = new NodeView(node);
                        AddElement(nodeView);
                        nodeView.MarkDirtyRepaint();
                    }
                });
            }
        }
        public void ClearView() {
            //this.Unbind();

            foreach (BaseNode node in ContentContainer.Nodes) {
                node.RemoveFromHierarchy();
            }

            foreach (BasePort port in ContentContainer.Ports) {
                port.RemoveFromHierarchy();
            }

            foreach (BaseEdge edge in ContentContainer.Edges) {
                edge.RemoveFromHierarchy();
            }
        }
        // public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        // {
        //     return ports.ToList()!.Where(endPort =>
        //             endPort.direction != startPort.direction &&
        //             endPort.node != startPort.node &&
        //             endPort.portType == startPort.portType)
        //         .ToList();
        // }

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