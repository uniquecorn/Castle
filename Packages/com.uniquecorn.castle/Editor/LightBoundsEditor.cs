using Castle.Rendering.Lights;
using UnityEditor;
using UnityEngine;

namespace Castle.Editor
{
    [CustomEditor(typeof(LightBounds))]
    public class LightBoundsEditor : UnityEditor.Editor
    {
        private const float handleSize = 0.04f;
        private const float pickSize = 0.06f;
        public void OnSceneGUI()
        {
            if (target is not LightBounds lightBounds) return;
            var mouseWorldPoint = MouseWorldPoint(lightBounds.transform);
            var closestEdgePoint = Vector3.zero;
            var closestEdgeIndex = -1;
            var closestEdgeDistance = 0f;
            var snapped = false;
            for (var i = 0; i < lightBounds.points.Length; i++)
            {
                var point = lightBounds.TransformedPointWrapped(i);
                if (!snapped) snapped = (point - mouseWorldPoint).sqrMagnitude < 0.01f;
                var size = HandleUtility.GetHandleSize(point) * (snapped ? 1.5f : 1f);
                var next = lightBounds.TransformedPointWrapped(i + 1);
                var mouseEdgePoint = ClosestPointInEdge(point,next,mouseWorldPoint, out var sqrDistanceToMouse);
                if (i == 0 || closestEdgeDistance > sqrDistanceToMouse)
                {
                    closestEdgePoint = mouseEdgePoint;
                    closestEdgeIndex = i;
                    closestEdgeDistance = sqrDistanceToMouse;
                }
                Handles.DrawLine(point, next);
                if (Event.current.alt)
                {
                    if (!Handles.Button(point, Quaternion.identity, size * handleSize, size * pickSize,
                            Handles.DotHandleCap)) continue;
                    Undo.RecordObject(lightBounds, "Remove Point");
                    ArrayUtility.RemoveAt(ref lightBounds.points, i);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    point = Handles.FreeMoveHandle(point, size * handleSize,Vector3.zero,Handles.DotHandleCap);
                    if (!EditorGUI.EndChangeCheck()) continue;
                    Undo.RecordObject(lightBounds, "Move Point");
                    lightBounds.points[i] = lightBounds.transform.InverseTransformPoint(point);
                }
                lightBounds.SetDirty();
            }
            if (!snapped && !Event.current.alt && closestEdgeIndex >= 0)
            {
                var size = HandleUtility.GetHandleSize(closestEdgePoint);
                if (!Handles.Button(closestEdgePoint, Quaternion.identity, size * handleSize, size * pickSize,
                        Handles.DotHandleCap)) return;
                ArrayUtility.Insert(ref lightBounds.points, closestEdgeIndex + 1,
                    closestEdgePoint - lightBounds.transform.position);
                lightBounds.SetDirty();
            }
        }
        Vector3 ClosestPointInEdge(Vector2 p0,Vector2 p1,Vector2 mouseWorldPoint, out float sqrDistanceToMouse)
        {
            var dir1 = mouseWorldPoint - p0;
            var dir2 = p1 - p0;
            var point = p0 + (Mathf.Clamp01(Vector2.Dot(dir1, dir2.normalized) / dir2.magnitude) * dir2);
            sqrDistanceToMouse = (point - mouseWorldPoint).sqrMagnitude;
            return point;
        }
        Vector3 MouseWorldPoint(Transform transform) => GUIToWorld(Event.current.mousePosition,transform.forward,transform.localToWorldMatrix.MultiplyPoint3x4(Vector3.zero));
        Vector3 GUIToWorld(Vector2 guiPosition, Vector3 planeNormal, Vector3 planePos)
        {
            var worldPos = Handles.inverseMatrix.MultiplyPoint(guiPosition);
            if (!Camera.current) return worldPos;
            var ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            planeNormal = Handles.matrix.MultiplyVector(planeNormal);
            planePos = Handles.matrix.MultiplyPoint(planePos);
            var plane = new Plane(planeNormal, planePos);
            if (plane.Raycast(ray, out var distance))
            {
                worldPos = Handles.inverseMatrix.MultiplyPoint(ray.GetPoint(distance));
            }
            return worldPos;
        }
    }
}