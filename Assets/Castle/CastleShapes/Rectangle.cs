
namespace Castle.CastleShapes
{
    public class Rectangle : Box
    {
        public Rectangle(float width, float height)
        {
            Width = width;
            Height = height;
        }
        
        private float height;
        private float width;

        public override float Width 
        {
            get => width;
            set => width = value;
        }

        public override float Height 
        {
            get => height;
            set => height = value;
        }
    }
}