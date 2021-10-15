using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    /// <summary>
    /// It's an ARandom with 4 states, more here later. This one has a good guaranteed minimum period, (2 to the 65) - 2.
    /// </summary>
    [Serializable]
    public class StrangerRandom : ARandom, IEquatable<StrangerRandom?>
    {
        static StrangerRandom()
        {
            RegisterTag("StrR", new StrangerRandom(1UL, 1UL, 1UL, 1UL));
        }
        private ulong _a, _b;
        /**
         * The first state; can be any long except 0.
         */
        public ulong stateA { get => _a; set => _a = value == 0UL ? 0xD3833E804F4C574BUL : value; }
        /**
         * The second state; can be any long except 0.
         */
        public ulong stateB { get => _b; set => _b = value == 0UL ? 0x790B300BF9FE738FUL : value; }
        /**
         * The third state; can be any long. If this has just been set to some value, then the next call to
         * {@link #nextLong()} will return that value as-is. Later calls will be more random.
         */
        public ulong stateC { get; set; }
        /**
         * The fourth state; can be any long.
         */
        public ulong stateD { get; set; }

        public static ulong Jump(ulong state)
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
        /**
         * Creates a new StrangerRandom with a random state.
         */
        public StrangerRandom()
        {
            stateA = MakeSeed();
            _b = Jump(_a);
            stateC = MakeSeed();
            stateD = MakeSeed();
        }

        /**
         * Creates a new StrangerRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #setSeed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public StrangerRandom(ulong seed)
        {
            Seed(seed);
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
            SetState(stateA, stateC, stateD);
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
            this.stateA = stateA;
            this.stateB = stateB;
            this.stateC = stateC;
            this.stateD = stateD;
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
        /// This does not support <see cref="IRandom.Skip(ulong)"/>.
        /// </summary>
        public override bool SupportsSkip => false;
        /// <summary>
        /// This does not support <see cref="IRandom.PreviousUlong()"/>.
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
                    return stateA;
                case 1:
                    return stateB;
                case 2:
                    return stateC;
                default:
                    return stateD;
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
                    stateA = value;
                    break;
                case 1:
                    stateB = value;
                    break;
                case 2:
                    stateC = value;
                    break;
                default:
                    stateD = value;
                    break;
            }
        }

        /**
         * This initializes all 4 states of the generator to random values based on the given seed.
         * (2 to the 64) possible initial generator states can be produced here, all with a different
         * first value returned by {@link #nextLong()} (because {@code stateC} is guaranteed to be
         * different for every different {@code seed}).
         * @param seed the initial seed; may be any long
         */
        public override void Seed(ulong seed)
        {
            stateA = seed ^ 0xFA346CBFD5890825UL;
            if (stateA == 0L) stateA = 0xD3833E804F4C574BUL;
            _b = Jump(_a);
            stateC = Jump(seed ^ 0x05CB93402A76F7DAUL);
            stateD = ~seed;
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
            this.stateA = stateA;
            this.stateB = stateB;
            this.stateC = stateC;
            this.stateD = stateD;
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
            this.stateA = stateA;
            _b = Jump(_a);
            this.stateC = stateC;
            this.stateD = stateD;
        }

        public override ulong NextUlong()
        {
            ulong fa = _a;
            ulong fb = _b;
            ulong fc = stateC;
            ulong fd = stateD;
            _a = fb ^ fb << 7;
            _b = fa ^ fa >> 9;
            fd.RotateLeftInPlace(39);
            stateC = fd - fb;
            stateD = fa - fc + 0xC6BC279692B5C323L;
            return fc;
        }

        public override IRandom Copy() => new StrangerRandom(stateA, stateB, stateC, stateD);
        public override string StringSerialize() => $"#StrR`{stateA:X}~{stateB:X}~{stateC:X}~{stateD:X}`";
        public override IRandom StringDeserialize(string data)
        {
            int idx = data.IndexOf('`');
            stateA = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateB = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateC = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateD = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (      data.IndexOf('`', idx + 1))), 16);
            return this;
        }

        public override bool Equals(object? obj) => Equals(obj as StrangerRandom);
        public bool Equals(StrangerRandom? other) => other != null && stateA == other.stateA && stateB == other.stateB && stateC == other.stateC && stateD == other.stateD;
        public override int GetHashCode() => HashCode.Combine(stateA, stateB, stateC, stateD);

        public static bool operator ==(StrangerRandom? left, StrangerRandom? right) => EqualityComparer<StrangerRandom>.Default.Equals(left, right);
        public static bool operator !=(StrangerRandom? left, StrangerRandom? right) => !(left == right);
    }
}
