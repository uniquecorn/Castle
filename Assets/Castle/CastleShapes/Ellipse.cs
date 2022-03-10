using System;
using UnityEngine;

namespace Castle.CastleShapes
{
    [Serializable]
    public class Ellipse : Circle
    {
        [SerializeField, HideInInspector]
        private float radiusX;
        [SerializeField, HideInInspector]
        private float radiusY;

        public Ellipse(float radiusX, float radiusY, int roundedCornerRes=0, float roundedCornerRadius=0) 
            : base(Mathf.Max(radiusX, radiusY),roundedCornerRes, roundedCornerRadius )
        {
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public float RadiusX
        {
            get => radiusX;
            set => radiusX = value;
        }

        public float RadiusY
        {
            get => radiusY;
            set => radiusY = value;
        }

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
        public static void Draw(Vector3 offset, float radiusX, float radiusY)
        {
            new Ellipse(radiusX,radiusY).Draw(offset);
        }
    }
}