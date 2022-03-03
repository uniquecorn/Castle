using UnityEngine;

namespace Castle.CastleShapes
{
    public class Square : Shape
    {
        //Rider throws warning about having virtual property in constructor that gets overriden in derived class.
        private float size;

        public Square(float size, int roundedCornerRes = 0, float roundedCornerRadius = 0) : base(roundedCornerRes,
            roundedCornerRadius)
        {
            this.size = size;
        }

        public virtual float Size
        {
            get => size;
            set => size = value;
        }

        protected override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[4];
                vertices[0] = new Vector3(-Size / 2, -Size / 2);
                vertices[1] = new Vector3(-Size / 2, Size / 2);
                vertices[2] = new Vector3(Size / 2, Size / 2);
                vertices[3] = new Vector3(Size / 2, -Size / 2);
                return vertices;
            }
        }

        public static void Draw(Vector3 offset, float size, int roundedCornerRes = 0, float roundedCornerRadius = 0)
        {
            new Square(size, roundedCornerRes, roundedCornerRadius).Draw(offset);
        }
    }
}