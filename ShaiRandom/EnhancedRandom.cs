using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    public static class BitExtensions
    {
        /// <summary>
        /// Bitwise left-rotation of a ulong by amt, in bits.
        /// </summary>
        /// <param name="ul">The ulong to rotate left.</param>
        /// <param name="amt">How many bits to rotate.</param>
        /// <returns>The rotated ul.</returns>
        public static ulong RotateLeft(this ulong ul, int amt) => (ul << amt) | (ul >> 64 - amt);

        /// <summary>
        /// Bitwise right-rotation of a ulong by amt, in bits.
        /// </summary>
        /// <param name="ul">The ulong to rotate right.</param>
        /// <param name="amt">How many bits to rotate.</param>
        /// <returns>The rotated ul.</returns>
        public static ulong RotateRight(this ulong ul, int amt) => (ul >> amt) | (ul << 64 - amt);
    }
    [Serializable]
    public abstract class ARandom

    {
        private static readonly float FLOAT_ADJUST = MathF.Pow(2f, -24f);
        private static readonly double DOUBLE_ADJUST = Math.Pow(2.0, -53.0);
        protected static readonly Random SeedingRandom = new Random();

        protected static ulong MakeSeed()
        {
            return (ulong)SeedingRandom.Next() ^ (ulong)SeedingRandom.Next() << 21 ^ (ulong)SeedingRandom.Next() << 42;
        }

        protected ARandom()
        {
        }

        protected ARandom(ulong seed)
        {
            Seed(seed);
        }
        protected ARandom(ARandom other)
        {
            SetWith(other);
        }

        /**
         * Sets the seed of this random number generator using a single
         * {@code ulong} seed. This should behave exactly the same as if a new
         * object of this type was created with the constructor that takes a single
         * {@code ulong} value. This does not necessarily assign the state
         * variable(s) of the implementation with the exact contents of seed, so
         * {@link #getSelectedState(int)} should not be expected to return
         * {@code seed} after this, though it may. If this implementation has more
         * than one {@code ulong} of state, then the expectation is that none of
         * those state variables will be exactly equal to {@code seed} (almost all
         * of the time).
         *
         * @param seed the initial seed
         */
        public abstract void Seed(ulong seed);

        /**
         * Gets the number of possible state variables that can be selected with
         * {@link #getSelectedState(int)} or {@link #SelectState(int, ulong)}.
         * This must be implemented. An implementation that has only one {@code ulong}
         * state, like a SplitMix64 generator, should return {@code 1}. A
         * generator that permits setting two different {@code ulong} values, like
         * {@link LaserRandom}, should return {@code 2}. Much larger values are
         * possible for types like the Mersenne Twister or some CMWC generators.
         * @return the non-negative number of selections possible for state variables
         */
        public abstract int StateCount { get; }

        private static Dictionary<string, ARandom> TAGS = new Dictionary<string, ARandom>();

        /// <summary>
        /// Registers an instance of a subclass of ARandom by a four-character string tag.
        /// </summary>
        /// <param name="tag">The four-character string that will identify a type.</param>
        /// <param name="instance">An instance of a subclass of ARandom, which will be copied as
        /// needed; its value does not matter, as long as it is non-null.</param>
        protected static void RegisterTag(string tag, ARandom instance)
        {
            if(tag.Length == 4)
                TAGS.Add(tag, instance);
        }

        /// <summary>
        /// Produces a string that encodes the type and full state of this generator.
        /// </summary>
        /// <returns>An encoded string that stores the type and full state of this generator.</returns>
        public abstract string StringSerialize();

        /// <summary>
        /// Given a string produced by <see cref="StringSerialize"/>, if the specified type is compatible,
        /// then this method sets the state of this ARandom to the specified stored state.
        /// </summary>
        /// <param name="data">A string produced by StringSerialize.</param>
        /// <returns>This ARandom, after modifications.</returns>
        public abstract ARandom StringDeserialize(string data);

        public static ARandom Deserialize(string data)
        {
            if(data.StartsWith('W'))
                return new TRWrapper(TAGS[data.Substring(1, 4)].StringDeserialize(data));
            return TAGS[data.Substring(1, 4)].Copy().StringDeserialize(data);
        }
        /**
         * Gets a selected state value from this EnhancedRandom. The number of possible selections
         * is up to the implementing class, and is accessible via {@link #StateCount}, but
         * negative values for {@code selection} are typically not tolerated. This should return
         * the exact value of the selected state, assuming it is implemented. The default
         * implementation throws an NotSupportedException, and implementors only have to
         * allow reading the state if they choose to implement this differently. If this method
         * is intended to be used, {@link #StateCount} must also be implemented.
         * @param selection used to select which state variable to get; generally non-negative
         * @return the exact value of the selected state
         */
        public virtual ulong SelectState(int selection)
        {
            throw new NotSupportedException("SelectState() not supported.");
        }

        /**
         * Sets a selected state value to the given ulong {@code value}. The number of possible
         * selections is up to the implementing class, but negative values for {@code selection}
         * are typically not tolerated. Implementors are permitted to change {@code value} if it
         * is not valid, but they should not alter it if it is valid. The public implementation
         * calls {@link #Seed(ulong)} with {@code value}, which doesn't need changing if the
         * generator has one state that is set verbatim by Seed(). Otherwise, this method
         * should be implemented when {@link #SelectState(int)} is and the state is allowed
         * to be set by users. Having accurate ways to get and set the full state of a random
         * number generator makes it much easier to serialize and deserialize that class.
         * @param selection used to select which state variable to set; generally non-negative
         * @param value the exact value to use for the selected state, if valid
         */
        public virtual void SetSelectedState(int selection, ulong value)
        {
            Seed(value);
        }

        /**
         * Sets each state variable to the given {@code state}. If {@link #StateCount} is
         * 1, then this should set the whole state to the given value using
         * {@link #SetSelectedState(int, ulong)}. If StateCount is more than 1, then all
         * states will be set in the same way (using SetSelectedState(), all to {@code state}).
         * @param state the ulong value to use for each state variable
         */
        public virtual void SetState(ulong state)
        {
            for (int i = StateCount - 1; i >= 0; i--)
            {
                SetSelectedState(i, state);
            }
        }

        /**
         * Sets each state variable to either {@code stateA} or {@code stateB}, alternating.
         * This uses {@link #SetSelectedState(int, ulong)} to set the values. If there is one
         * state variable ({@link #StateCount} is 1), then this only sets that state
         * variable to stateA. If there are two state variables, the first is set to stateA,
         * and the second to stateB. If there are more, it reuses stateA, then stateB, then
         * stateA, and so on until all variables are set.
         * @param stateA the ulong value to use for states at index 0, 2, 4, 6...
         * @param stateB the ulong value to use for states at index 1, 3, 5, 7...
         */
        public virtual void SetState(ulong stateA, ulong stateB)
        {
            int c = StateCount;
            for (int i = 0; i < c; i += 2)
            {
                SetSelectedState(i, stateA);
            }
            for (int i = 1; i < c; i += 2)
            {
                SetSelectedState(i, stateB);
            }
        }

        /**
         * Sets each state variable to {@code stateA}, {@code stateB}, or {@code stateC},
         * alternating. This uses {@link #SetSelectedState(int, ulong)} to set the values.
         * If there is one state variable ({@link #StateCount} is 1), then this only
         * sets that state variable to stateA. If there are two state variables, the first
         * is set to stateA, and the second to stateB. With three state variables, the
         * first is set to stateA, the second to stateB, and the third to stateC. If there
         * are more, it reuses stateA, then stateB, then stateC, then stateA, and so on
         * until all variables are set.
         * @param stateA the ulong value to use for states at index 0, 3, 6, 9...
         * @param stateB the ulong value to use for states at index 1, 4, 7, 10...
         * @param stateC the ulong value to use for states at index 2, 5, 8, 11...
         */
        public virtual void SetState(ulong stateA, ulong stateB, ulong stateC)
        {
            int c = StateCount;
            for (int i = 0; i < c; i += 3)
            {
                SetSelectedState(i, stateA);
            }
            for (int i = 1; i < c; i += 3)
            {
                SetSelectedState(i, stateB);
            }
            for (int i = 2; i < c; i += 3)
            {
                SetSelectedState(i, stateC);
            }
        }

        /**
         * Sets each state variable to {@code stateA}, {@code stateB}, {@code stateC}, or
         * {@code stateD}, alternating. This uses {@link #SetSelectedState(int, ulong)} to
         * set the values. If there is one state variable ({@link #StateCount} is 1),
         * then this only sets that state variable to stateA. If there are two state
         * variables, the first is set to stateA, and the second to stateB. With three
         * state variables, the first is set to stateA, the second to stateB, and the third
         * to stateC. With four state variables, the first is set to stateA, the second to
         * stateB, the third to stateC, and the fourth to stateD. If there are more, it
         * reuses stateA, then stateB, then stateC, then stateD, then stateA, and so on
         * until all variables are set.
         * @param stateA the ulong value to use for states at index 0, 4, 8, 12...
         * @param stateB the ulong value to use for states at index 1, 5, 9, 13...
         * @param stateC the ulong value to use for states at index 2, 6, 10, 14...
         * @param stateD the ulong value to use for states at index 3, 7, 11, 15...
         */
        public virtual void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
        {
            int c = StateCount;
            for (int i = 0; i < c; i += 4)
            {
                SetSelectedState(i, stateA);
            }
            for (int i = 1; i < c; i += 4)
            {
                SetSelectedState(i, stateB);
            }
            for (int i = 2; i < c; i += 4)
            {
                SetSelectedState(i, stateC);
            }
            for (int i = 3; i < c; i += 4)
            {
                SetSelectedState(i, stateD);
            }
        }

        /**
         * Sets all state variables to alternating values chosen from {@code states}. If states is empty,
         * then this does nothing, and leaves the current generator unchanged. This works for
         * generators with any {@link #StateCount}, but may allocate an array if states is
         * used as a varargs (you can pass an existing array without needing to allocate). This
         * uses {@link #SetSelectedState(int, ulong)} to change the states.
         * @param states an array or varargs of ulong values to use as states
         */
        public virtual void SetState(params ulong[] states)
        {
            int c = StateCount, sl = states.Length;
            for (int b = 0; b < sl; b++)
            {
                for (int i = b; i < c; i += sl)
                {
                    SetSelectedState(i, states[b]);
                }
            }
        }

        /**
         * Returns the next pseudorandom, uniformly distributed {@code ulong}
         * value from this random number generator's sequence. The general
         * contract of {@code NextUlong} is that one {@code ulong} value is
         * pseudorandomly generated and returned.
         *
         * @return the next pseudorandom, uniformly distributed {@code ulong}
         * value from this random number generator's sequence
         */
        public abstract ulong NextUlong();

        public long NextLong()
        {
            return (long)NextUlong();
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code long} value
         * between 0 (inclusive) and the specified value (exclusive), drawn from
         * this random number generator's sequence.  The general contract of
         * {@code nextLong} is that one {@code long} value in the specified range
         * is pseudorandomly generated and returned.  All {@code bound} possible
         * {@code long} values are produced with (approximately) equal
         * probability, though there is a small amount of bias depending on the bound.
         *
         * <br> Note that this advances the state by the same amount as a single call to
         * {@link #nextLong()}, which allows methods like {@link #skip(long)} to function
         * correctly, but introduces some bias when {@code bound} is very large. This will
         * also advance the state if {@code bound} is 0 or negative, so usage with a variable
         * bound will advance the state reliably.
         *
         * <br> This method has some bias, particularly on larger bounds. Actually measuring
         * bias with bounds in the trillions or greater is challenging but not impossible, so
         * don't use this for a real-money gambling purpose. The bias isn't especially
         * significant, though.
         *
         * @see #nextInt(int) Here's a note about the bias present in the bounded generation.
         * @param bound the upper bound (exclusive). If negative or 0, this always returns 0.
         * @return the next pseudorandom, uniformly distributed {@code long}
         * value between zero (inclusive) and {@code bound} (exclusive)
         * from this random number generator's sequence
         */
        public ulong NextUlong(ulong bound)
        {
            return NextUlong(0UL, bound);
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code long} value between an
         * inner bound of 0 (inclusive) and the specified {@code outerBound} (exclusive).
         * This is meant for cases where the outer bound may be negative, especially if
         * the bound is unknown or may be user-specified. A negative outer bound is used
         * as the lower bound; a positive outer bound is used as the upper bound. An outer
         * bound of -1, 0, or 1 will always return 0, keeping the bound exclusive (except
         * for outer bound 0).
         *
         * <p>Note that this advances the state by the same amount as a single call to
         * {@link #nextLong()}, which allows methods like {@link #skip(long)} to function
         * correctly, but introduces some bias when {@code bound} is very large. This
         * method should be about as fast as {@link #nextLong(long)} , unlike the speed
         * difference between {@link #nextInt(int)} and {@link #nextSignedInt(int)}.
         *
         * @see #nextInt(int) Here's a note about the bias present in the bounded generation.
         * @param outerBound the outer exclusive bound; may be any long value, allowing negative
         * @return a pseudorandom long between 0 (inclusive) and outerBound (exclusive)
         */
        public long NextLong(long outerBound)
        {
            return NextLong(0L, outerBound);
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code long} value between the
         * specified {@code innerBound} (inclusive) and the specified {@code outerBound}
         * (exclusive). If {@code outerBound} is less than or equal to {@code innerBound},
         * this always returns {@code innerBound}.
         *
         * @see #nextInt(int) Here's a note about the bias present in the bounded generation.
         * @param inner the inclusive inner bound; may be any long, allowing negative
         * @param outer the exclusive outer bound; must be greater than innerBound (otherwise this returns innerBound)
         * @return a pseudorandom long between innerBound (inclusive) and outerBound (exclusive)
         */
        public ulong NextUlong(ulong inner, ulong outer)
        {
            ulong rand = NextUlong();
            if (inner >= outer) return inner;
            ulong bound = outer - inner;
            ulong randLow = rand & 0xFFFFFFFFUL;
            ulong boundLow = bound & 0xFFFFFFFFUL;
            ulong randHigh = (rand >> 32);
            ulong boundHigh = (bound >> 32);
            return inner + (randHigh * boundLow >> 32) + (randLow * boundHigh >> 32) + randHigh * boundHigh;
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code long} value between the
         * specified {@code innerBound} (inclusive) and the specified {@code outerBound}
         * (exclusive). This is meant for cases where either bound may be negative,
         * especially if the bounds are unknown or may be user-specified.
         *
         * @see #nextInt(int) Here's a note about the bias present in the bounded generation.
         * @param inner the inclusive inner bound; may be any long, allowing negative
         * @param outer the exclusive outer bound; may be any long, allowing negative
         * @return a pseudorandom long between innerBound (inclusive) and outerBound (exclusive)
         */

        //TODO: I'm not sure if this works as expected. Can't run unit tests without some implementation.
        public long NextLong(long inner, long outer)
        {
            ulong rand = NextUlong();
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

        /**
 * Generates the next pseudorandom number with a specific maximum size in bits (not a max number).
 * If you want to get a random number in a range, you should usually use {@link #nextInt(int)} instead.
 * For some specific cases, this method is more efficient and less biased than {@link #nextInt(int)}.
 * For {@code bits} values between 1 and 30, this should be similar in effect to
 * {@code nextInt(1 << bits)}; though it won't typically produce the same values, they will have
 * the correct range. If {@code bits} is 31, this can return any non-negative {@code int}; note that
 * {@code nextInt(1 << 31)} won't behave this way because {@code 1 << 31} is negative. If
 * {@code bits} is 32 (or 0), this can return any {@code int}.
 *
 * <p>The general contract of {@code next} is that it returns an
 * {@code int} value and if the argument {@code bits} is between
 * {@code 1} and {@code 32} (inclusive), then that many low-order
 * bits of the returned value will be (approximately) independently
 * chosen bit values, each of which is (approximately) equally
 * likely to be {@code 0} or {@code 1}.
 * <p>
 * Note that you can give this values for {@code bits} that are outside its expected range of 1 to 32,
 * but the value used, as long as bits is positive, will effectively be {@code bits % 32}. As stated
 * before, a value of 0 for bits is the same as a value of 32.<p>
 *
 * @param bits the amount of random bits to request, from 1 to 32
 * @return the next pseudorandom value from this random number
 * generator's sequence
 */
        public uint NextBits(int bits)
        {
            return (uint)(NextUlong() >> 64 - bits);
        }

        /**
         * Generates random bytes and places them into a user-supplied
         * byte array.  The number of random bytes produced is equal to
         * the length of the byte array.
         *
         * @param bytes the byte array to fill with random bytes
         * @throws NullPointerException if the byte array is null
         */
        public void NextBytes(byte[] bytes)
        {
            int bl = bytes.Length;
            for (int i = 0; i < bl;)
            {
                int n = Math.Min(bl - i, 8);
                for (ulong r = NextUlong(); n-- > 0; r >>= 8)
                {
                    bytes[i++] = (byte)r;
                }
            }
        }

        /**
         * Returns the next pseudorandom, uniformly distributed {@code int}
         * value from this random number generator's sequence. The general
         * contract of {@code nextInt} is that one {@code int} value is
         * pseudorandomly generated and returned. All 2<sup>32</sup> possible
         * {@code int} values are produced with (approximately) equal probability.
         *
         * @return the next pseudorandom, uniformly distributed {@code int}
         * value from this random number generator's sequence
         */
        public int NextInt()
        {
            return (int)NextUlong();
        }

        /// <summary>
        /// Gets a random uint by using the low 32 bits of NextUlong(); this can return any uint.
        /// </summary>
        /// <returns>Any random uint.</returns>
        public uint NextUint()
        {
            return (uint)NextUlong();
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code int} value
         * between 0 (inclusive) and the specified value (exclusive), drawn from
         * this random number generator's sequence.  The general contract of
         * {@code nextInt} is that one {@code int} value in the specified range
         * is pseudorandomly generated and returned.  All {@code bound} possible
         * {@code int} values are produced with (approximately) equal
         * probability.
         * <br>
         * It should be mentioned that the technique this uses has some bias, depending
         * on {@code bound}, but it typically isn't measurable without specifically looking
         * for it. Using the method this does allows this method to always advance the state
         * by one step, instead of a varying and unpredictable amount with the more typical
         * ways of rejection-sampling random numbers and only using numbers that can produce
         * an int within the bound without bias.
         * See <a href="https://www.pcg-random.org/posts/bounded-rands.html">M.E. O'Neill's
         * blog about random numbers</a> for discussion of alternative, unbiased methods.
         *
         * @param bound the upper bound (exclusive). If negative or 0, this always returns 0.
         * @return the next pseudorandom, uniformly distributed {@code int}
         * value between zero (inclusive) and {@code bound} (exclusive)
         * from this random number generator's sequence
         */
        public uint NextUint(uint bound)
        {
            return (uint)(bound * (NextUlong() & 0xFFFFFFFFUL) >> 32);
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code int} value between an
         * inner bound of 0 (inclusive) and the specified {@code outerBound} (exclusive).
         * This is meant for cases where the outer bound may be negative, especially if
         * the bound is unknown or may be user-specified. A negative outer bound is used
         * as the lower bound; a positive outer bound is used as the upper bound. An outer
         * bound of -1, 0, or 1 will always return 0, keeping the bound exclusive (except
         * for outer bound 0).
         *
         * @see #nextInt(int) Here's a note about the bias present in the bounded generation.
         * @param outerBound the outer exclusive bound; may be any int value, allowing negative
         * @return a pseudorandom int between 0 (inclusive) and outerBound (exclusive)
         */
        public int NextInt(int outerBound)
        {
            outerBound = (int)(outerBound * ((long)NextUlong() & 0xFFFFFFFFL) >> 32);
            return outerBound + (outerBound >> 31);
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code int} value between the
         * specified {@code innerBound} (inclusive) and the specified {@code outerBound}
         * (exclusive). If {@code outerBound} is less than or equal to {@code innerBound},
         * this always returns {@code innerBound}. This is significantly slower than
         * {@link #nextInt(int)} or {@link #nextSignedInt(int)},
         * because this handles even ranges that go from large negative numbers to large
         * positive numbers, and since that would be larger than the largest possible int,
         * this has to use {@link #nextLong(long)}.
         *
         * <br> For any case where outerBound might be valid but less than innerBound, you
         * can use {@link #nextSignedInt(int, int)}. If outerBound is less than innerBound
         * here, this simply returns innerBound.
         *
         * @see #nextInt(int) Here's a note about the bias present in the bounded generation.
         * @param innerBound the inclusive inner bound; may be any int, allowing negative
         * @param outerBound the exclusive outer bound; must be greater than innerBound (otherwise this returns innerBound)
         * @return a pseudorandom int between innerBound (inclusive) and outerBound (exclusive)
         */
        public uint NextUint(uint innerBound, uint outerBound)
        {
            return (uint)NextUlong(innerBound, outerBound);
        }

        /**
         * Returns a pseudorandom, uniformly distributed {@code int} value between the
         * specified {@code innerBound} (inclusive) and the specified {@code outerBound}
         * (exclusive). This is meant for cases where either bound may be negative,
         * especially if the bounds are unknown or may be user-specified. It is slightly
         * slower than {@link #nextInt(int, int)}, and significantly slower than
         * {@link #nextInt(int)} or {@link #nextSignedInt(int)}. This last part is
         * because this handles even ranges that go from large negative numbers to large
         * positive numbers, and since that range is larger than the largest possible int,
         * this has to use {@link #nextSignedLong(long)}.
         *
         * @see #nextInt(int) Here's a note about the bias present in the bounded generation.
         * @param innerBound the inclusive inner bound; may be any int, allowing negative
         * @param outerBound the exclusive outer bound; may be any int, allowing negative
         * @return a pseudorandom int between innerBound (inclusive) and outerBound (exclusive)
         */
        public int NextInt(int innerBound, int outerBound)
        {
            return (int)NextLong(innerBound, outerBound);
        }


        /**
         * Returns the next pseudorandom, uniformly distributed
 * {@code bool} value from this random number generator's
 * sequence. The general contract of {@code NextBool} is that one
 * {@code bool} value is pseudorandomly generated and returned.  The
 * values {@code true} and {@code false} are produced with
 * (approximately) equal probability.
 * 
 * The default implementation is equivalent to a sign check on {@link #NextUlong()},
 * returning true if the generated long is negative. This is typically the safest
 * way to implement this method; many types of generators have less statistical
 * quality on their lowest bit, so just returning based on the lowest bit isn't
 * always a good idea.
 *
 * @return the next pseudorandom, uniformly distributed
 * {@code bool} value from this random number generator's
 * sequence
 */
        public virtual bool NextBool()
        {
            return (NextUlong() & 0x8000000000000000UL) == 0x8000000000000000UL;
        }

        /**
         * Returns the next pseudorandom, uniformly distributed {@code float}
         * value between {@code 0.0} (inclusive) and {@code 1.0} (exclusive)
         * from this random number generator's sequence.
         *
         * <p>The general contract of {@code NextFloat} is that one
         * {@code float} value, chosen (approximately) uniformly from the
         * range {@code 0.0f} (inclusive) to {@code 1.0f} (exclusive), is
         * pseudorandomly generated and returned. All 2<sup>24</sup> possible
         * {@code float} values of the form <i>m&nbsp;x&nbsp;</i>2<sup>-24</sup>,
         * where <i>m</i> is a positive integer less than 2<sup>24</sup>, are
         * produced with (approximately) equal probability.
         *
         * <p>The public implementation uses the upper 24 bits of {@link #nextLong()},
         * with an unsigned right shift and a multiply by a very small float
         * ({@code 5.9604645E-8f} or {@code 0x1p-24f}). It tends to be fast if
         * nextLong() is fast, but alternative implementations could use 24 bits of
         * {@link #nextInt()} (or just {@link #next(int)}, giving it {@code 24})
         * if that generator doesn't efficiently generate 64-bit longs.<p>
         *
         * @return the next pseudorandom, uniformly distributed {@code float}
         * value between {@code 0.0} and {@code 1.0} from this
         * random number generator's sequence
         */
        public virtual float NextFloat()
        {
            return (NextUlong() >> 40) * FLOAT_ADJUST;
        }

        /**
         * Gets a pseudo-random float between 0 (inclusive) and {@code outerBound} (exclusive).
         * The outerBound may be positive or negative.
         * Exactly the same as {@code NextFloat() * outerBound}.
         * @param outerBound the exclusive outer bound
         * @return a float between 0 (inclusive) and {@code outerBound} (exclusive)
         */
        public float NextFloat(float outerBound)
        {
            return NextFloat() * outerBound;
        }

        /**
         * Gets a pseudo-random float between {@code innerBound} (inclusive) and {@code outerBound} (exclusive).
         * Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
         * inclusive and which is exclusive.
         * @param innerBound the inclusive inner bound; may be negative
         * @param outerBound the exclusive outer bound; may be negative
         * @return a float between {@code innerBound} (inclusive) and {@code outerBound} (exclusive)
         */
        public float NextFloat(float innerBound, float outerBound)
        {
            return innerBound + NextFloat() * (outerBound - innerBound);
        }

        /**
         * Returns the next pseudorandom, uniformly distributed
         * {@code double} value between {@code 0.0} (inclusive) and {@code 1.0}
         * (exclusive) from this random number generator's sequence.
         *
         * <p>The general contract of {@code NextDouble} is that one
         * {@code double} value, chosen (approximately) uniformly from the
         * range {@code 0.0d} (inclusive) to {@code 1.0d} (exclusive), is
         * pseudorandomly generated and returned.
         *
         * <p>The default implementation uses the upper 53 bits of {@link #nextLong()},
         * with an unsigned right shift and a multiply by a very small double
         * ({@code 1.1102230246251565E-16}, or {@code 0x1p-53}). It should perform well
         * if nextLong() performs well, and is expected to perform less well if the
         * generator naturally produces 32 or fewer bits at a time.<p>
         *
         * @return the next pseudorandom, uniformly distributed {@code double}
         * value between {@code 0.0} and {@code 1.0} from this
         * random number generator's sequence
         */
        public virtual double NextDouble()
        {
            return (NextUlong() >> 11) * DOUBLE_ADJUST;
        }

        /**
         * Gets a pseudo-random double between 0 (inclusive) and {@code outerBound} (exclusive).
         * The outerBound may be positive or negative.
         * Exactly the same as {@code NextDouble() * outerBound}.
         * @param outerBound the exclusive outer bound
         * @return a double between 0 (inclusive) and {@code outerBound} (exclusive)
         */
        public double NextDouble(double outerBound)
        {
            return NextDouble() * outerBound;
        }

        /**
         * Gets a pseudo-random double between {@code innerBound} (inclusive) and {@code outerBound} (exclusive).
         * Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
         * inclusive and which is exclusive.
         * @param innerBound the inclusive inner bound; may be negative
         * @param outerBound the exclusive outer bound; may be negative
         * @return a double between {@code innerBound} (inclusive) and {@code outerBound} (exclusive)
         */
        public double NextDouble(double innerBound, double outerBound)
        {
            return innerBound + NextDouble() * (outerBound - innerBound);
        }

        /**
         * This is just like {@link #NextDouble()}, returning a double between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
         * It returns 1.0 extremely rarely, 0.000000000000011102230246251565% of the time if there is no bias in the generator, but it
         * can happen. This uses {@link #nextLong(long)} internally, so it may have some bias towards or against specific
         * subtly-different results.
         * @return a double between 0.0, inclusive, and 1.0, inclusive
         */
        public double NextInclusiveDouble()
        {
            return NextUlong(0x20000000000001L) * DOUBLE_ADJUST;
        }

        /**
         * Just like {@link #NextDouble(double)}, but this is inclusive on both 0.0 and {@code outerBound}.
         * It may be important to note that it returns outerBound on only 0.000000000000011102230246251565% of calls.
         * @param outerBound the outer inclusive bound; may be positive or negative
         * @return a double between 0.0, inclusive, and {@code outerBound}, inclusive
         */
        public double NextInclusiveDouble(double outerBound)
        {
            return NextInclusiveDouble() * outerBound;
        }

        /**
         * Just like {@link #NextDouble(double, double)}, but this is inclusive on both {@code innerBound} and {@code outerBound}.
         * It may be important to note that it returns outerBound on only 0.000000000000011102230246251565% of calls, if it can
         * return it at all because of floating-point imprecision when innerBound is a larger number.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer inclusive bound; may be positive or negative
         * @return a double between {@code innerBound}, inclusive, and {@code outerBound}, inclusive
         */
        public double NextInclusiveDouble(double innerBound, double outerBound)
        {
            return innerBound + NextInclusiveDouble() * (outerBound - innerBound);
        }

        /**
         * This is just like {@link #NextFloat()}, returning a float between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
         * It returns 1.0 rarely, 0.00000596046412226771% of the time if there is no bias in the generator, but it can happen. This method
         * has been tested by generating 268435456 (or 0x10000000) random ints with {@link #nextInt(int)}, and just before the end of that
         * it had generated every one of the 16777217 roughly-equidistant floats this is able to produce. Not all seeds and streams are
         * likely to accomplish that in the same time, or at all, depending on the generator.
         * @return a float between 0.0, inclusive, and 1.0, inclusive
         */
        public float NextInclusiveFloat()
        {
            return NextInt(0x1000001) * FLOAT_ADJUST;
        }

        /**
         * Just like {@link #NextFloat(float)}, but this is inclusive on both 0.0 and {@code outerBound}.
         * It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls.
         * @param outerBound the outer inclusive bound; may be positive or negative
         * @return a float between 0.0, inclusive, and {@code outerBound}, inclusive
         */
        public float NextInclusiveFloat(float outerBound)
        {
            return NextInclusiveFloat() * outerBound;
        }

        /**
         * Just like {@link #NextFloat(float, float)}, but this is inclusive on both {@code innerBound} and {@code outerBound}.
         * It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls, if it can return
         * it at all because of floating-point imprecision when innerBound is a larger number.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer inclusive bound; may be positive or negative
         * @return a float between {@code innerBound}, inclusive, and {@code outerBound}, inclusive
         */
        public float NextInclusiveFloat(float innerBound, float outerBound)
        {
            return innerBound + NextInclusiveFloat() * (outerBound - innerBound);
        }

        ///**
        // * Gets a random double between 0.0 and 1.0, exclusive at both ends; this method is also more uniform than
        // * {@link #NextDouble()} if you use the bit-patterns of the returned doubles. This is a simplified version of
        // * <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>. This can return double
        // * values between 2.710505431213761E-20 and 0.9999999999999999, or 0x1.0p-65 and 0x1.fffffffffffffp-1 in hex
        // * notation. It cannot return 0 or 1. Most cases can instead use {@link #nextExclusiveDoubleEquidistant()}, which is
        // * implemented more traditionally but may have different performance. This method can also return doubles that
        // * are extremely close to 0, but can't return doubles that are as close to 1, due to limits of doubles.
        // * However, nextExclusiveDoubleEquidistant() can return only a minimum value that is as distant from 0 as its maximum
        // * value is distant from 1.
        // * <br>
        // * To compare, NextDouble() and nextExclusiveDoubleEquidistant() are less likely to produce a "1" bit for their
        // * lowest 5 bits of mantissa/significand (the least significant bits numerically, but potentially important
        // * for some uses), with the least significant bit produced half as often as the most significant bit in the
        // * mantissa. As for this method, it has approximately the same likelihood of producing a "1" bit for any
        // * position in the mantissa.
        // * <br>
        // * The default implementation may have different performance characteristics than {@link #NextDouble()},
        // * because this doesn't perform any floating-point multiplication or division, and instead assembles bits
        // * obtained by one call to {@link #nextLong()}. This uses {@link BitConversion#longBitsToDouble(long)} and
        // * {@link Long#numberOfTrailingZeros(long)}, both of which typically have optimized intrinsics on HotSpot,
        // * and this is branchless and loopless, unlike the original algorithm by Allen Downey. When compared with
        // * {@link #nextExclusiveDoubleEquidistant()}, this method performs better on at least HotSpot JVMs.
        // * @return a random uniform double between 0 and 1 (both exclusive)
        // */
        //public double nextExclusiveDouble()
        //       {
        //           long bits = NextLong();
        //           return BitConverter.Int64BitsToDouble(1022L - Long.numberOfTrailingZeros(bits) << 52
        //               | bits >>> 12);
        //       }

        /**
         * Gets a random double between 0.0 and 1.0, exclusive at both ends. This can return double
         * values between 1.1102230246251565E-16 and 0.9999999999999999, or 0x1.0p-53 and 0x1.fffffffffffffp-1 in hex
         * notation. It cannot return 0 or 1, and its minimum and maximum results are equally distant from 0 and from
         * 1, respectively. Some usages may prefer {@link #nextExclusiveDouble()}, which is
         * better-distributed if you consider the bit representation of the returned doubles, tends to perform
         * better, and can return doubles that much closer to 0 than this can.
         * <br>
         * The default implementation simply uses {@link #nextLong(long)} to get a uniformly-chosen long between 1 and
         * (2 to the 53) - 1, both inclusive, and multiplies it by (2 to the -53). Using larger values than (2 to the
         * 53) would cause issues with the double math.
         * @return a random uniform double between 0 and 1 (both exclusive)
         */
        public double NextExclusiveDouble()
        {
            return (NextUlong(0x1FFFFFFFFFFFFFL) + 1L) * DOUBLE_ADJUST;
        }

        /**
         * Just like {@link #NextDouble(double)}, but this is exclusive on both 0.0 and {@code outerBound}.
         * Like {@link #nextExclusiveDouble()}, which this uses, this may have better bit-distribution of
         * double values, and it may also be better able to produce very small doubles when {@code outerBound} is large.
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @return a double between 0.0, exclusive, and {@code outerBound}, exclusive
         */
        public double NextExclusiveDouble(double outerBound)
        {
            return NextExclusiveDouble() * outerBound;
        }

        /**
         * Just like {@link #NextDouble(double, double)}, but this is exclusive on both {@code innerBound} and {@code outerBound}.
         * Like {@link #nextExclusiveDouble()}, which this uses,, this may have better bit-distribution of double values,
         * and it may also be better able to produce doubles close to innerBound when {@code outerBound - innerBound} is large.
         * @param innerBound the inner exclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @return a double between {@code innerBound}, exclusive, and {@code outerBound}, exclusive
         */
        public double NextExclusiveDouble(double innerBound, double outerBound)
        {
            return innerBound + NextExclusiveDouble() * (outerBound - innerBound);
        }

        ///**
        // * Gets a random float between 0.0 and 1.0, exclusive at both ends. This method is also more uniform than
        // * {@link #NextFloat()} if you use the bit-patterns of the returned floats. This is a simplified version of
        // * <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>. This version can
        // * return float values between 2.7105054E-20 to 0.99999994, or 0x1.0p-65 to 0x1.fffffep-1 in hex notation.
        // * It cannot return 0 or 1. To compare, NextFloat() is less likely to produce a "1" bit for its
        // * lowest 5 bits of mantissa/significand (the least significant bits numerically, but potentially important
        // * for some uses), with the least significant bit produced half as often as the most significant bit in the
        // * mantissa. As for this method, it has approximately the same likelihood of producing a "1" bit for any
        // * position in the mantissa.
        // * <br>
        // * The default implementation may have different performance characteristics than {@link #NextFloat()},
        // * because this doesn't perform any floating-point multiplication or division, and instead assembles bits
        // * obtained by one call to {@link #nextLong()}. This uses {@link BitConversion#intBitsToFloat(int)} and
        // * {@link Long#numberOfTrailingZeros(long)}, both of which typically have optimized intrinsics on HotSpot,
        // * and this is branchless and loopless, unlike the original algorithm by Allen Downey. When compared with
        // * {@link #nextExclusiveFloatEquidistant()}, this method performs better on at least HotSpot JVMs.
        // * @return a random uniform float between 0 and 1 (both exclusive)
        // */
        //public float nextExclusiveFloat()
        //       {
        //           final long bits = nextLong();
        //           return BitConversion.intBitsToFloat(126 - Long.numberOfTrailingZeros(bits) << 23
        //               | (int)(bits >>> 41));
        //       }

        /**
         * Gets a random float between 0.0 and 1.0, exclusive at both ends. This can return float
         * values between 5.9604645E-8 and 0.99999994, or 0x1.0p-24 and 0x1.fffffep-1 in hex notation.
         * It cannot return 0 or 1, and its minimum and maximum results are equally distant from 0 and from
         * 1, respectively. Some usages may prefer {@link #nextExclusiveFloat()}, which is
         * better-distributed if you consider the bit representation of the returned floats, tends to perform
         * better, and can return floats that much closer to 0 than this can.
         * <br>
         * The default implementation simply uses {@link #nextInt(int)} to get a uniformly-chosen int between 1 and
         * (2 to the 24) - 1, both inclusive, and multiplies it by (2 to the -24). Using larger values than (2 to the
         * 24) would cause issues with the float math.
         * @return a random uniform float between 0 and 1 (both exclusive)
         */
        public float NextExclusiveFloat()
        {
            return (NextUint(0xFFFFFFU) + 1) * FLOAT_ADJUST;
        }

        /**
         * Just like {@link #NextFloat(float)}, but this is exclusive on both 0.0 and {@code outerBound}.
         * Like {@link #nextExclusiveFloat()}, this may have better bit-distribution of float values, and
         * it may also be better able to produce very small floats when {@code outerBound} is large.
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @return a float between 0.0, exclusive, and {@code outerBound}, exclusive
         */
        public float NextExclusiveFloat(float outerBound)
        {
            return NextExclusiveFloat() * outerBound;
        }

        /**
         * Just like {@link #NextFloat(float, float)}, but this is exclusive on both {@code innerBound} and {@code outerBound}.
         * Like {@link #nextExclusiveFloat()}, this may have better bit-distribution of float values, and
         * it may also be better able to produce floats close to innerBound when {@code outerBound - innerBound} is large.
         * @param innerBound the inner exclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @return a float between {@code innerBound}, exclusive, and {@code outerBound}, exclusive
         */
        public float NextExclusiveFloat(float innerBound, float outerBound)
        {
            return innerBound + NextExclusiveFloat() * (outerBound - innerBound);
        }


        /**
     * A way of taking a double in the (0.0, 1.0) range and mapping it to a Gaussian or normal distribution, so high
     * inputs correspond to high outputs, and similarly for the low range. This is centered on 0.0 and its standard
     * deviation seems to be 1.0 (the same as {@link java.util.Random#nextGaussian()}). If this is given an input of 0.0
     * or less, it returns -38.5, which is slightly less than the result when given {@link Double#MIN_VALUE}. If it is
     * given an input of 1.0 or more, it returns 38.5, which is significantly larger than the result when given the
     * largest double less than 1.0 (this value is further from 1.0 than {@link Double#MIN_VALUE} is from 0.0). If
     * given {@link Double#NaN}, it returns whatever {@link Math#copySign(double, double)} returns for the arguments
     * {@code 38.5, Double.NaN}, which is implementation-dependent. It uses an algorithm by Peter John Acklam, as
     * implemented by Sherali Karimov.
     * <a href="https://web.archive.org/web/20150910002142/http://home.online.no/~pjacklam/notes/invnorm/impl/karimov/StatUtil.java">Original source</a>.
     * <a href="https://web.archive.org/web/20151030215612/http://home.online.no/~pjacklam/notes/invnorm/">Information on the algorithm</a>.
     * <a href="https://en.wikipedia.org/wiki/Probit_function">Wikipedia's page on the probit function</a> may help, but
     * is more likely to just be confusing.
     * <br>
     * Acklam's algorithm and Karimov's implementation are both quite fast. This appears faster than generating
     * Gaussian-distributed numbers using either the Box-Muller Transform or Marsaglia's Polar Method, though it isn't
     * as precise and can't produce as extreme min and max results in the extreme cases they should appear. If given
     * a typical uniform random {@code double} that's exclusive on 1.0, it won't produce a result higher than
     * {@code 8.209536145151493}, and will only produce results of at least {@code -8.209536145151493} if 0.0 is
     * excluded from the inputs (if 0.0 is an input, the result is {@code -38.5}). A chief advantage of using this with
     * a random number generator is that it only requires one random double to obtain one Gaussian value;
     * {@link java.util.Random#nextGaussian()} generates at least two random doubles for each two Gaussian values, but
     * may rarely require much more random generation.
     * <br>
     * This can be used both as an optimization for generating Gaussian random values, and as a way of generating
     * Gaussian values that match a pattern present in the inputs (which you could have by using a sub-random sequence
     * as the input, such as those produced by a van der Corput, Halton, Sobol or R2 sequence). Most methods of generating
     * Gaussian values (e.g. Box-Muller and Marsaglia polar) do not have any way to preserve a particular pattern.
     *
     * @param d should be between 0 and 1, exclusive, but other values are tolerated
     * @return a normal-distributed double centered on 0.0; all results will be between -38.5 and 38.5, both inclusive
     */
        public static double Probit(double d)
        {
            if (d <= 0)
            {
                return -38.5;
            }
            else if (d >= 1)
            {
                return 38.5;
            }
            else if (d < 0.02425)
            {
                double q = Math.Sqrt(-2.0 * Math.Log(d));
                return (((((-7.784894002430293e-03 * q + -3.223964580411365e-01) * q + -2.400758277161838e+00) * q + -2.549732539343734e+00) * q + 4.374664141464968e+00) * q + 2.938163982698783e+00) / (
                    (((7.784695709041462e-03 * q + 3.224671290700398e-01) * q + 2.445134137142996e+00) * q + 3.754408661907416e+00) * q + 1.0);
            }
            else if (0.97575 < d)
            {
                double q = Math.Sqrt(-2.0 * Math.Log(1 - d));
                return -(((((-7.784894002430293e-03 * q + -3.223964580411365e-01) * q + -2.400758277161838e+00) * q + -2.549732539343734e+00) * q + 4.374664141464968e+00) * q + 2.938163982698783e+00) / (
                    (((7.784695709041462e-03 * q + 3.224671290700398e-01) * q + 2.445134137142996e+00) * q + 3.754408661907416e+00) * q + 1.0);
            }
            else
            {
                double q = d - 0.5;
                double r = q * q;
                return (((((-3.969683028665376e+01 * r + 2.209460984245205e+02) * r + -2.759285104469687e+02) * r + 1.383577518672690e+02) * r + -3.066479806614716e+01) * r + 2.506628277459239e+00) * q / (
                    ((((-5.447609879822406e+01 * r + 1.615858368580409e+02) * r + -1.556989798598866e+02) * r + 6.680131188771972e+01) * r + -1.328068155288572e+01) * r + 1.0);
            }
        }

        /// <summary>
        /// Gets a normally-distributed (Gaussian) double, with a the specified mean (default 0.0) and standard deviation (default 1.0).
        /// If the standard deviation is 1.0 and the mean is 0.0, then this can produce results between -38.5 and 38.5 (both extremely rarely).
        /// </summary>
        /// <returns>A double from the normal distribution with the specified mean (default 0.0) and standard deviation (default 1.0).</returns>
        public double NextNormal(double mean = 0.0, double stdDev = 1.0)
        {
            return Probit(NextInclusiveDouble());
        }

        /// <summary>
        /// (Optional) If implemented, this should jump the generator forward by the given number of steps as distance and return the result of NextUlong()
        /// as if called at that step. The distance can be negative if a long is cast to a ulong, which jumps backwards if the period of the generator is 2 to the 64.
        /// </summary>
        /// <param name="distance">How many steps to jump forward</param>
        /// <returns>The result of what NextUlong() would return at the now-current jumped state.</returns>
        public virtual ulong Skip(ulong distance)
        {
            throw new NotSupportedException("Skip() is not implemented for this generator.");
        }

        /// <summary>
        /// (Optional) If implemented, jumps the generator back to the previous state and returns what NextUlong() would have produced at that state.
        /// </summary>
        /// <remarks>
        /// The default implementation calls Skip() with the equivalent of (ulong)(-1L) . If Skip() is not implemented, this throws a NotSupportedException.
        /// </remarks>
        /// <returns>The result of what NextUlong() would return at the previous state.</returns>
        public virtual ulong PreviousUlong()
        {
            return Skip(0xFFFFFFFFFFFFFFFFUL);
        }

        /// <summary>
        /// Returns a full copy (deep, if necessary) of this IEnhancedRandom.
        /// </summary>
        /// <returns>A copy of this IEnhancedRandom.</returns>
        public abstract ARandom Copy();

        /// <summary>
        /// Sets each state in this IEnhancedRandom to the corresponding state in the other IEnhancedRandom.
        /// This generally only works correctly if both objects have the same class.
        /// </summary>
        /// <param name="other">Another IEnhancedRandom that almost always should have the same class as this one.</param>
        public void SetWith(ARandom other)
        {
            int myCount = StateCount, otherCount = other.StateCount;
            int i = 0;
            for (; i < myCount && i < otherCount; i++)
            {
                SetSelectedState(i, other.SelectState(i));
            }
            for (; i < myCount; i++)
            {
                SetSelectedState(i, 0xFFFFFFFFFFFFFFFFUL);
            }
        }

        /**
 * Given two EnhancedRandom objects that could have the same or different classes,
 * this returns true if they have the same class and same state, or false otherwise.
 * Both of the arguments should implement {@link #getSelectedState(int)}, or this
 * will throw an UnsupportedOperationException. This can be useful for comparing
 * EnhancedRandom classes that do not implement equals(), for whatever reason.
 * @param left an EnhancedRandom to compare for equality
 * @param right another EnhancedRandom to compare for equality
 * @return true if the two EnhancedRandom objects have the same class and state, or false otherwise
 */
        public static bool AreEqual(ARandom left, ARandom right)
        {
            if (left == right)
                return true;
            if (left.GetType() != right.GetType())
                return false;

            int count = left.StateCount;
            for (int i = 0; i < count; i++)
            {
                if (left.SelectState(i) != right.SelectState(i))
                    return false;
            }
            return true;
        }

        /**
 * Returns true if a random value between 0 and 1 is less than the specified value.
 *
 * @param chance a float between 0.0 and 1.0; higher values are more likely to result in true
 * @return a bool selected with the given {@code chance} of being true
 */
        public bool NextBool(float chance)
        {
            return NextFloat() < chance;
        }

        /**
         * Returns -1 or 1, randomly.
         *
         * @return -1 or 1, selected with approximately equal likelihood
         */
        public int NextSign()
        {
            return 1 | NextInt() >> 31;
        }

        /**
         * Returns a triangularly distributed random number between -1.0 (exclusive) and 1.0 (exclusive), where values around zero are
         * more likely. Advances the state twice.
         * <p>
         * This is an optimized version of {@link #NextTriangular(float, float, float) NextTriangular(-1, 1, 0)}
         */
        public float NextTriangular()
        {
            return NextFloat() - NextFloat();
        }

        /**
         * Returns a triangularly distributed random number between {@code -max} (exclusive) and {@code max} (exclusive), where values
         * around zero are more likely. Advances the state twice.
         * <p>
         * This is an optimized version of {@link #nextTriangular(float, float, float) NextTriangular(-max, max, 0)}
         *
         * @param max the upper limit
         */
        public float NextTriangular(float max)
        {
            return (NextFloat() - NextFloat()) * max;
        }

        /**
         * Returns a triangularly distributed random number between {@code min} (inclusive) and {@code max} (exclusive), where the
         * {@code mode} argument defaults to the midpoint between the bounds, giving a symmetric distribution. Advances the state once.
         * <p>
         * This method is equivalent of {@link #nextTriangular(float, float, float) NextTriangular(min, max, (min + max) * 0.5f)}
         *
         * @param min the lower limit
         * @param max the upper limit
         */
        public float NextTriangular(float min, float max)
        {
            return NextTriangular(min, max, (min + max) * 0.5f);
        }

        /**
         * Returns a triangularly distributed random number between {@code min} (inclusive) and {@code max} (exclusive), where values
         * around {@code mode} are more likely. Advances the state once.
         *
         * @param min  the lower limit
         * @param max  the upper limit
         * @param mode the point around which the values are more likely
         */
        public float NextTriangular(float min, float max, float mode)
        {
            float u = NextFloat();
            float d = max - min;
            if (u <= (mode - min) / d) { return min + MathF.Sqrt(u * d * (mode - min)); }
            return max - MathF.Sqrt((1 - u) * d * (max - mode));
        }

        /**
 * Returns the minimum result of {@code trials} calls to {@link #NextInt(int, int)} using the given {@code innerBound}
 * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
 * The higher trials is, the lower the average value this returns.
 * @param innerBound the inner inclusive bound; may be positive or negative
 * @param outerBound the outer exclusive bound; may be positive or negative
 * @param trials how many random numbers to acquire and compare
 * @return the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found
 */
        public int MinIntOf(int innerBound, int outerBound, int trials)
        {
            int v = NextInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextInt(innerBound, outerBound));
            }
            return v;
        }

        /**
         * Returns the maximum result of {@code trials} calls to {@link #NextInt(int, int)} using the given {@code innerBound}
         * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
         * The higher trials is, the higher the average value this returns.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @param trials how many random numbers to acquire and compare
         * @return the highest random number between innerBound (inclusive) and outerBound (exclusive) this found
         */
        public int MaxIntOf(int innerBound, int outerBound, int trials)
        {
            int v = NextInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextInt(innerBound, outerBound));
            }
            return v;
        }

        /**
         * Returns the minimum result of {@code trials} calls to {@link #NextLong(long, long)} using the given {@code innerBound}
         * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
         * The higher trials is, the lower the average value this returns.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @param trials how many random numbers to acquire and compare
         * @return the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found
         */
        public long MinLongOf(long innerBound, long outerBound, int trials)
        {
            long v = NextLong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextLong(innerBound, outerBound));
            }
            return v;
        }

        /**
         * Returns the maximum result of {@code trials} calls to {@link #NextLong(long, long)} using the given {@code innerBound}
         * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
         * The higher trials is, the higher the average value this returns.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @param trials how many random numbers to acquire and compare
         * @return the highest random number between innerBound (inclusive) and outerBound (exclusive) this found
         */
        public long MaxLongOf(long innerBound, long outerBound, int trials)
        {
            long v = NextLong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextLong(innerBound, outerBound));
            }
            return v;
        }

        /**
         * Returns the minimum result of {@code trials} calls to {@link #NextDouble(double, double)} using the given {@code innerBound}
         * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
         * The higher trials is, the lower the average value this returns.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @param trials how many random numbers to acquire and compare
         * @return the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found
         */
        public double MinDoubleOf(double innerBound, double outerBound, int trials)
        {
            double v = NextDouble(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextDouble(innerBound, outerBound));
            }
            return v;
        }

        /**
         * Returns the maximum result of {@code trials} calls to {@link #NextDouble(double, double)} using the given {@code innerBound}
         * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
         * The higher trials is, the higher the average value this returns.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @param trials how many random numbers to acquire and compare
         * @return the highest random number between innerBound (inclusive) and outerBound (exclusive) this found
         */
        public double MaxDoubleOf(double innerBound, double outerBound, int trials)
        {
            double v = NextDouble(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextDouble(innerBound, outerBound));
            }
            return v;
        }

        /**
         * Returns the minimum result of {@code trials} calls to {@link #NextFloat(float, float)} using the given {@code innerBound}
         * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
         * The higher trials is, the lower the average value this returns.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @param trials how many random numbers to acquire and compare
         * @return the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found
         */
        public float MinFloatOf(float innerBound, float outerBound, int trials)
        {
            float v = NextFloat(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextFloat(innerBound, outerBound));
            }
            return v;
        }

        /**
         * Returns the maximum result of {@code trials} calls to {@link #NextFloat(float, float)} using the given {@code innerBound}
         * and {@code outerBound}. The innerBound is inclusive; the outerBound is exclusive.
         * The higher trials is, the higher the average value this returns.
         * @param innerBound the inner inclusive bound; may be positive or negative
         * @param outerBound the outer exclusive bound; may be positive or negative
         * @param trials how many random numbers to acquire and compare
         * @return the highest random number between innerBound (inclusive) and outerBound (exclusive) this found
         */
        public float MaxFloatOf(float innerBound, float outerBound, int trials)
        {
            float v = NextFloat(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextFloat(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Gets a randomly-chosen item from the given non-null, non-empty array.
        /// </summary>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="array">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen item from array.</returns>
        public T RandomElement<T>(T[] array)
        {
            return array[NextInt(array.Length)];
        }

        /// <summary>
        /// Gets a randomly-chosen item from the given non-null, non-empty IList.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen item from list.</returns>
        public T RandomElement<T>(IList<T> list)
        {
            return list[NextInt(list.Count)];
        }

        /**
         * Shuffles the given array in-place pseudo-randomly, using this to generate
         * {@code items.Length - 1} random numbers and using the Fisher-Yates (also called Knuth) shuffle algorithm.
         *
         * @param items an array of some reference type; must be non-null but may contain null items
         */
        public void Shuffle<T>(T[] items)
        {
            Shuffle(items, 0, items.Length);
        }

        /**
         * Shuffles a section of the given array in-place pseudo-randomly, using this to generate
         * {@code length - 1} random numbers and using the Fisher-Yates (also called Knuth) shuffle algorithm.
         *
         * @param items an array of some reference type; must be non-null but may contain null items
         * @param offset the index of the first element of the array that can be shuffled
         * @param length the length of the section to shuffle
         */
        public void Shuffle<T>(T[] items, int offset, int length)
        {
            offset = Math.Min(Math.Max(0, offset), items.Length);
            length = Math.Min(items.Length - offset, Math.Max(0, length));
            for (int i = offset + length - 1; i > offset; i--)
            {
                int ii = NextInt(offset, i + 1);
                T temp = items[i];
                items[i] = items[ii];
                items[ii] = temp;
            }
        }
        /**
         * Shuffles a section of the given array in-place pseudo-randomly, using this to generate
         * {@code length - 1} random numbers and using the Fisher-Yates (also called Knuth) shuffle algorithm.
         *
         * @param items an array of some reference type; must be non-null but may contain null items
         * @param offset the index of the first element of the array that can be shuffled
         * @param length the length of the section to shuffle
         */
        public void Shuffle<T>(IList<T> items, int offset, int length)
        {
            offset = Math.Min(Math.Max(0, offset), items.Count);
            length = Math.Min(items.Count - offset, Math.Max(0, length));
            for (int i = offset + length - 1; i > offset; i--)
            {
                int ii = NextInt(offset, i + 1);
                T temp = items[i];
                items[i] = items[ii];
                items[ii] = temp;
            }
        }

    }
}
