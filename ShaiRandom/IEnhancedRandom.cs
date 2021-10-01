using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    public interface IEnhancedRandom
    {
        private static readonly float FLOAT_ADJUST = MathF.Pow(2f, -24f);
        private static readonly double DOUBLE_ADJUST = Math.Pow(2.0, -53.0);

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
        public void Seed(ulong seed);

        /**
         * Gets the number of possible state variables that can be selected with
         * {@link #getSelectedState(int)} or {@link #SelectState(int, ulong)}.
         * This defaults to returning 0, making no state variable available for
         * reading or writing. An implementation that has only one {@code ulong}
         * state, like a SplitMix64 generator, should return {@code 1}. A
         * generator that permits setting two different {@code ulong} values, like
         * {@link LaserRandom}, should return {@code 2}. Much larger values are
         * possible for types like the Mersenne Twister or some CMWC generators.
         * @return the non-negative number of selections possible for state variables
         */
        public int StateCount => 0;
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
        public ulong SelectState(int selection)
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
        public void SetSelectedState(int selection, ulong value)
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
        public void SetState(ulong state)
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
        public void SetState(ulong stateA, ulong stateB)
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
        public void SetState(ulong stateA, ulong stateB, ulong stateC)
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
        public void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
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
        public void SetState(params ulong[] states)
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
        public ulong NextUlong();

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
            for (int i = 0; i < bl;) {
                int n = Math.Min(bl - i, 8);
                for (ulong r = NextUlong(); n-- > 0; r >>= 8) {
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
         * for outer bound 0). This method is slightly slower than {@link #nextInt(int)}.
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
            return (uint)(innerBound + NextUlong(outerBound - innerBound & 0xFFFFFFFFUL));
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
            return (int)(innerBound + NextLong(outerBound - innerBound & 0xFFFFFFFFL));
        }
        /**
 * Returns the next pseudorandom, uniformly distributed
 * {@code boolean} value from this random number generator's
 * sequence. The general contract of {@code NextBool} is that one
 * {@code boolean} value is pseudorandomly generated and returned.  The
 * values {@code true} and {@code false} are produced with
 * (approximately) equal probability.
 * <br>
 * The default implementation is equivalent to a sign check on {@link #NextUlong()},
 * returning true if the generated long is negative. This is typically the safest
 * way to implement this method; many types of generators have less statistical
 * quality on their lowest bit, so just returning based on the lowest bit isn't
 * always a good idea.
 *
 * @return the next pseudorandom, uniformly distributed
 * {@code boolean} value from this random number generator's
 * sequence
 */
        public bool NextBool()
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
	public float NextFloat()
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
	public double NextDouble()
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

    }
}
