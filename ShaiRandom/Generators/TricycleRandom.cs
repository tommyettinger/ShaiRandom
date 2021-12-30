using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    /// <summary>
    /// It's an AbstractRandom with 3 states, more here later.
    /// </summary>
    [Serializable]
    public class TricycleRandom : AbstractRandom, IEquatable<TricycleRandom?>
    {
        /// <summary>
        /// The identifying tag here is "TriR" .
        /// </summary>
        public override string Tag => "TriR";

        static TricycleRandom()
        {
            RegisterTag(new TricycleRandom(1UL, 1UL, 1UL));
        }
        /**
         * The first state; can be any long. If this has just been set to some value, then the next call to
         * {@link #nextLong()} will return that value as-is. Later calls will be more random.
         */
        public ulong StateA { get; set; }
        /**
         * The second state; can be any long.
         */
        public ulong StateB { get; set; }
        /**
         * The third state; can be any long.
         */
        public ulong StateC { get; set; }

        /**
         * Creates a new TricycleRandom with a random state.
         */
        public TricycleRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
        }

        /**
         * Creates a new TricycleRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #Seed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public TricycleRandom(ulong seed)
        {
            Seed(seed);
        }

        /**
         * Creates a new TricycleRandom with the given three states; all {@code long} values are permitted.
         * These states will be used verbatim.
         * @param stateA any {@code long} value
         * @param stateB any {@code long} value
         * @param stateC any {@code long} value
         */
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
        /**
         * Gets the state determined by {@code selection}, as-is. The value for selection should be
         * between 0 and 2, inclusive; if it is any other value this gets state C as if 2 was given.
         * @param selection used to select which state variable to get; generally 0, 1, or 2
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
                default:
                    return StateC;
            }
        }

        /**
         * Sets one of the states, determined by {@code selection}, to {@code value}, as-is.
         * Selections 0, 1, and 2 refer to states A, B, and C, and if the selection is anything
         * else, this treats it as 2 and sets stateC.
         * @param selection used to select which state variable to set; generally 0, 1, or 2
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
                default:
                    StateC = value;
                    break;
            }
        }

        /**
         * This initializes all 3 states of the generator to different random values based on the given seed.
         * (2 to the 64) possible initial generator states can be produced here, all with a different
         * first value returned by {@link #nextLong()} (because {@code stateA} is guaranteed to be
         * different for every different {@code seed}).
         * @param seed the initial seed; may be any long
         */
        public override void Seed(ulong seed)
        {
            unchecked
            {
                ulong x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                StateA = x ^ x >> 27;
                x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                StateB = x ^ x >> 27;
                x = (seed + 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                StateC = x ^ x >> 27;
            }
        }

        /**
         * Sets the state completely to the given three state variables.
         * This is the same as calling {@link #setStateA(long)}, {@link #setStateB(long)},
         * and {@link #setStateC(long)} as a group. You may want
         * to call {@link #nextLong()} a few times after setting the states like this, unless
         * the value for stateA (in particular) is already adequately random; the first call
         * to {@link #nextLong()}, if it is made immediately after calling this, will return {@code stateA} as-is.
         * @param stateA the first state; this will be returned as-is if the next call is to {@link #nextLong()}
         * @param stateB the second state; can be any long
         * @param stateC the third state; can be any long
         */
        public override void SetState(ulong stateA, ulong stateB, ulong stateC)
        {
            this.StateA = stateA;
            this.StateB = stateB;
            this.StateC = stateC;
        }

        /// <inheritdoc />
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
        public override IEnhancedRandom Copy() => new TricycleRandom(StateA, StateB, StateC);

        /// <inheritdoc />
        public override string StringSerialize() => $"#TriR`{StateA:X}~{StateB:X}~{StateC:X}`";

        /// <inheritdoc />
        public override IEnhancedRandom StringDeserialize(string data)
        {
            int idx = data.IndexOf('`');
            StateA = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            StateB = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            StateC = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (      data.IndexOf('`', idx + 1))), 16);
            return this;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as TricycleRandom);

        /// <inheritdoc />
        public bool Equals(TricycleRandom? other) => other != null && StateA == other.StateA && StateB == other.StateB && StateC == other.StateC;

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(StateA, StateB, StateC);

        public static bool operator ==(TricycleRandom? left, TricycleRandom? right) => EqualityComparer<TricycleRandom>.Default.Equals(left, right);
        public static bool operator !=(TricycleRandom? left, TricycleRandom? right) => !(left == right);
    }
}
