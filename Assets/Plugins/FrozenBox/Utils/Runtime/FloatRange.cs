using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace FrozenBox.Utils
{
    [Serializable, BurstCompile]
    public struct FloatRange : IEquatable<FloatRange>
    {
        public static readonly FloatRange ZeroToOne = new(0f, 1f);
        public static readonly FloatRange Unlimited = new(float.MinValue, float.MaxValue);
        public static readonly FloatRange PositiveUnlimited = new(0f, float.MaxValue);
        
        public float Min;
        public float Max;
        
        public float Length => Max - Min;

        public FloatRange(float min, float max)
        {
            Assert.IsTrue(min <= max, $"Min ({min}) <= Max ({max})");
            Min = min;
            Max = max;
        }

        public static FloatRange Symmetry(float value) => new(-value, value);
        public static FloatRange FromZero(float value) => new(0f, value);
        public static FloatRange ToZero(float value) => new(value, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float value) 
            => value >= Min && value <= Max;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Clamp(float value) 
            => math.clamp(value, Min, Max);

        public void Deconstruct(out float min, out float max)
        {
            min = Min;
            max = Max;
        }
        
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public bool Equals(FloatRange other) 
            => Min == other.Min && Max == other.Max;

        public override bool Equals(object? obj) 
            => obj is FloatRange other && Equals(other);

        public override int GetHashCode() 
            => HashCode.Combine(Min, Max);

        public static bool operator ==(FloatRange left, FloatRange right) 
            => left.Equals(right);

        public static bool operator !=(FloatRange left, FloatRange right) 
            => !(left == right);

        public static implicit operator FloatRange((float min, float max) tuple) 
            => new(tuple.min, tuple.max);
    }
}