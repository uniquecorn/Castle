using UnityEngine;

namespace Castle.Shapes
{
    public class RoundedRectangle : RoundedBox
    {
        public float width, height;
        protected override float Width => width;
        protected override float Height => height;
    }
}