using UnityEngine;

namespace Castle.CastleShapes
{
    public class Star : Circle
    {
        public Star(int resolution, float innerRadius, float outerRadius, int roundedCornerRes=0, float roundedCornerRadius=0) : base(resolution, outerRadius, roundedCornerRes, roundedCornerRadius)
        {    
            Radius = outerRadius;
            Resolution = resolution;
            InnerRadius = innerRadius;
            CornerResolution = roundedCornerRes;
            CornerRadius = roundedCornerRadius;
        }

        public float InnerRadius { get; set; }

        protected override Vector3[] Vertices
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
        
        public static void Draw(Vector3 offset,int resolution,float innerRad, float outerRad, int roundedCornerRes=0, float roundedCornerRadius=0)
        {
            new Star(resolution ,innerRad, outerRad,roundedCornerRes,roundedCornerRadius).Draw(offset);
        }
    }
}
