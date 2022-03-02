using Castle.Shapes;
using UnityEngine;

namespace Castle.CastleShapes
{
    public class Star : Circle
    {
        public Star(int resolution, float innerRadius, float outerRadius) : base(resolution, outerRadius)
        {    
            Radius = outerRadius;
            Resolution = resolution;
            InnerRadius = innerRadius;

            // innerCircle.Radius = innerRadius;
            // innerCircle.Resolution = resolution;
            // innerCircle = new Circle(resolution, innerRadius);

        }
        
        // private readonly Circle innerCircle;
        
        public float InnerRadius
        {
            // get => innerCircle.Radius;
            // set => innerCircle.Radius = value;
            get;
            set;
        }

        public override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[Resolution*2];
                vertices[0] = new Vector3(0, Radius);
                vertices[1] = new Vector3(0, InnerRadius);
                // vertices[1] = new Vector3(0, innerCircle.Radius);
                vertices[1] = Quaternion.Euler(0, 0, -180f / Resolution) * vertices[1];
                for (var i = 2; i < vertices.Length; i++)
                {
                    vertices[i] = Quaternion.Euler(0, 0, -360f / Resolution) * vertices[i - 2];
                }
                return vertices;
            }
        }
        
        public static void Draw(Vector3 offset,int resolution,float innerRad, float outerRad)
        {
            new Star(resolution ,innerRad, outerRad).Draw(offset);
        }
    }
}
