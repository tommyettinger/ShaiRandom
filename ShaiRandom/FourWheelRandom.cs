using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    /// <summary>
    /// It's an ARandom with 4 states, more here later.
    /// </summary>
    [Serializable]
    public class FourWheelRandom : ARandom, IEquatable<FourWheelRandom?>
    {
        static FourWheelRandom()
        {
            RegisterTag("FoWR", new FourWheelRandom(1UL, 1UL, 1UL, 1UL));
        }
        /**
         * The first state; can be any long.
         */
        public ulong stateA { get; set; }
        /**
         * The second state; can be any long.
         */
        public ulong stateB { get; set; }
        /**
         * The third state; can be any long.
         */
        public ulong stateC { get; set; }
        /**
         * The fourth state; can be any long. If this has just been set to some value, then the next call to
         * {@link #nextLong()} will return that value as-is. Later calls will be more random.
         */
        public ulong stateD { get; set; }

        /**
         * Creates a new FourWheelRandom with a random state.
         */
        public FourWheelRandom()
        {
            stateA = MakeSeed();
            stateB = MakeSeed();
            stateC = MakeSeed();
            stateD = MakeSeed();
        }

        /**
         * Creates a new FourWheelRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #setSeed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public FourWheelRandom(ulong seed)
        {
            Seed(seed);
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
        /// This supports <see cref="PreviousUlong()"/>.
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
         * first value returned by {@link #nextLong()} (because {@code stateD} is guaranteed to be
         * different for every different {@code seed}).
         * @param seed the initial seed; may be any long
         */
        public override void Seed(ulong seed)
        {
            ulong x = (seed += 0x9E3779B97F4A7C15UL);
            x ^= x >> 27;
            x *= 0x3C79AC492BA7B653L;
            x ^= x >> 33;
            x *= 0x1C69B3F74AC4AE35L;
            stateA = x ^ x >> 27;
            x = (seed += 0x9E3779B97F4A7C15L);
            x ^= x >> 27;
            x *= 0x3C79AC492BA7B653L;
            x ^= x >> 33;
            x *= 0x1C69B3F74AC4AE35L;
            stateB = x ^ x >> 27;
            x = (seed += 0x9E3779B97F4A7C15L);
            x ^= x >> 27;
            x *= 0x3C79AC492BA7B653L;
            x ^= x >> 33;
            x *= 0x1C69B3F74AC4AE35L;
            stateC = x ^ x >> 27;
            x = (seed + 0x9E3779B97F4A7C15L);
            x ^= x >> 27;
            x *= 0x3C79AC492BA7B653L;
            x ^= x >> 33;
            x *= 0x1C69B3F74AC4AE35L;
            stateD = x ^ x >> 27;
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
            this.stateA = stateA;
            this.stateB = stateB;
            this.stateC = stateC;
            this.stateD = stateD;
        }

        public override ulong NextUlong()
        {
            ulong fa = stateA;
            ulong fb = stateB;
            ulong fc = stateC;
            ulong fd = stateD;
            stateA = 0xD1342543DE82EF95L * fd;
            stateB = fa + 0xC6BC279692B5C323L;
            stateC = fb.RotateLeft(47) - fd;
            stateD = fb ^ fc;
            return fd;
        }

        public override ulong PreviousUlong()
        {
            ulong fa = stateA;
            ulong fb = stateB;
            ulong fc = stateC;
            stateD = 0x572B5EE77A54E3BDUL * fa;
            stateA = fb - 0xC6BC279692B5C323L;
            stateB = (fc + stateD).RotateRight(47);
            stateC = stateD ^ stateB;
            return 0x572B5EE77A54E3BDUL * stateA;
        }

        public override IRandom Copy() => new FourWheelRandom(stateA, stateB, stateC, stateD);
        public override string StringSerialize() => $"#FoWR`{stateA:X}~{stateB:X}~{stateC:X}~{stateD:X}`";
        public override IRandom StringDeserialize(string data)
        {
            int idx = data.IndexOf('`');
            stateA = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateB = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateC = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateD = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (      data.IndexOf('`', idx + 1))), 16);
            return this;
        }

        public override bool Equals(object? obj) => Equals(obj as FourWheelRandom);
        public bool Equals(FourWheelRandom? other) => other != null && stateA == other.stateA && stateB == other.stateB && stateC == other.stateC && stateD == other.stateD;
        public override int GetHashCode() => HashCode.Combine(stateA, stateB, stateC, stateD);

        public static bool operator ==(FourWheelRandom? left, FourWheelRandom? right) => EqualityComparer<FourWheelRandom>.Default.Equals(left, right);
        public static bool operator !=(FourWheelRandom? left, FourWheelRandom? right) => !(left == right);
    }
}
