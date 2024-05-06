using UnityEditor;
using UnityEngine;

namespace Castle.Editor
{
    [CustomPropertyDrawer(typeof(ParenthookAttribute))]
    public class ParenthookPropertyDrawer : HookPropertyDrawer
    {
        protected override bool TryFindTarget(Component main,System.Type type,out Component component)
        {
            if(main.transform.parent!=null) return main.transform.parent.TryGetComponent(type, out component);
            component = null;
            return false;
        }
    }
}