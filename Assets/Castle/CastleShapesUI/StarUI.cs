using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    public class StarUI : ShapesUI<Star, SquareBoundEnum>
    {
        [SerializeField, HideInInspector]
        private SquareBoundEnum boundBy;

        public override SquareBoundEnum BoundBy
        {
            get => boundBy;
            set
            {
                boundBy = value;
                ShapeUpdate();
            }
        }

        
        [BoxGroup("Dimensions"), ShowInInspector, PropertyRange(1, 60)]
        public int Resolution
        {
            get => ShapeToDraw.Resolution;
            set => ShapeToDraw.Resolution = value;
        }
        [BoxGroup("Dimensions"), LabelText("Radius"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectRadius
        {
            get => ShapeToDraw.Radius;
        }
        
        [BoxGroup("Dimensions"), LabelText("Inner Radius"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectInnerRadius
        {
            get => ShapeToDraw.InnerRadius;
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

        protected override Star SpawnShape()
        {
            return new Star(5,MinRectLength/2,MinRectLength/4, 8, MinRectLength/5 );
        }

        protected override void ResizeByRect()
        {
            var rect = Transform.rect;
            switch (BoundBy)
            {
                case SquareBoundEnum.Height:
                    ShapeToDraw.InnerRadius = rect.height/4;
                    ShapeToDraw.Radius = rect.height/2;
                    break;
                case SquareBoundEnum.Width:
                    ShapeToDraw.InnerRadius = rect.width/4;
                    ShapeToDraw.Radius = rect.width/2;
                    break;
                case SquareBoundEnum.SmallestLength:
                    ShapeToDraw.InnerRadius = MinRectLength/4;
                    ShapeToDraw.Radius = MinRectLength/2;
                    break;
                case SquareBoundEnum.WidestLength:
                    ShapeToDraw.InnerRadius = MaxRectLength/4;
                    ShapeToDraw.Radius = MaxRectLength/2;
                    break;
            }
        }

        protected override void ShapeValidation()
        {
        }

    }
}

