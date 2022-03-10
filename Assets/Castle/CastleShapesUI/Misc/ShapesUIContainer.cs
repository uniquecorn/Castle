using System;
using System.Collections;
using System.Collections.Generic;
using Castle.CastleShapesUI;
using UnityEditor;
using UnityEngine;


public class ShapesUIContainer : MonoBehaviour
{
    [SerializeField]
    public ShapesUIContainerData Container;

    private void OnEnable()
    {
        Container.ShapeToPick = (BaseShapeUI)EditorGUILayout.ObjectField(Container.ShapeToPick, typeof(BaseShapeUI), false);
    }
}


[Serializable]
public class ShapesUIContainerData
{
    [SerializeField] public enum MyEnum
    {
        
    }
    [SerializeField]
    public BaseShapeUI[] Shapes;

    [SerializeField] public BaseShapeUI ShapeToPick;
}
