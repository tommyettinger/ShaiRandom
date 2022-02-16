using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 4 states, built around a "chaotic" construction with no guarantee of a minimum period, but likely to be a very long one.
    /// </summary>
    /// <remarks>
    /// This is very fast... on the JVM. It isn't nearly as fast on .NET (any version), because smaller state sizes do better here. It's still fairly high-quality,
    /// with only one known failing test, and it only failed after 2 exabytes of data were processed (an extinction-saturation test).
    /// </remarks>
    public sealed class FourWheelRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "FoWR" .
        /// </summary>
        public override string DefaultTag => "FoWR";
        
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
        /// <remarks>If this has just been set to some value, then the next call to <see cref="NextULong()">NextUlong()</see> will return that value as-is. Later calls will be more random.</remarks>
        public ulong StateD { get; set; }

        /// <summary>
        /// Creates a new FourWheelRandom with a random state.
        /// </summary>
        public FourWheelRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
            StateD = MakeSeed();
        }

        /// <summary>
        /// Creates a new FourWheelRandom with the given seed; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="SetSeed(FourWheelRandom, ulong)">SetSeed(FourWheelRandom, ulong)</see>, to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public FourWheelRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new FourWheelRandom with the given four states; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        /// <param name="stateD">Any ulong.</param>
        public FourWheelRandom(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
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
        /// <remarks>
        /// (2 to the 64) possible initial generator states can be produced here, all with a different first value returned
        /// by <see cref="NextULong()">NextUlong()</see> (because stateD is guaranteed to be different for every different seed).
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(FourWheelRandom rng, ulong seed)
        {
            unchecked
            {
                ulong x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateA = x ^ x >> 27;
                x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateB = x ^ x >> 27;
                x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateC = x ^ x >> 27;
                x = (seed + 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateD = x ^ x >> 27;
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
                StateA = 0xD1342543DE82EF95UL * fd;
                StateB = fa + 0xC6BC279692B5C323UL;
                StateC = fb.RotateLeft(47) - fd;
                StateD = fb ^ fc;
            }
            return fd;
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
                StateA = 0xD1342543DE82EF95UL * fd;
                StateB = fa + 0xC6BC279692B5C323UL;
                StateC = fb.RotateLeft(47) - fd;
                StateD = fb ^ fc;
            }
            return BitConverter.Int64BitsToDouble((long)(fd >> 12) | 0x3FF0000000000000L) - 1.0;

        }
        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            ulong fd = StateD;
            unchecked
            {
                StateD = 0x572B5EE77A54E3BDUL * fa;
                StateA = fb - 0xC6BC279692B5C323UL;
                StateB = (fc + StateD).RotateRight(47);
                StateC = fd ^ StateB;
                return 0x572B5EE77A54E3BDUL * StateA;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new FourWheelRandom(StateA, StateB, StateC, StateD);
    }
}
