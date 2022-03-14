using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 2 states; low-quality, and not recommended for external use.
    /// It is pretty fast though, and the low quality is primarily a problem with lower bits.
    /// </summary>
    /// <remarks>
    /// The algorithm here is xorshift128+ , which is quite old now; see https://xoshiro.di.unimi.it/xorshift.php for more information on this family of algorithms.
    /// <br />
    /// This does not support <see cref="IEnhancedRandom.PreviousULong()"/> or <see cref="IEnhancedRandom.Skip(ulong)"/>.
    /// </remarks>
    public sealed class Xorshift128PlusRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "XSPR" .
        /// </summary>
        public override string DefaultTag => "XSPR";

        /// <summary>
        /// The first state; can be any ulong unless both states are 0.
        /// </summary>
        public ulong StateA { get; set; }
        /// <summary>
        /// The second state; can be any ulong unless both states are 0.
        /// </summary>
        public ulong StateB { get; set; }

        /// <summary>
        /// Creates a new Xorshift128PlusRandom with a random state.
        /// </summary>
        public Xorshift128PlusRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            if ((StateA | StateB) == 0UL) StateA = ulong.MaxValue;
        }

        /// <summary>
        /// Creates a new Xorshift128PlusRandom with the given seed; any ulong value is permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="Seed(ulong)">Seed(ulong)</see> to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public Xorshift128PlusRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new Xorshift128PlusRandom with the given two states; all ulong values are permitted, but stateB will always be made odd.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim, except if both are 0 (then StateA becomes ulong.MaxValue).
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong; if even, 1 will be added.</param>
        public Xorshift128PlusRandom(ulong stateA, ulong stateB)
        {
            StateA = stateA;
            StateB = stateB;
        }

        /// <summary>
        /// This generator has 2 ulong states, so this returns 2.
        /// </summary>
        public override int StateCount => 2;
        /// <summary>
        /// This supports <see cref="SelectState(int)"/>.
        /// </summary>
        public override bool SupportsReadAccess => true;
        /// <summary>
        /// This supports <see cref="SetSelectedState(int, ulong)"/>.
        /// </summary>
        public override bool SupportsWriteAccess => true;
        /// <summary>
        /// This does not support <see cref="IEnhancedRandom.Skip(ulong)"/>.
        /// </summary>
        public override bool SupportsSkip => false;
        /// <summary>
        /// This supports <see cref="IEnhancedRandom.Leap()"/>, with one call to Leap() equivalent to <code>Math.Pow(2, 64)</code> calls to <see cref="NextULong()"/>.
        /// </summary>
        public override bool SupportsLeap => true;
        /// <summary>
        /// This does not support <see cref="IEnhancedRandom.PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => false;

        /// <summary>
        /// Gets the state determined by selection, as-is.
        /// </summary>
        /// <remarks>The value for selection should be 0 or 1; if it is any other value this gets state A as if 0 was given.</remarks>
        /// <param name="selection">used to select which state variable to get; generally 0 or 1.</param>
        /// <returns>The value of the selected state.</returns>
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 1:
                    return StateB;
                default:
                    return StateA;
            }
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value, as-is.
        /// </summary>
        /// <remarks>
        /// Selections 0 and 1 refer to states A and B, and if the selection is anything else, this treats it as 0 and sets stateA.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0 or 1.</param>
        /// <param name="value">The exact value to use for the selected state, if valid.</param>
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 1:
                    StateB = value;
                    break;
                default:
                    StateA = value;
                    break;
            }
            if ((StateA | StateB) == 0UL) StateA = ulong.MaxValue;
        }

        /// <summary>
        /// This initializes all states of the generator to different pseudo-random values based on the given seed.
        /// </summary>
        /// <remarks>
        /// (2 to the 64) possible initial generator states can be produced here.
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(Xorshift128PlusRandom rng, ulong seed)
        {
            unchecked
            {
                ulong x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateA = x ^ x >> 27;
                x = (seed + 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateB = x ^ x >> 27;
            }
        }

        /// <summary>
        /// Sets the state completely to the given two state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as setting StateA and StateB as a group.
        /// You may want to call <see cref="NextULong()">NextUlong()</see> a few times after setting the states like this, unless
        /// the states have very little or no correlation, because certain very-similar combinations of seeds produce correlated sequences.
        /// </remarks>
        /// <param name="stateA">The first state; can be any ulong.</param>
        /// <param name="stateB">The second state; can be any odd ulong.</param>
        public override void SetState(ulong stateA, ulong stateB)
        {
            StateA = stateA;
            StateB = stateB;
            if ((StateA | StateB) == 0UL) StateA = ulong.MaxValue;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            var tx = StateA;
            var ty = StateB;
            StateA = ty;
            tx ^= tx << 23;
            tx ^= tx >> 17;
            tx ^= ty ^ (ty >> 26);
            StateB = tx;
            return tx + ty;
        }

        /// <inheritdoc />
        public override float NextSparseFloat()
        {
            var tx = StateA;
            var ty = StateB;
            StateA = ty;
            tx ^= tx << 23;
            tx ^= tx >> 17;
            tx ^= ty ^ (ty >> 26);
            StateB = tx;
            return BitConverter.Int32BitsToSingle((int)(tx + ty >> 41) | 0x3F800000) - 1f;
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            var tx = StateA;
            var ty = StateB;
            StateA = ty;
            tx ^= tx << 23;
            tx ^= tx >> 17;
            tx ^= ty ^ (ty >> 26);
            StateB = tx;
            return BitConverter.Int64BitsToDouble((long)((tx + ty) >> 12) | 0x3FF0000000000000L) - 1.0;
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new Xorshift128PlusRandom(StateA, StateB);

    }
}
