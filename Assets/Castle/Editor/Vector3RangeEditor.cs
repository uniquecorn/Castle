using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(Vector3RangeAttribute))]
public class Vector3RangeAttributeDrawer : PropertyDrawer
{
    const int helpHeight = 30;
    const int textHeight = 16;
    Vector3RangeAttribute rangeAttribute {  get { return (Vector3RangeAttribute)attribute;  } }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Color previous = GUI.color;
        GUI.color = !IsValid(property) ? Color.red : Color.white;
        Rect textFieldPosition = position;
        textFieldPosition.width = position.width;
        textFieldPosition.height = position.height;
        EditorGUI.BeginChangeCheck();
        Vector3 val = EditorGUI.Vector3Field(textFieldPosition, label, property.vector3Value);
        if (EditorGUI.EndChangeCheck())
        {
            if (rangeAttribute.bClamp)
            {
                val.x = Mathf.Clamp(val.x, rangeAttribute.fMinX, rangeAttribute.fMaxX);
                val.y = Mathf.Clamp(val.y, rangeAttribute.fMinY, rangeAttribute.fMaxY);
                val.z = Mathf.Clamp(val.z, rangeAttribute.fMinZ, rangeAttribute.fMaxZ);
            }
            property.vector3Value = val;
        }
        Rect helpPosition = position;
        helpPosition.y += 16;
        helpPosition.height = 16;
        DrawHelpBox(helpPosition, property);
        GUI.color = previous;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!IsValid(property))
        {
            return 32;
        }
        return base.GetPropertyHeight(property, label);
    }
    void DrawHelpBox(Rect position, SerializedProperty prop)
    {
        // No need for a help box if the pattern is valid.
        if (IsValid(prop))
            return;

        EditorGUI.HelpBox(position,string.Format("Invalid Range X [{0}]-[{1}] Y [{2}]-[{3}]", rangeAttribute.fMinX,rangeAttribute.fMaxX,rangeAttribute.fMinY,rangeAttribute.fMaxY), MessageType.Error);
    }
    bool IsValid(SerializedProperty prop)
    {
        Vector3 vector = prop.vector3Value;
        return vector.x >= rangeAttribute.fMinX && vector.x <= rangeAttribute.fMaxX && vector.y >= rangeAttribute.fMinY && vector.y <= rangeAttribute.fMaxY;
    }
}