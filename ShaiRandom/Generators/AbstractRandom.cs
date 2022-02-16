using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// The abstract parent class of nearly all random number generators here.
    /// </summary>
    /// <remarks>
    /// Almost all subclasses of AbstractRandom should implement <see cref="SelectState(int)"/> so that individual states can be retrieved; this is used by many of
    /// the other methods here, and some of them throw exceptions if that method is not available. Similarly, <see cref="SetSelectedState(int, ulong)"/> should
    /// be implemented to set specific states, especially if there is more than one state variable.
    /// </remarks>
    public abstract class AbstractRandom : IEnhancedRandom
    {
        /// <summary>
        /// 2^-24; used in the process of creating a single-precision floating point value in range [0, 1) based on a ulong.
        /// </summary>
        public static readonly float FloatAdjust = MathF.Pow(2f, -24f);

        /// <summary>
        /// 2^-53; used in the process of creating a double-precision floating point value in range [0, 1) based on a ulong.
        /// </summary>
        public static readonly double DoubleAdjust = Math.Pow(2.0, -53.0);

        /// <summary>
        /// Used by <see cref="MakeSeed"/> to produce mid-low quality random numbers as a starting seed, as a "don't care" option for seeding.
        /// </summary>
        protected static readonly Random SeedingRandom = new Random();

        /// <summary>
        /// Used by zero-argument constructors, typically, as a "don't care" option for seeding that creates a random ulong state.
        /// </summary>
        /// <returns>A random ulong from an unseeded random number generator, typically to be used as a random seed.</returns>
        protected static ulong MakeSeed()
        {
            unchecked {
                return (ulong)SeedingRandom.Next() ^ (ulong)SeedingRandom.Next() << 21 ^ (ulong)SeedingRandom.Next() << 42;
            }
        }

        /// <inheritdoc />
        public abstract void Seed(ulong seed);

        /// <inheritdoc />
        public abstract int StateCount { get; }

        /// <inheritdoc />
        public abstract bool SupportsReadAccess { get; }

        /// <inheritdoc />
        public abstract bool SupportsWriteAccess { get; }

        /// <inheritdoc />
        public abstract bool SupportsSkip { get; }

        /// <inheritdoc />
        public abstract bool SupportsPrevious { get; }

        /// <inheritdoc />
        public abstract string DefaultTag { get; }


        /// <inheritdoc />
        public virtual string StringSerialize()
        {
            var ser = new StringBuilder(Serializer.GetTag(this));
            ser.Append('`');
            if (StateCount > 0)
            {
                for (int i = 0; i < StateCount - 1; i++)
                {
                    ser.Append($"{SelectState(i):X}");
                    ser.Append('~');
                }

                ser.Append($"{SelectState(StateCount - 1):X}");
            }

            ser.Append('`');

            return ser.ToString();
        }

        /// <inheritdoc />
        public virtual IEnhancedRandom StringDeserialize(ReadOnlySpan<char> data)
        {
            if (StateCount > 0)
            {
                int idx = data.IndexOf('`');

                for (int i = 0; i < StateCount - 1; i++)
                    SetSelectedState(i, ulong.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), NumberStyles.HexNumber));

                SetSelectedState(StateCount - 1, ulong.Parse(data.Slice(idx + 1, -1 - idx + data.IndexOf('`', idx + 1)), NumberStyles.HexNumber));
            }

            return this;
        }

        /// <inheritdoc />
        public virtual ulong SelectState(int selection)
        {
            throw new NotSupportedException("SelectState() not supported.");
        }

        /// <inheritdoc />
        public virtual void SetSelectedState(int selection, ulong value)
        {
            Seed(value);
        }

        /// <inheritdoc />
        public virtual void SetState(ulong state) => ((IEnhancedRandom)this).SetState(state);

        /// <inheritdoc />
        public virtual void SetState(ulong stateA, ulong stateB) => ((IEnhancedRandom)this).SetState(stateA, stateB);

        /// <inheritdoc />
        public virtual void SetState(ulong stateA, ulong stateB, ulong stateC) => ((IEnhancedRandom)this).SetState(stateA, stateB, stateC);

        /// <inheritdoc />
        public virtual void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD) => ((IEnhancedRandom)this).SetState(stateA, stateB, stateC, stateD);

        /// <inheritdoc />
        public virtual void SetState(params ulong[] states) => ((IEnhancedRandom)this).SetState(states);

        /// <inheritdoc />
        public abstract ulong NextULong();

        /// <inheritdoc />
        public virtual long NextLong()
        {
            unchecked
            {
                return (long)NextULong();
            }
        }

        /// <inheritdoc />
        public ulong NextULong(ulong bound)
        {
            return NextULong(0UL, bound);
        }

        /// <inheritdoc />
        public long NextLong(long outerBound)
        {
            return NextLong(0L, outerBound);
        }

        /// <inheritdoc />
        public virtual ulong NextULong(ulong inner, ulong outer)
        {
            ulong rand = NextULong();
            if (outer < inner)
            {
                ulong t = outer;
                outer = inner + 1UL;
                inner = t + 1UL;
            }
            ulong bound = outer - inner;
            ulong randLow = rand & 0xFFFFFFFFUL;
            ulong boundLow = bound & 0xFFFFFFFFUL;
            ulong randHigh = (rand >> 32);
            ulong boundHigh = (bound >> 32);
            return inner + (randHigh * boundLow >> 32) + (randLow * boundHigh >> 32) + randHigh * boundHigh;
        }

        /// <inheritdoc />
        public virtual long NextLong(long inner, long outer)
        {
            ulong rand = NextULong();
            ulong i2, o2;
            if (outer < inner)
            {
                ulong t = (ulong)outer;
                o2 = (ulong)inner + 1UL;
                i2 = t + 1UL;
            }
            else
            {
                o2 = (ulong)outer;
                i2 = (ulong)inner;
            }
            ulong bound = o2 - i2;
            ulong randLow = rand & 0xFFFFFFFFUL;
            ulong boundLow = bound & 0xFFFFFFFFUL;
            ulong randHigh = (rand >> 32);
            ulong boundHigh = (bound >> 32);
            return (long)(i2 + (randHigh * boundLow >> 32) + (randLow * boundHigh >> 32) + randHigh * boundHigh);
        }

        /// <summary>
        /// Generates the next pseudorandom number with a specific maximum size in bits (not a max number).
        /// </summary>
        /// <remarks>
        /// If you want to get a random number in a range, you should usually use <see cref="NextUInt(uint)"/> instead.
        /// However, for some specific cases, this method is more efficient and less biased than <see cref="NextUInt(uint)"/>
        /// If you know you need a number from a range from 0 (inclusive) to a power of two (exclusive), you can use this method optimally.
        /// <br/>
        /// Note that you can give this values for bits that are outside its expected range of 1 to 32,
        /// but the value used, as long as bits is positive, will effectively be <code>bits % 32</code>. As stated
        /// before, a value of 0 for bits is the same as a value of 32.
        /// </remarks>
        /// <param name="bits">The amount of random bits to request, from 1 to 32.</param>
        /// <returns>The next pseudorandom value from this random number generator's sequence.</returns>
        ///
        public virtual uint NextBits(int bits)
        {
            return (uint)(NextULong() >> 64 - bits);
        }

        /// <inheritdoc />
        public void NextBytes(Span<byte> bytes)
        {
            int bl = bytes.Length;
            for (int i = 0; i < bl;)
            {
                int n = Math.Min(bl - i, 8);
                for (ulong r = NextULong(); n-- > 0; r >>= 8)
                {
                    bytes[i++] = (byte)r;
                }
            }
        }

        /// <inheritdoc />
        public virtual int NextInt()
        {
            return (int)NextULong();
        }

        /// <inheritdoc />
        public virtual uint NextUInt()
        {
            return (uint)NextULong();
        }

        /// <inheritdoc />
        public virtual uint NextUInt(uint bound)
        {
            return (uint)(bound * (NextULong() & 0xFFFFFFFFUL) >> 32);
        }

        /// <inheritdoc />
        public virtual int NextInt(int outerBound)
        {
            outerBound = (int)(outerBound * ((long)NextULong() & 0xFFFFFFFFL) >> 32);
            return outerBound - (outerBound >> 31);
        }

        /// <inheritdoc />
        public uint NextUInt(uint innerBound, uint outerBound)
        {
            return (uint)NextULong(innerBound, outerBound);
        }

        /// <inheritdoc />
        public int NextInt(int innerBound, int outerBound)
        {
            return (int)NextLong(innerBound, outerBound);
        }

        /// <inheritdoc />
        public virtual bool NextBool()
        {
            return (NextULong() & 0x8000000000000000UL) == 0x8000000000000000UL;
        }

        /// <inheritdoc />
        public virtual float NextFloat()
        {
            return (NextULong() >> 40) * FloatAdjust;
        }

        /// <inheritdoc />
        public float NextFloat(float outerBound)
        {
            float f = NextFloat() * outerBound;
            if (f >= outerBound && outerBound > 0f) return BitExtensions.BitDecrement(outerBound);
            if (f <= outerBound && outerBound < 0f) return BitExtensions.BitIncrement(outerBound);
            return f;

        }

        /// <inheritdoc />
        public float NextFloat(float innerBound, float outerBound)
        {
            float f = innerBound + NextFloat() * (outerBound - innerBound);
            if (f >= outerBound && outerBound > innerBound) return BitExtensions.BitDecrement(outerBound);
            if (f <= outerBound && outerBound < innerBound) return BitExtensions.BitIncrement(outerBound);
            return f;
        }


        /// <inheritdoc />
        public virtual double NextDouble()
        {
            return (NextULong() >> 11) * DoubleAdjust;
        }

        /// <inheritdoc />
        public double NextDouble(double outerBound)
        {
            double d = NextDouble() * outerBound;
            if (d >= outerBound && outerBound > 0.0) return BitExtensions.BitDecrement(outerBound);
            if (d <= outerBound && outerBound < 0.0) return BitExtensions.BitIncrement(outerBound);
            return d;
        }

        /// <inheritdoc />
        public double NextDouble(double innerBound, double outerBound)
        {
            double d = innerBound + NextDouble() * (outerBound - innerBound);
            if (d >= outerBound && outerBound > innerBound) return BitExtensions.BitDecrement(outerBound);
            if (d <= outerBound && outerBound < innerBound) return BitExtensions.BitIncrement(outerBound);
            return d;
        }

        //            return BitConverter.Int32BitsToSingle((int)(NextULong() >> 41) | 0x3F800000) - 1f;

        /// <summary>
        /// Uses the most-significant 52 bits of value to generate a double between 0.0 (inclusive) and 1.0 (exclusive).
        /// This is not capable of producing many double values in that range; it can produce exactly half of the count
        /// of values <see cref="NextDouble()"/> can.
        /// </summary>
        /// <remarks>
        /// As "unsafe" methods go, this is very safe. It needs to get the bit representation of a ulong quickly, which works well using unsafe methods.
        /// </remarks>
        /// <param name="value">Any ulong, typically a random one.</param>
        /// <returns>A double between 0.0 (inclusive) and 1.0 (exclusive).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double UnsafeFormDouble(ulong value)
        {
            value = (value >> 12) | 0x3FF0000000000000UL;
            return *((double*)&value) - 1.0;
        }

        /// <inheritdoc />
        public virtual double NextSparseDouble()
        {
            return BitConverter.Int64BitsToDouble((long)(NextULong() >> 12) | 0x3FF0000000000000L) - 1.0;
        }

        /// <inheritdoc />
        public double NextSparseDouble(double outerBound)
        {
            return NextSparseDouble() * outerBound;
        }

        /// <inheritdoc />
        public double NextSparseDouble(double innerBound, double outerBound)
        {
            return innerBound + NextDouble() * (outerBound - innerBound);
        }

        /// <inheritdoc />
        public virtual double NextInclusiveDouble()
        {
            return NextULong(0x20000000000001L) * DoubleAdjust;
        }

        /// <inheritdoc />
        public double NextInclusiveDouble(double outerBound)
        {
            return NextInclusiveDouble() * outerBound;
        }

        /// <inheritdoc />
        public double NextInclusiveDouble(double innerBound, double outerBound)
        {
            return innerBound + NextInclusiveDouble() * (outerBound - innerBound);
        }

        /// <inheritdoc />
        public virtual float NextInclusiveFloat()
        {
            return NextInt(0x1000001) * FloatAdjust;
        }

        /// <inheritdoc />
        public float NextInclusiveFloat(float outerBound)
        {
            return NextInclusiveFloat() * outerBound;
        }

        /// <inheritdoc />
        public float NextInclusiveFloat(float innerBound, float outerBound)
        {
            return innerBound + NextInclusiveFloat() * (outerBound - innerBound);
        }

        /// <inheritdoc />
        public virtual decimal NextInclusiveDecimal()
        {
            unchecked
            {
                ulong bits = NextULong(0x204fce5e3e250262UL);
                return new decimal(NextInt(0x10000001), (int)(bits & 0xFFFFFFFFUL), (int)(bits >> 32), false, 28);
            }
        }

        /// <inheritdoc />
        public decimal NextInclusiveDecimal(decimal outerBound)
        {
            return NextInclusiveDecimal() * outerBound;
        }

        /// <inheritdoc />
        public decimal NextInclusiveDecimal(decimal innerBound, decimal outerBound)
        {
            return innerBound + NextInclusiveDecimal() * (outerBound - innerBound);
        }

        /// <summary>
        /// Gets a random double between 0.0 and 1.0, exclusive at both ends, using a technique that can produce more of the valid values for a double
        /// (near to 0) than other methods.
        /// </summary>
        /// <remarks>
        /// The code for this is small, but extremely unorthodox. The technique is related to <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>,
        /// but because the ability to get the number of leading or trailing zeros is in a method not present in .NET Standard, we get close to that by using
        /// <see cref="BitConverter.DoubleToInt64Bits(double)"/> on a negative long and using its exponent bits directly. The smallest double this can return is 1.0842021724855044E-19 ; the largest it
        /// can return is 0.9999999999999999 . The smallest result is significantly closer to 0 than <see cref="NextDouble()"/> can produce without actually returning 0.
        /// <br/>If you decide to edit this, be advised: here be dragons.
        /// </remarks>
        /// <returns>A double between 0.0 and 1.0, exclusive at both ends.</returns>
        public virtual double NextExclusiveDouble()
        {
            long bits = NextLong();
            return BitConverter.Int64BitsToDouble((0x7C10000000000000L + (BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) & -0x0010000000000000L)) | (~bits & 0x000FFFFFFFFFFFFFL));
        }

        /// <inheritdoc />
        public double NextExclusiveDouble(double outerBound)
        {
            return NextExclusiveDouble(0.0, outerBound);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public double NextExclusiveDouble(double innerBound, double outerBound)
        {
            double v = innerBound + NextDouble() * (outerBound - innerBound);
            double high = Math.Max(innerBound, outerBound);
            if (v >= high && innerBound != outerBound)
            {
                if (high == 0.0) return -1.0842021724855044E-19;
                long bits = BitConverter.DoubleToInt64Bits(high);
                return BitConverter.Int64BitsToDouble(bits - (bits >> 63 | 1L));
            }
            double low = Math.Min(innerBound, outerBound);
            if (v <= low && innerBound != outerBound)
            {
                if (low == 0.0) return 1.0842021724855044E-19;
                long bits = BitConverter.DoubleToInt64Bits(low);
                return BitConverter.Int64BitsToDouble(bits + (bits >> 63 | 1L));
            }
            return v;
        }

        /// <inheritdoc />
        public virtual float NextExclusiveFloat()
        {
            long bits = NextLong();
            return BitConverter.Int32BitsToSingle((1089 + (int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) >> 52) << 23) | ((int)~bits & 0x007FFFFF));
        }

        /// <inheritdoc />
        public float NextExclusiveFloat(float outerBound)
        {
            return NextExclusiveFloat(0f, outerBound);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public float NextExclusiveFloat(float innerBound, float outerBound)
        {
            float v = innerBound + NextFloat() * (outerBound - innerBound);
            float high = Math.Max(innerBound, outerBound);
            if (v >= high && innerBound != outerBound)
            {
                if (high == 0f) return -1.0842022E-19f;
                int bits = BitConverter.SingleToInt32Bits(high);
                return BitConverter.Int32BitsToSingle(bits - (bits >> 31 | 1));
            }
            float low = Math.Min(innerBound, outerBound);
            if (v <= low && innerBound != outerBound)
            {
                if (low == 0f) return 1.0842022E-19f;
                int bits = BitConverter.SingleToInt32Bits(low);
                return BitConverter.Int32BitsToSingle(bits + (bits >> 31 | 1));
            }
            return v;
        }

        /// <inheritdoc />
        public virtual decimal NextExclusiveDecimal()
        {
            unchecked
            {
                ulong bits = NextULong(0x204fce5e3e250262UL);
                return new decimal(NextInt(0xFFFFFFF) + 1, (int)(bits & 0xFFFFFFFFUL), (int)(bits >> 32), false, 28);
            }
        }

        /// <inheritdoc />
        public decimal NextExclusiveDecimal(decimal outerBound)
        {
            return NextExclusiveDecimal() * outerBound;
        }

        /// <inheritdoc />
        public decimal NextExclusiveDecimal(decimal innerBound, decimal outerBound)
        {
            return innerBound + NextExclusiveDecimal() * (outerBound - innerBound);
        }

        /// <inheritdoc />
        public virtual decimal NextDecimal()
        {
            unchecked
            {
                ulong bits = NextULong(0x204fce5e3e250262UL);
                return new decimal((int)NextBits(28), (int)(bits & 0xFFFFFFFFUL), (int)(bits >> 32), false, 28);
            }
        }
        /// <inheritdoc />
        public decimal NextDecimal(decimal outerBound)
        {
            return NextDecimal() * outerBound;
        }
        /// <inheritdoc />
        public decimal NextDecimal(decimal innerBound, decimal outerBound)
        {
            return innerBound + NextDecimal() * (outerBound - innerBound);
        }

        /// <inheritdoc />
        public virtual ulong Skip(ulong distance)
        {
            throw new NotSupportedException("Skip() is not implemented for this generator.");
        }

        /// <summary>
        /// (Optional) If implemented, jumps the generator back to the previous state and returns what NextULong() would have produced at that state.
        /// </summary>
        /// <remarks>
        /// The default implementation calls <see cref="Skip(ulong)"/> with the equivalent of (ulong)(-1L) . If Skip() is not implemented, this throws a NotSupportedException.
        /// Be aware that if Skip() has a non-constant-time implementation, the default here will generally take the most time possible for that method.
        /// </remarks>
        /// <returns>The result of what NextULong() would return at the previous state.</returns>
        public virtual ulong PreviousULong()
        {
            return Skip(0xFFFFFFFFFFFFFFFFUL);
        }

        /// <inheritdoc />
        public abstract IEnhancedRandom Copy();
    }
}
