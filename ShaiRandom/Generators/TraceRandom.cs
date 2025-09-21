using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 6 states, built around a "medium-chaotic" construction with a guaranteed minimum
    /// period of 2 to the 64, but likely much longer. One state acts as a "stream" that typically doesn't change over
    /// the lifetime of a generator, and allows a different sequence to be produced given the same initial other states.
    /// </summary>
    /// <remarks>
    /// Tommy Ettinger:
    /// This is "medium-chaotic" because it has six states and performs five math operations per update (two adds, two
    /// XORs, and a bitwise rotation, with a good compiler able to handle those as instruction-level parallel
    /// operations. The "stream" doesn't need to change unless a new state is requested, such as by deserialization.
    ///
    /// This generator is a tiny bit slower than AceRandom. There are at minimum 2 to the 28 unique streams that can be
    /// obtained by giving small-enough odd ulong values as the stream; odd stream inputs less than 536870912 will all
    /// be changed into distinct streams this actually uses (that will all be much larger).
    /// </remarks>
    public sealed class TraceRandom : AbstractRandom
    {
        private ulong _stateF;

        /// <summary>
        /// The identifying tag here is "TrcR" .
        /// </summary>
        public override string DefaultTag => "TrcR";

        /// <summary>
        /// The first state; can be any ulong.
        /// </summary>
        public ulong StateA { get; set; }
        /// <summary>
        /// The second state; can be any ulong.
        /// </summary>
        public ulong StateB { get; set; }
        /// <summary>
        /// The third state; can be any ulong.
        /// </summary>
        public ulong StateC { get; set; }
        /// <summary>
        /// The fourth state; can be any ulong.
        /// </summary>
        public ulong StateD { get; set; }
        /// <summary>
        /// The fifth state; can be any ulong.
        /// </summary>
        public ulong StateE { get; set; }

        /// <summary>
        /// The fifth state; this is limited to holding an odd ulong that "rates" to a threshold of 1 by
        /// <see cref="MathUtils.RateGamma(ulong)"/>. The setter will enforce this constraint by calling
        /// <see cref="MathUtils.FixGamma(ulong, int)"/> with a threshold of 1. If you pass in only odd ulong values
        /// less than 536870912, then all values this can be assigned will be distinct.
        /// </summary>
        public ulong StateF
        {
            get => _stateF;
            set => _stateF = MathUtils.FixGamma(value, 1);
        }

        /// <summary>
        /// Creates a new TraceRandom with a random state. This will always produce a stream (StateF) value that
        /// satisfies its constraint, but does not guarantee that the chosen StateF will be distinct from any chosen for
        /// other generators.
        /// </summary>
        public TraceRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
            StateD = MakeSeed();
            StateE = MakeSeed();
            StateF = MakeSeed();
        }

        /// <summary>
        /// Creates a new TraceRandom with the given seed; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="SetSeed(TraceRandom, ulong)">SetSeed(TraceRandom, ulong)</see>, to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public TraceRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new TraceRandom with the given six states; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim, except for StateF, will be changed if it does not already fit its
        /// constraint. If copying an existing generator, StateF will already fit its constraint and will be used as-is.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        /// <param name="stateD">Any ulong.</param>
        /// <param name="stateE">Any ulong.</param>
        public TraceRandom(ulong stateA, ulong stateB, ulong stateC, ulong stateD, ulong stateE, ulong stateF)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
            StateE = stateE;
            StateF = stateF;
        }

        /// <summary>
        /// This generator has 5 ulong states, so this returns 5.
        /// </summary>
        public override int StateCount => 6;
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
        /// This does not support <see cref="IEnhancedRandom.Leap()"/>.
        /// </summary>
        public override bool SupportsLeap => false;
        /// <summary>
        /// This supports <see cref="PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => true;

        /// <summary>
        /// Gets the state determined by selection, as-is.
        /// </summary>
        /// <remarks>The value for selection should be between 0 and 3, inclusive; if it is any other value this gets state D as if 3 was given.</remarks>
        /// <param name="selection">used to select which state variable to get; generally 0, 1, 2, or 3.</param>
        /// <returns>The value of the selected state.</returns>
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 0:
                    return StateA;
                case 1:
                    return StateB;
                case 2:
                    return StateC;
                case 3:
                    return StateD;
                case 4:
                    return StateE;
                default:
                    return StateF;
            }
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value. For states 0 through 4 inclusive, the state is
        /// set as-is; for state 5 (which is also the default), it may be changed if it does not fit the StateF constraint.
        /// </summary>
        /// <remarks>
        /// Selections 0, 1, 2, 3, 4, and 5 refer to states A, B, C, D, E, and F and if the selection is anything else, this treats it as 5 and sets stateF.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0, 1, 2, 3, 4, or 5.</param>
        /// <param name="value">The exact value to use for the selected state, if valid.</param>
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 0:
                    StateA = value;
                    break;
                case 1:
                    StateB = value;
                    break;
                case 2:
                    StateC = value;
                    break;
                case 3:
                    StateD = value;
                    break;
                case 4:
                    StateE = value;
                    break;
                default:
                    StateF = value;
                    break;
            }
        }

        /// <summary>
        /// Initializes all 6 states of the generator to random values based on the given seed.
        /// </summary>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(TraceRandom rng, ulong seed)
        {
            unchecked
            {
                seed = (seed ^ 0x1C69B3F74AC4AE35UL) * 0x3C79AC492BA7B653UL; // an XLCG
                rng.StateA = seed ^ ~0xC6BC279692B5C323UL;
                seed ^= seed >> 32;
                rng.StateB = seed ^ 0xD3833E804F4C574BUL;
                seed *= 0xBEA225F9EB34556DUL;                               // MX3 unary hash
                seed ^= seed >> 29;
                rng.StateC = seed ^ ~0xD3833E804F4C574BUL;                  // updates are spread across the MX3 hash
                seed *= 0xBEA225F9EB34556DUL;
                seed ^= seed >> 32;
                rng.StateD = seed ^ 0xC6BC279692B5C323UL;
                seed *= 0xBEA225F9EB34556DUL;
                seed ^= seed >> 29;
                rng.StateE = seed;
                seed ^= (seed * seed) | 7UL;
                seed ^= seed >> 27;
                rng.StateF = seed ^ 0xBEA225F9EB34556DUL;
            }
        }

        /// <summary>
        /// Sets the state completely to the given six state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as setting StateA, StateB, StateC, StateD, StateE, and StateF as a group.
        /// </remarks>
        /// <param name="stateA">The first state; can be any ulong.</param>
        /// <param name="stateB">The second state; can be any ulong.</param>
        /// <param name="stateC">The third state; can be any ulong.</param>
        /// <param name="stateD">The fourth state; can be any ulong.</param>
        /// <param name="stateE">The fifth state; can be any ulong.</param>
        /// <param name="stateF">The sixth state; must be odd and may be changed to satisfy a constraint.</param>
        public override void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD, ulong stateE, ulong stateF)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
            StateE = stateE;
            StateF = stateF;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            ulong fd = StateD;
            ulong fe = StateE;
            unchecked
            {
                StateA = fa + 0x9E3779B97F4A7C15L;
                StateB = fa ^ fe;
                StateC = fb + fd;
                StateD = fc.RotateLeft(52);
                return StateE = fb - fc;
            }
        }

        /// <inheritdoc />
        public override float NextSparseFloat()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            ulong fd = StateD;
            ulong fe = StateE;
            unchecked
            {
                StateA = fa + 0x9E3779B97F4A7C15L;
                StateB = fa ^ fe;
                StateC = fb + fd;
                StateD = fc.RotateLeft(52);
                return BitConverter.Int32BitsToSingle((int)((StateE = fb - fc) >> 41) | 0x3F800000) - 1f;
            }
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            ulong fd = StateD;
            ulong fe = StateE;
            unchecked
            {
                StateA = fa + 0x9E3779B97F4A7C15L;
                StateB = fa ^ fe;
                StateC = fb + fd;
                StateD = fc.RotateLeft(52);
                return BitConverter.Int64BitsToDouble((long)((StateE = fb - fc) >> 12) | 0x3FF0000000000000L) - 1.0;
            }
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            unchecked
            {
                ulong fb = StateB;
                ulong fc = StateC;
                ulong fd = StateD;
                ulong fe = StateE;
                StateA -= 0x9E3779B97F4A7C15L;
                StateC = fd.RotateRight(52);
                StateB = StateC + fe;
                StateD = fc - StateB;
                StateE = fb ^ StateA;
                return fe;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new TraceRandom(StateA, StateB, StateC, StateD, StateE, StateF);
    }
}
