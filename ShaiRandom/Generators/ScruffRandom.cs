using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 4 states, built around a "medium-chaotic" construction with a guaranteed minimum period of 2 to the 64, but likely much longer.
    /// </summary>
    /// <remarks>
    /// Tommy Ettinger:
    /// This is "medium-chaotic" because it has four states and performs five math operations per update (a multiplication, a bitwise rotation, an add, a XOR, and a
    /// subtraction), with a good compiler able to handle those as instruction-level parallel operations.
    ///
    /// This generator isn't quite as fast as WhiskerRandom (on the JVM), but offers a strong period guarantee, and has been tested just as thoroughly.
    /// </remarks>
    public sealed class ScruffRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "ScrR" .
        /// </summary>
        public override string DefaultTag => "ScrR";
        
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
        /// Creates a new ScruffRandom with a random state.
        /// </summary>
        public ScruffRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
            StateD = MakeSeed();
        }

        /// <summary>
        /// Creates a new ScruffRandom with the given seed; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="SetSeed(ScruffRandom, ulong)">SetSeed(ScruffRandom, ulong)</see>, to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public ScruffRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new ScruffRandom with the given four states; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        /// <param name="stateD">Any ulong.</param>
        public ScruffRandom(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
        }

        /// <summary>
        /// This generator has 4 ulong states, so this returns 4.
        /// </summary>
        public override int StateCount => 4;
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
                default:
                    return StateD;
            }
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value, as-is.
        /// </summary>
        /// <remarks>
        /// Selections 0, 1, 2, and 3 refer to states A, B, C, and D,  and if the selection is anything else, this treats it as 3 and sets stateD.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0, 1, 2, or 3.</param>
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
                default:
                    StateD = value;
                    break;
            }
        }

        /// <summary>
        /// Initializes all 4 states of the generator to random values based on the given seed.
        /// </summary>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(ScruffRandom rng, ulong seed)
        {
            unchecked
            {
                seed ^= 0xEFA239AADFF080FFL; // somewhat-arbitrary choice from various harmonic numbers and their roots
                rng.StateA = seed;
                rng.StateC = ~seed;
                seed ^= seed >> 32;
                seed *= 0xBEA225F9EB34556DL;
                seed ^= seed >> 29;
                seed *= 0xBEA225F9EB34556DL;
                seed ^= seed >> 32;
                seed *= 0xBEA225F9EB34556DL;
                seed ^= seed >> 29;
                rng.StateB = seed ^ 0xC6BC279692B5C323L;
                rng.StateD = seed ^ ~0xC6BC279692B5C323L;
            }
        }

        /// <summary>
        /// Sets the state completely to the given four state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as setting StateA, StateB, StateC, and StateD as a group.
        /// You may want to call <see cref="NextULong()">NextUlong()</see> a few times after setting the states like this, unless
        /// the value for stateD (in particular) is already adequately random; the first call to NextULong(), if it is made immediately after calling this, will return stateD as-is.
        /// </remarks>
        /// <param name="stateA">The first state; can be any ulong.</param>
        /// <param name="stateB">The second state; can be any ulong.</param>
        /// <param name="stateC">The third state; can be any ulong.</param>
        /// <param name="stateD">The fourth state; this will be returned as-is if the next call is to NextULong().</param>
        public override void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            ulong fd = StateD;
            unchecked
            {
                StateA = fa + 0x9E3779B97F4A7C15L;
                StateB = fd * 0xD1342543DE82EF95L;
                StateC = fa ^ fb;
                StateD = fc.RotateLeft(21);
                return fd - fc;
            }
        }

        /// <inheritdoc />
        public override float NextSparseFloat()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            ulong fd = StateD;
            unchecked
            {
                StateA = fa + 0x9E3779B97F4A7C15L;
                StateB = fd * 0xD1342543DE82EF95L;
                StateC = fa ^ fb;
                StateD = fc.RotateLeft(21);
                return BitConverter.Int32BitsToSingle((int)(fd - fc >> 41) | 0x3F800000) - 1f;
            }
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            ulong fd = StateD;
            unchecked
            {
                StateA = fa + 0x9E3779B97F4A7C15L;
                StateB = fd * 0xD1342543DE82EF95L;
                StateC = fa ^ fb;
                StateD = fc.RotateLeft(21);
                return BitConverter.Int64BitsToDouble((long)(fd - fc >> 12) | 0x3FF0000000000000L) - 1.0;
            }
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            unchecked
            {
                ulong c = StateC;
                StateC = StateD.RotateRight(21);
                StateD = StateB * 0x572B5EE77A54E3BDUL; // modular multiplicative inverse of 0xD1342543DE82EF95L
                StateB = c ^ (StateA -= 0x9E3779B97F4A7C15UL);
                return StateD - StateC;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new ScruffRandom(StateA, StateB, StateC, StateD);
    }
}
