

using Castle.CastleShapes;

namespace Castle.Shapes
{
    public class Square : Box
    {
        public Square(float size)
        {
            this.size = size;
        }
        
        private float size;
        
        public override float Width 
        {
            get => size;
            set => size = value;
        }
        public override float Height
        {
            get => size;
            set => size = value;
        }
    }
}