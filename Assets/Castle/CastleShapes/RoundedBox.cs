using UnityEngine;

namespace Castle.Shapes
{
    public abstract class RoundedBox : Box
    {
        public float radius;
        public int resolution;
        protected override Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[(resolution + 1) * 4];
                var quadrantOrigins = new[] {
                    new Vector3(-Width / 2, -Height / 2) + new Vector3(radius, radius),
                    new Vector3(-Width / 2, Height / 2) + new Vector3(radius, -radius),
                    new Vector3(Width / 2, Height / 2) + new Vector3(-radius, -radius),
                    new Vector3(Width / 2, -Height / 2) + new Vector3(-radius, radius)
                };
                for (var q = 0; q < 4; q++)
                {
                    var quadrantStart = quadrantOrigins[q] + Quaternion.Euler(0, 0, -90f * q) * Vector3.down * radius;
                    if (resolution <= 0) continue;
                    for (var i = 0; i <= resolution; i++)
                    {
                        vertices[q * (resolution + 1) + i] = (Quaternion.Euler(0, 0, i * (-90f / resolution)) * (quadrantStart - quadrantOrigins[q])) + quadrantOrigins[q];
                    }
                }
                return vertices;
            }
        }
    }
}