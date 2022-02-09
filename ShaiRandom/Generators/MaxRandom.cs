using System;
using System.Diagnostics.CodeAnalysis;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// A "random" number generator which implements all generation functions such that they return the maximum possible
    /// value that could be returned by an actual generator implementing the <see cref="IEnhancedRandom"/> contract.
    /// For example, MaxRandom.NextULong() always returns <see cref="ulong.MaxValue"/>,
    /// MaxRandom.NextULong(1, 3) always returns 2, and so on.
    /// </summary>
    /// <remarks>
    /// This generator can be useful for unit testing or debugging algorithms that use random number generators, since
    /// bugs involving improper bounds tend to show up at either extreme of valid bounds.
    ///
    /// Although this generator does not inherit from <see cref="AbstractRandom"/>, it uses the same conceptual processes
    /// for determining min and max numbers that can be returned; so in terms of how exclusive bounds are handled, this
    /// generator performs identically to the theoretical maximums for the same values in an AbstractRandom.
    /// </remarks>
    public class MaxRandom : IEnhancedRandom
    {
        /// <summary>
        /// Static instance of this generator that can be used in most cases to prevent allocation, since this generator
        /// has no associated state.
        /// </summary>
        public static readonly MaxRandom Instance = new MaxRandom();

        /// <inheritdoc />
        public int StateCount => 0;

        /// <summary>
        /// Doesn't support reading state, since there is no state to read.
        /// </summary>
        public bool SupportsReadAccess => false;

        /// <summary>
        /// Doesn't support setting state, since there is no state to set.
        /// </summary>
        public bool SupportsWriteAccess => false;

        /// <summary>
        /// Supports <see cref="Skip"/>.
        /// </summary>
        public bool SupportsSkip => true;

        /// <summary>
        /// Supports <see cref="IEnhancedRandom.PreviousULong"/>.
        /// </summary>
        public bool SupportsPrevious => true;

        /// <summary>
        /// Tag for this case is "MinR".
        /// </summary>
        public string DefaultTag => "MaxR";

        /// <summary>
        /// Returns a new MinRandom generator; this must be equivalent to the current one, since there is no state.
        /// </summary>
        /// <returns>A new MinRandom generator.</returns>
        public IEnhancedRandom Copy() => new MaxRandom();

        /// <inheritdoc />
        public string StringSerialize() => $"{Serializer.GetTag(this)}``";

        /// <inheritdoc />
        public IEnhancedRandom StringDeserialize(ReadOnlySpan<char> data) => Instance;

        /// <summary>
        /// Not supported; this generator has no state.
        /// </summary>
        /// <param name="selection"/>
        public ulong SelectState(int selection) => throw new NotSupportedException();

        /// <summary>
        /// Not supported; this generator has no state.
        /// </summary>
        /// <param name="selection"/>
        /// <param name="value"/>
        public void SetSelectedState(int selection, ulong value) => throw new NotSupportedException();

        /// <summary>
        /// Does nothing, since this generator has no state.
        /// </summary>
        /// <param name="seed"/>
        public void Seed(ulong seed)
        { }

        /// <summary>
        /// Does nothing since the return value is always predetermined based on the parameters or implicit bounds.
        /// </summary>
        /// <param name="distance"/>
        /// <returns><see cref="ulong.MaxValue"/>.</returns>
        public ulong Skip(ulong distance) => NextULong();

        /// <summary>
        /// Does nothing, since this generator has no state.  Always returns <see cref="ulong.MaxValue"/>.
        /// </summary>
        /// <returns><see cref="ulong.MaxValue"/></returns>
        public ulong PreviousULong() => NextULong();

        /// <summary>
        /// Always returns <see cref="ulong.MaxValue"/>.
        /// </summary>
        /// <returns><see cref="ulong.MaxValue"/>.</returns>
        public ulong NextULong() => ulong.MaxValue;

        /// <summary>
        /// Always returns <see cref="long.MaxValue"/>.
        /// </summary>
        /// <returns><see cref="long.MaxValue"/>.</returns>
        public long NextLong() => long.MaxValue;

        /// <summary>
        /// Returns the maximum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive).
        /// </summary>
        /// <param name="bound"/>
        /// <returns>The maximum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive)</returns>
        public ulong NextULong(ulong bound) => NextULong(0, bound);

        /// <summary>
        /// Returns the maximum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public long NextLong(long outerBound) => NextLong(0, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds (considering <paramref name="outer"/> to be exclusive).
        /// </summary>
        /// <param name="inner"/>
        /// <param name="outer"/>
        /// <returns>The maximum of the defined bounds (considering <paramref name="outer"/> to be exclusive)</returns>
        public ulong NextULong(ulong inner, ulong outer)
        {
            // Special case
            if (inner == outer) return inner;

            // Exclusive bound; round outer toward inner and take the maximum inclusive bound
            return Math.Max(inner, outer > inner ? outer - 1 : outer + 1);
        }

        /// <summary>
        /// Returns the maximum of the defined bounds (considering <paramref name="outer"/> to be exclusive).
        /// </summary>
        /// <param name="inner"/>
        /// <param name="outer"/>
        /// <returns>The maximum of the defined bounds (considering <paramref name="outer"/> to be exclusive)</returns>
        public long NextLong(long inner, long outer)
        {
            // Special case
            if (inner == outer) return inner;

            // Exclusive bound; round toward inner and take the maximum inclusive bound
            outer = outer < inner ? outer + 1 : outer - 1;
            return Math.Max(inner, outer);
        }

        /// <summary>
        /// Returns a value with the least significant <paramref name="bits"/> % 32 bits set.
        /// </summary>
        /// <param name="bits"/>
        /// <returns>A value with the least significant <paramref name="bits"/> % 32 bits set.</returns>
        public uint NextBits(int bits) => (uint)(1 << (bits % 32)) - 1;

        /// <summary>
        /// Fills the buffer with <see cref="byte.MaxValue"/>.
        /// </summary>
        /// <param name="bytes">The buffer to fill.</param>
        public void NextBytes(Span<byte> bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = byte.MaxValue;
        }

        /// <summary>
        /// Always returns <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns><see cref="int.MaxValue"/>.</returns>
        public int NextInt() => int.MaxValue;

        /// <summary>
        /// Always returns <see cref="uint.MaxValue"/>.
        /// </summary>
        /// <returns><see cref="uint.MaxValue"/>.</returns>
        public uint NextUInt() => uint.MaxValue;

        /// <summary>
        /// Returns the maximum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive).
        /// </summary>
        /// <param name="bound"/>
        /// <returns>The maximum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive)</returns>
        public uint NextUInt(uint bound) => (uint)NextULong(bound);

        /// <summary>
        /// Returns the maximum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public int NextInt(int outerBound) => (int)NextLong(outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public uint NextUInt(uint innerBound, uint outerBound) => (uint)NextULong(innerBound, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public int NextInt(int innerBound, int outerBound) => (int)NextLong(innerBound, outerBound);

        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <returns>True</returns>
        public bool NextBool() => true;

        /// <summary>
        /// Always returns 1.0f - <see cref="AbstractRandom.FloatAdjust"/>.
        /// </summary>
        /// <returns>1.0f - <see cref="AbstractRandom.FloatAdjust"/></returns>
        public float NextFloat() => 1.0f - AbstractRandom.FloatAdjust;

        /// <summary>
        /// Returns the maximum of 0.0f and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextFloat(float)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0.0f and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public float NextFloat(float outerBound) => NextFloat(0, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextFloat(float, float)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public float NextFloat(float innerBound, float outerBound)
        {
            // Note: this breaks exclusivity with, for example innerBound=1.8f and outerBound=1.9f (it returns 1.9f);
            // but the AbstractRandom implementation can as well
            var startingVal = innerBound <= outerBound ? NextFloat() : 0f;
            return innerBound + startingVal * (outerBound - innerBound);
        }

        /// <summary>
        /// Always returns 1.0 - AbstractRandom.DoubleAdjust.
        /// </summary>
        /// <returns>1.0 - AbstractRandom.DoubleAdjust</returns>
        public double NextDouble() => 1.0 - AbstractRandom.DoubleAdjust;

        /// <summary>
        /// Returns the maximum of 0.0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDouble(double)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0.0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public double NextDouble(double outerBound) => NextDouble(0, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDouble(double, double)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public double NextDouble(double innerBound, double outerBound)
        {
            // Note: this breaks exclusivity with, for example innerBound=1.8 and outerBound=1.9 (it returns 1.9);
            // but the AbstractRandom implementation can as well
            var startingVal = innerBound <= outerBound ? NextDouble() : 0.0;
            return innerBound + startingVal * (outerBound - innerBound);
        }

        /// <summary>
        /// Always returns 0.9999999999999999999999999999M.
        /// </summary>
        /// <returns>0.9999999999999999999999999999M</returns>
        public decimal NextDecimal() => 0.9999999999999999999999999999M;

        /// <summary>
        /// Returns the maximum of 0.0M and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDecimal(decimal)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0.0M and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public decimal NextDecimal(decimal outerBound) => NextDecimal(0, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDecimal(decimal, decimal)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public decimal NextDecimal(decimal innerBound, decimal outerBound)
        {
            if (innerBound < outerBound)
                return innerBound + new decimal(0xFFFFFFF, 0x3e250260, 0x204fce5e, false, 28) * (outerBound - innerBound);
            else
                return outerBound + new decimal(0xFFFFFFF, 0x3e250260, 0x204fce5e, false, 28) * (innerBound - outerBound);

        }

        /// <summary>
        /// Always returns 1.0.
        /// </summary>
        /// <returns>1.0</returns>
        public double NextInclusiveDouble() => 1.0;

        /// <summary>
        /// Returns the maximum of 0.0 and the defined bound.
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0.0 and the defined bound</returns>
        public double NextInclusiveDouble(double outerBound) => NextInclusiveDouble(0, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds.
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds</returns>
        public double NextInclusiveDouble(double innerBound, double outerBound) => Math.Max(innerBound, outerBound);

        /// <summary>
        /// Always returns 1.0f.
        /// </summary>
        /// <returns>1.0f</returns>
        public float NextInclusiveFloat() => 1.0f;

        /// <summary>
        /// Returns the maximum of 0.0f and the defined bound.
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0.0f and the defined bound</returns>
        public float NextInclusiveFloat(float outerBound) => NextInclusiveFloat(0f, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds.
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds</returns>
        public float NextInclusiveFloat(float innerBound, float outerBound) => MathF.Max(innerBound, outerBound);

        /// <summary>
        /// Always returns 1.0M.
        /// </summary>
        /// <returns>1.0M</returns>
        public decimal NextInclusiveDecimal() => 1.0M;

        /// <summary>
        /// Returns the maximum of 0.0M and the defined bound.
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0.0M and the defined bound</returns>
        public decimal NextInclusiveDecimal(decimal outerBound) => NextInclusiveDecimal(0M, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds.
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds</returns>
        public decimal NextInclusiveDecimal(decimal innerBound, decimal outerBound) => Math.Max(innerBound, outerBound);

        /// <summary>
        /// Always returns 1.0 - AbstractRandom.DoubleAdjust.
        /// </summary>
        /// <returns>1.0 - AbstractRandom.DoubleAdjust</returns>
        public double NextExclusiveDouble() => NextDouble();

        /// <summary>
        /// Returns the maximum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveDouble(double)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive)</returns>
        public double NextExclusiveDouble(double outerBound) => NextExclusiveDouble(0.0, outerBound);

        /// <summary>
        /// Returns the maximum of the defined bounds (considering both bounds to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveDouble(double, double)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The maximum of the defined bounds (considering both bounds to be exclusive)</returns>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public double NextExclusiveDouble(double innerBound, double outerBound)
        {
            double nextDouble = innerBound >= outerBound ? 0 : NextDouble();
            double v = innerBound + nextDouble * (outerBound - innerBound);
            double high = Math.Max(innerBound, outerBound);
            if (v >= high && innerBound != outerBound)
            {
                if (high == 0.0) return -1.0842021724855044E-19;
                long bits = BitConverter.DoubleToInt64Bits(high);
                return BitConverter.Int64BitsToDouble(bits - (bits >> 63 | 1L));
            }
            return v;
        }

        /// <summary>
        /// Always returns 1.0f - AbstractRandom.FloatAdjust.
        /// </summary>
        /// <returns>1.0 - AbstractRandom.FloatAdjust</returns>
        public float NextExclusiveFloat() => NextFloat();

        /// <summary>
        /// Returns the maximum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveFloat(float)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive)</returns>
        public float NextExclusiveFloat(float outerBound) => NextExclusiveFloat(0f, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering both bounds to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveFloat(float, float)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> or <paramref name="innerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering both bounds to be exclusive)</returns>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public float NextExclusiveFloat(float innerBound, float outerBound)
        {
            float nextFloat = innerBound >= outerBound ? 0f : 1.0f - AbstractRandom.FloatAdjust;
            float v = innerBound + nextFloat * (outerBound - innerBound);
            float high = Math.Max(innerBound, outerBound);
            if (v >= high && innerBound != outerBound)
            {
                if (high == 0f) return -1.0842022E-19f;
                int bits = BitConverter.SingleToInt32Bits(high);
                return BitConverter.Int32BitsToSingle(bits - (bits >> 31 | 1));
            }
            return v;
        }

        /// <summary>
        /// Always returns 0.9999999999999999999999999999M.
        /// </summary>
        /// <returns>0.9999999999999999999999999999M</returns>
        public decimal NextExclusiveDecimal() => NextDecimal();

        /// <summary>
        /// Returns the maximum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveDecimal(decimal)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The maximum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive)</returns>
        public decimal NextExclusiveDecimal(decimal outerBound) => NextExclusiveDecimal(0M, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering both bounds to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveDecimal(decimal, decimal)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering both bounds to be exclusive)</returns>
        public decimal NextExclusiveDecimal(decimal innerBound, decimal outerBound)
        {
            unchecked
            {
                ulong bits = innerBound <= outerBound ? 0x204fce5e3e250261UL : 0;
                var decimalValue = new decimal(0xFFFFFFF, (int)(bits & 0xFFFFFFFFUL), (int)(bits >> 32), false, 28);

                return innerBound + decimalValue * (outerBound - innerBound);
            }
        }
    }
}
