// using UnityEngine;
//
// namespace Castle.CastleShapes
// {
//     public abstract class RoundedBox : Box
//     {
//         public float Radius;
//         public int Resolution;
//         protected override Vector3[] Vertices
//         {
//             get
//             {
//                 var vertices = new Vector3[(Resolution + 1) * 4];
//                 var quadrantOrigins = new[] {
//                     new Vector3(-Width / 2, -Height / 2) + new Vector3(Radius, Radius),
//                     new Vector3(-Width / 2, Height / 2) + new Vector3(Radius, -Radius),
//                     new Vector3(Width / 2, Height / 2) + new Vector3(-Radius, -Radius),
//                     new Vector3(Width / 2, -Height / 2) + new Vector3(-Radius, Radius)
//                 };
//                 
//                 for (var q = 0; q < 4; q++)
//                 {
//                     var quadrantStart = quadrantOrigins[q] + Quaternion.Euler(0, 0, -360/4 * q) * Vector3.down * Radius;
//                     
//                     if (Resolution <= 0) continue;
//                     for (var i = 0; i <= Resolution; i++)
//                     {
//                         vertices[q * (Resolution + 1) + i] = (Quaternion.Euler(0, 0, i * (-90f / Resolution)) * (quadrantStart - quadrantOrigins[q])) + quadrantOrigins[q];
//                     }
//                 }
//                 return vertices;
//             }
//         }
//     }
// }