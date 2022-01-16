using UnityEngine;

namespace Castle.Shapes
{
    public class Square : Box
    {
        public float size;
        protected override float Width => size;
        protected override float Height => size;
    }
}