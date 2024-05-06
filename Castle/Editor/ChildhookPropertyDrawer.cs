using UnityEditor;
using UnityEngine;

namespace Castle.Editor
{
    [CustomPropertyDrawer(typeof(ChildhookAttribute))]
    public class ChildhookPropertyDrawer : HookPropertyDrawer
    {
        protected override bool TryFindTarget(Component main, System.Type type, out Component component)
        {
            component = main.GetComponentInChildren(type);
            return component != null;
        }
    }
}