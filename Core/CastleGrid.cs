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
        private static CastleGrid[] _gridsAlloc,_smallAlloc;
        private static List<CastleGrid> _lineAlloc;
        private static Vector3[] _posAlloc;
        public CastleGrid(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public CastleGrid(Vector3 vector)
        {
            x = Mathf.RoundToInt(vector.x);
            y = Mathf.RoundToInt(vector.y);
        }

        public int Length => x + y;
        public int Size => x * y;
        public CastleGrid Abs()
        {
            return new CastleGrid((x + (x >> 31)) ^ (x >> 31), (y + (y >> 31)) ^ (y >> 31));
        }

        public CastleGrid Clamp(int min = 0, int max = 1) => new(Mathf.Clamp(x, min, max), Mathf.Clamp(y, min, max));
        public int Flatten(int height) => x * height + y;
        public static CastleGrid FromFlat(int index, int height) =>
            new(index / height, index % height);
        public CastleGrid Index(int i) => new(i % x, Mathf.FloorToInt((float)i / x));
        public CastleGrid Shift(CastleGrid shift) => Shift(shift.x, shift.y);
        public CastleGrid Shift(int dx, int dy) => new(x + dx, y + dy);
        public CastleGrid Subtract(CastleGrid subtract) => Subtract(subtract.x, subtract.y);
        public CastleGrid Subtract(int dx, int dy) => new(x - dx, y - dy);
        public int SqrMag() => (x * x + y * y);
        public float Mag() => Mathf.Sqrt(SqrMag());
        public CastleGrid Reverse() => new(-x, -y);
        public CastleGrid Flip() => new(y, x);
        public CastleGrid Dist(CastleGrid other,bool abs = true) => abs ?  new CastleGrid(Mathf.Abs(other.x - x), Mathf.Abs(other.y - y)) : new CastleGrid(other.x - x, other.y - y);
        public int Distance(CastleGrid other)
        {
            var dx = other.x - x;
            var dy = other.y - y;
            return ((dx + (dx >> 31)) ^ (dx >> 31)) + ((dy + (dy >> 31)) ^ (dy >> 31));
        }

        public int SquareDistance(CastleGrid other) => (other.x - x) * (other.x - x) + (other.y - y) * (other.y - y);
        public static CastleGrid Zero() => new(0, 0);
        public static CastleGrid One() => new(1, 1);
        public static CastleGrid Right() => new(1, 0);
        public static CastleGrid Up() => new(0, 1);
        public override string ToString() => x + "," + y;
        public static CastleGrid FromVector(Vector2 position) => new(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
        public override bool Equals(object obj) => obj is CastleGrid other && Equals(other);
        public bool Equals(CastleGrid other) => other.x == x && other.y == y;
        public bool Equals(CastleGrid a, CastleGrid b) => a.x == b.x && a.y == b.y;

        public static CastleGrid operator *(CastleGrid a, CastleGrid b) => new(a.x*b.x,a.y*b.y);
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
            Random.InitState(GetHashCode());
            var tau = Mathf.PI * 2;
            var startAngle = Random.value * tau;
            if (totalPositions <= 1)
            {
                var yAxis = 1f / (7 - totalPositions);
                float s = Mathf.Sin(startAngle);
                float c = Mathf.Cos(startAngle);
                return new Vector3(-(yAxis * s) + x, (yAxis * c) + y);
            }
            else
            {
                var angleDiff = tau / totalPositions;
                var yAxis = Mathf.Lerp(0.1f, 0.4f, Tools.InverseLerp(0, 5, totalPositions));
                float s = Mathf.Sin(startAngle+ (angleDiff * positionIndex));
                float c = Mathf.Cos(startAngle + (angleDiff * positionIndex));
                return new Vector3(-(yAxis * s) + x, (yAxis * c) + y);
            }
        }

        public void GetPositions(int totalPositions, out Vector3[] positions)
        {
            if (_posAlloc == null) _posAlloc = new Vector3[10];
            positions = _posAlloc;
            Random.InitState(GetHashCode());
            var tau = Mathf.PI * 2;
            var startAngle = Random.value * tau;
            if (totalPositions <= 1)
            {
                var yAxis = 1f / (7 - totalPositions);

                float s = Mathf.Sin(startAngle);
                float c = Mathf.Cos(startAngle);
                _posAlloc[0] = new Vector3(-(yAxis * s) + x, (yAxis * c) + y);
            }
            else
            {
                var angleDiff = tau / totalPositions;
                var yAxis = Mathf.Lerp(0.1f, 0.4f, Tools.InverseLerp(0, 5, totalPositions));
                for (var i = 0; i < totalPositions; i++)
                {
                    float s = Mathf.Sin(startAngle+ (angleDiff * i));
                    float c = Mathf.Cos(startAngle + (angleDiff * i));
                    _posAlloc[i] = new Vector3(-(yAxis * s) + x, (yAxis * c) + y);
                }
            }
        }
        public List<CastleGrid> Line(CastleGrid end)
        {
            if (_lineAlloc == null)
            {
                _lineAlloc = new List<CastleGrid>(64);
            }
            _lineAlloc.Clear();
            if (end == this)
            {
                _lineAlloc.Add(end);
                return _lineAlloc;
            }
            var d = 0;

            var dx = end.x - x;
            var dy = end.y - y;
            dx = (dx + (dx >> 31)) ^ (dx >> 31);
            dy = (dy + (dy >> 31)) ^ (dy >> 31);
            var dx2 = 2 * dx; // slope scaling factors to
            var dy2 = 2 * dy; // avoid floating point

            var ix = x < end.x ? 1 : -1; // increment direction
            var iy = y < end.y ? 1 : -1;

            var sX = x;
            var sY = y;

            if (dx >= dy) {
                while (true) {
                    _lineAlloc.Add(new CastleGrid(sX,sY));
                    if (sX == end.x)
                        break;
                    sX += ix;
                    d += dy2;
                    if (d > dx) {
                        sY += iy;
                        d -= dx2;
                    }
                }
            } else {
                while (true) {
                    _lineAlloc.Add(new CastleGrid(sX,sY));
                    if (sY == end.y)
                        break;
                    sY += iy;
                    d += dx2;
                    if (d > dy) {
                        sX += ix;
                        d -= dy2;
                    }
                }
            }
            return _lineAlloc;
        }
        public static int GetGridsAroundNonAlloc(CastleGrid grid, out CastleGrid[] grids, int width = 1, int height = 1)
        {
            var num = (width + 2 + height) * 2;
            if (_gridsAlloc == null) _gridsAlloc = new CastleGrid[512];
            if (_gridsAlloc.Length < num)
            {
                _gridsAlloc = new CastleGrid[_gridsAlloc.Length * Mathf.NextPowerOfTwo(Mathf.CeilToInt((float) num / _gridsAlloc.Length))];
            }
            var h = (width + 2) * 2;
            for (var x = 0; x < width + 2; x++)
            {
                var _x = (grid.x - 1) + x;
                _gridsAlloc[x*2] = new CastleGrid(_x, grid.y - 1);
                _gridsAlloc[(x * 2)+1] = new CastleGrid(_x, grid.y + height);
                //Debug.Log(x * 2 + "," + ((x * 2) + 1));
            }
            for (var y = 0; y < height; y++)
            {
                _gridsAlloc[h + (y*2)] = new CastleGrid(grid.x - 1, grid.y+y);
                _gridsAlloc[h + (y * 2)+1] = new CastleGrid(grid.x + width, grid.y+y);
            }
            grids = _gridsAlloc;
            return num;
        }
    }
}