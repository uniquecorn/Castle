using UnityEngine;

namespace Castle.Shapes
{
    public abstract class Box : Shape
    {
        protected abstract float Width { get; }
        protected abstract float Height { get; }
        protected override Vector3[] Vertices
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