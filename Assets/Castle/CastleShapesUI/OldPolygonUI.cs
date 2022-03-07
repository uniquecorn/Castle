using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    public class OldPolygonUI : ShapesUI<Polygon, SquareBoundEnum>
    {

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

        protected override Polygon SpawnShape()
        {
            return new Polygon( 3, MinRectLength/2);
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
            // ShapeToDraw = new Polygon( 3, MinRectLength/2);
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