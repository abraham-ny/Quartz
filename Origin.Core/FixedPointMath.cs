using System;
using System.Runtime.CompilerServices;

namespace Origin.Core
{
    public static class FixedPointMath
    {
        private const int FRACTIONAL_BITS = 16;
        private const int FIXED_ONE = 1 << FRACTIONAL_BITS;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToFixed(float value) => (int)(value * FIXED_ONE);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(int fixedValue) => fixedValue / (float)FIXED_ONE;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Add(int a, int b) => a + b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Subtract(int a, int b) => a - b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Multiply(int a, int b) => (int)(((long)a * b) >> FRACTIONAL_BITS);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Divide(int a, int b) => (int)(((long)a << FRACTIONAL_BITS) / b);

        // Additional fixed-point math operations can be added here
    }
}
