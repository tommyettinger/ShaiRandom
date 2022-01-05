using System;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 4 states, more here later. This one has a good guaranteed minimum period, (2 to the 65) - 2.
    /// </summary>
    public class StrangerRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "StrR" .
        /// </summary>
        public override string Tag => "StrR";

        static StrangerRandom()
        {
            RegisterTag(new StrangerRandom(1UL, 1UL, 1UL, 1UL));
        }
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
        /**
         * Creates a new StrangerRandom with a random state.
         */
        public StrangerRandom()
        {
            StateA = MakeSeed();
            _b = Jump(_a);
            StateC = MakeSeed();
            StateD = MakeSeed();
        }

        /**
         * Creates a new StrangerRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #setSeed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public StrangerRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /**
         * Creates a new StrangerRandom with the given stateA to be used to get values for stateA and stateB, plus the given
         * stateC and stateD that will be used verbatim. For stateA, 0 is not permitted, but all other ulongs are.
         * For stateC and stateD, all {@code long} values are permitted.
         * @param stateA any {@code ulong} value
         * @param stateC any {@code ulong} value
         * @param stateD any {@code ulong} value
         */
        public StrangerRandom(ulong stateA, ulong stateC, ulong stateD)
        {
            StateA = stateA;
            _b = Jump(_a);
            StateC = stateC;
            StateD = stateD;
        }

        /**
         * Creates a new StrangerRandom with the given four states; 0 is not permitted for stateA or stateB, but
         * all states are otherwise used verbatim.
         * @param stateA any {@code long} value except 0
         * @param stateB any {@code long} value except 0
         * @param stateC any {@code long} value
         * @param stateD any {@code long} value
         */
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
        /// This does not support <see cref="IEnhancedRandom.PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => false;
        /**
         * Gets the state determined by {@code selection}, as-is. The value for selection should be
         * between 0 and 3, inclusive; if it is any other value this gets state D as if 3 was given.
         * @param selection used to select which state variable to get; generally 0, 1, 2, or 3
         * @return the value of the selected state
         */
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

        /**
         * Sets one of the states, determined by {@code selection}, to {@code value}, as-is.
         * Selections 0, 1, 2, and 3 refer to states A, B, C, and D,  and if the selection is anything
         * else, this treats it as 3 and sets stateD.
         * @param selection used to select which state variable to set; generally 0, 1, 2, or 3
         * @param value the exact value to use for the selected state, if valid
         */
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

        /**
         * This initializes all 4 states of the generator to random values based on the given seed.
         * (2 to the 64) possible initial generator states can be produced here, all with a different
         * first value returned by {@link #nextULong()} (because {@code stateC} is guaranteed to be
         * different for every different {@code seed}).
         * @param seed the initial seed; may be any long
         */
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(StrangerRandom rng, ulong seed)
        {
            rng.StateA = seed ^ 0xFA346CBFD5890825UL;
            if (rng.StateA == 0UL) rng.StateA = 0xD3833E804F4C574BUL;
            rng._b = Jump(rng._a);
            rng.StateC = Jump(rng._b - seed);
            rng.StateD = Jump(rng.StateC + 0xC6BC279692B5C323UL);
        }

        /**
         * Sets the state completely to the given four state variables.
         * This is the same as calling {@link #setStateA(long)}, {@link #setStateB(long)},
         * {@link #setStateC(long)}, and {@link #setStateD(long)} as a group. You may want
         * to call {@link #nextLong()} a few times after setting the states like this, unless
         * the value for stateC (in particular) is already adequately random; the first call
         * to {@link #nextLong()}, if it is made immediately after calling this, will return {@code stateD} as-is.
         * @param stateA the first state; can be any long
         * @param stateB the second state; can be any long
         * @param stateC the third state; can be any long
         * @param stateD the fourth state; this will be returned as-is if the next call is to {@link #nextLong()}
         */
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
        public override IEnhancedRandom Copy() => new StrangerRandom(StateA, StateB, StateC, StateD);

        /// <inheritdoc />
        public override string StringSerialize() => $"#StrR`{StateA:X}~{StateB:X}~{StateC:X}~{StateD:X}`";

        /// <inheritdoc />
        public override IEnhancedRandom StringDeserialize(string data)
        {
            int idx = data.IndexOf('`');
            StateA = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            StateB = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            StateC = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            StateD = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (      data.IndexOf('`', idx + 1))), 16);
            return this;
        }
    }
}
