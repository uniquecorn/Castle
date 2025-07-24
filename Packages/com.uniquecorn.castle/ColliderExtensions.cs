using UnityEngine;

namespace Castle
{
    public static class ColliderExtensions
    {
        public static float Area(this Collider2D coll)
        {
            switch (coll)
            {
                case PolygonCollider2D poly:
                    var result = 0f;
                    for (int p = poly.points.Length - 1, q = 0; q < poly.points.Length; p = q++)
                    {
                        result += Vector3.Cross(poly.points[q], poly.points[p]).magnitude;
                    }
                    return result/2;
                case CircleCollider2D circ:
                    return Mathf.PI * Mathf.Pow(circ.radius,2);
                case BoxCollider2D box:
                    return box.size.x * box.size.y;
                case CapsuleCollider2D cap:
                    var sizeY = (cap.size.y - cap.size.x) * cap.size.x;
                    var circle = Mathf.PI * Mathf.Pow(cap.size.x / 2,2);
                    return sizeY + circle;
                default:
                    return 1;
            }
        }
    }
}