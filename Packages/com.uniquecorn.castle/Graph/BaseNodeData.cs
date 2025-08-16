using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
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

        public virtual void CreatePorts()
        {
            //SetInputs();
            //SetOutputs();
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
        public virtual bool CanConnectFrom(string input, PortIdentifier output)
        {
            return true;
        }
        public virtual bool CanConnectTo(string output, PortIdentifier input)
        {
            return true;
        }
#if UNITY_EDITOR
        public void GetPorts(out List<PortIdentifier> inputs, out List<PortIdentifier> outputs)
        {
            inputs = new List<PortIdentifier>(2);
            outputs = new List<PortIdentifier>(2);
            foreach (var attr in GetType().GetCustomAttributes<PortAttribute>(true))
            {
                foreach (var port in attr.ports)
                {
                    switch (attr)
                    {
                        case InputPortAttribute:
                            inputs.Add(new PortIdentifier(nodeID, port));
                            break;
                        case OutputPortAttribute:
                            outputs.Add(new PortIdentifier(nodeID, port));
                            break;
                    }
                }
            }
        }
#endif
    }
}