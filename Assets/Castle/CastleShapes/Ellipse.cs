using UnityEngine;

namespace Castle.CastleShapes
{
    public class Ellipse : Circle
    {
        public Ellipse(int resolution, float radiusX, float radiusY, int roundedCornerRes=0, float roundedCornerRadius=0) : base(Mathf.Max(radiusX, radiusY),roundedCornerRes, roundedCornerRadius )
        {
            Resolution = resolution;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }

        protected override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[Resolution];
                for (var i = 0; i < vertices.Length; i++)
                {
                    var angle = i * (360f / Resolution);
                    vertices[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * RadiusX,Mathf.Cos(Mathf.Deg2Rad * angle) * RadiusY);
                }
                return vertices;
            }
        }
        public static void Draw(Vector3 offset,int resolution, float radiusX, float radiusY)
        {
            new Ellipse(resolution,radiusX,radiusY).Draw(offset);
        }
    }
}