using System;
using System.Diagnostics.CodeAnalysis;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// A "random" number generator which implements all generation functions such that they return the minimum possible
    /// value that could be returned by an actual generator implementing the <see cref="IEnhancedRandom"/> contract.
    /// For example, MinRandom.NextULong() always returns <see cref="ulong.MinValue"/>,
    /// MinRandom.NextULong(1, 3) always returns 1, and so on.
    /// </summary>
    /// <remarks>
    /// This generator can be useful for unit testing or debugging algorithms that use random number generators, since
    /// bugs involving improper bounds tend to show up at either extreme of valid bounds.
    ///
    /// Although this generator does not inherit from <see cref="AbstractRandom"/>, it uses the same conceptual processes
    /// for determining min and max numbers that can be returned; so in terms of how exclusive bounds are handled, this
    /// generator performs identically to the theoretical minimums for the same values in an AbstractRandom.
    /// </remarks>
    public class MinRandom : IEnhancedRandom
    {
        /// <summary>
        /// Static instance of this generator that can be used in most cases to prevent allocation, since this generator
        /// has no associated state.
        /// </summary>
        public static readonly MinRandom Instance = new MinRandom();

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
        public string DefaultTag => "MinR";

        /// <summary>
        /// Returns a new MinRandom generator; this must be equivalent to the current one, since there is no state.
        /// </summary>
        /// <returns>A new MinRandom generator.</returns>
        public IEnhancedRandom Copy() => new MinRandom();

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
        /// <returns><see cref="ulong.MinValue"/>.</returns>
        public ulong Skip(ulong distance) => NextULong();

        /// <summary>
        /// Does nothing, since this generator has no state.  Always returns <see cref="ulong.MinValue"/>.
        /// </summary>
        /// <returns><see cref="ulong.MinValue"/></returns>
        public ulong PreviousULong() => NextULong();

        /// <summary>
        /// Always returns <see cref="ulong.MinValue"/>.
        /// </summary>
        /// <returns><see cref="ulong.MinValue"/>.</returns>
        public ulong NextULong() => ulong.MinValue;

        /// <summary>
        /// Always returns <see cref="long.MinValue"/>.
        /// </summary>
        /// <returns><see cref="long.MinValue"/>.</returns>
        public long NextLong() => long.MinValue;

        /// <summary>
        /// Returns the minimum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive).
        /// </summary>
        /// <param name="bound"/>
        /// <returns>The minimum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive)</returns>
        public ulong NextULong(ulong bound) => NextULong(0, bound);

        /// <summary>
        /// Returns the minimum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public long NextLong(long outerBound) => NextLong(0, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering <paramref name="outer"/> to be exclusive).
        /// </summary>
        /// <param name="inner"/>
        /// <param name="outer"/>
        /// <returns>The minimum of the defined bounds (considering <paramref name="outer"/> to be exclusive)</returns>
        public ulong NextULong(ulong inner, ulong outer)
        {
            // Special case
            if (inner == outer) return inner;

            // Exclusive bound; round outer toward inner and take the minimum inclusive bound
            return Math.Min(inner, outer > inner ? outer - 1 : outer + 1);
        }

        /// <summary>
        /// Returns the minimum of the defined bounds (considering <paramref name="outer"/> to be exclusive).
        /// </summary>
        /// <param name="inner"/>
        /// <param name="outer"/>
        /// <returns>The minimum of the defined bounds (considering <paramref name="outer"/> to be exclusive)</returns>
        public long NextLong(long inner, long outer)
        {
            // Special case
            if (inner == outer) return inner;

            // Exclusive bound; round toward inner and take the minimum inclusive bound
            outer = outer < inner ? outer + 1 : outer - 1;
            return Math.Min(inner, outer);
        }

        /// <summary>
        /// Always returns 0 (no bits set).
        /// </summary>
        /// <param name="bits"/>
        /// <returns>A number with no bits set (eg. 0)</returns>
        public uint NextBits(int bits) => 0u;

        /// <summary>
        /// Fills the buffer with <see cref="byte.MinValue"/>.
        /// </summary>
        /// <param name="bytes">The buffer to fill.</param>
        public void NextBytes(Span<byte> bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = byte.MinValue;
        }

        /// <summary>
        /// Always returns <see cref="int.MinValue"/>.
        /// </summary>
        /// <returns><see cref="int.MinValue"/>.</returns>
        public int NextInt() => int.MinValue;

        /// <summary>
        /// Always returns <see cref="uint.MinValue"/>.
        /// </summary>
        /// <returns><see cref="uint.MinValue"/>.</returns>
        public uint NextUInt() => uint.MinValue;

        /// <summary>
        /// Returns the minimum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive).
        /// </summary>
        /// <param name="bound"/>
        /// <returns>The minimum of 0 and the defined bound (considering <paramref name="bound"/> to be exclusive)</returns>
        public uint NextUInt(uint bound) => (uint)NextULong(bound);

        /// <summary>
        /// Returns the minimum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public int NextInt(int outerBound) => (int)NextLong(outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public uint NextUInt(uint innerBound, uint outerBound) => (uint)NextULong(innerBound, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public int NextInt(int innerBound, int outerBound) => (int)NextLong(innerBound, outerBound);

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <returns>False</returns>
        public bool NextBool() => false;

        /// <summary>
        /// Always returns 0.0f.
        /// </summary>
        /// <returns>0.0f</returns>
        public float NextFloat() => 0.0f;

        /// <summary>
        /// Returns the minimum of 0.0f and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextFloat(float)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0.0f and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public float NextFloat(float outerBound) => NextFloat(0, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextFloat(float, float)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public float NextFloat(float innerBound, float outerBound)
        {
            // Note: this breaks exclusivity with, for example innerBound=1.9f and outerBound=1.8f (it returns 1.8f);
            // but the AbstractRandom implementation can as well
            var startingVal = innerBound > outerBound ? 1.0f - AbstractRandom.FloatAdjust : 0f;
            return innerBound + startingVal * (outerBound - innerBound);
        }

        /// <summary>
        /// Always returns 0.0.
        /// </summary>
        /// <returns>0.0</returns>
        public double NextDouble() => 0.0;

        /// <summary>
        /// Returns the minimum of 0.0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDouble(double)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0.0 and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public double NextDouble(double outerBound) => NextDouble(0, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDouble(double, double)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public double NextDouble(double innerBound, double outerBound)
        {
            // Note: this breaks exclusivity with, for example innerBound=1.9 and outerBound=1.8 (it returns 1.8);
            // but the AbstractRandom implementation can as well
            var startingVal = innerBound > outerBound ? 1.0 - AbstractRandom.DoubleAdjust : 0.0;
            return innerBound + startingVal * (outerBound - innerBound);
        }

        /// <summary>
        /// Always returns 0.0M.
        /// </summary>
        /// <returns>0.0M</returns>
        public decimal NextDecimal() => 0.0M;

        /// <summary>
        /// Returns the minimum of 0.0M and the defined bound (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDecimal(decimal)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0.0M and the defined bound (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public decimal NextDecimal(decimal outerBound) => NextDecimal(0, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextDecimal(decimal, decimal)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering <paramref name="outerBound"/> to be exclusive)</returns>
        public decimal NextDecimal(decimal innerBound, decimal outerBound)
        {
            unchecked
            {
                ulong bits = innerBound > outerBound ? 0x204fce5e3e250261UL : 0;
                var decimalValue = new decimal((int)NextBits(28), (int)(bits & 0xFFFFFFFFUL), (int)(bits >> 32), false, 28);
                return innerBound + decimalValue * (outerBound - innerBound);
            }
        }

        /// <summary>
        /// Always returns 0.0.
        /// </summary>
        /// <returns>0.0</returns>
        public double NextInclusiveDouble() => 0.0;

        /// <summary>
        /// Returns the minimum of 0.0 and the defined bound.
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0.0 and the defined bound</returns>
        public double NextInclusiveDouble(double outerBound) => NextInclusiveDouble(0.0, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds.
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds</returns>
        public double NextInclusiveDouble(double innerBound, double outerBound) => Math.Min(innerBound, outerBound);

        /// <summary>
        /// Always returns 0.0f.
        /// </summary>
        /// <returns>0.0f</returns>
        public float NextInclusiveFloat() => 0.0f;

        /// <summary>
        /// Returns the minimum of 0.0f and the defined bound.
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0.0f and the defined bound</returns>
        public float NextInclusiveFloat(float outerBound) => NextInclusiveFloat(0.0f, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds.
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds</returns>
        public float NextInclusiveFloat(float innerBound, float outerBound) => MathF.Min(innerBound, outerBound);

        /// <summary>
        /// Always returns 0.0M.
        /// </summary>
        /// <returns>0.0M</returns>
        public decimal NextInclusiveDecimal() => 0.0M;

        /// <summary>
        /// Returns the minimum of 0.0M and the defined bound.
        /// </summary>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0.0M and the defined bound</returns>
        public decimal NextInclusiveDecimal(decimal outerBound) => NextInclusiveDecimal(0.0M, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds.
        /// </summary>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds</returns>
        public decimal NextInclusiveDecimal(decimal innerBound, decimal outerBound) => Math.Min(innerBound, outerBound);

        /// <summary>
        /// Always returns 1.0842021724855044E-19 (the minimum value of <see cref="AbstractRandom.NextExclusiveDouble()"/>.
        /// </summary>
        /// <returns>1.0842021724855044E-19</returns>
        public double NextExclusiveDouble() => 1.0842021724855044E-19;

        /// <summary>
        /// Returns the minimum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveDouble(double)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive)</returns>
        public double NextExclusiveDouble(double outerBound) => NextExclusiveDouble(0.0, outerBound);

        /// <summary>
        /// Returns the minimum of the defined bounds (considering both bounds to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveDouble(double, double)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="innerBound"/>
        /// <param name="outerBound"/>
        /// <returns>The minimum of the defined bounds (considering both bounds to be exclusive)</returns>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public double NextExclusiveDouble(double innerBound, double outerBound)
        {
            double nextDouble = innerBound < outerBound ? 0 : 1.0 - AbstractRandom.DoubleAdjust;
            double v = innerBound + nextDouble * (outerBound - innerBound);
            double low = Math.Min(innerBound, outerBound);
            if (v <= low && innerBound != outerBound) {
                if (low == 0.0) return 1.0842021724855044E-19;
                long bits = BitConverter.DoubleToInt64Bits(low);
                return BitConverter.Int64BitsToDouble(bits + (bits >> 63 | 1L));
            }
            return v;
        }

        /// <summary>
        /// Always returns 1.0842022E-19f.
        /// </summary>
        /// <returns>1.0842022E-19f</returns>
        public float NextExclusiveFloat() => 1.0842022E-19f;

        /// <summary>
        /// Returns the minimum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveFloat(float)"/>
        /// in terms of how close it can get to given bounds, etc.  Currently, it also shares issues with the AbstractRandom
        /// implementation which can cause it to return <paramref name="outerBound"/> inclusive with some values.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive)</returns>
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
            float nextFloat = innerBound < outerBound ? 0f : 1.0f - AbstractRandom.FloatAdjust;
            float v = innerBound + nextFloat * (outerBound - innerBound);
            float low = Math.Min(innerBound, outerBound);
            if (v <= low && innerBound != outerBound)
            {
                if (low == 0f) return 1.0842022E-19f;
                int bits = BitConverter.SingleToInt32Bits(low);
                return BitConverter.Int32BitsToSingle(bits + (bits >> 31 | 1));
            }
            return v;
        }

        /// <summary>
        /// Always returns 0.0000000000000000000000000001M.
        /// </summary>
        /// <returns>0.0000000000000000000000000001M</returns>
        public decimal NextExclusiveDecimal() => new decimal(1, 0, 0, false, 28);

        /// <summary>
        /// Returns the minimum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive).
        /// </summary>
        /// <remarks>
        /// In general, this function has the same characteristics of <see cref="AbstractRandom.NextExclusiveDecimal(decimal)"/>
        /// in terms of how close it can get to given bounds, etc.
        /// </remarks>
        /// <param name="outerBound"/>
        /// <returns>The minimum of 0 and the defined bound (considering both 0 and <paramref name="outerBound"/> to be exclusive)</returns>
        public decimal NextExclusiveDecimal(decimal outerBound) => NextExclusiveDecimal(0.0M, outerBound);

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
                ulong bits = innerBound > outerBound ? 0x204fce5e3e250261UL : 0;
                var decimalValue = new decimal(1, (int)(bits & 0xFFFFFFFFUL), (int)(bits >> 32), false, 28);

                return innerBound + decimalValue * (outerBound - innerBound);
            }
        }
    }
}
