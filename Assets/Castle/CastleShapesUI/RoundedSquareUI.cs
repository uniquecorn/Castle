// using Castle.CastleShapes;
// using Castle.Shapes;
// using Sirenix.OdinInspector;
//
// namespace Castle.CastleShapesUI
// {
//     public class RoundedSquareUI : RoundedBoxUI<SquareBoundEnum>
//     {
//
//         [BoxGroup("Dimensions"), LabelText("Size"), ShowIf("BoundByRect"), ShowInInspector]
//         public float RectSize
//         {
//             get => ShapeToDraw.Width;
//         }
//
//         [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
//         public float Size
//         {
//             get => ShapeToDraw.Width;
//             set => ShapeToDraw.Width = value;
//         }
//
//         protected override void OnEnable()
//         {
//             base.OnEnable();
//             ShapeToDraw = new RoundedSquare(10,MinRectLength/4,MinRectLength);
//         }
//
//         public override void ResizeByRect()
//         {
//             var rect = Transform.rect;
//             Size = BoundBy switch
//             {
//                 SquareBoundEnum.Height => rect.height,
//                 SquareBoundEnum.Width => rect.width,
//                 SquareBoundEnum.SmallestLength => MinRectLength,
//                 SquareBoundEnum.WidestLength => MaxRectLength,
//                 _ => Size
//             };
//         }
//     }
// }
//     