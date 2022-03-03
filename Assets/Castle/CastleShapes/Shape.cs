using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Castle.CastleShapes
{
    public static class Bezier {

        public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }
    
        public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
            return
                2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1);
        }

    }

    public abstract class Shape
    {
        protected Shape(int cornerResolution = 10, float cornerRadius = 5 )
        {
            CornerResolution = cornerResolution;
            CornerRadius = cornerRadius;
        }
        
        protected abstract Vector3[] Vertices { get; }
        public float CornerRadius { get; set; }
        public int CornerResolution { get; set; }
        public virtual float MaxCornerRadius => CornerRadius;

        public Vector3[] VerticesWithCenter(Vector3 offset) => VerticesWithCenter(offset, false);
        
        public Vector3[] VerticesWithCenter(Vector3 offset, bool hasRoundedCorner)
        {
            var verticesPrimo = hasRoundedCorner? VerticesWithRoundedCorner(Vertices) : Vertices;
            var vertices = new Vector3[verticesPrimo.Length + 1];
            vertices[0] = offset; //centerpoint = Vector3.zero + offset
            for (var i = 1; i < verticesPrimo.Length + 1; i++)
            {
                vertices[i] = verticesPrimo[i - 1] + offset;
            }
            return vertices;
        }

        protected Vector3[] VerticesWithRoundedCorner(Vector3[] shape) =>  VerticesWithRoundedCorner(shape, CornerResolution, CornerRadius);

        protected static Vector3[] VerticesWithRoundedCorner(Vector3[] shape, int roundedCornerResolution, float roundedCornerRadius) 
        {
            if (roundedCornerResolution < 0) throw new ArgumentOutOfRangeException(nameof(roundedCornerResolution) + "is below 0.");
            if (roundedCornerRadius < 0) throw new ArgumentOutOfRangeException(nameof(roundedCornerRadius) + "is below 0.");
            if (roundedCornerResolution == 0) return shape;

                //shape = "corners" = all vertex in a polygon
                //TODO: add variable to specify which vertex to target for rounding
                if (shape.Length < 3) return shape;
                var vertices = new Vector3[shape.Length * (roundedCornerResolution + 1)];
            
                for (var iShapeVert = 0; iShapeVert < shape.Length; iShapeVert++)
                {
                    var prevVert = iShapeVert - 1 >= 0 ? shape[iShapeVert - 1] : shape[^1];
                    var nextVert = iShapeVert + 1 < shape.Length ? shape[iShapeVert + 1] : shape[0];

                    var cornerVert = shape[iShapeVert];
                    var p0 = cornerVert - Vector3.Normalize(cornerVert - prevVert) * roundedCornerRadius ;
                    var p2 = cornerVert - Vector3.Normalize(cornerVert - nextVert) * roundedCornerRadius ;
                
                    // Debug.Log($"TargetVertIndex  = {iShapeVert}, P0 : {p0}, P1 : {cornerVert}, P2 : {p2}");
                    // var centerPoint = nextVert - prevVert;

                    for (var iCornerRes = 0; iCornerRes <= roundedCornerResolution; iCornerRes++)
                    {
                        // ReSharper disable once PossibleLossOfFraction
                        // Basically, can be 1, intended loss of fraction
                        float time = (float)iCornerRes / roundedCornerResolution;
                        var index = iShapeVert * (roundedCornerResolution+1) + iCornerRes;
                    
                        vertices[index] = Bezier.GetPoint(p0, cornerVert, p2, time);
                    
                    }
                }
                return vertices;
        }
        
        //Gizmos Methods

        public void DrawTransformedGizmos(Transform transform)
        {
            var transformedVertices = new Vector3[Vertices.Length];
            for (var i = 0; i < Vertices.Length; i++)
            {
                transformedVertices[i] = transform.TransformPoint(Vertices[i]);
            }
            Draw(transformedVertices);
        }
        public void DrawTransformedGizmos(Transform transform,Vector3 offset)
        {
            var transformedVertices = new Vector3[Vertices.Length];
            for (var i = 0; i < Vertices.Length; i++)
            {
                transformedVertices[i] = transform.TransformPoint(Vertices[i])+offset;
            }
            Draw(transformedVertices);
        }

        public void Draw(Vector3 offset)
        {
            var verticesPrimo = VerticesWithRoundedCorner(Vertices);
            var transformedVertices = new Vector3[verticesPrimo.Length];
            for (var i = 0; i < verticesPrimo.Length; i++)
            {
                transformedVertices[i] = verticesPrimo[i]+offset;
            }
            Draw(transformedVertices);
        }
        
        public void Draw() => Draw(Vertices);
        
        /// <summary>
        /// Draw Vertices with Gizmos
        /// <param name="offset">
        /// <para>Will offset the shape from <see cref="UnityEngine.Vector3.zero"> Vector3.zero </see> of GameObject's <see cref="UnityEngine.RectTransform"/></para>
        /// </param>
        /// <param name="roundedCornerResolution">
        /// Must be >= 0
        /// </param>
        /// <param name="roundedCornerRadius">
        /// Must be >= 0
        /// </param>
        /// <param name="clamped">
        /// Clamp rounded corner radius
        /// </param>
        /// </summary>
        private static void Draw(Vector3[] vertices, int roundedCornerResolution = 0, float roundedCornerRadius = 0, float maxCornerRadius = 0, bool clamped = false)
        {
            var validatedRoundedCornerRadius = clamped ? Mathf.Clamp(roundedCornerRadius, 0, maxCornerRadius) : roundedCornerRadius;
            var verticesPrimo = VerticesWithRoundedCorner(vertices ,roundedCornerResolution, validatedRoundedCornerRadius);
            if (verticesPrimo == null) return;
            for (var i = 0; i < verticesPrimo.Length; i++)
            {
                Gizmos.DrawLine(verticesPrimo[i], i == verticesPrimo.Length - 1 ? verticesPrimo[0] : verticesPrimo[i + 1]);
            }
        }
    }
}