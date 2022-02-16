using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 4 states, more here later. This one has a good guaranteed minimum period, (2 to the 65) - 2.
    /// </summary>
    public sealed class StrangerRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "StrR" .
        /// </summary>
        public override string DefaultTag => "StrR";

        private ulong _a, _b;
        /// <summary>
        /// The first state; can be any ulong except 0.
        /// </summary>
        public ulong StateA { get => _a; set => _a = value == 0UL ? 0xD3833E804F4C574BUL : value; }
        /// <summary>
        /// The second state; can be any ulong except 0.
        /// </summary>
        public ulong StateB { get => _b; set => _b = value == 0UL ? 0x790B300BF9FE738FUL : value; }

        /// <summary>
        /// The third state; can be any ulong. If this has just been set to some value, then the next call to
        /// <see cref="NextULong()"/> will return that value as-is. Later calls will be more random.
        /// </summary>
        public ulong StateC { get; set; }
        /// <summary>
        /// The fourth state; can be any ulong.
        /// </summary>
        public ulong StateD { get; set; }

        /// <summary>
        /// Used to get a value for stateB that is very distant from the given state in a xorshift generator sequence.
        /// </summary>
        /// <remarks>
        /// This produces a state that is equivalent to stepping backwards 7046029254386353131 steps from state in the xorshift generator
        /// sequence this uses for states A and B.
        /// </remarks>
        /// <param name="state">Must be non-zero, but can otherwise be any ulong.</param>
        /// <returns>Another state that will be very distant in a xorshift generator sequence, suitable as a "StateB" when state is "StateA."</returns>
        public static ulong Jump(ulong state)
        {
            unchecked
            {
                ulong poly = 0x5556837749D9A17FUL;
                ulong val = 0L, b = 1L;
                for (int i = 0; i < 63; i++, b <<= 1)
                {
                    if ((poly & b) != 0L) val ^= state;
                    state ^= state << 7;
                    state ^= state >> 9;
                }
                return val;
            }
        }

        /// <summary>
        /// Creates a new StrangerRandom with a random state.
        /// </summary>
        public StrangerRandom()
        {
            StateA = MakeSeed();
            _b = Jump(_a);
            StateC = MakeSeed();
            StateD = MakeSeed();
        }

        /// <summary>
        /// Creates a new StrangerRandom with the given seed; any ulong value is permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="Seed(ulong)">Seed(ulong)</see> to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public StrangerRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new FourWheelRandom with the given stateA to be used to get values for stateA and stateB, plus the given
        /// stateC and stateD that will be used verbatim.
        /// </summary>
        /// <remarks>
        /// For stateA, 0 is not permitted, but all other ulongs are. For stateC and stateD, all ulong values are permitted.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        /// <param name="stateD">Any ulong.</param>
        public StrangerRandom(ulong stateA, ulong stateC, ulong stateD)
        {
            StateA = stateA;
            _b = Jump(_a);
            StateC = stateC;
            StateD = stateD;
        }

        /// <summary>
        /// Creates a new FourWheelRandom with the given four states.
        /// </summary>
        /// <remarks>
        /// 0 is not permitted for stateA or stateB, but all states are otherwise used verbatim.
        /// </remarks>
        /// <param name="stateA">Any ulong except 0.</param>
        /// <param name="stateB">Any ulong except 0.</param>
        /// <param name="stateC">Any ulong.</param>
        /// <param name="stateD">Any ulong.</param>
        public StrangerRandom(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
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
        /// by <see cref="NextULong()">NextUlong()</see> (because stateC is guaranteed to be different for every different seed).
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(StrangerRandom rng, ulong seed)
        {
            rng.StateA = seed ^ 0xFA346CBFD5890825UL;
            if (rng.StateA == 0UL) rng.StateA = 0xD3833E804F4C574BUL;
            rng._b = Jump(rng._a);
            rng.StateC = Jump(rng._b - seed);
            rng.StateD = Jump(rng.StateC + 0xC6BC279692B5C323UL);
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
        /// <summary>
        /// Sets the A and B states in this using the giveb stateA, and sets states C and D to stateC and stateD verbatim.
        /// This uses state A verbatim unless it is 0, and sets state B based on state A so it is very separated in its sequence.
        /// </summary>
        /// <param name="stateA">Can be any ulong except 0.</param>
        /// <param name="stateC">Can be any ulong.</param>
        /// <param name="stateD">Can be any ulong.</param>
        public override void SetState(ulong stateA, ulong stateC, ulong stateD)
        {
            StateA = stateA;
            _b = Jump(_a);
            StateC = stateC;
            StateD = stateD;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            ulong fa = _a;
            ulong fb = _b;
            ulong fc = StateC;
            ulong fd = StateD;
            unchecked
            {
                _a = fb ^ fb << 7;
                _b = fa ^ fa >> 9;
                StateC = fd.RotateLeft(39) - fb;
                StateD = fa - fc + 0xC6BC279692B5C323UL;
                return fc;
            }
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            ulong fa = _a;
            ulong fb = _b;
            ulong fc = StateC;
            ulong fd = StateD;
            unchecked
            {
                _a = fb ^ fb << 7;
                _b = fa ^ fa >> 9;
                StateC = fd.RotateLeft(39) - fb;
                StateD = fa - fc + 0xC6BC279692B5C323UL;
            }
            return BitConverter.Int64BitsToDouble((long)(fc >> 12) | 0x3FF0000000000000L) - 1.0;
        }

        /// <inheritdoc/>
        public override ulong PreviousULong()
        {
            ulong fa = _a;
            ulong fb = _b;
            ulong fc = StateC;
            ulong fd = StateD;
            ulong t = fb ^ fb >> 9;
            t ^= t >> 18;
            t ^= t >> 36;
            ulong m = fa ^ fa << 7;
            m ^= m << 14;
            m ^= m << 28;
            m ^= m << 56;
            StateA = t;
            StateB = m;
            StateC = t - fd + 0xC6BC279692B5C323UL;
            StateD = (fc + m).RotateRight(39);
            t = m ^ m >> 9;
            t ^= t >> 18;
            t ^= t >> 36;
            return t - StateD + 0xC6BC279692B5C323UL;
        }


        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new StrangerRandom(StateA, StateB, StateC, StateD);
    }
}
