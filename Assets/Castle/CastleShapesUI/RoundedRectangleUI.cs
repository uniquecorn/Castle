// using System;
// using Castle.CastleShapes;
// using Castle.Shapes;
// using Sirenix.OdinInspector;
//
// namespace Castle.CastleShapesUI
// {
//     public class RoundedRectangleUI : RoundedBoxUI<RectangleBoundEnum>
//     {
//         [BoxGroup("Dimensions"), LabelText("Height"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Height || BoundByRect && BoundBy == RectangleBoundEnum.WidthAndHeight"), ShowInInspector]
//         public float RectHeight
//         {
//             get => ShapeToDraw.Height;
//         }
//     
//         [BoxGroup("Dimensions"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Width || !BoundByRect"),ShowInInspector] 
//         public float Height
//         {
//             get => ShapeToDraw.Height;
//             set => ShapeToDraw.Height = value;
//         }
//
//         [BoxGroup("Dimensions"), LabelText("Width"),ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Width || BoundByRect && BoundBy == RectangleBoundEnum.WidthAndHeight"),ShowInInspector] 
//         public float RectWidth
//         {
//             get => ShapeToDraw.Width;
//         }
//     
//         [BoxGroup("Dimensions"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Height || !BoundByRect"), ShowInInspector] 
//         public float Width
//         {
//             get => ShapeToDraw.Width;
//             set => ShapeToDraw.Width = value;
//         }
//         protected override void OnEnable()
//         {
//             base.OnEnable();
//             var rect = Transform.rect;
//             ShapeToDraw = new RoundedRectangle(10,MinRectLength / 4,rect.height,rect.width);
//         }
//         public override void ResizeByRect()
//         {
//             var rect = Transform.rect;
//             switch (BoundBy)
//             {
//                 case RectangleBoundEnum.WidthAndHeight:
//                     Width = rect.width;
//                     Height = rect.height;
//                     break;
//                 case RectangleBoundEnum.Width:
//                     Width = rect.width;
//                     break;
//                 case RectangleBoundEnum.Height:
//                     Height = rect.height;
//                     break;
//             }
//         }
//     }
// }