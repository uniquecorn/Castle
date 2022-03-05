using UnityEngine;

namespace Castle.CastleShapes
{
    public class Polygon : Shape
    {
        public Polygon(int resolution, float radius, int roundedCornerRes=0, float roundedCornerRadius=0) : base(roundedCornerRes, roundedCornerRadius)
        {
            this.resolution = resolution;
            Radius = radius;
        }
        

        public virtual int Resolution
        {
            get => resolution;
            set => resolution = value;
        }
        protected int resolution;

        public float Radius { get; set; }

        protected override Vector3[] Vertices
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

        public static void Draw(Vector3 offset,int resolution,float radius, int roundedCornerRes=0, float roundedCornerRadius=0)
        {
            new Polygon(resolution, radius, roundedCornerRes, roundedCornerRadius).Draw(offset);
        }
    }
}
