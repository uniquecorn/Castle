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
            // miniMap = new MiniMap {anchored = false};
            // miniMap.SetPosition(new Rect(10, 30, 300, 300));
            // Add(miniMap);
            var blackboard = new Blackboard(this);
            blackboard.Add(new BlackboardSection { title = "Graph Settings" });
            blackboard.SetPosition(new Rect(10, 30, 300, 300));
            Add(blackboard);
        }

        public void Inspect(BaseGraph graph)
        {
            this.graph = graph;
            DeleteElements(graphElements);
            if (graph != null)
            {
                foreach (var node in graph)
                {
                    var view = new NodeView(node);
                    AddElement(view);
                }
            }
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