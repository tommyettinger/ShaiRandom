using System;

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
        public string Tag => "MaxR";

        static MaxRandom()
        {
            AbstractRandom.RegisterTag(new MaxRandom());
        }

        /// <summary>
        /// Returns a new MinRandom generator; this must be equivalent to the current one, since there is no state.
        /// </summary>
        /// <returns>A new MinRandom generator.</returns>
        public IEnhancedRandom Copy() => new MaxRandom();

        /// <inheritdoc />
        public string StringSerialize() => $"#{Tag}``";

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

        public double NextDouble() => throw new NotImplementedException();

        public double NextDouble(double outerBound) => throw new NotImplementedException();

        public double NextDouble(double innerBound, double outerBound) => throw new NotImplementedException();

        public decimal NextDecimal() => throw new NotImplementedException();

        public decimal NextDecimal(decimal outerBound) => throw new NotImplementedException();

        public decimal NextDecimal(decimal innerBound, decimal outerBound) => throw new NotImplementedException();

        public double NextInclusiveDouble() => throw new NotImplementedException();

        public double NextInclusiveDouble(double outerBound) => throw new NotImplementedException();

        public double NextInclusiveDouble(double innerBound, double outerBound) => throw new NotImplementedException();

        public float NextInclusiveFloat() => throw new NotImplementedException();

        public float NextInclusiveFloat(float outerBound) => throw new NotImplementedException();

        public float NextInclusiveFloat(float innerBound, float outerBound) => throw new NotImplementedException();

        public decimal NextInclusiveDecimal() => throw new NotImplementedException();

        public decimal NextInclusiveDecimal(decimal outerBound) => throw new NotImplementedException();

        public decimal NextInclusiveDecimal(decimal innerBound, decimal outerBound) => throw new NotImplementedException();

        public double NextExclusiveDouble() => throw new NotImplementedException();

        public double NextExclusiveDouble(double outerBound) => throw new NotImplementedException();

        public double NextExclusiveDouble(double innerBound, double outerBound) => throw new NotImplementedException();

        public float NextExclusiveFloat() => throw new NotImplementedException();

        public float NextExclusiveFloat(float outerBound) => throw new NotImplementedException();

        public float NextExclusiveFloat(float innerBound, float outerBound) => throw new NotImplementedException();

        public decimal NextExclusiveDecimal() => throw new NotImplementedException();

        public decimal NextExclusiveDecimal(decimal outerBound) => throw new NotImplementedException();

        public decimal NextExclusiveDecimal(decimal innerBound, decimal outerBound) => throw new NotImplementedException();
    }
}
