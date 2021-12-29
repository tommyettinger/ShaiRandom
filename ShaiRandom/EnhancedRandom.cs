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
        /// Bitwise left-rotation of a ulong by amt, in bits; assigns to ul in-place.
        /// </summary>
        /// <param name="ul">The ulong to rotate left; will be assigned to.</param>
        /// <param name="amt">How many bits to rotate.</param>
        public static void RotateLeftInPlace(ref this ulong ul, int amt) => ul = (ul << amt) | (ul >> 64 - amt);

        /// <summary>
        /// Bitwise right-rotation of a ulong by amt, in bits.
        /// </summary>
        /// <param name="ul">The ulong to rotate right.</param>
        /// <param name="amt">How many bits to rotate.</param>
        /// <returns>The rotated ul.</returns>
        public static ulong RotateRight(this ulong ul, int amt) => (ul >> amt) | (ul << 64 - amt);
        /// <summary>
        /// Bitwise right-rotation of a ulong by amt, in bits; assigns to ul in-place.
        /// </summary>
        /// <param name="ul">The ulong to rotate right; will be assigned to.</param>
        /// <param name="amt">How many bits to rotate.</param>
        public static void RotateRightInPlace(ref this ulong ul, int amt) => ul = (ul >> amt) | (ul << 64 - amt);
    }

    public interface IRandom
    {
        void Seed(ulong seed);
        /// <summary>
        /// Gets the number of possible state variables that can be selected with
        /// <see cref="SelectState(int)"/> or <see cref="SetSelectedState(int, ulong)"/>,
        /// even if those methods are not publicly accessible. An implementation that has only
        /// one ulong or uint state, like <see cref="DistinctRandom"/>, should produce 1.
        /// An implementation that has two uint or ulong states should produce 2, etc.
        /// This is always a non-negative number; though discouraged, it is allowed to be 0 for
        /// generators that attempt to conceal their state.
        /// </summary>
        int StateCount { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="SelectState(int)"/>, or false if that method is unsupported.
        /// </summary>
        bool SupportsReadAccess { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="SetSelectedState(int, ulong)"/>, or false if that method is unsupported.
        /// </summary>
        bool SupportsWriteAccess { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="Skip(ulong)"/>, or false if that method is unsupported.
        /// </summary>
        bool SupportsSkip { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="PreviousULong"/>, or false if that method is unsupported.
        /// </summary>
        bool SupportsPrevious { get; }

        /// <summary>
        /// Returns a full copy (deep, if necessary) of this IRandom.
        /// </summary>
        /// <returns>A copy of this IRandom.</returns>
        IRandom Copy();

        /// <summary>
        /// Produces a string that encodes the type and full state of this generator.
        /// This is an optional operation for classes that only implement IRandom; AbstractRandom requires an implementation.
        /// </summary>
        /// <returns>An encoded string that stores the type and full state of this generator.</returns>
        string StringSerialize();

        /// <summary>
        /// Given a string produced by <see cref="StringSerialize"/>, if the specified type is compatible,
        /// then this method sets the state of this IRandom to the specified stored state.
        /// This is an optional operation for classes that only implement IRandom; AbstractRandom requires an implementation.
        /// </summary>
        /// <param name="data">A string produced by StringSerialize.</param>
        /// <returns>This IRandom, after modifications.</returns>
        IRandom StringDeserialize(string data);

        ulong SelectState(int selection);
        void SetSelectedState(int selection, ulong value);
        void SetState(ulong state);

        /// <summary>
        /// Sets each state variable to either stateA or stateB, alternating.
        /// This uses <see cref="SetSelectedState(int, ulong)"/> to set the values. If there is one
        /// state variable (<see cref="StateCount"/> is 1), then this only sets that state
        /// variable to stateA. If there are two state variables, the first is set to stateA,
        /// and the second to stateB. If there are more, it reuses stateA, then stateB, then
        /// stateA, and so on until all variables are set.
        /// <param name="stateA">the ulong value to use for states at index 0, 2, 4, 6...</param>
        /// <param name="stateB">the ulong value to use for states at index 1, 3, 5, 7...</param>
        void SetState(ulong stateA, ulong stateB);

        /// <summary>
        /// Sets each state variable to stateA, stateB, or stateC,
        /// alternating. This uses <see cref="SetSelectedState(int, ulong)"/> to set the values.
        /// If there is one state variable (<see cref="StateCount"/> is 1), then this only
        /// sets that state variable to stateA. If there are two state variables, the first
        /// is set to stateA, and the second to stateB. With three state variables, the
        /// first is set to stateA, the second to stateB, and the third to stateC. If there
        /// are more, it reuses stateA, then stateB, then stateC, then stateA, and so on
        /// until all variables are set.
        /// <param name="stateA">the ulong value to use for states at index 0, 3, 6, 9...</param>
        /// <param name="stateB">the ulong value to use for states at index 1, 4, 7, 10...</param>
        /// <param name="stateC">the ulong value to use for states at index 2, 5, 8, 11...</param>
        void SetState(ulong stateA, ulong stateB, ulong stateC);

        /// <summary>
        /// Sets each state variable to stateA, stateB, stateC, or
        /// stateD, alternating. This uses <see cref="SetSelectedState(int, ulong)"/> to
        /// set the values. If there is one state variable (<see cref="StateCount"/> is 1),
        /// then this only sets that state variable to stateA. If there are two state
        /// variables, the first is set to stateA, and the second to stateB. With three
        /// state variables, the first is set to stateA, the second to stateB, and the third
        /// to stateC. With four state variables, the first is set to stateA, the second to
        /// stateB, the third to stateC, and the fourth to stateD. If there are more, it
        /// reuses stateA, then stateB, then stateC, then stateD, then stateA, and so on
        /// until all variables are set.
        /// <param name="stateA">the ulong value to use for states at index 0, 4, 8, 12...</param>
        /// <param name="stateB">the ulong value to use for states at index 1, 5, 9, 13...</param>
        /// <param name="stateC">the ulong value to use for states at index 2, 6, 10, 14...</param>
        /// <param name="stateD">the ulong value to use for states at index 3, 7, 11, 15...</param>
        void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD);

        /// <summary>
        /// Sets all state variables to alternating values chosen from states. If states is empty,
        /// then this does nothing, and leaves the current generator unchanged. This works for
        /// generators with any <see cref="StateCount"/>, but may allocate an array if states is
        /// used as a varargs (you can pass an existing array without needing to allocate). This
        /// uses <see cref="SetSelectedState(int, ulong)"/> to change the states.
        /// <param name="states">an array or varargs of ulong values to use as states</param>
        void SetState(params ulong[] states);
        /// <summary>
        /// Can return any ulong.
        /// </summary>
        /// <returns>A random ulong, which can have any ulong value.</returns>
        ulong NextULong();

        /// <summary>
        /// Can return any long, positive or negative.
        /// If you specifically want a non-negative long, you can use <code>(long)(NextULong() >> 1)</code>,
        /// which can return any long that is not negative.
        /// </summary>
        /// <returns>A random long, which may be positive or negative, and can have any long value.</returns>
        long NextLong();

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value
        /// between 0 (inclusive) and the specified value (exclusive), drawn from
        /// this random number generator's sequence.  The general contract of
        /// nextLong is that one long value in the specified range
        /// is pseudorandomly generated and returned.  All bound possible
        /// long values are produced with (approximately) equal
        /// probability, though there may be a small amount of bias depending on the
        /// implementation and the bound.
        /// <param name="bound">the upper bound (exclusive). If negative or 0, this always returns 0.</param>
        /// <returns>the next pseudorandom, uniformly distributed long</returns>
        /// value between zero (inclusive) and bound (exclusive)
        /// from this random number generator's sequence
        ulong NextULong(ulong bound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between an
        /// inner bound of 0 (inclusive) and the specified outerBound (exclusive).
        /// This is meant for cases where the outer bound may be negative, especially if
        /// the bound is unknown or may be user-specified. A negative outer bound is used
        /// as the lower bound; a positive outer bound is used as the upper bound. An outer
        /// bound of -1, 0, or 1 will always return 0, keeping the bound exclusive (except
        /// for outer bound 0).
        /// <param name="outerBound">the outer exclusive bound; may be any long value, allowing negative</param>
        /// <returns>a pseudorandom long between 0 (inclusive) and outerBound (exclusive)</returns>
        long NextLong(long outerBound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). If outerBound is less than or equal to innerBound,
        /// this always returns innerBound.
        /// <param name="inner">the inclusive inner bound; may be any long, allowing negative</param>
        /// <param name="outer">the exclusive outer bound; must be greater than innerBound (otherwise this returns innerBound)</param>
        /// <returns>a pseudorandom long between innerBound (inclusive) and outerBound (exclusive)</returns>
        ulong NextULong(ulong inner, ulong outer);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). This is meant for cases where either bound may be negative,
        /// especially if the bounds are unknown or may be user-specified.
        /// <param name="inner">the inclusive inner bound; may be any long, allowing negative</param>
        /// <param name="outer">the exclusive outer bound; may be any long, allowing negative</param>
        /// <returns>a pseudorandom long between innerBound (inclusive) and outerBound (exclusive)</returns>
        long NextLong(long inner, long outer);

        /// <summary>
        /// Generates the next pseudorandom number with a specific maximum size in bits (not a max number).
        /// If you want to get a random number in a range, you should usually use {@link #nextInt(int)} instead.
        /// <br/>The general contract of next is that it returns an
        /// uint value and if the argument bits is between
        /// 1 and 32 (inclusive), then that many low-order
        /// bits of the returned value will be (approximately) independently
        /// chosen bit values, each of which is (approximately) equally
        /// likely to be 0 or 1.
        /// <br/>
        /// Note that you can give this values for bits that are outside its expected range of 1 to 32,
        /// but the value used, as long as bits is positive, will effectively be {@code bits % 32}. As stated
        /// before, a value of 0 for bits is the same as a value of 32.<br/>
        /// <param name="bits">the amount of random bits to request, from 1 to 32</param>
        /// <returns>the next pseudorandom value from this random number</returns>
        /// generator's sequence
        uint NextBits(int bits);

        /// <summary>
        /// Generates random bytes and places them into a user-supplied
        /// byte array.  The number of random bytes produced is equal to
        /// the length of the byte array.
        /// <param name="bytes">the byte array to fill with random bytes</param>
        /// @throws NullPointerException if the byte array is null
        void NextBytes(byte[] bytes);

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed int
        /// value from this random number generator's sequence. The general
        /// contract of nextInt is that one int value is
        /// pseudorandomly generated and returned. All 2<sup>32</sup> possible
        /// int values are produced with (approximately) equal probability.
        /// <returns>the next pseudorandom, uniformly distributed int</returns>
        /// value from this random number generator's sequence
        int NextInt();

        /// <summary>
        /// Gets a random uint by using the low 32 bits of NextULong(); this can return any uint.
        /// </summary>
        /// <returns>Any random uint.</returns>
        uint NextUInt();

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value
        /// between 0 (inclusive) and the specified value (exclusive), drawn from
        /// this random number generator's sequence.  The general contract of
        /// nextInt is that one int value in the specified range
        /// is pseudorandomly generated and returned.  All bound possible
        /// int values are produced with (approximately) equal
        /// probability.
        /// <br/>
        /// It should be mentioned that the technique this uses has some bias, depending
        /// on bound, but it typically isn't measurable without specifically looking
        /// for it. Using the method this does allows this method to always advance the state
        /// by one step, instead of a varying and unpredictable amount with the more typical
        /// ways of rejection-sampling random numbers and only using numbers that can produce
        /// an int within the bound without bias.
        /// See <a href="https://www.pcg-random.org/posts/bounded-rands.html">M.E. O'Neill's
        /// blog about random numbers</a> for discussion of alternative, unbiased methods.
        /// <param name="bound">the upper bound (exclusive). If negative or 0, this always returns 0.</param>
        /// <returns>the next pseudorandom, uniformly distributed int</returns>
        /// value between zero (inclusive) and bound (exclusive)
        /// from this random number generator's sequence
        uint NextUInt(uint bound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between an
        /// inner bound of 0 (inclusive) and the specified outerBound (exclusive).
        /// This is meant for cases where the outer bound may be negative, especially if
        /// the bound is unknown or may be user-specified. A negative outer bound is used
        /// as the lower bound; a positive outer bound is used as the upper bound. An outer
        /// bound of -1, 0, or 1 will always return 0, keeping the bound exclusive (except
        /// for outer bound 0).
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="outerBound">the outer exclusive bound; may be any int value, allowing negative</param>
        /// <returns>a pseudorandom int between 0 (inclusive) and outerBound (exclusive)</returns>
        int NextInt(int outerBound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). If outerBound is less than or equal to innerBound,
        /// this always returns innerBound. This is significantly slower than
        /// {@link #nextInt(int)} or {@link #nextSignedInt(int)},
        /// because this handles even ranges that go from large negative numbers to large
        /// positive numbers, and since that would be larger than the largest possible int,
        /// this has to use {@link #nextLong(long)}.
        /// <br/> For any case where outerBound might be valid but less than innerBound, you
        /// can use {@link #nextSignedInt(int, int)}. If outerBound is less than innerBound
        /// here, this simply returns innerBound.
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="innerBound">the inclusive inner bound; may be any int, allowing negative</param>
        /// <param name="outerBound">the exclusive outer bound; must be greater than innerBound (otherwise this returns innerBound)</param>
        /// <returns>a pseudorandom int between innerBound (inclusive) and outerBound (exclusive)</returns>
        uint NextUInt(uint innerBound, uint outerBound);
        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). This is meant for cases where either bound may be negative,
        /// especially if the bounds are unknown or may be user-specified. It is slightly
        /// slower than {@link #nextInt(int, int)}, and significantly slower than
        /// {@link #nextInt(int)} or {@link #nextSignedInt(int)}. This last part is
        /// because this handles even ranges that go from large negative numbers to large
        /// positive numbers, and since that range is larger than the largest possible int,
        /// this has to use {@link #nextSignedLong(long)}.
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="innerBound">the inclusive inner bound; may be any int, allowing negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be any int, allowing negative</param>
        /// <returns>a pseudorandom int between innerBound (inclusive) and outerBound (exclusive)</returns>
        int NextInt(int innerBound, int outerBound);


        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed
        /// bool value from this random number generator's
        /// sequence. The general contract of NextBool is that one
        /// bool value is pseudorandomly generated and returned.  The
        /// values true and false are produced with
        /// (approximately) equal probability.
        /// 
        /// The default implementation is equivalent to a sign check on {@link #NextULong()},
        /// returning true if the generated long is negative. This is typically the safest
        /// way to implement this method; many types of generators have less statistical
        /// quality on their lowest bit, so just returning based on the lowest bit isn't
        /// always a good idea.
        /// <returns>the next pseudorandom, uniformly distributed</returns>
        /// bool value from this random number generator's
        /// sequence
        bool NextBool();

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed float
        /// value between 0.0 (inclusive) and 1.0 (exclusive)
        /// from this random number generator's sequence.
        /// <br/>The general contract of NextFloat is that one
        /// float value, chosen (approximately) uniformly from the
        /// range 0.0f (inclusive) to 1.0f (exclusive), is
        /// pseudorandomly generated and returned. All 2<sup>24</sup> possible
        /// float values of the form <i>m&nbsp;x&nbsp;</i>2<sup>-24</sup>,
        /// where <i>m</i> is a positive integer less than 2<sup>24</sup>, are
        /// produced with (approximately) equal probability.
        /// <br/>The public implementation uses the upper 24 bits of <see cref="NextLong()"/>,
        /// with an unsigned right shift and a multiply by a very small float
        /// ({@code 5.9604645E-8f} or {@code 0x1p-24f}). It tends to be fast if
        /// nextLong() is fast, but alternative implementations could use 24 bits of
        /// {@link #nextInt()} (or just {@link #next(int)}, giving it 24)
        /// if that generator doesn't efficiently generate 64-bit longs.<br/>
        /// <returns>the next pseudorandom, uniformly distributed float</returns>
        /// value between 0.0 and 1.0 from this
        /// random number generator's sequence
        float NextFloat();

        /// <summary>
        /// Gets a pseudo-random float between 0 (inclusive) and outerBound (exclusive).
        /// The outerBound may be positive or negative.
        /// Exactly the same as {@code NextFloat() * outerBound}.
        /// <param name="outerBound">the exclusive outer bound</param>
        /// <returns>a float between 0 (inclusive) and outerBound (exclusive)</returns>
        float NextFloat(float outerBound);

        /// <summary>
        /// Gets a pseudo-random float between innerBound (inclusive) and outerBound (exclusive).
        /// Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
        /// inclusive and which is exclusive.
        /// <param name="innerBound">the inclusive inner bound; may be negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be negative</param>
        /// <returns>a float between innerBound (inclusive) and outerBound (exclusive)</returns>
        float NextFloat(float innerBound, float outerBound);

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed
        /// double value between 0.0 (inclusive) and 1.0
        /// (exclusive) from this random number generator's sequence.
        /// <br/>The general contract of NextDouble is that one
        /// double value, chosen (approximately) uniformly from the
        /// range {@code 0.0d} (inclusive) to {@code 1.0d} (exclusive), is
        /// pseudorandomly generated and returned.
        /// <br/>The default implementation uses the upper 53 bits of <see cref="NextLong()"/>,
        /// with an unsigned right shift and a multiply by a very small double
        /// ({@code 1.1102230246251565E-16}, or {@code 0x1p-53}). It should perform well
        /// if nextLong() performs well, and is expected to perform less well if the
        /// generator naturally produces 32 or fewer bits at a time.<br/>
        /// <returns>the next pseudorandom, uniformly distributed double</returns>
        /// value between 0.0 and 1.0 from this
        /// random number generator's sequence
        double NextDouble();

        /// <summary>
        /// Gets a pseudo-random double between 0 (inclusive) and outerBound (exclusive).
        /// The outerBound may be positive or negative.
        /// Exactly the same as {@code NextDouble() * outerBound}.
        /// <param name="outerBound">the exclusive outer bound</param>
        /// <returns>a double between 0 (inclusive) and outerBound (exclusive)</returns>
        double NextDouble(double outerBound);

        /// <summary>
        /// Gets a pseudo-random double between innerBound (inclusive) and outerBound (exclusive).
        /// Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
        /// inclusive and which is exclusive.
        /// <param name="innerBound">the inclusive inner bound; may be negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be negative</param>
        /// <returns>a double between innerBound (inclusive) and outerBound (exclusive)</returns>
        double NextDouble(double innerBound, double outerBound);

        /// <summary>
        /// This is just like {@link #NextDouble()}, returning a double between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
        /// It returns 1.0 extremely rarely, 0.000000000000011102230246251565% of the time if there is no bias in the generator, but it
        /// can happen. This uses {@link #nextLong(long)} internally, so it may have some bias towards or against specific
        /// subtly-different results.
        /// <returns>a double between 0.0, inclusive, and 1.0, inclusive</returns>
        double NextInclusiveDouble();

        /// <summary>
        /// Just like {@link #NextDouble(double)}, but this is inclusive on both 0.0 and outerBound.
        /// It may be important to note that it returns outerBound on only 0.000000000000011102230246251565% of calls.
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a double between 0.0, inclusive, and outerBound, inclusive</returns>
        double NextInclusiveDouble(double outerBound);

        /// <summary>
        /// Just like {@link #NextDouble(double, double)}, but this is inclusive on both innerBound and outerBound.
        /// It may be important to note that it returns outerBound on only 0.000000000000011102230246251565% of calls, if it can
        /// return it at all because of floating-point imprecision when innerBound is a larger number.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a double between innerBound, inclusive, and outerBound, inclusive</returns>
        double NextInclusiveDouble(double innerBound, double outerBound);

        /// <summary>
        /// This is just like {@link #NextFloat()}, returning a float between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
        /// It returns 1.0 rarely, 0.00000596046412226771% of the time if there is no bias in the generator, but it can happen. This method
        /// has been tested by generating 268435456 (or 0x10000000) random ints with {@link #nextInt(int)}, and just before the end of that
        /// it had generated every one of the 16777217 roughly-equidistant floats this is able to produce. Not all seeds and streams are
        /// likely to accomplish that in the same time, or at all, depending on the generator.
        /// <returns>a float between 0.0, inclusive, and 1.0, inclusive</returns>
        float NextInclusiveFloat();

        /// <summary>
        /// Just like {@link #NextFloat(float)}, but this is inclusive on both 0.0 and outerBound.
        /// It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls.
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a float between 0.0, inclusive, and outerBound, inclusive</returns>
        float NextInclusiveFloat(float outerBound);

        /// <summary>
        /// Just like {@link #NextFloat(float, float)}, but this is inclusive on both innerBound and outerBound.
        /// It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls, if it can return
        /// it at all because of floating-point imprecision when innerBound is a larger number.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a float between innerBound, inclusive, and outerBound, inclusive</returns>
        float NextInclusiveFloat(float innerBound, float outerBound);

        /// <summary>
        /// Gets a random double between 0.0 and 1.0, exclusive at both ends. This can return double
        /// values between 1.1102230246251564E-16 and 0.9999999999999999, or 0x1.fffffffffffffp-54 and 0x1.fffffffffffffp-1 in hex
        /// notation. It cannot return 0 or 1.
        /// <br/>
        /// The default implementation simply uses <see cref="NextLong()"/> to get a uniform long, shifts it to remove 11 bits, adds 1, and
        /// multiplies by a value just slightly less than what nextDouble() usually uses.
        /// <returns>a random uniform double between 0 and 1 (both exclusive)</returns>
        double NextExclusiveDouble();

        /// <summary>
        /// Just like {@link #NextDouble(double)}, but this is exclusive on both 0.0 and outerBound.
        /// Like {@link #nextExclusiveDouble()}, which this uses, this may have better bit-distribution of
        /// double values, and it may also be better able to produce very small doubles when outerBound is large.
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a double between 0.0, exclusive, and outerBound, exclusive</returns>
        double NextExclusiveDouble(double outerBound);

        /// <summary>
        /// Just like {@link #NextDouble(double, double)}, but this is exclusive on both innerBound and outerBound.
        /// Like {@link #nextExclusiveDouble()}, which this uses,, this may have better bit-distribution of double values,
        /// and it may also be better able to produce doubles close to innerBound when {@code outerBound - innerBound} is large.
        /// <param name="innerBound">the inner exclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a double between innerBound, exclusive, and outerBound, exclusive</returns>
        double NextExclusiveDouble(double innerBound, double outerBound);

        /// <summary>
        /// Gets a random float between 0.0 and 1.0, exclusive at both ends. This can return float
        /// values between 5.9604645E-8 and 0.99999994, or 0x1.0p-24 and 0x1.fffffep-1 in hex notation.
        /// It cannot return 0 or 1, and its minimum and maximum results are equally distant from 0 and from
        /// 1, respectively. Some usages may prefer {@link #nextExclusiveFloat()}, which is
        /// better-distributed if you consider the bit representation of the returned floats, tends to perform
        /// better, and can return floats that much closer to 0 than this can.
        /// <br/>
        /// The default implementation simply uses {@link #nextInt(int)} to get a uniformly-chosen int between 1 and
        /// (2 to the 24) - 1, both inclusive, and multiplies it by (2 to the -24). Using larger values than (2 to the
        /// 24) would cause issues with the float math.
        /// <returns>a random uniform float between 0 and 1 (both exclusive)</returns>
        float NextExclusiveFloat();

        /// <summary>
        /// Just like {@link #NextFloat(float)}, but this is exclusive on both 0.0 and outerBound.
        /// Like {@link #nextExclusiveFloat()}, this may have better bit-distribution of float values, and
        /// it may also be better able to produce very small floats when outerBound is large.
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a float between 0.0, exclusive, and outerBound, exclusive</returns>
        float NextExclusiveFloat(float outerBound);

        /// <summary>
        /// Just like {@link #NextFloat(float, float)}, but this is exclusive on both innerBound and outerBound.
        /// Like {@link #nextExclusiveFloat()}, this may have better bit-distribution of float values, and
        /// it may also be better able to produce floats close to innerBound when {@code outerBound - innerBound} is large.
        /// <param name="innerBound">the inner exclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a float between innerBound, exclusive, and outerBound, exclusive</returns>
        float NextExclusiveFloat(float innerBound, float outerBound);

        /// <summary>
        /// Gets a normally-distributed (Gaussian) double, with a the specified mean (default 0.0) and standard deviation (default 1.0).
        /// </summary>
        /// <returns>A double from the normal distribution with the specified mean (default 0.0) and standard deviation (default 1.0).</returns>
        double NextNormal(double mean = 0.0, double stdDev = 1.0);
        /// <summary>
        /// (Optional) If implemented, this should jump the generator forward by the given number of steps as distance and return the result of NextULong()
        /// as if called at that step. The distance can be negative if a long is cast to a ulong, which jumps backwards if the period of the generator is 2 to the 64.
        /// </summary>
        /// <param name="distance">How many steps to jump forward</param>
        /// <returns>The result of what NextULong() would return at the now-current jumped state.</returns>
        ulong Skip(ulong distance);

        /// <summary>
        /// (Optional) If implemented, jumps the generator back to the previous state and returns what NextULong() would have produced at that state.
        /// </summary>
        /// <returns>The result of what NextULong() would return at the previous state.</returns>
        ulong PreviousULong();

        /// <summary>
        /// Sets each state in this IRandom to the corresponding state in the other IRandom.
        /// This generally only works correctly if both objects have the same class, but may also function correctly if this, other, or both are wrappers
        /// around the same type of IRandom.
        /// </summary>
        /// <param name="other">Another IRandom that almost always should have the same class as this one, or wrap an IRandom with the same class.</param>
        void SetWith(IRandom other);

        /// <summary>
        /// Returns true if a random value between 0 and 1 is less than the specified value.
        /// <param name="chance">a float between 0.0 and 1.0; higher values are more likely to result in true</param>
        /// <returns>a bool selected with the given chance of being true</returns>
        bool NextBool(float chance);

        /// <summary>
        /// Returns -1 or 1, randomly.
        /// <returns>-1 or 1, selected with approximately equal likelihood</returns>
        int NextSign();

        /// <summary>
        /// Returns a triangularly distributed random number between -1.0 (exclusive) and 1.0 (exclusive), where values around zero are
        /// more likely.
        float NextTriangular();

        /// <summary>
        /// Returns a triangularly distributed random number between {@code -max} (exclusive) and max (exclusive), where values
        /// around zero are more likely.
        /// <param name="max">the upper limit</param>
        float NextTriangular(float max);

        /// <summary>
        /// Returns a triangularly distributed random number between min (inclusive) and max (exclusive), where the
        /// mode argument defaults to the midpoint between the bounds, giving a symmetric distribution.
        /// <br/>
        /// This method is equivalent of {@link #nextTriangular(float, float, float) NextTriangular(min, max, (min + max) * 0.5f)}
        /// <param name="min">the lower limit</param>
        /// <param name="max">the upper limit</param>
        float NextTriangular(float min, float max);

        /// <summary>
        /// Returns a triangularly distributed random number between min (inclusive) and max (exclusive), where values
        /// around mode are more likely.
        /// <param name="min"> the lower limit</param>
        /// <param name="max"> the upper limit</param>
        /// <param name="mode">the point around which the values are more likely</param>
        float NextTriangular(float min, float max, float mode);

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        int MinIntOf(int innerBound, int outerBound, int trials);

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        int MaxIntOf(int innerBound, int outerBound, int trials);

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextLong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        long MinLongOf(long innerBound, long outerBound, int trials);

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextLong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        long MaxLongOf(long innerBound, long outerBound, int trials);
        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextUInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        uint MinUIntOf(uint innerBound, uint outerBound, int trials);

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextUInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        uint MaxUIntOf(uint innerBound, uint outerBound, int trials);

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextULong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        ulong MinULongOf(ulong innerBound, ulong outerBound, int trials);

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextULong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        ulong MaxULongOf(ulong innerBound, ulong outerBound, int trials);

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextDouble(double, double)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        double MinDoubleOf(double innerBound, double outerBound, int trials);

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextDouble(double, double)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        double MaxDoubleOf(double innerBound, double outerBound, int trials);

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextFloat(float, float)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        float MinFloatOf(float innerBound, float outerBound, int trials);

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextFloat(float, float)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        float MaxFloatOf(float innerBound, float outerBound, int trials);


        /// <summary>
        /// Gets a randomly-chosen item from the given non-null, non-empty array.
        /// </summary>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="array">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen item from array.</returns>
        T RandomElement<T>(T[] array);

        /// <summary>
        /// Gets a randomly-chosen item from the given non-null, non-empty IList.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen item from list.</returns>
        T RandomElement<T>(IList<T> list);

        /// <summary>
        /// Shuffles the given array in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an array of some reference type; must be non-null but may contain null items</param>
        void Shuffle<T>(T[] items);

        /// <summary>
        /// Shuffles the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        void Shuffle<T>(IList<T> items);

        /// <summary>
        /// Shuffles a section of the given array in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an array of some reference type; must be non-null but may contain null items</param>
        /// <param name="offset">the index of the first element of the array that can be shuffled</param>
        /// <param name="length">the length of the section to shuffle</param>
        void Shuffle<T>(T[] items, int offset, int length);

        /// <summary>
        /// Shuffles a section of the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <param name="offset">the index of the first element of the IList that can be shuffled</param>
        /// <param name="length">the length of the section to shuffle</param>
        void Shuffle<T>(IList<T> items, int offset, int length);

    }








    /// <summary>
    /// The abstract parent class of nearly all random number generators here.
    /// </summary>
    /// <remarks>
    /// Almost all subclasses of AbstractRandom should implement <see cref="SelectState(int)"/> so that individual states can be retrieved; this is used by many of
    /// the other methods here, and some of them throw exceptions if that method is not available. Similarly, <see cref="SetSelectedState(int, ulong)"/> should
    /// be implemented to set specific states, especially if there is more than one state variable.
    /// </remarks>
    [Serializable]
    public abstract class AbstractRandom : IRandom
    {
        private static readonly float FLOAT_ADJUST = MathF.Pow(2f, -24f);
        private static readonly double DOUBLE_ADJUST = Math.Pow(2.0, -53.0);
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
        /// <summary>
        /// Must have a zero-argument constructor.
        /// </summary>
        protected AbstractRandom()
        {
        }
        /// <summary>
        /// This calls <see cref="Seed(ulong)"/> with it seed by default.
        /// </summary>
        /// <param name="seed">A ulong that will either be used as a state verbatim or, more commonly, to determine multiple states.</param>
        protected AbstractRandom(ulong seed)
        {
            Seed(seed);
        }
        /// <summary>
        /// Copies another AbstractRandom, typically with the same class, into this newly-constructed one.
        /// </summary>
        /// <param name="other">Another AbstractRandom to copy into this one.</param>
        protected AbstractRandom(AbstractRandom other)
        {
            SetWith(other);
        }

        /// <summary>
        /// Sets the seed of this random number generator using a single ulong seed.
        /// </summary>
        /// <remarks>
        /// This does not necessarily assign the state variable(s) of the implementation with the exact contents of seed,
        /// so <see cref="SelectState(int)"/> should not be expected to return seed after this, though it may.
        /// </remarks>
        /// <param name="seed"></param>
        public abstract void Seed(ulong seed);
        /// <summary>
        /// Gets the number of possible state variables that can be selected with
        /// <see cref="SelectState(int)"/> or <see cref="SetSelectedState(int, ulong)"/>,
        /// even if those methods are not publicly accessible. An implementation that has only
        /// one ulong or uint state, like <see cref="DistinctRandom"/>, should produce 1.
        /// An implementation that has two uint or ulong states should produce 2, etc.
        /// This is always a non-negative number; though discouraged, it is allowed to be 0 for
        /// generators that attempt to conceal their state.
        /// </summary>
        public abstract int StateCount { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="SelectState(int)"/>, or false if that method is unsupported.
        /// </summary>
        public abstract bool SupportsReadAccess { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="SetSelectedState(int, ulong)"/>, or false if that method is unsupported.
        /// </summary>
        public abstract bool SupportsWriteAccess { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="Skip(ulong)"/>, or false if that method is unsupported.
        /// </summary>
        public abstract bool SupportsSkip { get; }

        /// <summary>
        /// This should be true if the implementation supports <see cref="PreviousULong"/>, or false if that method is unsupported.
        /// </summary>
        public abstract bool SupportsPrevious { get; }

        /// <summary>
        /// The exactly-four-character string that will identify this AbstractRandom for serialization purposes.
        /// </summary>
        public abstract string Tag { get; }

        private static Dictionary<string, IRandom> TAGS = new Dictionary<string, IRandom>();

        /// <summary>
        /// Registers an instance of a subclass of AbstractRandom by its four-character string <see cref="Tag"/>.
        /// </summary>
        /// <param name="instance">An instance of a subclass of AbstractRandom, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a four-character <see cref="Tag"/>.</param>
        /// <returns>Returns true if the tag was successfully registered for the first time, or false if the tags are unchanged.</returns>
        protected static bool RegisterTag(AbstractRandom instance)
        {
            if (TAGS.ContainsKey(instance.Tag)) return false;
            if (instance.Tag.Length == 4)
            {
                TAGS.Add(instance.Tag, instance);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Produces a string that encodes the type and full state of this generator.
        /// </summary>
        /// <returns>An encoded string that stores the type and full state of this generator.</returns>
        public abstract string StringSerialize();

        /// <summary>
        /// Given a string produced by <see cref="StringSerialize"/>, if the specified type is compatible,
        /// then this method sets the state of this IRandom to the specified stored state.
        /// </summary>
        /// <param name="data">A string produced by StringSerialize.</param>
        /// <returns>This IRandom, after modifications.</returns>
        public abstract IRandom StringDeserialize(string data);

        /// <summary>
        /// Given a string produced by <see cref="StringSerialize()"/> on any valid subclass of AbstractRandom,
        /// this returns a new IRandom with the same implementation and state it had when it was serialized.
        /// This handles all AbstractRandom implementations in this library, including <see cref="TRWrapper"/> and
        /// <see cref="ReversingWrapper"/> (both of which it currently handles with a special case).
        /// </summary>
        /// <param name="data">A string produced by an AbstractRandom's StringSerialize() method.</param>
        /// <returns>A newly-allocated IRandom matching the implementation and state of the serialized AbstractRandom.</returns>
        public static IRandom Deserialize(string data)
        {
            if (data.StartsWith('T'))
                return new TRWrapper(TAGS[data.Substring(1, 4)].Copy().StringDeserialize(data));
            if (data.StartsWith('R'))
                return new ReversingWrapper(TAGS[data.Substring(1, 4)].Copy().StringDeserialize(data));
            return TAGS[data.Substring(1, 4)].Copy().StringDeserialize(data);
        }

        /// <summary>
        /// Gets a selected state value from this AbstractRandom, by index.
        /// </summary>
        /// <remarks>
        /// The number of possible selections is up to the implementing class, and is accessible via <see cref="StateCount"/>, but negative values for selection are typically not tolerated.
        /// This should return the exact value of the selected state, assuming it is implemented. The default implementation throws an NotSupportedException, and implementors only have to
        /// allow reading the state if they choose to implement this differently. If this method is intended to be used, <see cref="StateCount"/> must also be implemented.
        /// </remarks>
        /// <param name="selection">The index of the state to retrieve.</param>
        /// <returns>The value of the state corresponding to selection</returns>
        /// <exception cref="NotSupportedException">If the generator does not allow reading its state.</exception>
        public virtual ulong SelectState(int selection)
        {
            throw new NotSupportedException("SelectState() not supported.");
        }

        /// <summary>
        /// Sets a selected state value to the given ulong value.
        /// </summary>
        /// <remarks>
        /// The number of possible selections is up to the implementing class, but selection should be at least 0 and less than <see cref="StateCount"/>.
        /// Implementors are permitted to change value if it is not valid, but they should not alter it if it is valid.
        /// The public implementation calls <see cref="Seed(ulong)"/> with value, which doesn't need changing if the generator has one state that is set verbatim by Seed().
        /// Otherwise, this method should be implemented when <see cref="SelectState(int)"/> is and the state is allowed to be set by users.
        /// Having accurate ways to get and set the full state of a random number generator makes it much easier to serialize and deserialize that class.
        /// </remarks>
        /// <param name="selection">The index of the state to set.</param>
        /// <param name="value">The value to try to use for the selected state.</param>
        public virtual void SetSelectedState(int selection, ulong value)
        {
            Seed(value);
        }

        /// <summary>
        /// Sets every state variable to the given state.
        /// </summary>
        /// <remarks>
        /// If <see cref="StateCount"/> is 1, then this should set the whole state to the given value using <see cref="SetSelectedState(int, ulong)"/>.
        /// If StateCount is more than 1, then all states will be set in the same way (using SetSelectedState(), all to state).</remarks>
        /// <param name="state">The ulong variable to use for every state variable.</param>
        public virtual void SetState(ulong state)
        {
            for (int i = StateCount - 1; i >= 0; i--)
            {
                SetSelectedState(i, state);
            }
        }
        
        /// <summary>
        /// Sets each state variable to either stateA or stateB, alternating.
        /// </summary>
        /// <remarks>
        /// This uses <see cref="SetSelectedState(int, ulong)"/> to set the values.
        /// If there is one state variable (<see cref="StateCount"/> is 1), then this only sets that state variable to stateA.
        /// If there are two state variables, the first is set to stateA, and the second to stateB.
        ///  there are more, it reuses stateA, then stateB, then stateA, and so on until all variables are set.
        /// </remarks>
        /// <param name="stateA">The ulong value to use for states at index 0, 2, 4, 6...</param>
        /// <param name="stateB">The ulong value to use for states at index 1, 3, 5, 7...</param>
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

        /// <summary>
        /// Sets each state variable to stateA, stateB, or stateC, alternating.
        /// </summary>
        /// <remarks>
        /// This uses <see cref="SetSelectedState(int, ulong)"/> to set the values.
        /// If there is one state variable (<see cref="StateCount"/> is 1), then this only
        /// sets that state variable to stateA. If there are two state variables, the first
        /// is set to stateA, and the second to stateB. With three state variables, the
        /// first is set to stateA, the second to stateB, and the third to stateC. If there
        /// are more, it reuses stateA, then stateB, then stateC, then stateA, and so on
        /// until all variables are set.
        /// </remarks>
        /// <param name="stateA">The ulong value to use for states at index 0, 3, 6, 9...</param>
        /// <param name="stateB">The ulong value to use for states at index 1, 4, 7, 10...</param>
        /// <param name="stateC">The ulong value to use for states at index 2, 5, 8, 11...</param>
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

        /// <summary>
        /// Sets each state variable to stateA, stateB, stateC, or stateD, alternating.
        /// </summary>
        /// <remarks>
        /// This uses <see cref="SetSelectedState(int, ulong)"/> to
        /// set the values. If there is one state variable (<see cref="StateCount"/> is 1),
        /// then this only sets that state variable to stateA. If there are two state
        /// variables, the first is set to stateA, and the second to stateB. With three
        /// state variables, the first is set to stateA, the second to stateB, and the third
        /// to stateC. With four state variables, the first is set to stateA, the second to
        /// stateB, the third to stateC, and the fourth to stateD. If there are more, it
        /// reuses stateA, then stateB, then stateC, then stateD, then stateA, and so on
        /// until all variables are set.
        /// </remarks>
        /// <param name="stateA">the ulong value to use for states at index 0, 4, 8, 12...</param>
        /// <param name="stateB">the ulong value to use for states at index 1, 5, 9, 13...</param>
        /// <param name="stateC">the ulong value to use for states at index 2, 6, 10, 14...</param>
        /// <param name="stateD">the ulong value to use for states at index 3, 7, 11, 15...</param>
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

        /// <summary>
        /// Sets all state variables to cycling values chosen from states.'
        /// </summary>
        /// <remarks>
        /// If states is empty, then this does nothing, and leaves the current generator unchanged.
        /// This works for generators with any <see cref="StateCount"/>, but may allocate an array if states is
        /// used as params (you can pass an existing array without needing to allocate). This
        /// uses <see cref="SetSelectedState(int, ulong)"/> to change the states.
        /// </remarks>
        /// <param name="states">An array or params array of ulong values to use as states.</param>
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

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed ulong
        /// value from this random number generator's sequence, in the full range of all possible ulong values.
        /// <remarks>
        /// The general contract of NextULong is that one ulong value is
        /// pseudorandomly generated and returned.
        /// </remarks>
        /// <returns>The next pseudorandom, uniformly distributed ulong value from this random number generator's sequence.</returns>
        public abstract ulong NextULong();

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed long
        /// value from this random number generator's sequence, in the full range of all possible long values (positive and negative).
        /// </summary>
        /// <returns>The next pseudorandom, uniformly distributed long value from this random number generator's sequence.</returns>
        public long NextLong()
        {
            unchecked
            {
                return (long)NextULong();
            }
        }

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value
        /// between 0 (inclusive) and the specified value (exclusive), drawn from
        /// this random number generator's sequence.
        /// </summary>
        /// <remarks>
        /// The general contract of
        /// nextULong is that one ulong value in the specified range
        /// is pseudorandomly generated and returned.  All possible
        /// long values within the bound are produced with (approximately) equal
        /// probability, though there is a small amount of bias depending on the bound.
        /// <br/>
        /// Note that this advances the state by the same amount as a single call to
        /// <see cref="NextULong()"/>, which allows methods like <see cref="Skip(ulong)"/> to function
        /// correctly, but introduces some bias when bound is very large. This will
        /// also advance the state if bound is 0 or negative, so usage with a variable
        /// bound will advance the state reliably.
        /// <br/>
        /// This method has some bias, particularly on larger bounds. Actually measuring
        /// bias with bounds in the trillions or greater is challenging but not impossible, so
        /// don't use this for a real-money gambling purpose. The bias isn't especially
        /// significant, though.
        /// </remarks>
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="bound">the upper bound (exclusive). If negative or 0, this always returns 0.</param>
        /// <returns>the next pseudorandom, uniformly distributed long value between zero (inclusive) and bound (exclusive) from this random number generator's sequence</returns>
        public ulong NextULong(ulong bound)
        {
            return NextULong(0UL, bound);
        }

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between an
        /// inner bound of 0 (inclusive) and the specified outerBound (exclusive).
        /// </summary>
        /// <remarks>
        /// This is meant for cases where the outer bound may be negative, especially if
        /// the bound is unknown or may be user-specified. A negative outer bound is used
        /// as the lower bound; a positive outer bound is used as the upper bound. An outer
        /// bound of -1, 0, or 1 will always return 0, keeping the bound exclusive (except
        /// for outer bound 0).
        /// <br/>Note that this advances the state by the same amount as a single call to
        /// <see cref="NextULong()"/>, which allows methods like <see cref="Skip(ulong)"/> to function
        /// correctly, but introduces some bias when bound is very large.
        /// </remarks>
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="outerBound">the outer exclusive bound; may be any long value, allowing negative</param>
        /// <returns>a pseudorandom long between 0 (inclusive) and outerBound (exclusive)</returns>
        public long NextLong(long outerBound)
        {
            return NextLong(0L, outerBound);
        }

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). If outerBound is less than or equal to innerBound,
        /// this always returns innerBound.
        /// </summary>
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="inner">the inclusive inner bound; may be any long, allowing negative</param>
        /// <param name="outer">the exclusive outer bound; must be greater than innerBound (otherwise this returns innerBound)</param>
        /// <returns>a pseudorandom long between innerBound (inclusive) and outerBound (exclusive)</returns>
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

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). This is meant for cases where either bound may be negative,
        /// especially if the bounds are unknown or may be user-specified.
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="inner">the inclusive inner bound; may be any long, allowing negative</param>
        /// <param name="outer">the exclusive outer bound; may be any long, allowing negative</param>
        /// <returns>a pseudorandom long between innerBound (inclusive) and outerBound (exclusive)</returns>
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

        /// <summary>
        /// Generates random bytes and places them into a user-supplied
        /// byte array.  The number of random bytes produced is equal to
        /// the length of the byte array.
        /// <param name="bytes">the byte array to fill with random bytes</param>
        public void NextBytes(byte[] bytes)
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

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed int
        /// value from this random number generator's sequence. The general
        /// contract of nextInt is that one int value is
        /// pseudorandomly generated and returned. All 2<sup>32</sup> possible
        /// int values are produced with (approximately) equal probability.
        /// </summary>
        /// <returns>The next pseudorandom, uniformly distributed int value from this random number generator's sequence.</returns>
        public int NextInt()
        {
            return (int)NextULong();
        }

        /// <summary>
        /// Gets a random uint by using the low 32 bits of NextULong(); this can return any uint.
        /// </summary>
        /// <returns>Any random uint.</returns>
        public uint NextUInt()
        {
            return (uint)NextULong();
        }

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value
        /// between 0 (inclusive) and the specified value (exclusive), drawn from
        /// this random number generator's sequence.
        /// </summary>
        /// <remarks>The general contract of
        /// nextInt is that one int value in the specified range
        /// is pseudorandomly generated and returned.  All bound possible
        /// int values are produced with (approximately) equal
        /// probability.
        /// <br/>
        /// It should be mentioned that the technique this uses has some bias, depending
        /// on bound, but it typically isn't measurable without specifically looking
        /// for it. Using the method this does allows this method to always advance the state
        /// by one step, instead of a varying and unpredictable amount with the more typical
        /// ways of rejection-sampling random numbers and only using numbers that can produce
        /// an int within the bound without bias.
        /// See <a href="https://www.pcg-random.org/posts/bounded-rands.html">M.E. O'Neill's
        /// blog about random numbers</a> for discussion of alternative, unbiased methods.
        /// </remarks>
        /// <param name="bound">the upper bound (exclusive). If negative or 0, this always returns 0.</param>
        /// <returns>the next pseudorandom, uniformly distributed int</returns>
        /// value between zero (inclusive) and bound (exclusive)
        /// from this random number generator's sequence
        public uint NextUInt(uint bound)
        {
            return (uint)(bound * (NextULong() & 0xFFFFFFFFUL) >> 32);
        }

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between an
        /// inner bound of 0 (inclusive) and the specified outerBound (exclusive).
        /// This is meant for cases where the outer bound may be negative, especially if
        /// the bound is unknown or may be user-specified. A negative outer bound is used
        /// as the lower bound; a positive outer bound is used as the upper bound. An outer
        /// bound of -1, 0, or 1 will always return 0, keeping the bound exclusive (except
        /// for outer bound 0).
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="outerBound">the outer exclusive bound; may be any int value, allowing negative</param>
        /// <returns>a pseudorandom int between 0 (inclusive) and outerBound (exclusive)</returns>
        public int NextInt(int outerBound)
        {
            outerBound = (int)(outerBound * ((long)NextULong() & 0xFFFFFFFFL) >> 32);
            return outerBound + (outerBound >> 31);
        }

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). If outerBound is less than or equal to innerBound,
        /// this always returns innerBound. This is significantly slower than
        /// {@link #nextInt(int)} or {@link #nextSignedInt(int)},
        /// because this handles even ranges that go from large negative numbers to large
        /// positive numbers, and since that would be larger than the largest possible int,
        /// this has to use {@link #nextLong(long)}.
        /// <br/> For any case where outerBound might be valid but less than innerBound, you
        /// can use {@link #nextSignedInt(int, int)}. If outerBound is less than innerBound
        /// here, this simply returns innerBound.
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="innerBound">the inclusive inner bound; may be any int, allowing negative</param>
        /// <param name="outerBound">the exclusive outer bound; must be greater than innerBound (otherwise this returns innerBound)</param>
        /// <returns>a pseudorandom int between innerBound (inclusive) and outerBound (exclusive)</returns>
        public uint NextUInt(uint innerBound, uint outerBound)
        {
            return (uint)NextULong(innerBound, outerBound);
        }

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive). This is meant for cases where either bound may be negative,
        /// especially if the bounds are unknown or may be user-specified.
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="innerBound">the inclusive inner bound; may be any int, allowing negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be any int, allowing negative</param>
        /// <returns>a pseudorandom int between innerBound (inclusive) and outerBound (exclusive)</returns>
        public int NextInt(int innerBound, int outerBound)
        {
            return (int)NextLong(innerBound, outerBound);
        }

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed
        /// bool value from this random number generator's
        /// sequence. The general contract of NextBool is that one
        /// bool value is pseudorandomly generated and returned.  The
        /// values true and false are produced with
        /// (approximately) equal probability.
        /// 
        /// The default implementation is equivalent to a sign check on <see cref="NextLong()"/>
        /// returning true if the generated long is negative. This is typically the safest
        /// way to implement this method; many types of generators have less statistical
        /// quality on their lowest bit, so just returning based on the lowest bit isn't
        /// always a good idea.
        /// <returns>the next pseudorandom, uniformly distributed</returns>
        /// bool value from this random number generator's
        /// sequence
        public virtual bool NextBool()
        {
            return (NextULong() & 0x8000000000000000UL) == 0x8000000000000000UL;
        }

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed float
        /// value between 0.0 (inclusive) and 1.0 (exclusive)
        /// from this random number generator's sequence.
        /// </summary>
        /// <remarks>The general contract of NextFloat is that one
        /// float value, chosen (approximately) uniformly from the
        /// range 0.0f (inclusive) to 1.0f (exclusive), is
        /// pseudorandomly generated and returned. All 2<sup>24</sup> possible
        /// float values of the form <i>m&nbsp;x&nbsp;</i>2<sup>-24</sup>,
        /// where <i>m</i> is a positive integer less than 2<sup>24</sup>, are
        /// produced with (approximately) equal probability.
        /// <br/>The public implementation uses the upper 24 bits of <see cref="NextLong()"/>,
        /// with an unsigned right shift and a multiply by a very small float
        /// (5.9604645E-8f). It tends to be fast if
        /// nextLong() is fast, but alternative implementations could use 24 bits of
        /// {@link #nextInt()} (or just {@link #next(int)}, giving it 24)
        /// if that generator doesn't efficiently generate 64-bit longs.</remarks>
        /// <returns>the next pseudorandom, uniformly distributed float</returns>
        /// value between 0.0 and 1.0 from this
        /// random number generator's sequence
        public virtual float NextFloat()
        {
            return (NextULong() >> 40) * FLOAT_ADJUST;
        }

        /// <summary>
        /// Gets a pseudo-random float between 0 (inclusive) and outerBound (exclusive).
        /// The outerBound may be positive or negative.
        /// Exactly the same as {@code NextFloat() * outerBound}.
        /// <param name="outerBound">the exclusive outer bound</param>
        /// <returns>a float between 0 (inclusive) and outerBound (exclusive)</returns>
        public float NextFloat(float outerBound)
        {
            return NextFloat() * outerBound;
        }

        /// <summary>
        /// Gets a pseudo-random float between innerBound (inclusive) and outerBound (exclusive).
        /// Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
        /// inclusive and which is exclusive.
        /// <param name="innerBound">the inclusive inner bound; may be negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be negative</param>
        /// <returns>a float between innerBound (inclusive) and outerBound (exclusive)</returns>
        public float NextFloat(float innerBound, float outerBound)
        {
            return innerBound + NextFloat() * (outerBound - innerBound);
        }

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed
        /// double value between 0.0 (inclusive) and 1.0
        /// (exclusive) from this random number generator's sequence.
        /// </summary>
        /// <remarks>
        /// The general contract of NextDouble is that one
        /// double value, chosen (approximately) uniformly from the
        /// range 0.0 (inclusive) to 1.0 (exclusive), is
        /// pseudorandomly generated and returned.
        /// <br/>The default implementation uses the upper 53 bits of <see cref="NextLong()"/>,
        /// with an unsigned right shift and a multiply by a very small double
        /// (1.1102230246251565E-16). It should perform well
        /// if nextLong() performs well, and is expected to perform less well if the
        /// generator naturally produces 32 or fewer bits at a time.\
        /// </remarks>
        /// <returns>the next pseudorandom, uniformly distributed double</returns>
        /// value between 0.0 and 1.0 from this
        /// random number generator's sequence
        public virtual double NextDouble()
        {
            return (NextULong() >> 11) * DOUBLE_ADJUST;
        }

        /// <summary>
        /// Gets a pseudo-random double between 0 (inclusive) and outerBound (exclusive).
        /// The outerBound may be positive or negative.
        /// Exactly the same as {@code NextDouble() * outerBound}.
        /// <param name="outerBound">the exclusive outer bound</param>
        /// <returns>a double between 0 (inclusive) and outerBound (exclusive)</returns>
        public double NextDouble(double outerBound)
        {
            return NextDouble() * outerBound;
        }

        /// <summary>
        /// Gets a pseudo-random double between innerBound (inclusive) and outerBound (exclusive).
        /// Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
        /// inclusive and which is exclusive.
        /// <param name="innerBound">the inclusive inner bound; may be negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be negative</param>
        /// <returns>a double between innerBound (inclusive) and outerBound (exclusive)</returns>
        public double NextDouble(double innerBound, double outerBound)
        {
            return innerBound + NextDouble() * (outerBound - innerBound);
        }

        /// <summary>
        /// This is just like {@link #NextDouble()}, returning a double between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
        /// It returns 1.0 extremely rarely, 0.000000000000011102230246251565% of the time if there is no bias in the generator, but it
        /// can happen. This uses {@link #nextLong(long)} internally, so it may have some bias towards or against specific
        /// subtly-different results.
        /// <returns>a double between 0.0, inclusive, and 1.0, inclusive</returns>
        public double NextInclusiveDouble()
        {
            return NextULong(0x20000000000001L) * DOUBLE_ADJUST;
        }

        /// <summary>
        /// Just like {@link #NextDouble(double)}, but this is inclusive on both 0.0 and outerBound.
        /// It may be important to note that it returns outerBound on only 0.000000000000011102230246251565% of calls.
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a double between 0.0, inclusive, and outerBound, inclusive</returns>
        public double NextInclusiveDouble(double outerBound)
        {
            return NextInclusiveDouble() * outerBound;
        }

        /// <summary>
        /// Just like {@link #NextDouble(double, double)}, but this is inclusive on both innerBound and outerBound.
        /// It may be important to note that it returns outerBound on only 0.000000000000011102230246251565% of calls, if it can
        /// return it at all because of floating-point imprecision when innerBound is a larger number.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a double between innerBound, inclusive, and outerBound, inclusive</returns>
        public double NextInclusiveDouble(double innerBound, double outerBound)
        {
            return innerBound + NextInclusiveDouble() * (outerBound - innerBound);
        }

        /// <summary>
        /// This is just like {@link #NextFloat()}, returning a float between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
        /// It returns 1.0 rarely, 0.00000596046412226771% of the time if there is no bias in the generator, but it can happen. This method
        /// has been tested by generating 268435456 (or 0x10000000) random ints with {@link #nextInt(int)}, and just before the end of that
        /// it had generated every one of the 16777217 roughly-equidistant floats this is able to produce. Not all seeds and streams are
        /// likely to accomplish that in the same time, or at all, depending on the generator.
        /// <returns>a float between 0.0, inclusive, and 1.0, inclusive</returns>
        public float NextInclusiveFloat()
        {
            return NextInt(0x1000001) * FLOAT_ADJUST;
        }

        /// <summary>
        /// Just like {@link #NextFloat(float)}, but this is inclusive on both 0.0 and outerBound.
        /// It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls.
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a float between 0.0, inclusive, and outerBound, inclusive</returns>
        public float NextInclusiveFloat(float outerBound)
        {
            return NextInclusiveFloat() * outerBound;
        }

        /// <summary>
        /// Just like {@link #NextFloat(float, float)}, but this is inclusive on both innerBound and outerBound.
        /// It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls, if it can return
        /// it at all because of floating-point imprecision when innerBound is a larger number.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a float between innerBound, inclusive, and outerBound, inclusive</returns>
        public float NextInclusiveFloat(float innerBound, float outerBound)
        {
            return innerBound + NextInclusiveFloat() * (outerBound - innerBound);
        }

        //Commented out because it was replaced by the bitwise technique below, but we may want to switch back later or on some platforms.

        //**
        //* Gets a random double between 0.0 and 1.0, exclusive at both ends. This can return double
        //* values between 1.1102230246251564E-16 and 0.9999999999999999, or 0x1.fffffffffffffp-54 and 0x1.fffffffffffffp-1 in hex
        //* notation. It cannot return 0 or 1.
        //* <br/>
        //* The default implementation simply uses <see cref="NextLong()"/> to get a uniform long, shifts it to remove 11 bits, adds 1, and
        //* multiplies by a value just slightly less than what nextDouble() usually uses.
        //* @return a random uniform double between 0 and 1 (both exclusive)
        //*/
        //public double NextExclusiveDouble()
        //{
        //    return ((NextULong() >> 11) + 1UL) * 1.1102230246251564E-16;
        //}


        /// <summary>
        /// Gets a random double between 0.0 and 1.0, exclusive at both ends, using a technique that can produce more of the valid values for a double
        /// (near to 0) than other methods.
        /// </summary>
        /// <remarks>
        /// <br/>The code for this is small, but extremely unorthodox. The technique is related to <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>,
        /// but because the ability to get the number of leading or trailing zeros is in a method not present in .NET Standard, we get close to that by using
        /// BitConverter.DoubleToInt64Bits() on a negative long and using its exponent bits directly. The smallest double this can return is 1.0842021724855044E-19 ; the largest it
        /// can return is 0.9999999999999999 .
        /// </p>
        /// <br/>This is voodoo code.
        /// </p>
        /// </remarks>
        /// <returns>A double between 0.0 and 1.0, exclusive at both ends.</returns>
        public double NextExclusiveDouble()
        {
            long bits = NextLong();
            return BitConverter.Int64BitsToDouble((0x7C10000000000000L + (BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) & -0x0010000000000000L)) | (~bits & 0x000FFFFFFFFFFFFFL));
        }

        /// <summary>
        /// Just like {@link #NextDouble(double)}, but this is exclusive on both 0.0 and outerBound.
        /// Like {@link #nextExclusiveDouble()}, which this uses, this may have better bit-distribution of
        /// double values, and it may also be better able to produce very small doubles when outerBound is large.
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a double between 0.0, exclusive, and outerBound, exclusive</returns>
        public double NextExclusiveDouble(double outerBound)
        {
            return NextExclusiveDouble() * outerBound;
        }

        /// <summary>
        /// Just like {@link #NextDouble(double, double)}, but this is exclusive on both innerBound and outerBound.
        /// Like {@link #nextExclusiveDouble()}, which this uses,, this may have better bit-distribution of double values,
        /// and it may also be better able to produce doubles close to innerBound when {@code outerBound - innerBound} is large.
        /// <param name="innerBound">the inner exclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a double between innerBound, exclusive, and outerBound, exclusive</returns>
        public double NextExclusiveDouble(double innerBound, double outerBound)
        {
            return innerBound + NextExclusiveDouble() * (outerBound - innerBound);
        }

        /// <summary>
        /// Gets a random float between 0.0 and 1.0, exclusive at both ends. This can return float values between 1.0842022E-19 and 0.99999994; it cannot return 0 or 1.
        /// </summary>
        /// <remarks>
        /// Like <see cref="NextExclusiveDouble()"/>, this is absolute voodoo code. Its implementation generates one long and then does conversions both from int to float
        /// (to get the result), and from double to long (to get an approximation of log base 2). The code here is bat country, and should not be edited carelessly.</remarks>
        /// <returns>A random uniform float between 0 and 1 (both exclusive).</returns>
        public float NextExclusiveFloat()
        {
            // return ((NextUInt() >> 9) + 1u) * 5.960464E-8f;
            long bits = NextLong();
            return BitConverter.Int32BitsToSingle((1089 + (int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) >> 52) << 23) | ((int)~bits & 0x007FFFFF));
        }

        /// <summary>
        /// Just like {@link #NextFloat(float)}, but this is exclusive on both 0.0 and outerBound.
        /// Like {@link #nextExclusiveFloat()}, this may have better bit-distribution of float values, and
        /// it may also be better able to produce very small floats when outerBound is large.
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a float between 0.0, exclusive, and outerBound, exclusive</returns>
        public float NextExclusiveFloat(float outerBound)
        {
            return NextExclusiveFloat() * outerBound;
        }

        /// <summary>
        /// Just like {@link #NextFloat(float, float)}, but this is exclusive on both innerBound and outerBound.
        /// Like {@link #nextExclusiveFloat()}, this may have better bit-distribution of float values, and
        /// it may also be better able to produce floats close to innerBound when {@code outerBound - innerBound} is large.
        /// <param name="innerBound">the inner exclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a float between innerBound, exclusive, and outerBound, exclusive</returns>
        public float NextExclusiveFloat(float innerBound, float outerBound)
        {
            return innerBound + NextExclusiveFloat() * (outerBound - innerBound);
        }


        /// <summary>
        /// A way of taking a double in the (0.0, 1.0) range and mapping it to a Gaussian or normal distribution, so high
        /// inputs correspond to high outputs, and similarly for the low range. This is centered on 0.0 and its standard
        /// deviation seems to be 1.0 (the same as {@link java.util.Random#nextGaussian()}). If this is given an input of 0.0
        /// or less, it returns -38.5, which is slightly less than the result when given {@link Double#MIN_VALUE}. If it is
        /// given an input of 1.0 or more, it returns 38.5, which is significantly larger than the result when given the
        /// largest double less than 1.0 (this value is further from 1.0 than {@link Double#MIN_VALUE} is from 0.0). If
        /// given {@link Double#NaN}, it returns whatever {@link Math#copySign(double, double)} returns for the arguments
        /// {@code 38.5, Double.NaN}, which is implementation-dependent. It uses an algorithm by Peter John Acklam, as
        /// implemented by Sherali Karimov.
        /// <a href="https://web.archive.org/web/20150910002142/http://home.online.no/~pjacklam/notes/invnorm/impl/karimov/StatUtil.java">Original source</a>.
        /// <a href="https://web.archive.org/web/20151030215612/http://home.online.no/~pjacklam/notes/invnorm/">Information on the algorithm</a>.
        /// <a href="https://en.wikipedia.org/wiki/Probit_function">Wikipedia's page on the probit function</a> may help, but
        /// is more likely to just be confusing.
        /// <br/>
        /// Acklam's algorithm and Karimov's implementation are both quite fast. This appears faster than generating
        /// Gaussian-distributed numbers using either the Box-Muller Transform or Marsaglia's Polar Method, though it isn't
        /// as precise and can't produce as extreme min and max results in the extreme cases they should appear. If given
        /// a typical uniform random double that's exclusive on 1.0, it won't produce a result higher than
        /// {@code 8.209536145151493}, and will only produce results of at least {@code -8.209536145151493} if 0.0 is
        /// excluded from the inputs (if 0.0 is an input, the result is {@code -38.5}). A chief advantage of using this with
        /// a random number generator is that it only requires one random double to obtain one Gaussian value;
        /// {@link java.util.Random#nextGaussian()} generates at least two random doubles for each two Gaussian values, but
        /// may rarely require much more random generation.
        /// <br/>
        /// This can be used both as an optimization for generating Gaussian random values, and as a way of generating
        /// Gaussian values that match a pattern present in the inputs (which you could have by using a sub-random sequence
        /// as the input, such as those produced by a van der Corput, Halton, Sobol or R2 sequence). Most methods of generating
        /// Gaussian values (e.g. Box-Muller and Marsaglia polar) do not have any way to preserve a particular pattern.
        /// <param name="d">should be between 0 and 1, exclusive, but other values are tolerated</param>
        /// <returns>a normal-distributed double centered on 0.0; all results will be between -38.5 and 38.5, both inclusive</returns>
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
        /// If the standard deviation is 1.0 and the mean is 0.0, then this can produce results between -8.209536145151493 and 8.209536145151493 (both extremely rarely).
        /// </summary>
        /// <returns>A double from the normal distribution with the specified mean (default 0.0) and standard deviation (default 1.0).</returns>
        public double NextNormal(double mean = 0.0, double stdDev = 1.0)
        {
            return Probit(NextExclusiveDouble()) * stdDev + mean;
        }

        /// <summary>
        /// (Optional) If implemented, this should jump the generator forward by the given number of steps as distance and return the result of NextULong()
        /// as if called at that step. The distance can be negative if a long is cast to a ulong, which jumps backwards if the period of the generator is 2 to the 64.
        /// </summary>
        /// <param name="distance">How many steps to jump forward</param>
        /// <returns>The result of what NextULong() would return at the now-current jumped state.</returns>
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

        /// <summary>
        /// Returns a full copy (deep, if necessary) of this IRandom.
        /// </summary>
        /// <returns>A copy of this IRandom.</returns>
        public abstract IRandom Copy();

        /// <summary>
        /// Sets each state in this IRandom to the corresponding state in the other IRandom.
        /// This generally only works correctly if both objects have the same class.
        /// </summary>
        /// <param name="other">Another IRandom that almost always should have the same class as this one.</param>
        public void SetWith(IRandom other)
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

        /// <summary>
        /// Given two EnhancedRandom objects that could have the same or different classes,
        /// this returns true if they have the same class and same state, or false otherwise.
        /// </summary>
        /// <remarks>
        /// Both of the arguments should implement <see cref="SelectState(int)"/>, or this
        /// will throw an UnsupportedOperationException. This can be useful for comparing
        /// EnhancedRandom classes that do not implement Equals(), for whatever reason.
        /// </remarks>
        /// <param name="left">An EnhancedRandom to compare for equality</param>
        /// <param name="right">Another EnhancedRandom to compare for equality</param>
        /// <returns>true if the two EnhancedRandom objects have the same class and state, or false otherwise</returns>
        public static bool AreEqual(IRandom left, IRandom right)
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

        /// <summary>
        /// Returns true if a random value between 0 and 1 is less than the specified value.
        /// <param name="chance">A float between 0.0 and 1.0; higher values are more likely to result in true.</param>
        /// <returns>a bool selected with the given chance of being true</returns>
        public bool NextBool(float chance)
        {
            return NextFloat() < chance;
        }

        /// <summary>
        /// Returns -1 or 1, randomly.
        /// <returns>-1 or 1, selected with approximately equal likelihood</returns>
        public int NextSign()
        {
            return 1 | NextInt() >> 31;
        }

        /// <summary>
        /// Returns a triangularly distributed random number between -1.0 (exclusive) and 1.0 (exclusive), where values around zero are
        /// more likely. Advances the state twice.
        /// <br/>
        /// This is an optimized version of {@link #NextTriangular(float, float, float) NextTriangular(-1, 1, 0)}
        public float NextTriangular()
        {
            return NextFloat() - NextFloat();
        }

        /// <summary>
        /// Returns a triangularly distributed random number between {@code -max} (exclusive) and max (exclusive), where values
        /// around zero are more likely. Advances the state twice.
        /// <br/>
        /// This is an optimized version of {@link #nextTriangular(float, float, float) NextTriangular(-max, max, 0)}
        /// <param name="max">the upper limit</param>
        public float NextTriangular(float max)
        {
            return (NextFloat() - NextFloat()) * max;
        }

        /// <summary>
        /// Returns a triangularly distributed random number between min (inclusive) and max (exclusive), where the
        /// mode argument defaults to the midpoint between the bounds, giving a symmetric distribution. Advances the state once.
        /// <br/>
        /// This method is equivalent of {@link #nextTriangular(float, float, float) NextTriangular(min, max, (min + max) * 0.5f)}
        /// <param name="min">the lower limit</param>
        /// <param name="max">the upper limit</param>
        public float NextTriangular(float min, float max)
        {
            return NextTriangular(min, max, (min + max) * 0.5f);
        }

        /// <summary>
        /// Returns a triangularly distributed random number between min (inclusive) and max (exclusive), where values
        /// around mode are more likely. Advances the state once.
        /// <param name="min"> the lower limit</param>
        /// <param name="max"> the upper limit</param>
        /// <param name="mode">the point around which the values are more likely</param>
        public float NextTriangular(float min, float max, float mode)
        {
            float u = NextFloat();
            float d = max - min;
            if (u <= (mode - min) / d) { return min + MathF.Sqrt(u * d * (mode - min)); }
            return max - MathF.Sqrt((1 - u) * d * (max - mode));
        }

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public int MinIntOf(int innerBound, int outerBound, int trials)
        {
            int v = NextInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public int MaxIntOf(int innerBound, int outerBound, int trials)
        {
            int v = NextInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextLong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public long MinLongOf(long innerBound, long outerBound, int trials)
        {
            long v = NextLong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextLong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextLong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public long MaxLongOf(long innerBound, long outerBound, int trials)
        {
            long v = NextLong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextLong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextUInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public uint MinUIntOf(uint innerBound, uint outerBound, int trials)
        {
            uint v = NextUInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextUInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextUInt(int, int)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public uint MaxUIntOf(uint innerBound, uint outerBound, int trials)
        {
            uint v = NextUInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextUInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextULong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public ulong MinULongOf(ulong innerBound, ulong outerBound, int trials)
        {
            ulong v = NextULong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextULong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextULong(long, long)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public ulong MaxULongOf(ulong innerBound, ulong outerBound, int trials)
        {
            ulong v = NextULong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextULong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextDouble(double, double)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public double MinDoubleOf(double innerBound, double outerBound, int trials)
        {
            double v = NextDouble(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextDouble(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextDouble(double, double)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public double MaxDoubleOf(double innerBound, double outerBound, int trials)
        {
            double v = NextDouble(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, NextDouble(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to {@link #NextFloat(float, float)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public float MinFloatOf(float innerBound, float outerBound, int trials)
        {
            float v = NextFloat(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, NextFloat(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to {@link #NextFloat(float, float)} using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
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

        /// <summary>
        /// Shuffles the given array in-place pseudo-randomly, using this to generate
        /// {@code items.Length - 1} random numbers and using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an array of some reference type; must be non-null but may contain null items</param>
        public void Shuffle<T>(T[] items)
        {
            Shuffle(items, 0, items.Length);
        }

        /// <summary>
        /// Shuffles the given IList in-place pseudo-randomly, using this to generate
        /// {@code items.Count - 1} random numbers and using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        public void Shuffle<T>(IList<T> items)
        {
            Shuffle(items, 0, items.Count);
        }

        /// <summary>
        /// Shuffles a section of the given array in-place pseudo-randomly, using this to generate
        /// {@code length - 1} random numbers and using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an array of some reference type; must be non-null but may contain null items</param>
        /// <param name="offset">the index of the first element of the array that can be shuffled</param>
        /// <param name="length">the length of the section to shuffle</param>
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
        /// <summary>
        /// Shuffles a section of the given IList in-place pseudo-randomly, using this to generate
        /// {@code length - 1} random numbers and using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <param name="offset">the index of the first element of the IList that can be shuffled</param>
        /// <param name="length">the length of the section to shuffle</param>
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

    // /**
    // * Gets a random double between 0.0 and 1.0, exclusive at both ends; this method is also more uniform than
    // * {@link #NextDouble()} if you use the bit-patterns of the returned doubles. This is a simplified version of
    // * <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>. This can return double
    // * values between 2.710505431213761E-20 and 0.9999999999999999, or 0x1.0p-65 and 0x1.fffffffffffffp-1 in hex
    // * notation. It cannot return 0 or 1. Most cases can instead use {@link #nextExclusiveDoubleEquidistant()}, which is
    // * implemented more traditionally but may have different performance. This method can also return doubles that
    // * are extremely close to 0, but can't return doubles that are as close to 1, due to limits of doubles.
    // * However, nextExclusiveDoubleEquidistant() can return only a minimum value that is as distant from 0 as its maximum
    // * value is distant from 1.
    // * <br/>
    // * To compare, NextDouble() and nextExclusiveDoubleEquidistant() are less likely to produce a "1" bit for their
    // * lowest 5 bits of mantissa/significand (the least significant bits numerically, but potentially important
    // * for some uses), with the least significant bit produced half as often as the most significant bit in the
    // * mantissa. As for this method, it has approximately the same likelihood of producing a "1" bit for any
    // * position in the mantissa.
    // * <br/>
    // * The default implementation may have different performance characteristics than {@link #NextDouble()},
    // * because this doesn't perform any floating-point multiplication or division, and instead assembles bits
    // * obtained by one call to <see cref="NextLong()"/>. This uses {@link BitConversion#longBitsToDouble(long)} and
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

    // /**
    // * Gets a random float between 0.0 and 1.0, exclusive at both ends. This method is also more uniform than
    // * {@link #NextFloat()} if you use the bit-patterns of the returned floats. This is a simplified version of
    // * <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>. This version can
    // * return float values between 2.7105054E-20 to 0.99999994, or 0x1.0p-65 to 0x1.fffffep-1 in hex notation.
    // * It cannot return 0 or 1. To compare, NextFloat() is less likely to produce a "1" bit for its
    // * lowest 5 bits of mantissa/significand (the least significant bits numerically, but potentially important
    // * for some uses), with the least significant bit produced half as often as the most significant bit in the
    // * mantissa. As for this method, it has approximately the same likelihood of producing a "1" bit for any
    // * position in the mantissa.
    // * <br/>
    // * The default implementation may have different performance characteristics than {@link #NextFloat()},
    // * because this doesn't perform any floating-point multiplication or division, and instead assembles bits
    // * obtained by one call to <see cref="NextLong()"/>. This uses {@link BitConversion#intBitsToFloat(int)} and
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

}
