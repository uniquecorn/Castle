using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    [Serializable]
    public class RectangleUI : RectangleBoundShapesUI<Rectangle>
    {
        protected override string XLengthInspectorLabel => "Height";
        protected override string YLengthInspectorLabel => "Width";
        public override float XLength 
        {
            get => ShapeToDraw.Width;
            set => ShapeToDraw.Width = value;
        }
        public override float RectXLength 
        {
            get => ShapeToDraw.Width;
        }
        
        public override float YLength 
        {
            get => ShapeToDraw.Height;
            set => ShapeToDraw.Height = value;
        }

        public override float RectYLength 
        {
            get => ShapeToDraw.Height;
        }

        protected override void ShapeValidation(){}
        protected override Rectangle SpawnShape()
        {
            var rect = Transform.rect;
            return new Rectangle(rect.width, rect.height, 10, MinRectLength/4);     
        }
    }
}
