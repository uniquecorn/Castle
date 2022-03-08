using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.CastleShapesUI
{
    public class SquareUI : ShapesUI<Square,SquareBoundEnum>
    {
        [SerializeField, HideInInspector]
        private SquareBoundEnum boundBy;
        
        
        
        [BoxGroup("Dimensions"), LabelText("Size"), ShowIf("BoundByRect"), ShowInInspector]
        public float RectSize
        {
            get => ShapeToDraw.Size;
        }

        [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
        public float Size
        {
            get => ShapeToDraw.Size;
            set => ShapeToDraw.Size = value;
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

        protected override Square SpawnShape()
        {
            return new Square(MinRectLength, 10, MinRectLength/4);
        }

        protected override void ResizeByRect()
        {
            var rect = Transform.rect;
            Size = BoundBy switch
            {
                SquareBoundEnum.Height => rect.height,
                SquareBoundEnum.Width => rect.width,
                SquareBoundEnum.SmallestLength => MinRectLength,
                SquareBoundEnum.WidestLength => MaxRectLength,
                _ => Size
            };
        }

        protected override void ShapeValidation(){}
    
    }
}
