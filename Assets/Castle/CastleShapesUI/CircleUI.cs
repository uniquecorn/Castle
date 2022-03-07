// using System;
// using Castle.CastleShapes;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace Castle.CastleShapesUI
// {
//     public class CircleUI : ShapesUI<Circle, SquareBoundEnum>
//     {
//         [SerializeField, HideInInspector]
//         private SquareBoundEnum boundBy;
//         [SerializeField, HideInInspector]
//         private float radius;
//
//         [BoxGroup("Dimensions"),ShowInInspector]
//         public int Resolution
//         {
//             get => shapeToDraw.Resolution;
//         }
//
//         public override SquareBoundEnum BoundBy
//         {
//             get => boundBy;
//             set
//             {
//                 boundBy = value;
//                 if (!BoundByRect) return;
//                 ShapeValidation();
//                 ResizeByRect();
//             }
//         }
//
//         [BoxGroup("Dimensions"), LabelText("Radius"), ShowIf("BoundByRect"), ShowInInspector]
//         public float RectRadius
//         {
//             get => radius;
//         }
//
//         [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
//         public float Radius
//         {
//             get => radius;
//             set => radius = value;
//         }
//
//         protected override Circle SpawnShape()
//         {
//             
//             return new Circle(MinRectLength/2);
//         }
//
//         protected override void ResizeByRect()
//         {
//             var rect = Transform.rect;
//             Radius = BoundBy switch
//             {
//                 SquareBoundEnum.Height => rect.height/2,
//                 SquareBoundEnum.Width => rect.width/2,
//                 SquareBoundEnum.SmallestLength => MinRectLength/2,
//                 SquareBoundEnum.WidestLength => MaxRectLength/2,
//                 _ => Radius
//             };
//         }
//
//         protected override void ShapeValidation()
//         {
//         }
//
//         protected override void OnRectTransformDimensionsChange()
//         {
//             base.OnRectTransformDimensionsChange();
//             if (!BoundByRect) return;
//             ShapeValidation();
//             ResizeByRect();
//         }
//     }
// }