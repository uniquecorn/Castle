using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Castle.CastleShapes
{
    public static class Bezier {

        public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }
    
        public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            return
                2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1);
        }

    }
    public abstract class Shape
    {
        public abstract Vector3[] Vertices { get; }
        
        public Vector3[] VerticesWithCenter(Vector3 offset)
        {
            //returns Vertices but with [0] as offset, add offset top vertices undone
            Vector3[] vertices = new Vector3[Vertices.Length+1];
            vertices[0] = offset; //centerpoint = Vector3.zero + offset
            for (int i = 1; i < Vertices.Length + 1; i++)
            {
                vertices[i] = Vertices[i-1]+offset;
            }
            return vertices;
        }
        
        public void DrawTransformedGizmos(Transform transform)
        {
            var transformedVertices = new Vector3[Vertices.Length];
            for (var i = 0; i < Vertices.Length; i++)
            {
                transformedVertices[i] = transform.TransformPoint(Vertices[i]);
            }
            Draw(transformedVertices);
        }
        public void DrawTransformedGizmos(Transform transform,Vector3 offset)
        {
            var transformedVertices = new Vector3[Vertices.Length];
            for (var i = 0; i < Vertices.Length; i++)
            {
                transformedVertices[i] = transform.TransformPoint(Vertices[i])+offset;
            }
            Draw(transformedVertices);
        }
        public void Draw() => Draw(Vertices);

        
        public void Draw(Vector3 offset)
        {
            var transformedVertices = new Vector3[Vertices.Length];
            for (var i = 0; i < Vertices.Length; i++)
            {
                transformedVertices[i] = Vertices[i]+offset;
            }
            Draw(transformedVertices);
        }
        private static void Draw(Vector3[] vertices)
        {
            if (vertices == null) return;
            for (var i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawLine(vertices[i], i == vertices.Length - 1 ? vertices[0] : vertices[i + 1]);
            }
        }
    }
}