using System;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// The interface view of the functionality typically provided by <see cref="AbstractRandom"/>.
    /// </summary>
    public interface IEnhancedRandom
    {
        /// <summary>
        /// Sets the seed of this random number generator using a single ulong seed.
        /// </summary>
        /// <remarks>
        /// This does not necessarily assign the state variable(s) of the implementation with the exact contents of seed,
        /// so <see cref="SelectState(int)"/> should not be expected to return seed after this, though it may.
        /// </remarks>
        /// <param name="seed">May be any ulong; if the seed would give an invalid state, the generator is expected to correct that state.</param>
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
        /// The exactly-four-character string that may identify this IEnhancedRandom for serialization purposes.
        /// </summary>
        /// <remarks>
        /// If this is not four characters in length, it will be ignored, and this IEnhancedRandom will not be serializable by the mechanisms here.
        /// Wrapper classes that contain an IEnhancedRandom and alter some properties of it will often use a one-character tag, which is intentionally invalid
        /// because they cannot be serialized without the IEnhancedRandom they contain.
        /// </remarks>
        string Tag { get; }

        /// <summary>
        /// Returns a full copy (deep, if necessary) of this IEnhancedRandom.
        /// </summary>
        /// <returns>A copy of this IEnhancedRandom.</returns>
        IEnhancedRandom Copy();

        /// <summary>
        /// Produces a string that encodes the type and full state of this generator.
        /// This is an optional operation for classes that only implement IEnhancedRandom; AbstractRandom strongly encourages but does not require an implementation.
        /// </summary>
        /// <returns>An encoded string that stores the type and full state of this generator.</returns>
        string StringSerialize();

        /// <summary>
        /// Given data from a string produced by <see cref="StringSerialize"/>, if the specified type is compatible,
        /// then this method sets the state of this IEnhancedRandom to the specified stored state.
        /// This is an optional operation for classes that only implement IEnhancedRandom; AbstractRandom strongly encourages but does not require an implementation.
        /// </summary>
        /// <remarks>
        /// It is more common to call <see cref="AbstractRandom.Deserialize(ReadOnlySpan{char})"/> when the exact variety of IEnhancedRandom is not known.
        /// </remarks>
        /// <param name="data">Data from a string produced by StringSerialize.</param>
        /// <returns>This IEnhancedRandom, after modifications.</returns>
        IEnhancedRandom StringDeserialize(ReadOnlySpan<char> data);

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
        ulong SelectState(int selection);

        /// <summary>
        /// Sets a selected state value to the given ulong value.
        /// </summary>
        /// <remarks>
        /// The number of possible selections is up to the implementing class, but selection should be at least 0 and less than <see cref="StateCount"/>.
        /// Implementors are permitted to change value if it is not valid, but they should not alter it if it is valid.
        /// The basic implementation in AbstractRandom calls <see cref="Seed(ulong)"/> with value, which doesn't need changing if the generator has one state that is set verbatim by Seed().
        /// Otherwise, this method should be implemented when <see cref="SelectState(int)"/> is and the state is allowed to be set by users.
        /// Having accurate ways to get and set the full state of a random number generator makes it much easier to serialize and deserialize that class.
        /// </remarks>
        /// <param name="selection">The index of the state to set.</param>
        /// <param name="value">The value to try to use for the selected state.</param>
        void SetSelectedState(int selection, ulong value);

        /// <summary>
        /// Sets every state variable to the given state.
        /// </summary>
        /// <remarks>
        /// If <see cref="StateCount"/> is 1, then this should set the whole state to the given value using <see cref="SetSelectedState(int, ulong)"/>.
        /// If StateCount is more than 1, then all states will be set in the same way (using SetSelectedState(), all to state).
        /// </remarks>
        /// <param name="state">The ulong variable to use for every state variable.</param>
        void SetState(ulong state)
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
        void SetState(ulong stateA, ulong stateB)
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
        void SetState(ulong stateA, ulong stateB, ulong stateC)
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
        void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
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
        /// Sets all state variables to alternating values chosen from states. If states is empty,
        /// then this does nothing, and leaves the current generator unchanged. This works for
        /// generators with any <see cref="StateCount"/>, but may allocate an array if states is
        /// used as a varargs (you can pass an existing array without needing to allocate). This
        /// uses <see cref="SetSelectedState(int, ulong)"/> to change the states.
        /// </summary>
        /// <param name="states">an array or varargs of ulong values to use as states</param>
        void SetState(params ulong[] states)
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
        /// Can return any ulong.
        /// </summary>
        /// <returns>A random ulong, which can have any ulong value.</returns>
        ulong NextULong();

        /// <summary>
        /// Can return any long, positive or negative.
        /// If you specifically want a non-negative long, you can use <code>(NextLong() &amp; long.MaxValue)</code>,
        /// which can return any long that is not negative.
        /// </summary>
        /// <returns>A random long, which may be positive or negative, and can have any long value.</returns>
        long NextLong();

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value
        /// between 0 (inclusive) and the specified value (exclusive), drawn from
        /// this random number generator's sequence.
        /// </summary>
        /// <remarks>
        /// The general contract of
        /// nextLong is that one long value in the specified range
        /// is pseudorandomly generated and returned.  All bound possible
        /// long values are produced with (approximately) equal
        /// probability, though there may be a small amount of bias depending on the
        /// implementation and the bound. To generate a ulong that is inclusive on <see cref="ulong.MaxValue"/>,
        /// use <see cref="NextULong()"/>.
        /// </remarks>
        /// <param name="bound">the outer bound (exclusive). If 0, this always returns 0.</param>
        /// <returns>the next pseudorandom, uniformly distributed long
        /// value between zero (inclusive) and bound (exclusive)
        /// from this random number generator's sequence</returns>
        ulong NextULong(ulong bound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between an
        /// inner bound of 0 (inclusive) and the specified outerBound (exclusive).
        /// </summary>
        /// <remarks>
        /// To generate a long that is inclusive on <see cref="long.MaxValue"/>,
        /// use: <code>(NextLong() &amp; long.MaxValue)</code></remarks>
        /// <param name="outerBound">the outer bound (exclusive). If 0, this always returns 0. If negative, returns a non-positive result.</param>
        /// <returns>a pseudorandom long between 0 (inclusive) and outerBound (exclusive)</returns>
        long NextLong(long outerBound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between the
        /// specified inner bound (inclusive) and the specified outer bound
        /// (exclusive). If outer is less than inner,
        /// this still returns a value between the two, and inner is still inclusive,
        /// while outer is still exclusive. If outer and inner are equal, this returns inner.
        /// </summary>
        /// <param name="inner">the inclusive inner bound; may be any ulong</param>
        /// <param name="outer">the exclusive outer bound; may be any ulong, including less than inner</param>
        /// <returns>a pseudorandom ulong between inner (inclusive) and outer (exclusive)</returns>
        ulong NextULong(ulong inner, ulong outer);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed long value between the
        /// specified inner bound (inclusive) and the specified outer bound
        /// (exclusive). If outer is less than inner,
        /// this still returns a value between the two, and inner is still inclusive,
        /// while outer is still exclusive. If outer and inner are equal, this returns inner.
        /// </summary>
        /// <param name="inner">the inclusive inner bound; may be any long, allowing negative</param>
        /// <param name="outer">the exclusive outer bound; may be any long, allowing negative</param>
        /// <returns>a pseudorandom long between inner (inclusive) and outer (exclusive)</returns>
        long NextLong(long inner, long outer);

        /// <summary>
        /// Generates the next pseudorandom number with a specific maximum size in bits (not a max number).
        /// </summary>
        /// <remarks>
        /// If you want to get a random number in a range, you should usually use <see cref="NextInt(int)"/> instead.
        /// <br/>The general contract of next is that it returns an
        /// uint value and if the argument bits is between
        /// 1 and 32 (inclusive), then that many low-order
        /// bits of the returned value will be (approximately) independently
        /// chosen bit values, each of which is (approximately) equally
        /// likely to be 0 or 1.
        /// <br/>
        /// Note that you can give this values for bits that are outside its expected range of 1 to 32,
        /// but the value used, as long as bits is positive, will effectively be <code>bits % 32</code>. As stated
        /// before, a value of 0 for bits is the same as a value of 32.
        /// </remarks>
        /// <param name="bits">the amount of random bits to request, from 1 to 32</param>
        /// <returns>the next pseudorandom value from this random number
        /// generator's sequence</returns>
        uint NextBits(int bits);

        /// <summary>
        /// Generates random bytes and places them into a user-supplied
        /// span.  The number of random bytes produced is equal to
        /// the length of the span.
        /// </summary>
        /// <remarks>
        /// Note that this function can easily accept an array as well, or anything else that can convert to span either
        /// via either implicit or explicit conversion.  It can also fill only part of any such array or structure
        /// (see examples).
        /// <example>
        /// <code>
        /// // Fill whole array
        /// myRng.NextBytes(myArray);
        /// </code>
        /// </example>
        ///
        /// <example>
        /// <code>
        /// // Fill the three elements starting at index 1
        /// myRng.NextBytes(myArray.AsSpan(1, 3));
        /// </code>
        /// </example>
        ///
        /// <example>
        /// <code>
        /// // Fill all elements from index 1 to (but not including) the last element
        /// myRng.NextBytes(myArray.AsSpan(1..^1));
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="bytes">The Span to fill with random bytes.</param>
        void NextBytes(Span<byte> bytes);

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed int
        /// value from this random number generator's sequence.
        /// </summary>
        /// <remarks>
        /// The general
        /// contract of nextInt is that one int value is
        /// pseudorandomly generated and returned. All 2<sup>32</sup> possible
        /// int values are produced with (approximately) equal probability.
        /// </remarks>
        /// <returns>the next pseudorandom, uniformly distributed int
        /// value from this random number generator's sequence</returns>
        int NextInt();

        /// <summary>
        /// Gets a random uint by using the low 32 bits of NextULong(); this can return any uint.
        /// </summary>
        /// <remarks>
        /// All 2<sup>32</sup> possible
        /// uint values are produced with (approximately) equal probability.</remarks>
        /// <returns>Any random uint.</returns>
        uint NextUInt();

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed uint value
        /// between 0 (inclusive) and the specified value (exclusive), drawn from
        /// this random number generator's sequence.</summary>
        /// <remarks>The general contract of
        /// nextUInt is that one uint value in the specified range
        /// is pseudorandomly generated and returned.  All possible uint
        /// values less than bound are produced with (approximately) equal
        /// probability. To generate a uint that is inclusive on <see cref="uint.MaxValue"/>,
        /// use <see cref="NextUInt()"/>.
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
        /// <param name="bound">the upper bound (exclusive). If 0 or 1, this always returns 0.</param>
        /// <returns>the next pseudorandom, uniformly distributed int
        /// value between zero (inclusive) and bound (exclusive)
        /// from this random number generator's sequence</returns>
        uint NextUInt(uint bound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between an
        /// inner bound of 0 (inclusive) and the specified outerBound (exclusive).
        /// </summary>
        /// <remarks>
        /// If outerBound is less than or equal to 0,
        /// this always returns 0. To generate an int that is inclusive on <see cref="int.MaxValue"/>,
        /// use <code>(NextInt() &amp; int.MaxValue)</code>
        /// To generate any int, including negative ones, use <see cref="NextInt()">NextInt()</see>.
        /// </remarks>
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="outerBound">the outer exclusive bound; may be any int value, allowing negative</param>
        /// <returns>a pseudorandom int between 0 (inclusive) and outerBound (exclusive)</returns>
        int NextInt(int outerBound);

        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive).
        /// </summary>
        /// <remarks>
        /// If outer is less than inner,
        /// this still returns a value between the two, and inner is still inclusive,
        /// while outer is still exclusive. If outer and inner are equal, this returns inner.
        /// </remarks>
        /// <seealso cref="NextUInt(uint)"> Here's a note about the bias present in the bounded generation.</seealso>
        /// <param name="innerBound">the inclusive inner bound; may be any int, allowing negative</param>
        /// <param name="outerBound">the exclusive outer bound; must be greater than innerBound (otherwise this returns innerBound)</param>
        /// <returns>a pseudorandom int between innerBound (inclusive) and outerBound (exclusive)</returns>
        uint NextUInt(uint innerBound, uint outerBound);
        /// <summary>
        /// Returns a pseudorandom, uniformly distributed int value between the
        /// specified innerBound (inclusive) and the specified outerBound
        /// (exclusive).</summary>
        /// <remarks>
        /// If outer is less than inner,
        /// this still returns a value between the two, and inner is still inclusive,
        /// while outer is still exclusive. If outer and inner are equal, this returns inner.
        /// </remarks>
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
        /// </summary>
        /// <remarks>
        /// A typical implementation is equivalent to a sign check on <see cref="NextLong()"/>,
        /// returning true if the generated long is negative. This is typically the safest
        /// way to implement this method; many types of generators have less statistical
        /// quality on their lowest bit, so just returning based on the lowest bit isn't
        /// always a good idea.
        /// </remarks>
        /// <returns>the next pseudorandom, uniformly distributed</returns>
        /// bool value from this random number generator's
        /// sequence
        bool NextBool();

        /// <summary>
        /// Returns the next pseudorandom, uniformly distributed float
        /// value between 0.0 (inclusive) and 1.0 (exclusive)
        /// from this random number generator's sequence.
        /// </summary>
        /// <remarks>The general contract of NextFloat is that one
        /// float value, chosen (approximately) uniformly from the
        /// range 0.0f (inclusive) to 1.0f (exclusive), is
        /// pseudorandomly generated and returned. All 2<sup>24</sup> possible
        /// float values of the form <i>m x </i>2<sup>-24</sup>,
        /// where <i>m</i> is a positive integer less than 2<sup>24</sup>, are
        /// produced with (approximately) equal probability.
        /// <br/>The public implementation uses the upper 24 bits of <see cref="NextULong()"/>,
        /// with a right shift and a multiply by a very small float
        /// (5.9604645E-8f). It tends to be fast if
        /// NextULong() is fast, but alternative implementations could use 24 bits of
        /// <see cref="NextInt()"/> (or just <see cref="NextBits(int)"/>, giving it 24)
        /// if that generator doesn't efficiently generate 64-bit longs.</remarks>
        /// <returns>the next pseudorandom, uniformly distributed float
        /// value between 0.0 and 1.0 from this
        /// random number generator's sequence</returns>
        float NextFloat();

        /// <summary>
        /// Gets a pseudo-random float between 0 (inclusive) and outerBound (exclusive).
        /// The outerBound may be positive or negative.
        /// </summary>
        /// <remarks>
        /// Exactly the same as: <code>NextFloat() * outerBound</code>
        /// </remarks>
        /// <param name="outerBound">the exclusive outer bound</param>
        /// <returns>a float between 0 (inclusive) and outerBound (exclusive)</returns>
        float NextFloat(float outerBound);

        /// <summary>
        /// Gets a pseudo-random float between innerBound (inclusive) and outerBound (exclusive).
        /// Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
        /// inclusive and which is exclusive.
        /// </summary>
        /// <param name="innerBound">the inclusive inner bound; may be negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be negative</param>
        /// <returns>a float between innerBound (inclusive) and outerBound (exclusive)</returns>
        float NextFloat(float innerBound, float outerBound);

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
        /// <br/>The default implementation uses the upper 53 bits of <see cref="NextULong()"/>,
        /// with a right shift and a multiply by a very small double
        /// (1.1102230246251565E-16). It should perform well
        /// if nextLong() performs well, and is expected to perform less well if the
        /// generator naturally produces 32 or fewer bits at a time.\
        /// </remarks>
        /// <returns>the next pseudorandom, uniformly distributed double
        /// value between 0.0 and 1.0 from this
        /// random number generator's sequence</returns>
        double NextDouble();

        /// <summary>
        /// Gets a pseudo-random double between 0 (inclusive) and outerBound (exclusive).
        /// The outerBound may be positive or negative.
        /// </summary>
        /// <remarks>
        /// Exactly the same as: <code>NextDouble() * outerBound</code>
        /// </remarks>
        /// <param name="outerBound">the exclusive outer bound</param>
        /// <returns>a double between 0 (inclusive) and outerBound (exclusive)</returns>
        double NextDouble(double outerBound);

        /// <summary>
        /// Gets a pseudo-random double between innerBound (inclusive) and outerBound (exclusive).
        /// Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
        /// inclusive and which is exclusive.
        /// </summary>
        /// <param name="innerBound">the inclusive inner bound; may be negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be negative</param>
        /// <returns>a double between innerBound (inclusive) and outerBound (exclusive)</returns>
        double NextDouble(double innerBound, double outerBound);

        /// <summary>
        /// Returns the next pseudorandom, rather-uniformly distributed
        /// decimal value between 0.0M (inclusive) and 1.0M
        /// (exclusive) from this random number generator's sequence.
        /// </summary>
        /// <returns>A rather-uniform random decimal between 0.0M inclusive and 1.0M exclusive.</returns>
        decimal NextDecimal();

        /// <summary>
        /// Gets a pseudo-random float between 0M (inclusive) and outerBound (exclusive).
        /// The outerBound may be positive or negative.
        /// </summary>
        /// <remarks>
        /// Exactly the same as: <code>NextDecimal() * outerBound</code>
        /// </remarks>
        /// <param name="outerBound">the exclusive outer bound</param>
        /// <returns>a decimal between 0 (inclusive) and outerBound (exclusive)</returns>
        decimal NextDecimal(decimal outerBound);

        /// <summary>
        /// Gets a pseudo-random decimal between innerBound (inclusive) and outerBound (exclusive).
        /// Either, neither, or both of innerBound and outerBound may be negative; this does not change which is
        /// inclusive and which is exclusive.
        /// </summary>
        /// <param name="innerBound">the inclusive inner bound; may be negative</param>
        /// <param name="outerBound">the exclusive outer bound; may be negative</param>
        /// <returns>a decimal between innerBound (inclusive) and outerBound (exclusive)</returns>
        decimal NextDecimal(decimal innerBound, decimal outerBound);


        /// <summary>
        /// This is just like <see cref="NextDouble()"/>, returning a double between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
        /// </summary>
        /// <remarks>
        /// It returns 1.0 extremely rarely, 0.000000000000011102230246251565% of the time if there is no bias in the generator, but it
        /// can happen. This typically uses <see cref="NextULong(ulong)"/> internally, so it may have some bias towards or against specific
        /// subtly-different results.
        /// </remarks>
        /// <returns>a double between 0.0, inclusive, and 1.0, inclusive</returns>
        double NextInclusiveDouble();

        /// <summary>
        /// Just like <see cref="NextDouble(double)"/>, but this is inclusive on both 0.0 and outerBound.
        /// </summary>
        /// <remarks>
        /// It may be important to note that this returns outerBound on only 0.000000000000011102230246251565% of calls.
        /// </remarks>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a double between 0.0, inclusive, and outerBound, inclusive</returns>
        double NextInclusiveDouble(double outerBound);

        /// <summary>
        /// Just like <see cref="NextDouble(double, double)"/>, but this is inclusive on both innerBound and outerBound.
        /// </summary>
        /// <remarks>
        /// It may be important to note that this returns outerBound on only 0.000000000000011102230246251565% of calls, if it can
        /// return it at all because of floating-point imprecision when innerBound is a larger number.
        /// </remarks>
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a double between innerBound, inclusive, and outerBound, inclusive</returns>
        double NextInclusiveDouble(double innerBound, double outerBound);

        /// <summary>
        /// This is just like <see cref="NextFloat()"/>, returning a float between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
        /// </summary>
        /// <remarks>
        /// It returns 1.0 rarely, 0.00000596046412226771% of the time if there is no bias in the generator, but it can happen. This method
        /// has been tested by generating 268435456 (or 0x10000000) random ints with <see cref="NextInt(int)"/>, and just before the end of that
        /// it had generated every one of the 16777217 roughly-equidistant floats this is able to produce. Not all seeds and streams are
        /// likely to accomplish that in the same time, or at all, depending on the generator.
        /// </remarks>
        /// <returns>a float between 0.0, inclusive, and 1.0, inclusive</returns>
        float NextInclusiveFloat();

        /// <summary>
        /// Just like <see cref="NextFloat(float)"/>, but this is inclusive on both 0.0 and outerBound.
        /// </summary>
        /// <remarks>
        /// It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls.
        /// </remarks>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a float between 0.0, inclusive, and outerBound, inclusive</returns>
        float NextInclusiveFloat(float outerBound);

        /// <summary>
        /// Just like <see cref="NextFloat(float, float)"/>, but this is inclusive on both innerBound and outerBound.
        /// </summary>
        /// <remarks>
        /// It may be important to note that it returns outerBound on only 0.00000596046412226771% of calls, if it can return
        /// it at all because of floating-point imprecision when innerBound is a larger number.
        /// </remarks>
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a float between innerBound, inclusive, and outerBound, inclusive</returns>
        float NextInclusiveFloat(float innerBound, float outerBound);

        /// <summary>
        /// This is just like <see cref="NextDecimal()"/>, returning a float between 0 and 1, except that it is inclusive on both 0.0 and 1.0.
        /// </summary>
        /// <returns>a decimal between 0.0, inclusive, and 1.0, inclusive</returns>
        decimal NextInclusiveDecimal();

        /// <summary>
        /// Just like <see cref="NextDecimal(decimal)"/>, but this is inclusive on both 0.0 and outerBound.
        /// </summary>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a decimal between 0.0, inclusive, and outerBound, inclusive</returns>
        decimal NextInclusiveDecimal(decimal outerBound);

        /// <summary>
        /// Just like <see cref="NextDecimal(decimal, decimal)"/>, but this is inclusive on both innerBound and outerBound.
        /// </summary>
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer inclusive bound; may be positive or negative</param>
        /// <returns>a decimal between innerBound, inclusive, and outerBound, inclusive</returns>
        decimal NextInclusiveDecimal(decimal innerBound, decimal outerBound);

        /// <summary>
        /// Gets a random double between 0.0 and 1.0, exclusive at both ends, using a technique that can produce more of the valid values for a double
        /// (near to 0) than other methods.
        /// </summary>
        /// <remarks>
        /// This can be implemented in various ways; the simplest is to generate a number in the range between 1 (inclusive) and 2<sup>53</sup> (exclusive), then divide the result by 2<sup>53</sup>.
        /// The technique used in AbstractRandom is very different; it is related to <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>.
        /// Because the ability to get the number of leading or trailing zeros is in a method not present in .NET Standard, we get close to that by using
        /// <see cref="BitConverter.DoubleToInt64Bits(double)"/> on a negative long and using its exponent bits directly. The smallest double AbstractRandom can return is 1.0842021724855044E-19 ; the largest it
        /// can return is 0.9999999999999999 . The smallest result is significantly closer to 0 than <see cref="NextDouble()"/> can produce without actually returning 0, and also much closer than the first method.
        /// <br/>
        /// The method used by AbstractRandom has several possible variations; the one it uses now is about 25% slower or less than NextDouble(). If .NET 6 becomes the default framework, another implementation
        /// for this method becomes possible that outperforms NextDouble() and actually has an even better range as it approaches 0.0. This second method is not the default because it is over 300% slower on earlier,
        /// pre-.NET Core versions.
        /// </remarks>
        /// <returns>A double between 0.0 and 1.0, exclusive at both ends.</returns>

        double NextExclusiveDouble();

        /// <summary>
        /// Just like <see cref="NextDouble(double)"/>, but this is exclusive on both 0.0 and outerBound.
        /// </summary>
        /// <remarks>
        /// Like <see cref="NextExclusiveDouble()"/>, which this uses, this may have better bit-distribution of
        /// double values, and it may also be better able to produce very small doubles when outerBound is large.
        /// </remarks>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a double between 0.0, exclusive, and outerBound, exclusive</returns>
        double NextExclusiveDouble(double outerBound);

        /// <summary>
        /// Just like <see cref="NextDouble(double, double)"/>, but this is exclusive on both innerBound and outerBound.
        /// This also allows outerBound to be greater than or less than innerBound. If they are equal, this returns innerBound.
        /// </summary>
        /// <remarks>
        /// Like <see cref="NextExclusiveDouble()"/>, which this uses,, this may have better bit-distribution of double values,
        /// and it may also be better able to produce doubles close to innerBound when <code>outerBound - innerBound</code> is large.
        /// </remarks>
        /// <param name="innerBound">the inner exclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a double between innerBound, exclusive, and outerBound, exclusive</returns>
        double NextExclusiveDouble(double innerBound, double outerBound);

        /// <summary>
        /// Gets a random float between 0.0 and 1.0, exclusive at both ends. This cannot return 0 or 1.
        /// </summary>
        /// <remarks>
        /// This can be implemented in various ways; the simplest is to generate a number in the range between 1 (inclusive) and 2<sup>24</sup> (exclusive), then divide the result by 2<sup>24</sup>.
        /// The technique used in AbstractRandom is very different; it is related to <a href="https://allendowney.com/research/rand/">this algorithm by Allen Downey</a>.
        /// Because the ability to get the number of leading or trailing zeros is in a method not present in .NET Standard, we get close to that by using
        /// <see cref="BitConverter.SingleToInt32Bits(float)"/> on a negative long and using its exponent bits directly. The smallest float AbstractRandom can return is 1.0842022E-19; the largest it
        /// can return is 0.99999994 . The smallest result is significantly closer to 0 than <see cref="NextFloat()"/> can produce without actually returning 0, and also much closer than the first method.
        /// <br/>
        /// The method used by AbstractRandom has several possible variations; the one it uses now is about 25% slower or less than NextFloat(). If .NET 6 becomes the default framework, another implementation
        /// for this method becomes possible that outperforms NextFloat() and actually has an even better range as it approaches 0.0. This second method is not the default because it is over 300% slower on earlier,
        /// pre-.NET Core versions.
        /// </remarks>
        /// <returns>A random uniform float between 0 and 1 (both exclusive).</returns>
        float NextExclusiveFloat();

        /// <summary>
        /// Just like <see cref="NextFloat(float)"/>, but this is exclusive on both 0.0 and outerBound.
        /// If outerBound is 0, this returns 0.
        /// </summary>
        /// <remarks>
        /// Like <see cref="NextExclusiveFloat()"/>, this may have better bit-distribution of float values, and
        /// it may also be better able to produce very small floats when outerBound is large.
        /// </remarks>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a float between 0.0, exclusive, and outerBound, exclusive</returns>
        float NextExclusiveFloat(float outerBound);

        /// <summary>
        /// Just like <see cref="NextFloat(float, float)"/>, but this is exclusive on both innerBound and outerBound.
        /// This also allows outerBound to be greater than or less than innerBound. If they are equal, this returns innerBound.
        /// </summary>
        /// <remarks>
        /// Like <see cref="NextExclusiveFloat()"/>, this may have better bit-distribution of float values, and
        /// it may also be better able to produce floats close to innerBound when <code>outerBound - innerBound</code> is large.
        /// </remarks>
        /// <param name="innerBound">the inner exclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a float between innerBound, exclusive, and outerBound, exclusive</returns>
        float NextExclusiveFloat(float innerBound, float outerBound);

        /// <summary>
        /// Gets a random decimal between 0.0 and 1.0, exclusive at both ends. This can return decimal values between 1E-28 and 1 - 1E-28; it cannot return 0 or 1.
        /// </summary>
        /// <returns>A random uniform decimal between 0 and 1 (both exclusive).</returns>
        decimal NextExclusiveDecimal();

        /// <summary>
        /// Just like <see cref="NextDecimal(decimal)"/>, but this is exclusive on both 0.0 and outerBound.
        /// If outerBound is 0, this returns 0.
        /// </summary>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a decimal between 0.0, exclusive, and outerBound, exclusive</returns>
        decimal NextExclusiveDecimal(decimal outerBound);

        /// <summary>
        /// Just like <see cref="NextDecimal(decimal, decimal)"/>, but this is exclusive on both innerBound and outerBound.
        /// This also allows outerBound to be greater than or less than innerBound. If they are equal, this returns innerBound.
        /// </summary>
        /// <param name="innerBound">the inner exclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <returns>a decimal between innerBound, exclusive, and outerBound, exclusive</returns>
        decimal NextExclusiveDecimal(decimal innerBound, decimal outerBound);

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
    }
}
