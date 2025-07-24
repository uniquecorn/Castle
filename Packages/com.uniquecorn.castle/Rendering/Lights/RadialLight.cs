using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Rendering.Lights
{
    public class RadialLight : BaseLight
    {
        [Min(0.1f)]
        public float radius = 0.5f;
        public bool useOuterColor;
        [ShowIf("useOuterColor")]
        public Color outerColor;
        public override void UpdateMesh()
        {
            mesh.Clear();
            var resolution = Mathf.Max(16, Mathf.RoundToInt(radius * 16));
            var angle = 360f/resolution;
            vertices = new Vector3[resolution+1];
            triangles = new int[resolution*3];
            colors = new Color[vertices.Length];
            uvs = new Vector2[vertices.Length];
            vertices[0] = Vector3.zero;
            uvs[0] = Vector2.one / 2;
            //vertices[1] = Quaternion.Euler(0, 0, -angle / 2) * Vector3.up * radius;
            colors[0] = color;
            for (var i = 0; i < resolution; i++)
            {
                vertices[i+1] = Quaternion.Euler(0, 0, i * angle) * Vector3.up * radius;
                uvs[i + 1] = uvs[0]+((Vector2)vertices[i + 1] / radius);
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i+1;
                triangles[i * 3 + 2] = i == resolution - 1 ? 1 : i + 2;
                colors[i + 1] = useOuterColor ? outerColor : color;
            }
            foreach (var bounds in FindObjectsByType<LightBounds>(FindObjectsInactive.Exclude,FindObjectsSortMode.None))
            {
                if (!bounds.Overlaps(transform.position)) continue;
                for (var i = 0; i < resolution; i++)
                {
                    if (!bounds.CheckLine(transform.position, transform.position + vertices[i + 1],
                            out var intersection, out var alpha)) continue;
                    vertices[i+1] = intersection - (Vector2)transform.position;
                    uvs[i + 1] = uvs[0]+((Vector2)vertices[i + 1] / radius);
                    Color.Lerp(color, outerColor, alpha);
                    //colors[i + 1] = colors[0].Alpha(1-alpha);
                }
                break;
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles,0);
            mesh.SetColors(colors);
            mesh.SetUVs(0,uvs);
            meshFilter.mesh = mesh;
        }
    }
}