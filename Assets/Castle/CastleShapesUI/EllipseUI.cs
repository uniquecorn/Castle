// using System;
// using Castle.CastleShapes;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace Castle.CastleShapesUI
// {
//     public class EllipseUI : ShapesUI<Ellipse, RectangleBoundEnum>
//     {
//
//         [SerializeField, HideInInspector]
//         private RectangleBoundEnum boundBy;
//         [SerializeField, HideInInspector]
//         private int resolution;
//         [SerializeField, HideInInspector]
//         private float radiusX;
//         [SerializeField, HideInInspector]
//         private float radiusY;
//         
//         
//         [BoxGroup("Dimensions"), ShowInInspector, PropertyRange(1, 60)]
//         public int Resolution
//         {
//             get => resolution;
//             set => resolution = value;
//         }
//
//         [BoxGroup("Dimensions"), ShowIf("BoundByRect"), ShowInInspector]
//         public override RectangleBoundEnum BoundBy { 
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
//         [BoxGroup("Dimensions"), LabelText("RadiusX"), ShowIf("BoundByRect"), ShowInInspector]
//         public float RectRadiusX
//         {
//             get => radiusX;
//         }
//
//         [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
//         public float RadiusX
//         {
//             get => radiusX;
//             set => radiusX = value;
//         }
//     
//         [BoxGroup("Dimensions"), LabelText("RadiusY"), ShowIf("BoundByRect"), ShowInInspector]
//         public float RectRadiusY
//         {
//             get => radiusY;
//         }
//
//         [BoxGroup("Dimensions"), HideIf("BoundByRect"), ShowInInspector]
//         public float RadiusY
//         {
//             get => radiusY;
//             set => radiusY = value;
//         }
//
//         protected override Ellipse SpawnShape()
//         {
//             var rect = Transform.rect;
//             return new Ellipse(30,rect.width/2,rect.height/2);
//         }
//
//         protected override void ResizeByRect()
//         {
//             var rect = Transform.rect;
//             switch (BoundBy)
//             {
//                 case RectangleBoundEnum.WidthAndHeight:
//                     RadiusX = rect.width/2;
//                     RadiusY = rect.height/2;
//                     break;
//                 case RectangleBoundEnum.Width:
//                     RadiusX = rect.width/2;
//                     break;
//                 case RectangleBoundEnum.Height:
//                     RadiusY = rect.height/2;
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//         }
//
//         protected override void SetShape()
//         {
//             base.SetShape();
//             shapeToDraw.RadiusX = RadiusX;
//             shapeToDraw.RadiusY = radiusY;
//             shapeToDraw.Resolution = resolution;
//         }
//
//         protected override void ShapeValidation()
//         {
//             ValidateRadius();
//         }
//
//         protected void ValidateRadius()
//         {
//             var minRadius = CornerRadius = Mathf.Min(RadiusX, RadiusY);
//             var maxCornerRadius = minRadius / Resolution; 
//         }
//     }
// }