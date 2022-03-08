using System;
using UnityEngine;

namespace Castle.CastleShapes
{
    [Serializable]
    public class Circle : Polygon
    {

        public Circle(float radius, int roundedCornerRes=0, float roundedCornerRadius=0) : base(128, radius, roundedCornerRes, roundedCornerRadius)
        {
        }

        public override int Resolution => resolution;

        public static void Draw(Vector3 offset,float radius, int roundedCornerRes=0, float roundedCornerRadius=0)
        {
            new Circle(radius, roundedCornerRes, roundedCornerRadius).Draw(offset);
        }
    }
}
