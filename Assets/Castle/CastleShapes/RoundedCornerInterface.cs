
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Castle.CastleShapes
{
    public interface IRoundedCorners
    {
        int CornerResolution { get; set; }
        float CornerRadius { get; set; }

        protected Vector3[] VerticesWithRoundedCorner(Vector3[] shape)
        {
            //shape = "corners"
            if (shape.Length < 3) return shape;
            var vertices = new Vector3[shape.Length * (CornerResolution + 1)];
            
            for (var iShapeVert = 0; iShapeVert < shape.Length; iShapeVert++)
            {
                var prevVert = iShapeVert - 1 >= 0 ? shape[iShapeVert - 1] : shape[^1];
                var nextVert = iShapeVert + 1 < shape.Length ? shape[iShapeVert + 1] : shape[0];

                var cornerVert = shape[iShapeVert];
                var p0 = cornerVert - Vector3.Normalize(cornerVert - prevVert) * CornerRadius ;
                var p2 = cornerVert - Vector3.Normalize(cornerVert - nextVert) * CornerRadius ;
                
                // Debug.Log($"TargetVertIndex  = {iShapeVert}, P0 : {p0}, P1 : {cornerVert}, P2 : {p2}");
                // var centerPoint = nextVert - prevVert;

                for (var iCornerRes = 0; iCornerRes <= CornerResolution; iCornerRes++)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    // Basically, can be 1, intended loss of fraction
                    float time = (float)iCornerRes / CornerResolution;
                    var index = iShapeVert * (CornerResolution+1) + iCornerRes;
                    
                    vertices[index] = Bezier.GetPoint(p0, cornerVert, p2, time);
                    
                }
            }
            return vertices;
        }

        // private Vector3[] VerticesWithRoundedCornerWithCenter(Vector3[] shape, int cornerResolution, float cornerRadius, Vector3 offset)
        // {
        //     Vector3[] verticesWRC = VerticesWithRoundedCorner(shape, cornerResolution, cornerRadius);
        //     //returns Vertices but with [0] as offset, add offset top vertices undone
        //     Vector3[] vertices = new Vector3[verticesWRC.Length+1];
        //     vertices[0] = offset; //centerpoint = Vector3.zero + offset
        //     for (int i = 1; i < verticesWRC.Length; i++)
        //     {
        //         vertices[i] = verticesWRC[i-1]+offset;
        //     }
        //     return vertices;
        // }

        // public Vector3[] VerticesWithCenter()
        // {
        //     Vector3[] verticesWRC = VerticesWithRoundedCorner(shape, cornerResolution, cornerRadius);
        //          //returns Vertices but with [0] as offset, add offset top vertices undone
        //          Vector3[] vertices = new Vector3[verticesWRC.Length+1];
        //          vertices[0] = offset; //centerpoint = Vector3.zero + offset
        //          for (int i = 1; i < verticesWRC.Length; i++)
        //          {
        //              vertices[i] = verticesWRC[i-1]+offset;
        //          }
        //          return vertices;
        // }
            
        // Probably not using this one this time. Maybe.
        private Vector3 RoundedCorner(Vector3 cornerVertex, int cornerNo, float radius, int resolution, Vector3 origin)
        {
            //Get quadrant origin based on moving inward from the corner using radius
            var cornerQuadrantOrigin = cornerVertex + Vector3.Normalize(cornerVertex - origin) * radius;
        
            var roundedCornerVertices = new Vector3[resolution + 1];
        
            //Runs 4 times based on cornerNumber
            for (var cornerNum = 0; cornerNum < cornerNo; cornerNum++)
            {
                // Defining the roundedCorner origin for each corner based on established corner point...
                var rCornerStart = cornerVertex + Quaternion.Euler(0, 0, -360f * cornerNo) * Vector3.down * radius;

                //This is corner code, fill based on q. This is within a for loop based on q, which is between 0-3, for each corner of a 4 sided polygon.
                //Only fill if resolution <= 0, therefore the polygon is valid for rounded corners, else ignore.


                if (resolution <= 0) continue;
                //Note <= resolution, this is iterating for not only the corner, but also including the original quadrant start.
                for (var interatingResolution = 0; interatingResolution <= resolution; interatingResolution++)
                {
                    //q * (Resolution + 1) + i;

                    //q for quadrant 0-3, Resolution + 1?? , i stands for the resolution.

                    //if i = 0, q=0-3, resolution = 4, you are working on the quadrant corner top left with a resolution of 4 sides.
                
                    //How loop will go for top left corner 0
                    //0*5 + 0 = 0
                    //0*5 + 1 = 1
                    //0*5 + 2 = 2
                    //0*5 + 3 = 3
                    //0*5 + 4 = 4
                
                    //How loop will go for top right corner 1
                    //1*5 + 0 = 5
                    //1*5 + 1 = 6
                    //1*5 + 2 = 7
                    //1*5 + 3 = 8
                    //1*5 + 4 = 9
                
                
                    //so, assuming loop 1 and 2 for corners
                
                    //0-3 * (cornerNumbers(4) + 1) + index based on Resolution
                    //0*5 + 0 = 0 //Loop 1 for corner 0
                    //1*5 + 0 = 5 //Loop 1 for corner 0
                    //2*5 + 0 = 10 //Loop 1 for corner 0
                    //3*5 + 0 = 15 //Loop 1 for corner 0
                
                    //0-3 * (4+1) + index based on Resolution
                    //0*5 + 1 = 1 //Loop 2 for corner 0
                    //1*5 + 1 = 6 //Loop 2 for corner 0
                    //2*5 + 1 = 11 //Loop 2 for corner 0
                    //3*5 + 1 = 16 //Loop 2 for corner 0
                
                    //This code therefore first transform the corner into the origin of the rounded corner, then iterate through.
                /* C───────────────────────────────────4────────────────────────────────────────
                 * ^                   3
                 * │
                 * │         2
                 * │
                 * Radius of corner    
                 * │   1 <-- 2) subsequently, you get the rest by adding Quaternion.Euler(0, 0, i * (-90f / resolution)) * (rCornerStart - quadrantOrigins[cornerNum]) to Quadrant origin
                 * │
                 * │
                 * v
                 * 0 <┐──────────────Radius──────────>CQO<-- Corner "Quadrant" Origin based on Radius
                 * │  │
                 * │  └───  1) new rounded Corner Start iteration 0, you get this by multiplying original by Vector3.down * angle of tilt
                 * │
                 * │
                 * │
                 * │
                 * │                                                
                 * │
                 * │
                 * │
                 * │
                 * │                                                                            O <-- Actual Origin of Rectangle
                 */
                //logic only works on regular polygons,
                //actually logic only works on square and rectangles because it's reliant on Vector3.down
                //Switch to bezier
                
                }


            }

            var vertices = Vector3.zero;
            return vertices;
        }
    }

    public interface IRoundedCornersUI : IRoundedCorners
    {
        bool HasRoundedCorner { get; set; }
        
        public Vector3[] VerticesWithRoundedCornerWithCenter(Vector3[] shape,  Vector3 offset)
        {
            Vector3[] verticesWRC = VerticesWithRoundedCorner(shape);
            //returns Vertices but with [0] as offset, add offset top vertices undone
            Vector3[] vertices = new Vector3[verticesWRC.Length+1];
            vertices[0] = offset; //centerpoint = Vector3.zero + offset
            for (int i = 1; i < verticesWRC.Length; i++)
            {
                vertices[i] = verticesWRC[i-1]+offset;
            }
            return vertices;
        }
        
    }

}

