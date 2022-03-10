using System;
using System.Drawing;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Vector3 = UnityEngine.Vector3;

namespace Castle.CastleShapesUI
{
    
    [ExecuteInEditMode, RequireComponent(typeof(CanvasRenderer)), RequireComponent(typeof(Mask)), Serializable]
    public abstract class BaseShapeUI : MaskableGraphic
    {
        [SerializeField,HideInInspector]
        public new RectTransform transform;
        protected RectTransform Transform => transform ? transform : transform = (RectTransform)base.transform;
        protected float MinRectLength => Mathf.Min(Transform.rect.width, Transform.rect.height);
        protected float MaxRectLength => Mathf.Max(Transform.rect.width, Transform.rect.height);
        
        [TitleGroup("Properties"), ShowInInspector]
        public Texture MainTexture
        {
            get => mainTexture;
        }



        public override Texture mainTexture => texture;


        [TitleGroup("Properties"), ShowInInspector]
        public Texture texture;
        
        
        
        

#if UNITY_EDITOR
        [TitleGroup("Properties"), ShowInInspector]
        public bool EnableOffsetMove { get; set; }
        [BoxGroup("Dimensions"), HideIf("EnableOffsetMove"), LabelText("Offset"), ShowInInspector]
        private Vector3 DisableOffset => offset;
#endif
        [SerializeField, HideInInspector]
        private Vector3 offset;
        [BoxGroup("Dimensions"), ShowIf("EnableOffsetMove"), ShowInInspector]
        public Vector3 Offset
        {
            get => offset;
            set => offset = value;
        }
    }
    
    public abstract class ShapesUI<TShape, TBindableEnum> : BaseShapeUI where TShape : Shape // Changed to maskableGraphic so it can be masked with RectMask2D
    {
        protected TShape ShapeToDraw
        {
            get => shapeToDraw;
            set => shapeToDraw = value;
        }

        //Rounded Corner implementation
        [SerializeField, HideInInspector] private bool hasRoundedCorner;
        [SerializeField, HideInInspector] private TBindableEnum boundBy;
        [SerializeField, HideInInspector] private bool boundByRect;
        [SerializeField, HideInInspector] private TShape shapeToDraw;

        public int MaxResolution => ShapeToDraw.GetType() == typeof(ICircular) ? ((ICircular)ShapeToDraw).MaxResolution : ShapeToDraw.Resolution;

        //TODO:clamp value if ICircular
        [BoxGroup("Dimensions"),ShowInInspector]
        public virtual int Resolution
        {
            get => ShapeToDraw.Resolution;
            set => ShapeToDraw.Resolution = Mathf.Clamp(value,3,MaxResolution);
        }
        
        [TitleGroup("Properties"), ShowInInspector, HideIf("@this.ShapeToDraw.GetType() == typeof(Castle.CastleShapes.Circle)")] 
        public bool HasRoundedCorner
        {
            get => hasRoundedCorner;
            set => hasRoundedCorner = value;
        }

        [BoxGroup("Dimensions"), ShowIf("HasRoundedCorner"), ShowInInspector]
        public float CornerRadius
        {
            get => ShapeToDraw.CornerRadius;
            set => ShapeToDraw.CornerRadius = value;
        }
        
        [BoxGroup("Dimensions"), ShowIf("HasRoundedCorner"),ShowInInspector, PropertyRange(0, 10)]
        public int CornerResolution
        {
            get => ShapeToDraw.CornerResolution;
            set => ShapeToDraw.CornerResolution = value;
        }

        //Bindable Implements
        [TitleGroup("Properties"), ShowInInspector]
        public bool BoundByRect
        {
            get => boundByRect;
            set
            {
                boundByRect = value;
                ShapeUpdate();
            }
        }

        [BoxGroup("Dimensions"), ShowIf("BoundByRect"), ShowInInspector]
        public TBindableEnum BoundBy {
            get => boundBy;
            set
            {
                boundBy = value;
                ShapeUpdate();
            } 
        }

        //TODO: There is a chance I can move all the resize functions into ShapesUI class by predefining XY field for (SIZE)-based classes and X + Y fields (WIDTH AND HEIGHT)-based classes.
        
        /// <summary>
        /// 
        /// </summary>
        protected abstract void ResizeByRect();
        protected abstract void ShapeValidation();
        protected abstract TShape SpawnShape();

        protected void ShapeUpdate()
        {
            if(!boundByRect) return;
            ShapeValidation();
            ResizeByRect();
            // SetShape();
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            //TODO: Possible to get called when null, needs changing
            ShapeUpdate();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ShapeToDraw ??= SpawnShape();
            ShapeUpdate();
            // SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Updated OnPopulateMesh to user VertexHelper instead of mesh
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var verticesToDraw = ShapeToDraw.VerticesWithCenter(Offset, HasRoundedCorner);
            
            for (var i = 0; i < verticesToDraw.Length; i++)
            {
                UIVertex vertex = UIVertex.simpleVert;
                vertex.color = this.color;
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
    
    public enum SquareBoundEnum
    {
        Width = 0,
        Height,
        SmallestLength,
        WidestLength
    }
    
    public abstract class SquareBoundShapesUI<TShape> : ShapesUI<TShape, SquareBoundEnum> where TShape : Shape
    {
        protected abstract string LengthInspectorLabel { get; }
        
        [BoxGroup("Dimensions"), LabelText("$LengthInspectorLabel"), HideIf("BoundByRect"), ShowInInspector]
        public abstract float Length { get; set; }
        
        [BoxGroup("Dimensions"), LabelText("$LengthInspectorLabel"), ShowIf("BoundByRect"), ShowInInspector]
        public abstract float RectLength { get; }
        
        protected override void ResizeByRect()
        {
            var rect = Transform.rect;
            Length = BoundBy switch
            {
                SquareBoundEnum.Height => rect.height,
                SquareBoundEnum.Width => rect.width,
                SquareBoundEnum.SmallestLength => MinRectLength,
                SquareBoundEnum.WidestLength => MaxRectLength,
                _ => Length
            };
        }

    }

    public enum RectangleBoundEnum
    {
        WidthAndHeight = 0,
        Width,
        Height,
    }

    public abstract class RectangleBoundShapesUI<TShape> : ShapesUI<TShape, RectangleBoundEnum> where TShape : Shape
    {
        protected abstract string XLengthInspectorLabel { get; }
        protected abstract string YLengthInspectorLabel { get; }

        [BoxGroup("Dimensions"), LabelText("$XLengthInspectorLabel"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Height || !BoundByRect"), ShowInInspector] 
        public abstract float XLength { get; set; }
        [BoxGroup("Dimensions"), LabelText("$XLengthInspectorLabel"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Width || BoundByRect && BoundBy == RectangleBoundEnum.WidthAndHeight"),ShowInInspector] 
        public abstract float RectXLength { get; }

        [BoxGroup("Dimensions"), LabelText("$YLengthInspectorLabel"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Width || !BoundByRect"),ShowInInspector] 
        public abstract float YLength { get; set; }      
        [BoxGroup("Dimensions"), LabelText("$YLengthInspectorLabel"), ShowIf("@this.BoundByRect && BoundBy == RectangleBoundEnum.Height || BoundByRect && BoundBy == RectangleBoundEnum.WidthAndHeight"), ShowInInspector]
        public abstract float RectYLength { get; }

        protected override void ResizeByRect()
        {
            var rect = Transform.rect;
            switch (BoundBy)
            {
                case RectangleBoundEnum.WidthAndHeight:
                    XLength = rect.width;
                    YLength = rect.height;
                    break;
                case RectangleBoundEnum.Width:
                    XLength = rect.width;
                    break;
                case RectangleBoundEnum.Height:
                    YLength = rect.height;
                    break;
            }
        }
    }
}