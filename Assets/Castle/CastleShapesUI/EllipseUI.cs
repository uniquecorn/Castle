using Castle.CastleShapes;
using Castle.Shapes;
using Sirenix.OdinInspector;

namespace Castle.CastleShapesUI
{
    public class EllipseUI : ShapesUI<Ellipse, RectangleBoundEnum>
    {
        [ShowInInspector, PropertyRange(1, 60)]
        public int Resolution
        {
            get => ShapeToDraw.Resolution;
            set => ShapeToDraw.Resolution = value;
        }
    
        [BoxGroup("Dimensions"), ShowInInspector]
        public override bool BoundByRect { 
            get => boundByRect;
            set
            {
                boundByRect = value;
                if (!BoundByRect) return;
                ShapeValidation();
                ResizeByRect();
            } 
        }

        private bool boundByRect;
    
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
    
        [BoxGroup("Dimensions"), LabelText("RadiusX"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectRadiusX
        {
            get => ShapeToDraw.RadiusX;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float RadiusX
        {
            get => ShapeToDraw.RadiusX;
            set => ShapeToDraw.RadiusX = value;
        }
    
        [BoxGroup("Dimensions"), LabelText("RadiusY"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectRadiusY
        {
            get => ShapeToDraw.RadiusY;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float RadiusY
        {
            get => ShapeToDraw.RadiusY;
            set => ShapeToDraw.RadiusY = value;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            var rect = Transform.rect;
            ShapeToDraw = new Ellipse(30,rect.width/2,rect.height/2);
        }

        public override void ResizeByRect()
        {
            var rect = Transform.rect;
            switch (BoundBy)
            {
                case RectangleBoundEnum.WidthAndHeight:
                    RadiusX = rect.width/2;
                    RadiusY = rect.height/2;
                    break;
                case RectangleBoundEnum.Width:
                    RadiusX = rect.width/2;
                    break;
                case RectangleBoundEnum.Height:
                    RadiusY = rect.height/2;
                    break;
            }
        }
    
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }

        public override void ShapeValidation()
        {
        }
    }
}