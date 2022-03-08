using System;
using Castle.CastleShapes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

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
    
    [ExecuteInEditMode, RequireComponent(typeof(CanvasRenderer)), RequireComponent(typeof(Mask)), Serializable]
    public abstract class BaseShapeUI : MaskableGraphic
    {
        [SerializeField,HideInInspector]
        public new RectTransform transform;
        protected RectTransform Transform => transform ? transform : transform = (RectTransform)base.transform;
        protected float MinRectLength => Mathf.Min(Transform.rect.width, Transform.rect.height);
        protected float MaxRectLength => Mathf.Max(Transform.rect.width, Transform.rect.height);
        
        [TitleGroup("Properties"), ReadOnly, ShowInInspector]
        public Texture MainTexture
        {
            get => mainTexture;
        }
        

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
        
        [ShowInInspector] public Color SerializedColor => color;

        [ShowInInspector]
        protected TShape ShapeToDraw
        {
            get => shapeToDraw;
            set => shapeToDraw = value;
        }

        //Rounded Corner implementation
        [SerializeField, HideInInspector]
        private bool hasRoundedCorner;
        [SerializeField, HideInInspector]
        private bool boundByRect;
        [SerializeField]
        private TShape shapeToDraw;
        
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
        public abstract TBindableEnum BoundBy { get; set; }

        protected abstract void ResizeByRect();
        protected abstract void ShapeValidation();
        protected abstract TShape SpawnShape();

        protected void ShapeUpdate()
        {
            ShapeValidation();
            ResizeByRect();
            // SetShape();
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            //TODO: Possible to get called when null needs changing
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
            ShapeUpdate();
        }
    }
}