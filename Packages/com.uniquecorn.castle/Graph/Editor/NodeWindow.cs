using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;

namespace Castle.Graph.Editor
{
    public class NodeWindow : EditorWindow
    {
        public VisualTreeAsset m_InspectorXML;
        public CastleGraphView graphView;
        private BaseGraph graph;
        [MenuItem("Window/Node Window")]
        private static void OpenWindow()
        {
            GetWindow<NodeWindow>().Show();
        }
        private static void OpenWindow(BaseGraph graph)
        {
            var w = GetWindow<NodeWindow>();
            w.graph = graph;
            w.graphView.Inspect(graph);
            w.Show();
        }
        [OnOpenAsset]
        public static bool OnStateMachineOpened(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is BaseGraph graph)
            {
                OpenWindow(graph);
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            //Debug.Log("gui recreated!");
            m_InspectorXML.CloneTree(rootVisualElement);
            graphView = rootVisualElement.Q<CastleGraphView>();
            graphView.StretchToParentSize();
            if (graph != null)
            {
                graphView.Inspect(graph);
            }
        }
        void OnSelectionChange()
        {
            if (Selection.activeObject is BaseGraph graph)
            {
                graphView.Inspect(graph);
            }
        }

        private void OnInspectorUpdate()
        {
            if (graph != null)
            {
                hasUnsavedChanges = graph.IsDirty();
            }
        }

        public override void SaveChanges()
        {
            graph.Save();
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
        }
    }
}