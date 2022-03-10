using System;
using UnityEngine;

namespace Castle.CastleShapes
{
    [Serializable]
    public class Star : Polygon
    {
        [SerializeField, HideInInspector]
        private float innerRadius;

        public Star(int resolution, float outerRadius, float innerRadius,  int roundedCornerRes=0, float roundedCornerRadius=0) : base(resolution, outerRadius, roundedCornerRes, roundedCornerRadius)
        {    
            Radius = outerRadius;
            InnerRadius = innerRadius;
            CornerResolution = roundedCornerRes;
            CornerRadius = roundedCornerRadius;
        }

        public float InnerRadius
        {
            get => innerRadius;
            set => innerRadius = value;
        }

        public override int MaxResolution => 512;

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
        
        public static void Draw(Vector3 offset,int resolution, float outerRad,float innerRad, int roundedCornerRes=0, float roundedCornerRadius=0)
        {
            new Star(resolution, outerRad, innerRad,roundedCornerRes,roundedCornerRadius).Draw(offset);
        }
    }
}
