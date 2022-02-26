using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 3 states, with no guarantee of a minimum period but a statistically-likely high actual period.
    /// </summary>
    /// <remarks>
    /// This is like <see cref="RomuTrioRandom"/>, which also has three states and no guarantee of a minimum period for a given stream,
    /// but unlike RomuTrioRandom, this won't get stuck in a perpetual-zero state if all three states are 0. It may be faster or slower
    /// than RomuTrioRandom, though it is usually slower by a fraction of a nanosecond.
    /// <br />
    /// This supports <see cref="PreviousULong()"/> but not <see cref="IEnhancedRandom.Skip(ulong)"/>.
    /// </remarks>
    public sealed class TricycleRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "TriR" .
        /// </summary>
        public override string DefaultTag => "TriR";

        /// <summary>
        /// The first state; can be any ulong.
        /// </summary>
        /// <remarks> this has just been set to some value, then the next call to<see cref="NextULong">NextULong</see> will return that value as-is. Later calls will be more random.</remarks>

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
        /// Creates a new TricycleRandom with a random state.
        /// </summary>
        public TricycleRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
        }

        /// <summary>
        /// Creates a new TricycleRandom with the given seed; any ulong value is permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="Seed(ulong)">Seed(ulong)</see> to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public TricycleRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new RomuTrioRandom with the given three states; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        public TricycleRandom(ulong stateA, ulong stateB, ulong stateC)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
        }

        /// <summary>
        /// This generator has 3 ulong states, so this returns 3.
        /// </summary>
        public override int StateCount => 3;
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
        /// <remarks>The value for selection should be between 0 and 2, inclusive; if it is any other value this gets state C as if 2 was given.</remarks>
        /// <param name="selection">used to select which state variable to get; generally 0, 1, or 2.</param>
        /// <returns>The value of the selected state.</returns>
        public override ulong SelectState(int selection)
        {
            return selection switch
            {
                0 => StateA,
                1 => StateB,
                _ => StateC
            };
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value, as-is.
        /// </summary>
        /// <remarks>
        /// Selections 0, 1, and 2 refer to states A, B, and C,  and if the selection is anything else, this treats it as 2 and sets stateC.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0, 1, or 2.</param>
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
                default:
                    StateC = value;
                    break;
            }
        }

        /// <summary>
        /// This initializes all states of the generator to different pseudo-random values based on the given seed.
        /// </summary>
        /// <remarks>
        /// (2 to the 64) possible initial generator states can be produced here, all with a different
        /// first value returned by <see cref="NextULong()">NextULong()</see> (because stateA is guaranteed to be
        /// different for every different seed).
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(TricycleRandom rng, ulong seed)
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
                x = (seed + 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateC = x ^ x >> 27;
            }
        }

        /// <summary>
        /// Sets the state completely to the given three state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as setting StateA, setStateB, and StateC as a group.
        /// You may want to call <see cref="NextULong()">NextUlong()</see> a few times after setting the states like this, unless
        /// the value for stateA (in particular) is already adequately random; the first call to NextULong(), if it is made immediately after calling this, will return stateA as-is.
        /// </remarks>
        /// <param name="stateA">The first state; can be any ulong.</param>
        /// <param name="stateB">The second state; can be any ulong.</param>
        /// <param name="stateC">The third state; can be any ulong</param>
        public override void SetState(ulong stateA, ulong stateB, ulong stateC)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            unchecked
            {
                StateA = 0xD1342543DE82EF95UL * fc;
                StateB = fa ^ fb ^ fc;
                StateC = fb.RotateLeft(41) + 0xC6BC279692B5C323UL;
            }
            return fa;
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;

            StateC = 0x572B5EE77A54E3BDUL * fa;
            StateB = BitExtensions.RotateRight(fc - 0xC6BC279692B5C323UL, 41);
            StateA = fb ^ StateB ^ StateC;
            return StateB ^ 0x572B5EE77A54E3BDUL * StateA ^ BitExtensions.RotateRight(StateC - 0xC6BC279692B5C323UL, 41);
        }

        /// <inheritdoc />
        public override float NextSparseFloat()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            StateA = 0xD1342543DE82EF95UL * fc;
            StateB = fa ^ fb ^ fc;
            StateC = fb.RotateLeft(41) + 0xC6BC279692B5C323UL;
            return BitConverter.Int32BitsToSingle((int)(fa >> 41) | 0x3F800000) - 1f;
        }


        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            StateA = 0xD1342543DE82EF95UL * fc;
            StateB = fa ^ fb ^ fc;
            StateC = fb.RotateLeft(41) + 0xC6BC279692B5C323UL;
            return BitConverter.Int64BitsToDouble((long)(fa >> 12) | 0x3FF0000000000000L) - 1.0;
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new TricycleRandom(StateA, StateB, StateC);
    }
}
