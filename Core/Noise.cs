using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public static class Noise
{
    private static int[] _permutation;
    private static float[] _data;
    private static Vector2[] _gradients;

    private static void CalculatePermutation(System.Random rng,out int[] p)
    {
        p = Enumerable.Range(0, 256).ToArray();

        /// shuffle the array
        for (var i = 0; i < p.Length; i++)
        {
            var source = rng.Next(p.Length);
            (p[i], p[source]) = (p[source], p[i]);
        }
    }

    /// <summary>
    /// generate a new permutation.
    /// </summary>
    public static void Reseed(System.Random rng)
    {
        CalculatePermutation(rng,out _permutation);
        CalculateGradients(rng,out _gradients);
    }

    private static void CalculateGradients(System.Random rng,out Vector2[] grad)
    {
        grad = new Vector2[256];

        for (var i = 0; i < grad.Length; i++)
        {
            Vector2 gradient;

            do
            {
                gradient = new Vector2((float)(rng.NextDouble() * 2 - 1), (float)(rng.NextDouble() * 2 - 1));
            }
            while (gradient.sqrMagnitude >= 1);

            gradient.Normalize();

            grad[i] = gradient;
        }
    }

    private static float Drop(float t)
    {
        t = Mathf.Abs(t);
        return 1f - t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Q(float u, float v) => Drop(u) * Drop(v);

    private static float Get(float x, float y)
    {
        var cell = new Vector2((float)Mathf.Floor(x), (float)Mathf.Floor(y));

        var total = 0f;

        var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

        foreach (var n in corners)
        {
            var ij = cell + n;
            var uv = new Vector2(x - ij.x, y - ij.y);

            var index = _permutation[(int)ij.x % _permutation.Length];
            index = _permutation[(index + (int)ij.y) % _permutation.Length];

            var grad = _gradients[index % _gradients.Length];

            total += Q(uv.x, uv.y) * Vector2.Dot(grad, uv);
        }
        return Mathf.Max(Mathf.Min(total, 1f), -1f);
    }

    public static float[] Generate(System.Random rng, int size)
    {
        CalculatePermutation(rng,out _permutation);
        CalculateGradients(rng,out _gradients);

        _data = new float[size * size];

        var min = float.MaxValue;
        var max = float.MinValue;
        var frequency = 0.5f;
        var amplitude = 1f;
        //var persistence = 0.25f;
        var octaves = 8;
        for (var octave = 0; octave < octaves; octave++)
        {
            /// parallel loop - easy and fast.
            for (var o = 0; o < size * size; o++)
            {
                var i = o % size;
                var j = o / size;
                var noise = Get(i*frequency*1f/size, j*frequency*1f/size);
                noise = _data[j * size + i] += noise * amplitude;

                min = Mathf.Min(min, noise);
                max = Mathf.Max(max, noise);
            }

            frequency *= 2;
            amplitude /= 2;
        }
        //
        for (var o = 0; o < size * size; o++)
        {
            _data[o] = (_data[o] - min) / (max - min);
        }
        return _data;
    }
}