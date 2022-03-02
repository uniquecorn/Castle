// using Castle.CastleShapes;
// using Castle.CastleShapesUI;
// using Castle.Shapes;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// public abstract class RoundedBoxUI<TBindableEnum> : ShapesUI<RoundedBox, TBindableEnum> where TBindableEnum : System.Enum
// {
//     [ShowInInspector, PropertyRange(1, 30)]
//     public int Resolution
//     {
//         get => ShapeToDraw.Resolution;
//         set => ShapeToDraw.Resolution = value;
//     }
//     
//     [ShowInInspector, PropertyRange(0,"MaxRadius")]
//     public float Radius
//     {
//         get => ShapeToDraw.Radius;
//         set => ShapeToDraw.Radius = value;
//     }
//
//     public override void ShapeValidation()
//     {
//         ValidateRadius();
//     }
//     protected void ValidateRadius()
//     {
//         if (MinSize-Radius*2 < 0) Radius = MaxRadius > 0 ? MaxRadius : 0;
//     }
//
//     [BoxGroup("Dimensions"), ShowInInspector]
//     public override bool BoundByRect { 
//         get => boundByRect;
//         set
//         {
//             boundByRect = value;
//             ShapeValidation();
//             ResizeByRect();
//         }
//     }
//     private bool boundByRect;
//     
//     [BoxGroup("Dimensions"), ShowIf("BoundByRect"), ShowInInspector]
//     public override TBindableEnum BoundBy {  
//         get => boundBy;
//         set
//         {
//             boundBy = value;
//             if (!BoundByRect) return;
//             ShapeValidation();
//             ResizeByRect();
//         }
//     }
//     private TBindableEnum boundBy;
//     protected float MinSize => UnityEngine.Mathf.Min(ShapeToDraw.Width, ShapeToDraw.Height);
//     protected float MaxRadius => MinSize / 2;
// }
