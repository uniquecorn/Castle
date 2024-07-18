using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Graph
{
    [HideMonoScript]
    public class BaseNode : ScriptableObject
    {
        [HideInInspector]
        public long nodeID;
        [HideInInspector]
        public Vector2 position;

        public virtual int NodeWidth => 500;
        [Title("STRING TEST")] public string testField;
        [TextArea]
        public string bigText;
        public string[] testArray;
        [Button]
        void Test()
        {

        }
    }
}