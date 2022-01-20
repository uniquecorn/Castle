using System;
using UnityEngine;

namespace Castle.Shapes
{
    [Serializable]
    public class Circle : Shape
    {
        public int resolution;
        public float radius;
        protected override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[resolution];
                vertices[0] = new Vector3(0, radius);
                for (var i = 1; i < vertices.Length; i++)
                {
                    vertices[i] = Quaternion.Euler(0, 0, 360f / resolution) * vertices[i - 1];
                }
                return vertices;
            }
        }

        public static void Draw(Vector3 offset,float radius,int resolution)
        {
            new Circle{radius = radius ,resolution = resolution}.Draw(offset);
        }
    }
}
