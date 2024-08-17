using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Core
{
    [System.Serializable,InlineProperty]
    public struct CastleGrid : System.IEquatable<CastleGrid>, IEqualityComparer<CastleGrid>
    {
        [HorizontalGroup("Coords", Title = "@this.ToString()"), HideLabel, SuffixLabel("X", true)]
        public int x;
        [HorizontalGroup("Coords"), HideLabel, SuffixLabel("Y", true)]
        public int y;

        public CastleGrid(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public CastleGrid(Vector3 vector)
        {
            this.x = Mathf.RoundToInt(vector.x);
            this.y = Mathf.RoundToInt(vector.y);
        }
        public int Size => x * y;
        public static CastleGrid FromFlat(int index, int height) =>
            new(index / height, index % height);
        public CastleGrid Index(int i) => new(i % x, Mathf.FloorToInt((float)i / x));
        public CastleGrid Shift(CastleGrid shift) => Shift(shift.x, shift.y);
        public CastleGrid Shift(int dx, int dy) => new(this.x + dx, this.y + dy);
        public CastleGrid Subtract(CastleGrid subtract) => Subtract(subtract.x, subtract.y);
        public CastleGrid Subtract(int dx, int dy) => new(this.x - dx, this.y - dy);
        public int SqrMag() => (x * x + y * y);
        public float Mag() => Mathf.Sqrt(SqrMag());
        public CastleGrid Reverse() => new(-x, -y);
        public CastleGrid Flip() => new(y, x);
        public CastleGrid Dist(CastleGrid other,bool abs = true) => abs ?  new CastleGrid(Mathf.Abs(other.x - x), Mathf.Abs(other.y - y)) : new CastleGrid(other.x - x, other.y - y);
        public int Distance(CastleGrid other) => Mathf.Abs(other.x - x) + Mathf.Abs(other.y - y);
        public static CastleGrid Zero() => new(0, 0);
        public override string ToString() => x + "," + y;
        public static CastleGrid FromVector(Vector2 position) => new(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        public override bool Equals(object obj) => obj is CastleGrid other && Equals(other);
        public bool Equals(CastleGrid other) => other.x == x && other.y == y;
        public bool Equals(CastleGrid a, CastleGrid b) => a.x == b.x && a.y == b.y;
        public static CastleGrid operator *(CastleGrid grid, int factor)
            => new(grid.x*factor,grid.y*factor);
        public static CastleGrid operator *(int factor, CastleGrid grid)
            => new(grid.x*factor,grid.y*factor);
        public static CastleGrid operator +(CastleGrid a, CastleGrid b)
            => new(a.x+b.x,a.y+b.y);
        public static CastleGrid operator -(CastleGrid a, CastleGrid b)
            => new(a.x-b.x,a.y-b.y);
        public static bool operator ==(CastleGrid a, CastleGrid b)
            => a.Equals(b);
        public static bool operator !=(CastleGrid a, CastleGrid b)
            => !a.Equals(b);
        public int GetHashCode(CastleGrid obj)
        {
            unchecked
            {
                return (obj.x * 397) ^ obj.y;
            }
        }
        public Vector2 AsVector(float factor = 1) => new(factor * x, factor * y);
        public override int GetHashCode() => GetHashCode(this);
        public Vector3 GetPosition(int positionIndex=0, int totalPositions=1)
        {
            var rng = new System.Random(GetHashCode());
            var startAngle = (float) rng.NextDouble() * 360f;
            if (totalPositions <= 1)
            {
                return new Vector3(x, y) + Quaternion.Euler(0, 0, startAngle) * (Vector3.up / (7 - totalPositions));
            }

            var angleDiff = 360 / totalPositions;

            return new Vector3(x, y) +
                   Quaternion.Euler(0, 0, startAngle + (angleDiff * positionIndex)) * (Vector3.up * Mathf.Lerp(0.1f, 0.4f, Tools.InverseLerp(0, 5, totalPositions)));
        }
    }
}