using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    public class SquareUI : SquareBoundShapesUI<Square>
    {
        protected override string LengthInspectorLabel => "Size";
        public override float Length 
        {
            get => ShapeToDraw.Size;
            set => ShapeToDraw.Size = value;
        }

        public override float RectLength 
        {
            get => ShapeToDraw.Size;
        }

        protected override Square SpawnShape()
        {
            return new Square(MinRectLength, 10, MinRectLength/4);
        }


        protected override void ShapeValidation(){}
    
    }
}
