using UnityEngine;

namespace Castle.Shapes
{
    public abstract class Shape
    {
        protected abstract Vector3[] Vertices { get; }
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