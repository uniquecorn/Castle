using System;
using Castle.CastleShapes;
using Castle.Shapes;
using UnityEngine;
using UnityEngine.UI;

namespace Castle.CastleShapesUI
{
    public enum RectangleBoundEnum
    {
        WidthAndHeight = 0,
        Width,
        Height,
    }
    
    public enum SquareBoundEnum
    {
        Width = 0,
        Height,
        SmallestLength,
        WidestLength
    }
    
    /// <summary>
    /// Interface for shapes bindable by RectTransform 
    /// </summary>
    public interface IBindable<TEnum> where TEnum : Enum
    {   
        /// <summary>
        /// BoundByRect toggle
        /// <para>
        /// Implement Setter as follow to trigger RectResize side-effect on BoundBy toggle :
        /// <code>
        /**
        get => boundByRect;
        set
        {
            boundByRect = value;
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }*/
        ///</code></para>
        ///  <b>Odin Attributes To Implement: </b>
        ///  <list type="bullet">
        ///  <item>
        /// BoxGroup("Dimensions")
        /// </item>
        ///  <item>
        /// ShowInInspector
        /// </item>
        /// </list>
        ///  </summary>
        public bool BoundByRect { get; set; }
        
        /// <summary>
        /// BoundBy enum types
        /// <para>
        /// Implement Setter as follow to trigger RectResize side-effect on BoundBy toggle :
        /// <code>
        /**
        get => boundBy;
        set
        {
            boundBy = value;
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }*/
        ///</code></para>
        /// 
        ///  <b>Odin Attributes To Implement: </b>
        ///  <list type="bullet">
        ///  <item>
        /// BoxGroup("Dimensions")
        /// </item>
        ///  <item>
        /// ShowIf("BoundByRect")
        /// </item>
        ///  <item>
        /// ShowInInspector
        /// </item>
        /// </list>
        ///  </summary>
        public TEnum BoundBy { get; set; }
        /// <summary>
        /// Resize code bounded by Rect, based on BoundBy enum
        /// <example>
        /// <code>
        /**
        public override void ResizeByRect()
        {
            var rect = Transform.rect;
            switch (BoundBy)
            {
                case RectangleBoundEnum.WidthAndHeight:
                    Width = rect.width;
                    Height = rect.height;
                    break;
                case RectangleBoundEnum.Width:
                    Width = rect.width;
                    break;
                case RectangleBoundEnum.Height:
                    Height = rect.height;
                    break;
            }
        }
        //in base class
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }
        */
        /// </code>
        /// </example> 
        /// </summary>
        public void ResizeByRect();
        
        /// <summary>
        /// Validate Secondary Traits
        /// <example>
        /// <code>
        /**
        public override void ValidateShape()
        {
            //Insert code to ensure proper resizing of rounded corners in RoundedBoxUIs 
        }
        //in base class
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }
        */
        /// </code>
        /// </example> 
        /// </summary>
        public void ShapeValidation();

    }

    [ExecuteInEditMode,RequireComponent(typeof(CanvasRenderer))]
    public abstract class ShapesUI<TShape, TBindableEnum> : MaskableGraphic,  IBindable<TBindableEnum> where TShape : Shape where TBindableEnum : Enum // Changed to maskableGraphic so it can be masked with RectMask2D
    {
        [SerializeField,HideInInspector]
        private new RectTransform transform; 
        protected RectTransform Transform => transform ? transform : transform = (RectTransform)base.transform;
        protected TShape ShapeToDraw;
        public Vector3 offset;
        protected float MinRectLength => Mathf.Min(Transform.rect.width, Transform.rect.height);
        protected float MaxRectLength => Mathf.Max(Transform.rect.width, Transform.rect.height);

        //Bindable Implements
        public abstract bool BoundByRect { get; set; } 
        public abstract TBindableEnum BoundBy { get; set; } 
        public abstract void ResizeByRect(); 
        public abstract void ShapeValidation(); 
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!BoundByRect) return;
            ShapeValidation();
            ResizeByRect();
        }

        // Updated OnPopulateMesh to user VertexHelper instead of mesh
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var verticesToDraw = ShapeToDraw.VerticesWithCenter(offset);

            for (var i = 0; i < verticesToDraw.Length; i++)
            {
                UIVertex vertex = UIVertex.simpleVert;
                vertex.position = verticesToDraw[i];
                vh.AddVert(vertex);
            }
        
            for (var i = 0; i < verticesToDraw.Length-1; i++)
            {
                if (i == 0)
                {
                    // vh.AddTriangle(0, 1, verticesToDraw.Length-1);
                     vh.AddTriangle(verticesToDraw.Length-1, 1, 0);
                }
                // vh.AddTriangle(i+1,i,0);
                vh.AddTriangle(0,i,i+1);
            }
        }
    }
}