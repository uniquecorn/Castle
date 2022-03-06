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
        [SerializeField, HideInInspector]
        private int resolution;
        [SerializeField, HideInInspector]
        private float radius;
        [SerializeField, HideInInspector]
        private float innerRadius;

        public override SquareBoundEnum BoundBy
        {
            get => boundBy;
            set
            {
                boundBy = value;
                if (!BoundByRect) return;
                ShapeValidation();
                ResizeByRect();
                SetShape();
            }
        }

        
        [BoxGroup("Dimensions"), ShowInInspector, PropertyRange(1, 60)]
        public int Resolution
        {
            get => resolution;
            set => resolution = value;
        }
        [BoxGroup("Dimensions"), LabelText("Radius"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectRadius
        {
            get => radius;
        }
        
        [BoxGroup("Dimensions"), LabelText("Inner Radius"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectInnerRadius
        {
            get => innerRadius;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float Radius
        {
            get => radius;
            set => radius = value;
        }
        
        
        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float InnerRadius
        {
            get => innerRadius;
            set => innerRadius = value;
        }

        protected override void SpawnShape()
        {
            shapeToDraw = new Star(5,MinRectLength/4,MinRectLength/2);
            Resolution = shapeToDraw.Resolution;
            InnerRadius = shapeToDraw.InnerRadius;
            Radius = shapeToDraw.Radius;
        }

        protected override void ResizeByRect()
        {
            var rect = Transform.rect;
            switch (BoundBy)
            {
                case SquareBoundEnum.Height:
                    InnerRadius = rect.height/4;
                    Radius = rect.height/2;
                    break;
                case SquareBoundEnum.Width:
                    InnerRadius = rect.width/4;
                    Radius = rect.width/2;
                    break;
                case SquareBoundEnum.SmallestLength:
                    InnerRadius = MinRectLength/4;
                    Radius = MinRectLength/2;
                    break;
                case SquareBoundEnum.WidestLength:
                    InnerRadius = MaxRectLength/4;
                    Radius = MaxRectLength/2;
                    break;
            }
        }

        protected override void ShapeValidation()
        {
        }

        protected override void SetShape()
        {
            base.SetShape();
            shapeToDraw.InnerRadius = innerRadius;
            shapeToDraw.Radius = radius;
            shapeToDraw.Resolution = resolution;
            Debug.Log("Opps");
        }
    }
}

