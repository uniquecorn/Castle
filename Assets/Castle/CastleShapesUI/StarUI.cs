using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;

namespace Castle.CastleShapesUI
{
    public class StarUI : ShapesUI<Star, SquareBoundEnum>
    {
        [ShowInInspector, PropertyRange(1, 60)]
        public int Resolution
        {
            get => ShapeToDraw.Resolution;
            set => ShapeToDraw.Resolution = value;
        }

        [BoxGroup("Dimensions"), ShowIf("BoundByRect"), ShowInInspector]
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
        
        [BoxGroup("Dimensions"), LabelText("Radius"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectRadius
        {
            get => ShapeToDraw.Radius;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float Radius
        {
            get => ShapeToDraw.Radius;
            set => ShapeToDraw.Radius = value;
        }
        
        
        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float InnerRadius
        {
            get => ShapeToDraw.InnerRadius;
            set => ShapeToDraw.InnerRadius = value;
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
        }

        protected override void ShapeValidation()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ShapeToDraw = new Star(5,MinRectLength/4,MinRectLength/2);
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }
    }
}

