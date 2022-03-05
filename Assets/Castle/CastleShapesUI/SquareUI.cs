using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;

namespace Castle.CastleShapesUI
{
    public class SquareUI : ShapesUI<Square,SquareBoundEnum>
    {
        [BoxGroup("Dimensions"), LabelText("Size"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectSize
        {
            get => ShapeToDraw.Size;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float Size
        {
            get => ShapeToDraw.Size;
            set => ShapeToDraw.Size = value;
        }
        
        public override SquareBoundEnum BoundBy
        {
            get => boundBy;
            set
            {
                boundBy = value;
                if (!BoundByRect) return;
                ShapeValidation();
                ResizeByRect();
            }
        }

        private SquareBoundEnum boundBy;
    
        protected override void OnEnable()
        {
            base.OnEnable();
            var rect = Transform.rect;
            ShapeToDraw = new Square(MinRectLength);
        }

        protected override void ResizeByRect()
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

        protected override void ShapeValidation(){}
    
    }
}
