using UnityEngine;

namespace Castle
{
    public static class VectorExtensions
    {
        public static Vector2 ClosestVector(this Vector2 vec, params Vector2[] args)
        {
            var dist = (vec - args[0]).sqrMagnitude;
            var num = 0;
            for (var i = 1; i < args.Length; i++)
            {
                var dist2 = (vec - args[i]).sqrMagnitude;
                if (dist <= dist2) continue;
                dist = dist2;
                num = i;
            }
            return args[num];
        }
        public static void Move2D(this Transform t, float pos) => Move2D(t,new Vector3(pos,pos,0));
        public static void Move2D(this Transform t, Vector3 pos) => t.position = pos.RepZ(t);
        public static void Move2D(this Transform t, Transform t2) => Move2D(t, t2.position);
        public static void Move2DLerp(this Transform t, Vector3 pos, float snappiness = 10) => t.position = Vector3.Lerp(t.position, pos.RepZ(t), Time.deltaTime * snappiness);
        public static Vector3 ZeroZ(this Vector3 vec) => RepZ(vec, 0);
        public static Vector3 RepX(this Vector3 vec, float x) => new(x, vec.y, vec.z);
        public static Vector3 RepY(this Vector3 vec, float y) => new(vec.x, y, vec.z);
        public static Vector3 RepZ(this Vector3 vec,float z) => new(vec.x, vec.y, z);
        public static Vector3 RepZ(this Vector3 vec,Transform t) => RepZ(vec,t.position.z);
        public static Vector3 RepZ(this Vector2 vec, float z) => new(vec.x, vec.y, z);
        public static Vector3 Neg(this Vector3 vec) => new(-vec.x, -vec.y);
        public static Vector3 NegX(this Vector3 vec) => new(-vec.x, vec.y, vec.z);
        public static Vector3 NegY(this Vector3 vec) => new(vec.x, -vec.y, vec.z);
        public static Vector3 NegZ(this Vector3 vec) => new(vec.x, vec.y, -vec.z);
        public static Vector2 Neg(this Vector2 vec) => new(-vec.x, -vec.y);
        public static Vector2 NegX(this Vector2 vec) => new(-vec.x, vec.y);
        public static Vector2 NegY(this Vector2 vec) => new(vec.x, -vec.y);
        public static Vector3 Up(this Vector3 vec, float distance) => new (vec.x, vec.y + distance, vec.z);
        public static Vector3 Down(this Vector3 vec, float distance) => new (vec.x, vec.y - distance, vec.z);
        public static Vector3 Left(this Vector3 vec, float distance) => new (vec.x - distance, vec.y, vec.z);
        public static Vector3 Right(this Vector3 vec, float distance) => new (vec.x + distance, vec.y, vec.z);
        public static Vector2 Up(this Vector2 vec, float distance) => new (vec.x, vec.y + distance);
        public static Vector2 Down(this Vector2 vec, float distance) => new (vec.x, vec.y - distance);
        public static Vector2 Left(this Vector2 vec, float distance) => new (vec.x - distance, vec.y);
        public static Vector2 Right(this Vector2 vec, float distance) => new (vec.x + distance, vec.y);
        public static Vector3 Translate(this Vector3 vec, Vector2 move) => new(vec.x + move.x, vec.y + move.y, vec.z);
        public static Vector3 Translate(this Vector3 vec, float x,float y = 0,float z = 0) => new(vec.x + x, vec.y + y, vec.z + z);
        public static Vector3 Translate(this Vector3 vec,Vector3 move) => new(vec.x + move.x, vec.y + move.y, vec.z);
        public static Vector2 Translate(this Vector2 vec, float x,float y = 0) => new(vec.x + x, vec.y + y);
        public static void ChangeX(this RectTransform rectTransform,float x) => rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
        public static void ChangeY(this RectTransform rectTransform,float y) => rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
        public static void ChangeX(this Transform transform,float x,bool local = true)
        {
            if (local)
                transform.localPosition = transform.localPosition.RepX(x);
            else
                transform.position = transform.position.RepX(x);
        }

        public static void ChangeY(this Transform transform,float y,bool local = true)
        {
            if (local)
                transform.localPosition = transform.localPosition.RepY(y);
            else
                transform.position = transform.position.RepY(y);
        }
        public static void ChangeZ(this Transform transform,float z,bool local = true)
        {

            if (local)
                transform.localPosition = transform.localPosition.RepZ(z);
            else
                transform.position = transform.position.RepZ(z);
        }

    }
}