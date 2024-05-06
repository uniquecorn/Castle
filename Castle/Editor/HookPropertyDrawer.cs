using UnityEditor;
using UnityEngine;

namespace Castle.Editor
{
    public abstract class HookPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (TryFindTarget(property, out var component))
            {
                property.objectReferenceValue = component;
                return;
            }
            property.objectReferenceValue = null;
            EditorGUI.PropertyField(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            TryFindTarget(property, out _) ? 0 : base.GetPropertyHeight(property, label);

        protected bool TryFindTarget(SerializedProperty property, out Component component)
        {
            if (property.serializedObject.targetObject is Component main)
                return TryFindTarget(main, Tools.GetTypeFromProperty(property), out component);
            component = null;
            return false;
        }

        protected abstract bool TryFindTarget(Component main, System.Type type, out Component component);
    }
}