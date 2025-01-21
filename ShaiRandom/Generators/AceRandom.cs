using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 5 states, built around a "medium-chaotic" construction with a guaranteed minimum period of 2 to the 64, but likely much longer.
    /// </summary>
    /// <remarks>
    /// Tommy Ettinger:
    /// This is "medium-chaotic" because it has five states and performs five math operations per update (two adds, a bitwise rotation, a XOR, and a
    /// subtraction), with a good compiler able to handle those as instruction-level parallel operations.
    ///
    /// This generator is about as fast as WhiskerRandom (on the JVM), plus it offers a strong period guarantee, and has been tested just as thoroughly.
    /// </remarks>
    public sealed class AceRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "AceR" .
        /// </summary>
        public override string DefaultTag => "AceR";

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
        /// Creates a new AceRandom with a random state.
        /// </summary>
        public AceRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
            StateD = MakeSeed();
            StateE = MakeSeed();
        }

        /// <summary>
        /// Creates a new AceRandom with the given seed; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="SetSeed(AceRandom, ulong)">SetSeed(AceRandom, ulong)</see>, to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public AceRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new AceRandom with the given four states; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        /// <param name="stateD">Any ulong.</param>
        /// <param name="stateE">Any ulong.</param>
        public AceRandom(ulong stateA, ulong stateB, ulong stateC, ulong stateD, ulong stateE)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
            StateE = stateE;
        }

        /// <summary>
        /// This generator has 5 ulong states, so this returns 5.
        /// </summary>
        public override int StateCount => 5;
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
                default:
                    return StateE;
            }
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value, as-is.
        /// </summary>
        /// <remarks>
        /// Selections 0, 1, 2, 3, and 4 refer to states A, B, C, D, and E  and if the selection is anything else, this treats it as 4 and sets stateE.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0, 1, 2, 3, or 4.</param>
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
                default:
                    StateE = value;
                    break;
            }
        }

        /// <summary>
        /// Initializes all 4 states of the generator to random values based on the given seed.
        /// </summary>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(AceRandom rng, ulong seed)
        {
            unchecked
            {
                seed = (seed ^ 0x1C69B3F74AC4AE35L) * 0x3C79AC492BA7B653L; // an XLCG
                rng.StateA = seed ^ ~0xC6BC279692B5C323L;
                seed ^= seed >> 32;
                rng.StateB = seed ^ 0xD3833E804F4C574BL;
                seed *= 0xBEA225F9EB34556DL;                               // MX3 unary hash
                seed ^= seed >> 29;
                rng.StateC = seed ^ ~0xD3833E804F4C574BL;                      // updates are spread across the MX3 hash
                seed *= 0xBEA225F9EB34556DL;
                seed ^= seed >> 32;
                rng.StateD = seed ^ 0xC6BC279692B5C323L; ;
                seed *= 0xBEA225F9EB34556DL;
                seed ^= seed >> 29;
                rng.StateE = seed;
            }
        }

        /// <summary>
        /// Sets the state completely to the given five state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as setting StateA, StateB, StateC, StateD, and stateE as a group.
        /// </remarks>
        /// <param name="stateA">The first state; can be any ulong.</param>
        /// <param name="stateB">The second state; can be any ulong.</param>
        /// <param name="stateC">The third state; can be any ulong.</param>
        /// <param name="stateD">The fourth state; can be any ulong.</param>
        /// <param name="stateE">The fifth state; can be any ulong.</param>
        public override void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD, ulong stateE)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
            StateE = stateE;
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
        public override IEnhancedRandom Copy() => new AceRandom(StateA, StateB, StateC, StateD, StateE);
    }
}
