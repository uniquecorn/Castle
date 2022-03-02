using UnityEngine;

namespace Castle.CastleShapes
{
    public abstract class Box : Shape
    {
        public abstract float Width { get; set; }
        public abstract float Height { get; set; }

        public override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[4];
                vertices[0] = new Vector3(-Width / 2, -Height / 2);
                vertices[1] = new Vector3(-Width / 2, Height / 2);
                vertices[2] = new Vector3(Width / 2, Height / 2);
                vertices[3] = new Vector3(Width / 2, -Height / 2);
                return vertices;
            }
        }
    }
}