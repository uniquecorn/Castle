using UnityEngine;

namespace Castle.Shapes
{
    public class RoundedSquare : RoundedBox
    {
        public float size;
        protected override float Width => size;
        protected override float Height => size;
    }
}