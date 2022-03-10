using System;using Castle.CastleShapes;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    public class EllipseUI : RectangleBoundShapesUI<Ellipse>, ICircular
    {
        protected override string XLengthInspectorLabel => "Width";
        protected override string YLengthInspectorLabel => "Height";
        public override float XLength 
        {
            get => ShapeToDraw.RadiusX*2;
            set => ShapeToDraw.RadiusX = value/2;
        }
        public override float RectXLength => ShapeToDraw.RadiusX*2;

        public override float YLength 
        {
            get => ShapeToDraw.RadiusY*2;
            set => ShapeToDraw.RadiusY = value/2;
        }
        public override float RectYLength => ShapeToDraw.RadiusY*2;

        public float Radius 
        { 
            get => Mathf.Max(ShapeToDraw.RadiusX, ShapeToDraw.RadiusY);
            set
            {
                ShapeToDraw.RadiusX = value;
                ShapeToDraw.RadiusY = value;
            }
        }

        protected override Ellipse SpawnShape()
        {
            var rect = Transform.rect;
            return new Ellipse(rect.width/2,rect.height/2);
        }

        protected override void ShapeValidation()
        {
            ValidateRadius();
        }
        protected void ValidateRadius()
        {
            var minRadius = CornerRadius = Mathf.Min(ShapeToDraw.RadiusX, ShapeToDraw.RadiusY);
            var maxCornerRadius = minRadius / Resolution; 
        }
    }
}