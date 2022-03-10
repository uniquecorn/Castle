using Castle.CastleShapes;

namespace Castle.CastleShapesUI
{
    public class CircleUI : SquareBoundShapesUI<Circle>, ICircular
    {
        protected override string LengthInspectorLabel => "Circumference";
        public float Radius
        {
            get => ShapeToDraw.Radius;
            set => ShapeToDraw.Radius = value;
        }
        public override float Length 
        {
            get => ShapeToDraw.Radius*2;
            set => ShapeToDraw.Radius = value/2;
        }
        public override float RectLength
        {
            get => ShapeToDraw.Radius*2;
        }
        
        protected override Circle SpawnShape()
        {
            
            return new Circle(MinRectLength/2);
        }

        protected override void ShapeValidation()
        {
            
        }

    }
}