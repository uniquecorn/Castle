using Castle.CastleShapes;
using Sirenix.OdinInspector;

namespace Castle.CastleShapesUI
{
    public class PolygonUI : ShapesUI<Polygon, SquareBoundEnum>
    {

        private SquareBoundEnum boundBy;

        [BoxGroup("Dimensions"),ShowInInspector, PropertyRange(2,16)]
        public int Resolution
        {
            get => ShapeToDraw.Resolution;
            set => ShapeToDraw.Resolution = value;
        }
        public override SquareBoundEnum BoundBy
        {
            get => boundBy;
            set
            {
                boundBy = value;
                ShapeUpdate();
            }
        }
        
        [BoxGroup("Dimensions"), LabelText("Radius"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectRadius => ShapeToDraw.Radius;

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float Radius
        {
            get => ShapeToDraw.Radius;
            set => ShapeToDraw.Radius = value;
        }


        protected override void ResizeByRect()
        {
            var rect = Transform.rect;
            Radius = BoundBy switch
            {
                SquareBoundEnum.Height => rect.height/2,
                SquareBoundEnum.Width => rect.width/2,
                SquareBoundEnum.SmallestLength => MinRectLength/2,
                SquareBoundEnum.WidestLength => MaxRectLength/2,
                _ => Radius
            };
            // SetShape();
        }
        

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