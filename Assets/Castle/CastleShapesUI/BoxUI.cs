using Castle.CastleShapes;
using Castle.Shapes;
using Sirenix.OdinInspector;

namespace Castle.CastleShapesUI
{
    public abstract class BoxUI<TBindableEnum> : ShapesUI<Box, TBindableEnum> where TBindableEnum : System.Enum
    {
        
        [BoxGroup("Dimensions"), ShowInInspector]
        public override bool BoundByRect { 
            get => boundByRect;
            set
            {
                boundByRect = value;
                ShapeValidation();
                ResizeByRect();
            }
        }
        private bool boundByRect;
    
        [BoxGroup("Dimensions"), ShowIf("BoundByRect"), ShowInInspector]
        public override TBindableEnum BoundBy {  
            get => boundBy;
            set
            {
                boundBy = value;
                if (!BoundByRect) return;
                ShapeValidation();
                ResizeByRect();
            }
        }
        private TBindableEnum boundBy;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }

    }
}
