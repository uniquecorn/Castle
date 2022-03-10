
using Castle.CastleShapesUI;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Castle.Editor
{
    public abstract class OffsetHandleBase: OdinEditor
    {
        protected void OnSceneGUI()
        {
            var targetHandler = (BaseShapeUI) target;
            if (!targetHandler.EnableOffsetMove) return;
            Vector3 newTargetPosition = targetHandler.transform.position + targetHandler.Offset;
            float size = HandleUtility.GetHandleSize(newTargetPosition) * 0.3f;
            Vector3 snap = Vector3.one * 0.5f;
            
            EditorGUI.BeginChangeCheck();
            Handles.color = Color.green;;
            var point = Handles.FreeMoveHandle(newTargetPosition, Quaternion.identity, size, snap,Handles.CircleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                //Will be affected by scale, fix later.
                
                Undo.RecordObject(targetHandler, "Move Offset");
                targetHandler.Offset = point-targetHandler.transform.position;
                EditorUtility.SetDirty(targetHandler);
            
                targetHandler.SetVerticesDirty();
                
            }
        }
    }
    
    [CustomEditor(typeof(CircleUI)), CanEditMultipleObjects]
    public class OffsetHandleCircle:OffsetHandleBase{}
    
    [CustomEditor(typeof(EllipseUI)), CanEditMultipleObjects]
    public class OffsetHandleEllipse:OffsetHandleBase{}
    
    [CustomEditor(typeof(SquareUI)), CanEditMultipleObjects]
    public class OffsetHandleSquare:OffsetHandleBase{}
    
    [CustomEditor(typeof(RectangleUI)), CanEditMultipleObjects]
    public class OffsetHandleRectangle:OffsetHandleBase{}
    
    [CustomEditor(typeof(PolygonUI)), CanEditMultipleObjects]
    public class OffsetHandlePolygon:OffsetHandleBase{}
    
    [CustomEditor(typeof(StarUI)), CanEditMultipleObjects]
    public class OffsetHandleStar:OffsetHandleBase{}
    
}