using Castle.CastleShapes;

namespace Castle.CastleShapesUI
{
    public class PolygonUI : SquareBoundShapesUI<Polygon>, ICircular
    {
        protected override string LengthInspectorLabel => "Circumference";

        public float Radius
        {
            get => ShapeToDraw.Radius;
            set => ShapeToDraw.Radius = value;
        }
        public override float Length 
        {
            get => Radius*2;
            set => Radius = value/2;
        }
        public override float RectLength => Radius*2;

        protected override Polygon SpawnShape()
        {
            var defaultResolution = 5;
            return new Polygon(defaultResolution, MinRectLength / 2, 8, MinRectLength/defaultResolution);        
        }

        protected override void ShapeValidation()
        {
            
        }
    }
}