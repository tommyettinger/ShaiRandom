using System;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 4 states, more here later.
    /// </summary>
    public class FourWheelRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "FoWR" .
        /// </summary>
        public override string Tag => "FoWR";

        static FourWheelRandom()
        {
            RegisterTag(new FourWheelRandom(1UL, 1UL, 1UL, 1UL));
        }
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

        /**
         * Creates a new FourWheelRandom with a random state.
         */
        public FourWheelRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
            StateD = MakeSeed();
        }

        /**
         * Creates a new FourWheelRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #setSeed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public FourWheelRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /**
         * Creates a new FourWheelRandom with the given four states; all {@code long} values are permitted.
         * These states will be used verbatim.
         * @param stateA any {@code long} value
         * @param stateB any {@code long} value
         * @param stateC any {@code long} value
         * @param stateD any {@code long} value
         */
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
         * first value returned by {@link #nextLong()} (because {@code stateD} is guaranteed to be
         * different for every different {@code seed}).
         * @param seed the initial seed; may be any long
         */
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

        /**
         * Sets the state completely to the given four state variables.
         * This is the same as calling {@link #setStateA(long)}, {@link #setStateB(long)},
         * {@link #setStateC(long)}, and {@link #setStateD(long)} as a group. You may want
         * to call {@link #nextLong()} a few times after setting the states like this, unless
         * the value for stateD (in particular) is already adequately random; the first call
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

        /// <inheritdoc />
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
        public override ulong PreviousULong()
        {
            ulong fa = StateA;
            ulong fb = StateB;
            ulong fc = StateC;
            unchecked
            {
                StateD = 0x572B5EE77A54E3BDUL * fa;
                StateA = fb - 0xC6BC279692B5C323UL;
                StateB = (fc + StateD).RotateRight(47);
                StateC = StateD ^ StateB;
                return 0x572B5EE77A54E3BDUL * StateA;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new FourWheelRandom(StateA, StateB, StateC, StateD);

        /// <inheritdoc />
        public override string StringSerialize() => $"#FoWR`{StateA:X}~{StateB:X}~{StateC:X}~{StateD:X}`";

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
