using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle.Core
{
    [System.Serializable]
    public struct Grid : System.IEquatable<Grid>, IEqualityComparer<Grid>
    {
        [HorizontalGroup("Coords", Title = "@this.ToString()"), HideLabel, SuffixLabel("X", true)]
        public int x;
        [HorizontalGroup("Coords"), HideLabel, SuffixLabel("Y", true)]
        public int y;

        public Grid(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int Size => x * y;
        public Grid Index(int i) => new(i % x, Mathf.FloorToInt((float)i / x));
        public Grid Shift(Grid shift) => Shift(shift.x, shift.y);
        public Grid Shift(int dx, int dy) => new(this.x + dx, this.y + dy);
        public Grid Subtract(Grid subtract) => Subtract(subtract.x, subtract.y);
        public Grid Subtract(int dx, int dy) => new(this.x - dx, this.y - dy);
        public int SqrMag() => (x * x + y * y);
        public float Mag() => Mathf.Sqrt(SqrMag());
        public Grid Reverse() => new(-x, -y);
        public Grid Flip() => new(y, x);
        public Grid Dist(Grid other,bool abs = true) => abs ?  new Grid(Mathf.Abs(other.x - x), Mathf.Abs(other.y - y)) : new Grid(other.x - x, other.y - y);
        public int Distance(Grid other) => Mathf.Abs(other.x - x) + Mathf.Abs(other.y - y);
        public static Grid Zero() => new(0, 0);
        public override string ToString() => x + "," + y;
        public static Grid FromVector(Vector2 position) => new(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        public override bool Equals(object obj) => obj is Grid other && Equals(other);
        public bool Equals(Grid other) => other.x == x && other.y == y;
        public bool Equals(Grid a, Grid b) => a.x == b.x && a.y == b.y;
        public static Grid operator +(Grid a, Grid b)
            => new(a.x+b.x,a.y+b.y);
        public static Grid operator -(Grid a, Grid b)
            => new(a.x-b.x,a.y-b.y);
        public static bool operator ==(Grid a, Grid b)
            => a.Equals(b);
        public static bool operator !=(Grid a, Grid b)
            => !a.Equals(b);
        public int GetHashCode(Grid obj)
        {
            unchecked
            {
                return (obj.x * 397) ^ obj.y;
            }
        }
        public override int GetHashCode() => GetHashCode(this);
    }
}