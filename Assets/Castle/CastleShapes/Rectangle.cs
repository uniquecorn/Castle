
using System;
using UnityEngine;

namespace Castle.CastleShapes
{
    [Serializable]
    public class Rectangle : Square
    {       
        [SerializeField, HideInInspector]
        private float height;

        public Rectangle(float width, float height, int roundedCornerRes=0, float roundedCornerRadius=0) 
            : base(Mathf.Max(width, height), roundedCornerRes,roundedCornerRadius)
        {
            Width = width;
            Height = height;
        }

        public override float Size
        {
            get => Mathf.Max(Width, Height);
            set { 
                Height = value;
                Width = value;
            }
        }
        public float Width
        {
            get => width;
            set => width = value;
        }

        public float Height
        {
            get => height;
            set => height = value;
        }


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

        public static void Draw(Vector3 offset,float width, float height,int roundedCornerRes=0, float roundedCornerRadius=0, float maxRadius = 0, bool clamped = false)
        {
            new Rectangle(width,height,roundedCornerRes,roundedCornerRadius).Draw(offset);
        }
    }
}