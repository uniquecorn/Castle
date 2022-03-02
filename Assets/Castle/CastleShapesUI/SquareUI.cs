using Castle.Shapes;
using Sirenix.OdinInspector;

namespace Castle.CastleShapesUI
{
    public class SquareUI : BoxUI<SquareBoundEnum>
    {
        [BoxGroup("Dimensions"), LabelText("Size"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectSize
        {
            get => ShapeToDraw.Width;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float Size
        {
            get => ShapeToDraw.Width;
            set => ShapeToDraw.Width = value;
        }
    
        protected override void OnEnable()
        {
            base.OnEnable();
            var rect = Transform.rect;
            ShapeToDraw = new Square(MinRectLength);
        }
    
        public override void ResizeByRect()
        {
            var rect = Transform.rect;
            Size = BoundBy switch
            {
                SquareBoundEnum.Height => rect.height,
                SquareBoundEnum.Width => rect.width,
                SquareBoundEnum.SmallestLength => MinRectLength,
                SquareBoundEnum.WidestLength => MaxRectLength,
                _ => Size
            };
        }

        public override void ShapeValidation(){}
    
    }
}
