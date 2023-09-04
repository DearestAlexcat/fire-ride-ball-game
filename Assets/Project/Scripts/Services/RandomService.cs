using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;

namespace Game
{
    public sealed class RandomService
    {
        private readonly Random _rng;

        public RandomService(int? seed)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }
        
        public int Range(int min, int max)
        {
            return _rng.Next(min, max);
        }

        public int Range(int val)
        {
            return _rng.Next(val);
        }

        public float Range(float min, float max)
        {
            return min + (max - min) * (float)_rng.NextDouble();
        }
    }
}