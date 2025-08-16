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
                        TryConnect(connection);
                    }
                }
            }
        }

        public override void OnActionExecuted(Actions actionType, object data = null)
        {
            //Debug.Log(actionType);
            switch (actionType)
            {
                case Actions.EdgeCreate or Actions.EdgeDrop or Actions.EdgeDelete when data is Edge edge:
                    if (actionType == Actions.EdgeCreate)
                    {
                        var connection = new Connection()
                        {
                            input = (PortIdentifier)edge.Input.userData,
                            output = (PortIdentifier)edge.Output.userData
                        };
                        UnityEditor.ArrayUtility.Add(ref graph.connections, connection);
                        //graph.connections = graph.connections.AddToArray(connection);
                        AddElement(edge);
                    }
                    else if (actionType == Actions.EdgeDelete)
                    {
                        //edge.Input.userData
                        for (var i = 0; i < graph.connections.Length; i++)
                        {
                            if(!graph.connections[i].input.Equals(edge.Input.userData))continue;
                            if(!graph.connections[i].output.Equals(edge.Output.userData))continue;
                            UnityEditor.ArrayUtility.RemoveAt(ref graph.connections, i);
                            edge.RemoveFromHierarchy();
                            break;
                        }
                    }
                    break;
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
        public void RemoveNode(NodeView node)
        {
            foreach (var edge in ContentContainer.Edges)
            {
                if (edge.Input.ParentNode != node && edge.Output.ParentNode != node) continue;
                edge.RemoveFromHierarchy();
            }
            graph.RemoveNode(node.node);
            RemoveElement(node);
        }
        public void TryConnect(Connection connection)
        {
            var found = false;
            BasePort foundPort = null;
            foreach (var port in ContentContainer.Ports)
            {
                if (!found)
                {
                    if (connection.input.Equals(port.userData) || connection.output.Equals(port.userData))
                    {
                        found = true;
                        foundPort = port;
                    }
                }
                else
                {
                    if (foundPort.Direction == Direction.Input)
                    {
                        if (connection.output.Equals(port.userData))
                        {
                            ConnectPorts(foundPort,port);
                            return;
                        }
                    }
                    else if (foundPort.Direction == Direction.Output)
                    {
                        if (connection.input.Equals(port.userData))
                        {
                            ConnectPorts(port,foundPort);
                            return;
                        }
                    }
                }
            }
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