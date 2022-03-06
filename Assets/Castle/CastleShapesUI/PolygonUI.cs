using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    public class PolygonUI : ShapesUI<Polygon, SquareBoundEnum>
    {

        [SerializeField, HideInInspector]
        private SquareBoundEnum boundBy;
        [SerializeField, HideInInspector]
        private int resolution;
        [SerializeField, HideInInspector]
        private float radius;
        
        [BoxGroup("Dimensions"),ShowInInspector, PropertyRange(2,16)]
        public int Resolution
        {
            get => resolution;
            set => resolution = value;
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
                SetShape();
            }
        }
        
        [BoxGroup("Dimensions"), LabelText("Radius"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectRadius
        {
            get => radius;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        protected override void SpawnShape()
        {
            shapeToDraw = new Polygon( 3, MinRectLength/2);
            Resolution = shapeToDraw.Resolution;
            Radius = shapeToDraw.Radius;
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

        protected override void SetShape()
        {
            base.SetShape();
            shapeToDraw.Resolution = resolution;
            shapeToDraw.Radius = radius;
        }
    }
}