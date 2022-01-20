using UnityEngine;

namespace Castle.Shapes
{
    public class Ellipse : Shape
    {
        public int resolution;
        public float radiusX;
        public float radiusY;
        protected override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[resolution];
                for (var i = 0; i < vertices.Length; i++)
                {
                    var angle = i * (360f / resolution);
                    vertices[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX,Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY);
                }
                return vertices;
            }
        }
        public static void Draw(Vector3 offset,float radiusX, float radiusY , int resolution)
        {
            new Ellipse{radiusX = radiusX, radiusY = radiusY ,resolution = resolution}.Draw(offset);
        }
    }
}