using System;
using System.Collections.Generic;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 1 state, more here later. This one supports <see cref="Skip(ulong)"/>.
    /// Note that this generator only returns each ulong result exactly once over its period.
    /// </summary>
    [Serializable]
    public class DistinctRandom : AbstractRandom, IEquatable<DistinctRandom?>
    {
        /// <summary>
        /// The identifying tag here is "DisR" .
        /// </summary>
        public override string Tag => "DisR";
        static DistinctRandom()
        {
            RegisterTag(new DistinctRandom(1UL));
        }
        /**
         * The first state; can be any ulong.
         */
        public ulong State { get; set; }

        /**
         * Creates a new DistinctRandom with a random state.
         */
        public DistinctRandom()
        {
            State = MakeSeed();
        }

        /**
         * Creates a new DistinctRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to used as-is for the one state here.
         * @param seed any {@code long} value
         */
        public DistinctRandom(ulong seed)
        {
            State = seed;
        }

        /// <summary>
        /// This generator has 1 ulong state, so this returns 1.
        /// </summary>
        public override int StateCount => 1;
        /// <summary>
        /// This supports <see cref="SelectState(int)"/>.
        /// </summary>
        public override bool SupportsReadAccess => true;
        /// <summary>
        /// This supports <see cref="SetSelectedState(int, ulong)"/>.
        /// </summary>
        public override bool SupportsWriteAccess => true;
        /// <summary>
        /// This supports <see cref="IEnhancedRandom.Skip(ulong)"/>.
        /// </summary>
        public override bool SupportsSkip => true;
        /// <summary>
        /// This supports <see cref="PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => true;

        /**
         * Gets the state, regardless of {@code selection}, as-is.
         * @param selection ignored
         * @return the value of the selected state
         */
        public override ulong SelectState(int selection)
        {
            return State;
        }

        /**
         * Sets the state, regardless of {@code selection}, to {@code value}, as-is.
         * @param selection ignored
         * @param value the exact value to use for the selected state, if valid
         */
        public override void SetSelectedState(int selection, ulong value)
        {
            State = value;
        }

        /**
         * This initializes the states of the generator to the given seed, exactly.
         * All (2 to the 64) possible initial generator states can be produced here.
         * @param seed the initial seed; may be any ulong
         */
        public override void Seed(ulong seed)
        {
            State = seed;
        }

        /**
         * Sets the state completely to the given state variable.
         * This is the same as calling {@link #Seed(ulong)}.
         * @param stateA the first state; can be any ulong
         */
        public override void SetState(ulong state)
        {
            this.State = state;
        }

        /// <inheritdoc />
        public override ulong NextULong()
        {
            unchecked
            {
                ulong x = (State += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                return x ^ x >> 27;
            }
        }

        /// <inheritdoc />
        public override ulong Skip(ulong distance)
        {
            unchecked
            {
                ulong x = (State += 0x9E3779B97F4A7C15UL * distance);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                return x ^ x >> 27;
            }
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            unchecked
            {
                ulong x = (State -= 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                return x ^ x >> 27;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new DistinctRandom(State);

        /// <inheritdoc />
        public override string StringSerialize() => $"#DisR`{State:X}`";

        /// <inheritdoc />
        public override IEnhancedRandom StringDeserialize(string data)
        {
            int idx = data.IndexOf('`');
            State = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (      data.IndexOf('`', idx + 1))), 16);
            return this;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as DistinctRandom);

        /// <inheritdoc />
        public bool Equals(DistinctRandom? other) => other != null && State == other.State;

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(State);

        public static bool operator ==(DistinctRandom? left, DistinctRandom? right) => EqualityComparer<DistinctRandom>.Default.Equals(left, right);
        public static bool operator !=(DistinctRandom? left, DistinctRandom? right) => !(left == right);
    }
}
