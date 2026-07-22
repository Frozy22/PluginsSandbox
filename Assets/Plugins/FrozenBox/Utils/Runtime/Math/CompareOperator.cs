using System;
using System.Runtime.CompilerServices;
using Unity.Burst;

namespace FrozenBox.Utils.Math
{
    [Serializable]
    public enum CompareOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }

    [BurstCompile]
    public static class CompareOperatorExtensions
    {
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this CompareOperator compareOperator, int a, int b) 
            => MathUtils.Compare(a, b, compareOperator);
        
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this CompareOperator compareOperator, float a, float b) 
            => MathUtils.Compare(a, b, compareOperator);
    }
}