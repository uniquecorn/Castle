using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    public class RectangleUI : ShapesUI<Rectangle,RectangleBoundEnum>
    {
        
        [SerializeField, HideInInspector]
        private RectangleBoundEnum boundBy;
        
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
        protected override Rectangle SpawnShape()
        {
            var rect = Transform.rect;
            return new Rectangle(rect.width, rect.height, 10, MinRectLength/4);     
        }
    }
}
