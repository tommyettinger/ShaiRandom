using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 2 states, more here later.
    /// </summary>
    /// <remarks>
    /// This one supports <see cref="PreviousULong()"/>, but not <see cref="AbstractRandom.Skip(ulong)"/>.
    /// It's based on a permutation of an LCG, like PCG-Random by way of SplitMix64. A mizuchi is a mythological river dragon,
    /// and since this supports multiple streams (by changing StateB), the waterway theme seemed fitting.
    /// </remarks>
    public sealed class MizuchiRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "MizR" .
        /// </summary>
        public override string DefaultTag => "MizR";
        
        /// <summary>
        /// The first state; can be any ulong.
        /// </summary>
        public ulong StateA { get; set; }
        private ulong _b;
        /// <summary>
        /// The second state; can be any odd ulong (the last bit must be 1).
        /// </summary>
        public ulong StateB { get
            {
                return _b;
            }
            set
            {
                _b = value | 1UL;
            }
        }

        /// <summary>
        /// Creates a new MizuchiRandom with a random state.
        /// </summary>
        public MizuchiRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
        }

        /// <summary>
        /// Creates a new MizuchiRandom with the given seed; any ulong value is permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="Seed(ulong)">Seed(ulong)</see> to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public MizuchiRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new MizuchiRandom with the given two states; all ulong values are permitted, but stateB will always be made odd.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim, except if stateB is even (then 1 is added to it).
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong; if even, 1 will be added.</param>
        public MizuchiRandom(ulong stateA, ulong stateB)
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
        /// This supports <see cref="PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => true;

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
        }

        /// <summary>
        /// This initializes all states of the generator to different pseudo-random values based on the given seed.
        /// </summary>
        /// <remarks>
        /// (2 to the 64) possible initial generator states can be produced here.
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(MizuchiRandom rng, ulong seed)
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
                rng._b = (x ^ x >> 27) | 1UL;
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
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            unchecked
            {
                ulong z = (StateA = StateA * 0xF7C2EBC08F67F2B5UL + StateB);
                z = (z ^ z >> 23 ^ z >> 47) * 0xAEF17502108EF2D9UL;
                return z ^ z >> 25;
            }
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            unchecked
            {
                ulong z = (StateA = StateA * 0xF7C2EBC08F67F2B5UL + StateB);
                z = (z ^ z >> 23 ^ z >> 47) * 0xAEF17502108EF2D9UL;
                return BitConverter.Int64BitsToDouble((long)((z ^ z >> 25) >> 12) | 0x3FF0000000000000L) - 1.0;
            }
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            unchecked
            {
                ulong z = (StateA = (StateA - StateB) * 0x09795DFF8024EB9DUL);
                z = (z ^ z >> 23 ^ z >> 47) * 0xAEF17502108EF2D9UL;
                return z ^ z >> 25;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new MizuchiRandom(StateA, StateB);
    }
}
