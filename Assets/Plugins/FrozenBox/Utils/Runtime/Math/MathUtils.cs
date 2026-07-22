using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Mathematics;

namespace FrozenBox.Utils.Math
{
    [BurstCompile]
    public static class MathUtils
    {
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float a, float b) 
            => math.abs(a - b) < math.EPSILON;

        [BurstCompile]
        public static bool Compare(int a, int b, CompareOperator compareOperator)
        {
            return compareOperator switch
            {
                CompareOperator.Equals => a == b,
                CompareOperator.NotEquals => a != b,
                CompareOperator.GreaterThan => a > b,
                CompareOperator.GreaterThanOrEqualTo => a >= b,
                CompareOperator.LessThan => a < b,
                CompareOperator.LessThanOrEqualTo => a <= b,
                _ => throw new ArgumentOutOfRangeException(nameof(compareOperator), compareOperator, null)
            };
        }
        
        [BurstCompile]
        public static bool Compare(float a, float b, CompareOperator compareOperator)
        {
            var isEqual = Approximately(a, b);
            
            return compareOperator switch
            {
                CompareOperator.Equals => isEqual,
                CompareOperator.NotEquals => !isEqual,
                CompareOperator.GreaterThan => !isEqual && a > b,
                CompareOperator.GreaterThanOrEqualTo => isEqual || a >= b,
                CompareOperator.LessThan => !isEqual && a < b,
                CompareOperator.LessThanOrEqualTo => isEqual || a <= b,
                _ => throw new ArgumentOutOfRangeException(nameof(compareOperator), compareOperator, null)
            };
        }
        
        [BurstCompile]
        public static float Round(float value, int decimals)
        {
            var mult = math.pow(10, decimals);
            return math.round(value * mult) / mult;
        }
    }
}