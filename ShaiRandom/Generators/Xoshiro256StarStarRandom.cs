using System;
using System.Collections.Generic;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 4 states, implementing a known-rather-good algorithm, more here later.
    /// </summary>
    [Serializable]
    public class Xoshiro256StarStarRandom : AbstractRandom, IEquatable<Xoshiro256StarStarRandom?>
    {
        /// <summary>
        /// The identifying tag here is "XSSR" .
        /// </summary>
        public override string Tag => "XSSR";

        static Xoshiro256StarStarRandom()
        {
            RegisterTag(new Xoshiro256StarStarRandom(1UL, 1UL, 1UL, 1UL));
        }
        /**
         * The first state; can be any long except that the whole state must not all be 0.
         */
        public ulong StateA { get; set; }
        /**
         * The second state; can be any long except that the whole state must not all be 0.
         * This is the state that is scrambled and returned; if it is 0 before a number
         * is generated, then the next number will be 0.
         */
        public ulong StateB { get; set; }
        /**
         * The third state; can be any long except that the whole state must not all be 0.
         */
        public ulong StateC { get; set; }
        private ulong _d;
        /**
         * The fourth state; can be any long except that the whole state must not all be 0.
         * If all other states are 0, and this would be set to 0,
         * then this is instead set to 0xFFFFFFFFFFFFFFFFUL.
         */
        public ulong StateD
        {
            get => _d;
            set => _d = (StateA | StateB | StateC | value) == 0UL ? 0xFFFFFFFFFFFFFFFFUL : value;
        }


        /**
         * Creates a new Xoshiro256StarStarRandom with a random state.
         */
        public Xoshiro256StarStarRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
            StateD = MakeSeed();
        }

        /**
         * Creates a new Xoshiro256StarStarRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #setSeed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public Xoshiro256StarStarRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /**
         * Creates a new Xoshiro256StarStarRandom with the given four states; all {@code long} values are permitted.
         * These states will be used verbatim.
         * @param stateA any {@code long} value
         * @param stateB any {@code long} value
         * @param stateC any {@code long} value
         * @param stateD any {@code long} value
         */
        public Xoshiro256StarStarRandom(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
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
         * first value returned by {@link #nextLong()} (because {@code stateB} is guaranteed to be
         * different for every different {@code seed}).
         * @param seed the initial seed; may be any long
         */
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(Xoshiro256StarStarRandom rng, ulong seed)
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
                rng._d = x ^ x >> 27;
            }
        }

        /**
         * Sets the state completely to the given four state variables.
         * This is the same as calling {@link #setStateA(long)}, {@link #setStateB(long)},
         * {@link #setStateC(long)}, and {@link #setStateD(long)} as a group. You may want
         * to call {@link #nextLong()} a few times after setting the states like this, unless
         * the value for stateB (in particular) is already adequately random. If all parameters
         * are 0 here, this will assign 0xFFFFFFFFFFFFFFFFUL to stateD and 0 to the rest.
         * @param stateA the first state; can be any long
         * @param stateB the second state; can be any long
         * @param stateC the third state; can be any long
         * @param stateD the fourth state; can be any long
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
            unchecked
            {
                ulong result = (StateB * 5UL).RotateLeft(7) * 9UL;
                ulong t = StateB << 17;
                StateC ^= StateA;
                _d ^= StateB;
                StateB ^= StateC;
                StateA ^= _d;
                StateC ^= t;
                _d.RotateLeftInPlace(45);
                return result;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new Xoshiro256StarStarRandom(StateA, StateB, StateC, StateD);

        /// <inheritdoc />
        public override string StringSerialize() => $"#XSSR`{StateA:X}~{StateB:X}~{StateC:X}~{StateD:X}`";

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

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as Xoshiro256StarStarRandom);

        /// <inheritdoc />
        public bool Equals(Xoshiro256StarStarRandom? other) => other != null && StateA == other.StateA && StateB == other.StateB && StateC == other.StateC && StateD == other.StateD;

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(StateA, StateB, StateC, _d);

        public static bool operator ==(Xoshiro256StarStarRandom? left, Xoshiro256StarStarRandom? right) => EqualityComparer<Xoshiro256StarStarRandom>.Default.Equals(left, right);
        public static bool operator !=(Xoshiro256StarStarRandom? left, Xoshiro256StarStarRandom? right) => !(left == right);
    }
}
