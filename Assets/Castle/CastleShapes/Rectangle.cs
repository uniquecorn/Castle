using UnityEngine;

namespace Castle.Shapes
{
    [System.Serializable]
    public class Rectangle : Box
    {
        public float width, height;
        protected override float Width => width;
        protected override float Height => height;
    }
}