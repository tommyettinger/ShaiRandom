using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom
{
    /// <summary>
    /// Specific implementations of bitwise operations that .NET can optimize well in some cases.
    /// </summary>
    public static class BitExtensions
    {
        /// <summary>
        /// Bitwise left-rotation of a ulong by amt, in bits.
        /// </summary>
        /// <param name="ul">The ulong to rotate left.</param>
        /// <param name="amt">How many bits to rotate.</param>
        /// <returns>The rotated ul.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(this ulong ul, int amt) => (ul << amt) | (ul >> 64 - amt);

        /// <summary>
        /// Bitwise right-rotation of a ulong by amt, in bits.
        /// </summary>
        /// <param name="ul">The ulong to rotate right.</param>
        /// <param name="amt">How many bits to rotate.</param>
        /// <returns>The rotated ul.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateRight(this ulong ul, int amt) => (ul >> amt) | (ul << 64 - amt);

        /// <summary>
        /// If x is finite, returns the next smallest value that compares less than x.
        /// </summary>
        /// <remarks>This is almost the same as Math.BitDecrement in more recent .NET versions, but returns
        /// x as-is if it is not finite (matching the behavior in mathematics more accurately).
        /// It also returns negative Epsilon if x is 0.0, positive or negative.</remarks>
        /// <param name="x">The value that is slightly higher than desired.</param>
        /// <returns>The next smallest value that compares less than x.</returns>
        public static double BitDecrement(double x)
        {
            long bits = BitConverter.DoubleToInt64Bits(x);

            if ((bits & 0x7FF0000000000000L) >= 0x7FF0000000000000L)
            {
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns +Infinity
                return x;
            }

            return (x == 0.0)
                // +0.0 or -0.0 returns -double.Epsilon
                ? -double.Epsilon
                // Negative values need to be incremented
                // Positive values need to be decremented
                : BitConverter.Int64BitsToDouble(bits - (bits >> 63 | 1L));

        }

        /// <summary>
        /// If x is finite, returns the next largest value that compares greater than x.
        /// </summary>
        /// <remarks>This is almost the same as Math.BitIncrement in more recent .NET versions, but returns
        /// x as-is if it is not finite (matching the behavior in mathematics more accurately).
        /// It also returns Epsilon if x is 0.0f, positive or negative.</remarks>
        /// <param name="x">The value that is slightly lower than desired.</param>
        /// <returns>The next smallest value that compares greater than x.</returns>
        public static double BitIncrement(double x)
        {
            long bits = BitConverter.DoubleToInt64Bits(x);

            if ((bits & 0x7FF0000000000000L) >= 0x7FF0000000000000L)
            {
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns +Infinity
                return x;
            }

            return bits == 0.0 ?
                // +0.0 or -0.0 returns double.Epsilon
                double.Epsilon :
                // Negative values need to be decremented
                // Positive values need to be incremented
                BitConverter.Int64BitsToDouble(bits + (bits >> 63 | 1L));
        }

        /// <summary>
        /// If x is finite, returns a double that is stepsFromZero ULPs from x moving away from 0.
        /// </summary>
        /// <remarks>This acts like <see cref="BitIncrement(double)"/> or <see cref="BitDecrement(double)"/> depending
        /// on the sign of stepsFromZero, and can step more than once.
        /// It can move away from 0 if stepsFromZero is positive, or toward 0 if it is negative.
        /// If x is +0.0, then positive steps move toward positive infinity, and negative steps move toward negative
        /// infinity. This is reversed if x is -0.0 . If stepsFromZero is negative and larger (in ULPs) than the
        /// distance to zero, this returns 0.0 if x is positive, of -0.0 if x is negative. If this would return a double
        /// greater than <see cref="double.MaxValue"/>, it instead returns positive infinity; similarly, values lower
        /// than negative MaxValue return negative infinity. Overflow can result in other results being returned;
        /// stepsFromZero should generally be small in most cases (all int values are valid, for instance).
        /// </remarks>
        /// <param name="x">The starting value.</param>
        /// <param name="stepsFromZero">How many ULPs to move away from 0; may be negative to move toward 0 .</param>
        /// <returns>The double that is the given number of ULPs from x moving away from 0.</returns>
        public static double BitStep(double x, long stepsFromZero)
        {
            long bits = BitConverter.DoubleToInt64Bits(x),
                sign = bits & -0x8000000000000000L,
                mag = bits & 0x7FFFFFFFFFFFFFFFL;
            return (bits & 0x7FF0000000000000L) >= 0x7FF0000000000000L
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns +Infinity
                ? x
                : BitConverter.Int64BitsToDouble(Math.Clamp(mag + stepsFromZero, 0L, 0x7FF0000000000000L) | sign);

        }

        /// <summary>
        /// If x is finite, returns the next smallest value that compares less than x.
        /// </summary>
        /// <remarks>This is almost the same as Math.BitDecrement in more recent .NET versions, but returns
        /// x as-is if it is not finite (matching the behavior in mathematics more accurately).
        /// It also returns negative Epsilon if x is 0.0f, positive or negative.</remarks>
        /// <param name="x">The value that is slightly higher than desired.</param>
        /// <returns>The next smallest value that compares less than x.</returns>
        public static float BitDecrement(float x)
        {
            int bits = BitConverter.SingleToInt32Bits(x);

            if ((bits & 0x7F800000) >= 0x7F800000)
            {
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns +Infinity
                return x;
            }

            return x == 0f
                // -0.0 or +0.0 returns -float.Epsilon
                ? -float.Epsilon
                // Negative values need to be incremented
                // Positive values need to be decremented
                : BitConverter.Int32BitsToSingle(bits - (bits >> 31 | 1));
        }

        /// <summary>
        /// If x is finite, returns the next largest value that compares greater than x.
        /// </summary>
        /// <remarks>This is almost the same as Math.BitIncrement in more recent .NET versions, but returns
        /// x as-is if it is not finite (matching the behavior in mathematics more accurately).
        /// It also returns Epsilon if x is 0.0f, positive or negative.</remarks>
        /// <param name="x">The value that is slightly lower than desired.</param>
        /// <returns>The next smallest value that compares greater than x.</returns>
        public static float BitIncrement(float x)
        {
            int bits = BitConverter.SingleToInt32Bits(x);

            if ((bits & 0x7F800000) >= 0x7F800000)
            {
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns +Infinity
                return x;
            }

            return x == 0f
                // -0.0 or +0.0 returns float.Epsilon
                ? float.Epsilon
                // Negative values need to be decremented
                // Positive values need to be incremented
                : BitConverter.Int32BitsToSingle(bits + (bits >> 31 | 1));
        }

        /// <summary>
        /// If x is finite, returns a float that is stepsFromZero ULPs from x moving away from 0.
        /// </summary>
        /// <remarks>This acts like <see cref="BitIncrement(float)"/> or <see cref="BitDecrement(float)"/> depending
        /// on the sign of stepsFromZero, and can step more than once.
        /// It can move away from 0 if stepsFromZero is positive, or toward 0 if it is negative.
        /// If x is +0.0, then positive steps move toward positive infinity, and negative steps move toward negative
        /// infinity. This is reversed if x is -0.0 . If stepsFromZero is negative and larger (in ULPs) than the
        /// distance to zero, this returns 0.0 if x is positive, of -0.0 if x is negative. If this would return a float
        /// greater than <see cref="float.MaxValue"/>, it instead returns positive infinity; similarly, values lower
        /// than negative MaxValue return negative infinity. Overflow can result in other results being returned;
        /// stepsFromZero should generally be small in most cases (all short values are valid, for instance).
        /// </remarks>
        /// <param name="x">The starting value.</param>
        /// <param name="stepsFromZero">How many ULPs to move away from 0; may be negative to move toward 0 .</param>
        /// <returns>The float that is the given number of ULPs from x moving away from 0.</returns>
        public static float BitStep(float x, int stepsFromZero)
        {
            int bits = BitConverter.SingleToInt32Bits(x),
                sign = bits & -0x80000000,
                mag = bits & 0x7FFFFFFF;
            return (bits & 0x7F800000) >= 0x7F800000
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns +Infinity
                ? x
                : BitConverter.Int32BitsToSingle(Math.Clamp(mag + stepsFromZero, 0, 0x7F800000) | sign);
        }
    }
}
