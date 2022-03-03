using Castle.CastleShapes;
using Sirenix.OdinInspector;

namespace Castle.CastleShapesUI
{
    public class RectangleUI : ShapesUI<Rectangle,RectangleBoundEnum>
    {
        
        [BoxGroup("Dimensions"), LabelText("Height"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Height || BoundByRect && BoundBy == RectangleBoundEnum.WidthAndHeight"), ShowInInspector]
        public float RectHeight
        {
            get => ShapeToDraw.Height;
        }
    
        [BoxGroup("Dimensions"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Width || !BoundByRect"),ShowInInspector] 
        public float Height
        {
            get => ShapeToDraw.Height;
            set => ShapeToDraw.Height = value;
        }

        [BoxGroup("Dimensions"), LabelText("Width"),ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Width || BoundByRect && BoundBy == RectangleBoundEnum.WidthAndHeight"),ShowInInspector] 
        public float RectWidth
        {
            get => ShapeToDraw.Width;
        }
    
        [BoxGroup("Dimensions"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Height || !BoundByRect"), ShowInInspector] 
        public float Width
        {
            get => ShapeToDraw.Width;
            set => ShapeToDraw.Width = value;
        }
    
        protected override void OnEnable()
        {
            base.OnEnable();
            var rect = Transform.rect;
            ShapeToDraw = new Rectangle(rect.width, rect.height);
        }

        [BoxGroup("Dimensions"), ShowIf("BoundByRect"), ShowInInspector]
        public override RectangleBoundEnum BoundBy { 
            get => boundBy;
            set
            {
                boundBy = value;
                if (!BoundByRect) return;
                ShapeValidation();
                ResizeByRect();
            }
        }

        private RectangleBoundEnum boundBy;
        
        protected override void ResizeByRect()
        {
            var rect = Transform.rect;
            switch (BoundBy)
            {
                case RectangleBoundEnum.WidthAndHeight:
                    Width = rect.width;
                    Height = rect.height;
                    break;
                case RectangleBoundEnum.Width:
                    Width = rect.width;
                    break;
                case RectangleBoundEnum.Height:
                    Height = rect.height;
                    break;
            }
        }

        protected override void ShapeValidation(){}

    }
}
