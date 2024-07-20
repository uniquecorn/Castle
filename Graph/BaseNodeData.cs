using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Graph
{
    [HideMonoScript]
    public class BaseNodeData : ScriptableObject
    {
        [HideInInspector,System.NonSerialized]
        public BaseGraph graph;
        [HideInInspector]
        public long nodeID;
        [HideInInspector]
        public Vector2 position;
        [HideInInspector]
        public CastleDictionary<string,BasePortData> inputs;
        [HideInInspector]
        public CastleDictionary<string,BasePortData> outputs;
        public virtual int NodeWidth => 500;
        [Title("STRING TEST")] public string testField;
        [TextArea]
        public string bigText;
        public string[] testArray;
        [Button]
        void Test()
        {
            Debug.Log("???");
        }

        public virtual void CreatePorts()
        {
            //SetInputs();
            //SetOutputs();
        }

        private void OnValidate()
        {
            CreatePorts();
        }

        public void SetInputs(params BasePortData[] ports)
        {
            if (inputs == null)
            {
                inputs = new CastleDictionary<string, BasePortData>(ports.Length);
            }
            else
            {
                inputs.Clear();
                inputs.EnsureCapacity(ports.Length);
            }
            for (var i = 0; i < ports.Length; i++)
            {
                ports[i].node = this;
                inputs.Add(ports[i].name,ports[i]);
            }
        }
        public void SetOutputs(params BasePortData[] ports)
        {
            if (outputs == null)
            {
                outputs = new CastleDictionary<string, BasePortData>(ports.Length);
            }
            else
            {
                outputs.Clear();
                outputs.EnsureCapacity(ports.Length);
            }
            for (var i = 0; i < ports.Length; i++)
            {
                ports[i].node = this;
                outputs.Add(ports[i].name,ports[i]);
            }
        }
        void Reset()
        {
            CreatePorts();
        }
    }
}