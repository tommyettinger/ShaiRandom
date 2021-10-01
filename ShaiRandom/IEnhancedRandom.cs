using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    public interface IEnhancedRandom
    {
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
	public uint nextUint(uint bound)
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
	public uint nextUint(uint innerBound, uint outerBound)
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
	public int nextInt(int innerBound, int outerBound)
        {
            return (int)(innerBound + NextLong(outerBound - innerBound & 0xFFFFFFFFL));
        }


    }
}
