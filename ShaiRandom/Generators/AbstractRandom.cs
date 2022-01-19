using System;
using System.Collections.Generic;
using System.Globalization;
using ShaiRandom.Wrappers;

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
        private static readonly float s_floatAdjust = MathF.Pow(2f, -24f);
        private static readonly double s_doubleAdjust = Math.Pow(2.0, -53.0);
        /// <summary>
        /// Used by <see cref="MakeSeed"/> to produce mid-low quality random numbers as a starting seed, as a "don't care" option for seeding.
        /// </summary>
        protected static readonly Random SeedingRandom = new Random();

        /// <summary>
        /// Used by zero-argument constructors, typically, as a "don't care" option for seeding that creates a random ulong state.
        /// </summary>
        /// <returns></returns>
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
        public abstract string Tag { get; }

        private static Dictionary<string, IEnhancedRandom> TAGS = new Dictionary<string, IEnhancedRandom>();

        /// <summary>
        /// Registers an instance of an IEnhancedRandom implementation by its four-character string <see cref="Tag"/>.
        /// </summary>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a four-character <see cref="Tag"/>.</param>
        /// <returns>Returns true if the tag was successfully registered for the first time, or false if the tags are unchanged.</returns>
        public static bool RegisterTag(IEnhancedRandom instance)
        {
            if (TAGS.ContainsKey(instance.Tag)) return false;
            if (instance.Tag.Length == 4)
            {
                TAGS.Add(instance.Tag, instance);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public abstract string StringSerialize();

        /// <inheritdoc />
        public virtual IEnhancedRandom StringDeserialize(ReadOnlySpan<char> data)
        {
            int idx = data.IndexOf('`');

            for (int i = 0; i < StateCount - 1; i++)
                SetSelectedState(i, ulong.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), NumberStyles.HexNumber));

            SetSelectedState(StateCount - 1, ulong.Parse(data.Slice(idx + 1, -1 - idx + data.IndexOf('`', idx + 1)), NumberStyles.HexNumber));

            return this;
        }

        /// <summary>
        /// Given data from a string produced by <see cref="StringSerialize()"/> on any valid subclass of AbstractRandom,
        /// this returns a new IEnhancedRandom with the same implementation and state it had when it was serialized.
        /// This handles all AbstractRandom implementations in this library, including <see cref="TRGeneratorWrapper"/>,
        /// <see cref="ReversingWrapper"/>, and <see cref="ArchivalWrapper"/> (all of which it currently handles with a special case).
        /// </summary>
        /// <param name="data">Data from a string produced by an AbstractRandom's StringSerialize() method.</param>
        /// <returns>A newly-allocated IEnhancedRandom matching the implementation and state of the serialized AbstractRandom.</returns>
        public static IEnhancedRandom Deserialize(ReadOnlySpan<char> data)
        {
            if (data.Length <= 4)
                throw new ArgumentException("String given cannot represent a valid generator.");

            // Can't use Span as the key in a dictionary, so we have to allocate a string to perform the lookup.
            // When the feature linked here is implemented, we could get around this:
            // https://github.com/dotnet/runtime/issues/27229
            string tagData = new string(data.Slice(1, 4));
            return data[0] switch
            {
				'A' => new ArchivalWrapper(TAGS[tagData].Copy().StringDeserialize(data)),
                'T' => new TRGeneratorWrapper(TAGS[tagData].Copy().StringDeserialize(data)),
                'R' => new ReversingWrapper(TAGS[tagData].Copy().StringDeserialize(data)),
                _ => TAGS[tagData].Copy().StringDeserialize(data)
            };
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
        public long NextLong()
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
        public ulong NextULong(ulong inner, ulong outer)
        {
            ulong rand = NextULong();
            if (inner >= outer) return inner;
            ulong bound = outer - inner;
            ulong randLow = rand & 0xFFFFFFFFUL;
            ulong boundLow = bound & 0xFFFFFFFFUL;
            ulong randHigh = (rand >> 32);
            ulong boundHigh = (bound >> 32);
            return inner + (randHigh * boundLow >> 32) + (randLow * boundHigh >> 32) + randHigh * boundHigh;
        }

        /// <inheritdoc />
        public long NextLong(long inner, long outer)
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
        public uint NextBits(int bits)
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
        public int NextInt()
        {
            return (int)NextULong();
        }

        /// <inheritdoc />
        public uint NextUInt()
        {
            return (uint)NextULong();
        }

        /// <inheritdoc />
        public uint NextUInt(uint bound)
        {
            return (uint)(bound * (NextULong() & 0xFFFFFFFFUL) >> 32);
        }

        /// <inheritdoc />
        public int NextInt(int outerBound)
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
            return (NextULong() >> 40) * s_floatAdjust;
        }

        /// <inheritdoc />
        public float NextFloat(float outerBound)
        {
            return NextFloat() * outerBound;
        }

        /// <inheritdoc />
        public float NextFloat(float innerBound, float outerBound)
        {
            return innerBound + NextFloat() * (outerBound - innerBound);
        }


        /// <inheritdoc />
        public virtual double NextDouble()
        {
            return (NextULong() >> 11) * s_doubleAdjust;
        }

        /// <inheritdoc />
        public double NextDouble(double outerBound)
        {
            return NextDouble() * outerBound;
        }

        /// <inheritdoc />
        public double NextDouble(double innerBound, double outerBound)
        {
            return innerBound + NextDouble() * (outerBound - innerBound);
        }

        /// <inheritdoc />
        public double NextInclusiveDouble()
        {
            return NextULong(0x20000000000001L) * s_doubleAdjust;
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
        public float NextInclusiveFloat()
        {
            return NextInt(0x1000001) * s_floatAdjust;
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

        //Commented out because it was replaced by the bitwise technique below, but we may want to switch back later or on some platforms.

        // ///Gets a random double between 0.0 and 1.0, exclusive at both ends. This can return double
        // ///values between 1.1102230246251564E-16 and 0.9999999999999999, or 0x1.fffffffffffffp-54 and 0x1.fffffffffffffp-1 in hex
        // ///notation. It cannot return 0 or 1.
        // ///<br/>
        // ///The default implementation simply uses <see cref="NextLong()"/> to get a uniform long, shifts it to remove 11 bits, adds 1, and
        // ///multiplies by a value just slightly less than what nextDouble() usually uses.
        // ///@return a random uniform double between 0 and 1 (both exclusive)
        // ///
        //public double NextExclusiveDouble()
        //{
        //    return ((NextULong() >> 11) + 1UL) * 1.1102230246251564E-16;
        //}


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
        public double NextExclusiveDouble()
        {
            long bits = NextLong();
            return BitConverter.Int64BitsToDouble((0x7C10000000000000L + (BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) & -0x0010000000000000L)) | (~bits & 0x000FFFFFFFFFFFFFL));
        }


        /// <inheritdoc />
        public double NextExclusiveDouble(double outerBound)
        {
            return NextExclusiveDouble() * outerBound;
        }

        /// <inheritdoc />
        public double NextExclusiveDouble(double innerBound, double outerBound)
        {
            return innerBound + NextExclusiveDouble() * (outerBound - innerBound);
        }

        // return ((NextUInt() >> 9) + 1u) * 5.960464E-8f;

        /// <inheritdoc />
        public float NextExclusiveFloat()
        {
            long bits = NextLong();
            return BitConverter.Int32BitsToSingle((1089 + (int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) >> 52) << 23) | ((int)~bits & 0x007FFFFF));
        }

        /// <inheritdoc />
        public float NextExclusiveFloat(float outerBound)
        {
            return NextExclusiveFloat() * outerBound;
        }

        /// <inheritdoc />
        public float NextExclusiveFloat(float innerBound, float outerBound)
        {
            return innerBound + NextExclusiveFloat() * (outerBound - innerBound);
        }
        /// <inheritdoc />
        public decimal NextDecimal()
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
