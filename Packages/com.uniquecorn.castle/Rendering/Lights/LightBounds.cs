using UnityEngine;

namespace Castle.Rendering.Lights
{
    public class LightBounds : MonoBehaviour
{
    public Vector2 testPoint;
        public Vector2[] points;
        public float minX,maxX,minY,maxY;
        void Reset()
        {
            points = new Vector2[]{new(-1,1), new(1,1), new(1,-1),new(-1,-1)};
        }
        public bool Overlaps(Vector2 point)
        {
            point = transform.InverseTransformPoint(point);
            if (point.x < minX || point.x > maxX || point.y < minY || point.y > maxY) return false;
            var result = false;
            int j = points.Length - 1;
            for (var i = 0; i < points.Length; i++)
            {
                if (points[i].y < point.y && points[j].y >= point.y ||
                    points[j].y < point.y && points[i].y >= point.y)
                {
                    if (points[i].x + (point.y - points[i].y) /
                        (points[j].y - points[i].y) *
                        (points[j].x - points[i].x) < point.x)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public bool CheckLine(Vector2 origin, Vector2 dest, out Vector2 intersection,out float alpha)
        {
            if (Overlaps(dest))
            {
                alpha = 0;
                intersection = dest;
                return false;
            }
            for (var i = 0; i < points.Length; i++)
            {
                if (!Tools.Intersect(transform.InverseTransformPoint(origin), transform.InverseTransformPoint(dest),
                        points[i], points[
                            (i + 1) % points.Length], out var intersect,out alpha)) continue;
                //Debug.Log(det);
                intersection = transform.position.Translate(intersect);
                return true;
            }
            alpha = 0;
            intersection = dest;
            return false;

        }
        void OnValidate() => CalculateBounds();
        public void CalculateBounds()
        {
            for (var i = 0; i < points.Length; i++)
            {
                if (points[i].x < minX) minX = points[i].x;
                else if (points[i].x > maxX) maxX = points[i].x;
                if (points[i].y < minY) minY = points[i].y;
                else if (points[i].y > maxY) maxY = points[i].y;
            }
        }
        private void OnDrawGizmos()
        {
            for (var i = 0; i < points.Length; i++)
            {
                Gizmos.DrawLine(transform.position.Translate(points[i]), transform.position.Translate(points[(i + 1) % points.Length]));
            }
        }
}
}