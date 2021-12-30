using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    /// <summary>
    /// It's an AbstractRandom with 2 states, more here later.
    /// </summary>
    /// <remarks>
    /// This one supports <see cref="PreviousULong()"/>, but not <see cref="AbstractRandom.Skip(ulong)"/>.
    /// It's based on a permutation of an LCG, like PCG-Random by way of SplitMix64. A mizuchi is a mythological river dragon,
    /// and since this supports multiple streams (by changing StateB), the waterway theme seemed fitting.
    /// </remarks>
    [Serializable]
    public class MizuchiRandom : AbstractRandom, IEquatable<MizuchiRandom?>
    {
        /// <summary>
        /// The identifying tag here is "MizR" .
        /// </summary>
        public override string Tag => "MizR";

        static MizuchiRandom()
        {
            RegisterTag(new MizuchiRandom(1UL, 1UL));
        }
        /**
         * The first state; can be any ulong.
         */
        public ulong StateA { get; set; }
        private ulong _b;
        /**
         * The second state; can be any odd ulong (the last bit must be 1)
         */
        public ulong StateB { get
            {
                return _b;
            }
            set
            {
                _b = value | 1UL;
            }
        }

        /**
         * Creates a new MizuchiRandom with a random state.
         */
        public MizuchiRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
        }

        /**
         * Creates a new MizuchiRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #Seed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public MizuchiRandom(ulong seed)
        {
            Seed(seed);
        }

        /**
         * Creates a new MizuchiRandom with the given two states; all {@code long} values are permitted.
         * These states will be used verbatim.
         * @param stateA any {@code long} value
         * @param stateB any {@code long} value
         */
        public MizuchiRandom(ulong stateA, ulong stateB)
        {
            StateA = stateA;
            StateB = stateB;
        }

        /// <summary>
        /// This generator has 2 ulong states, so this returns 2.
        /// </summary>
        public override int StateCount => 2;
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
         * 0 or 1; if it is any other value this gets state A as if 0 was given.
         * @param selection used to select which state variable to get; generally 0 or 1
         * @return the value of the selected state
         */
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 1:
                    return StateB;
                default:
                    return StateA;
            }
        }

        /**
         * Sets one of the states, determined by {@code selection}, to {@code value}, as-is.
         * Selections 0 and 1 refer to states A and B, and if the selection is anything
         * else, this treats it as 0 and sets stateA.
         * @param selection used to select which state variable to set; generally 0 or 1
         * @param value the exact value to use for the selected state, if valid
         */
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 1:
                    StateB = value;
                    break;
                default:
                    StateA = value;
                    break;
            }
        }

        /**
         * This initializes both states of the generator to different random values based on the given seed.
         * (2 to the 64) possible initial generator states can be produced here.
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
                x = (seed + 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                _b = (x ^ x >> 27) | 1UL;
            }
        }

        /**
         * Sets the state completely to the given two state variables.
         * This is the same as calling {@link #setStateA(long)} and {@link #setStateB(long)}
         * as a group.
         * @param stateA the first state; can be any long
         * @param stateB the second state; can be any odd-number long
         */
        public override void SetState(ulong stateA, ulong stateB)
        {
            StateA = stateA;
            StateB = stateB;
        }

        /// <inheritdoc />
        public override ulong NextULong()
        {
            unchecked
            {
                ulong z = (StateA = StateA * 0xF7C2EBC08F67F2B5UL + StateB);
                z = (z ^ z >> 23 ^ z >> 47) * 0xAEF17502108EF2D9UL;
                return z ^ z >> 25;
            }
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            unchecked
            {
                ulong z = StateA;
                StateA = (StateA - StateB) * 0x09795DFF8024EB9DUL;
                z = (z ^ z >> 23 ^ z >> 47) * 0xAEF17502108EF2D9UL;
                return z ^ z >> 25;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new MizuchiRandom(StateA, StateB);

        /// <inheritdoc />
        public override string StringSerialize() => $"#MizR`{StateA:X}~{StateB:X}`";

        /// <inheritdoc />
        public override IEnhancedRandom StringDeserialize(string data)
        {
            int idx = data.IndexOf('`');
            StateA = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            StateB = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (      data.IndexOf('`', idx + 1))), 16);
            return this;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as MizuchiRandom);

        /// <inheritdoc />
        public bool Equals(MizuchiRandom? other) => other != null && StateA == other.StateA && StateB == other.StateB;

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(StateA, StateB);

        public static bool operator ==(MizuchiRandom? left, MizuchiRandom? right) => EqualityComparer<MizuchiRandom>.Default.Equals(left, right);
        public static bool operator !=(MizuchiRandom? left, MizuchiRandom? right) => !(left == right);
    }
}
