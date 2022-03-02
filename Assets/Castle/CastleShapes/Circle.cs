using Castle.Shapes;
using UnityEngine;

namespace Castle.CastleShapes
{
    public class Circle : Shape, IRoundedCorners
    {
        public Circle(int resolution, float radius)
        {
            Resolution = resolution;
            Radius = radius;

            CornerRadius = 100;
            CornerResolution = 5;
        }
        
        public int Resolution { get; set; }
        public float Radius { get; set; }
        public int CornerResolution { get; set; }
        public float CornerRadius { get; set; }

        public override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[Resolution];
                vertices[0] = new Vector3(0, Radius);
                for (var i = 1; i < vertices.Length; i++)
                {
                    vertices[i] = Quaternion.Euler(0, 0, -360f / Resolution) * vertices[i - 1];
                }
                return vertices;
            }
        }

        public static void Draw(Vector3 offset,int resolution,float radius)
        {
            new Circle(resolution ,radius).Draw(offset);
        }
    }
}
