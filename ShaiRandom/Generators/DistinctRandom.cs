using System;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 1 state that only returns each ulong result exactly once over its period.
    /// </summary>
    /// <remarks>
    /// This generator supports <see cref="Skip(ulong)"/>, along with all other optional operations.
    /// It is very similar to Java 8's SplittableRandom, though not identical, and only if using one stream of that generator.
    /// It uses the same pattern of generation; add a large odd constant to the state, then run the value of that state through a
    /// unary hash (SplittableRandom uses something close to murmurhash3's finalizer; this uses a similar function that is better
    /// in some measurable ways) and return the hash' result.
    /// </remarks>
    public class DistinctRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "DisR" .
        /// </summary>
        public override string Tag => "DisR";
        static DistinctRandom()
        {
            RegisterTag(new DistinctRandom(1UL));
        }

        /// <summary>
        /// The first state; can be any ulong.
        /// </summary>
        public ulong State { get; set; }

        /// <summary>
        /// Creates a new DistinctRandom with a random state.
        /// </summary>
        public DistinctRandom()
        {
            State = MakeSeed();
        }

        /// <summary>
        /// Creates a new DistinctRandom with the given four states; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be used verbatim as the State.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
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

        /// <summary>
        /// Gets the state, regardless of selection, as-is.
        /// </summary>
        /// <param name="selection">Ignored.</param>
        /// <returns>The value of the selected state.</returns>
        public override ulong SelectState(int selection)
        {
            return State;
        }

        /// <summary>
        /// Sets the State, regardless of selection, to value, as-is.
        /// </summary>
        /// <param name="selection">Ignored</param>
        /// <param name="value">The exact value to use for the State.</param>
        public override void SetSelectedState(int selection, ulong value)
        {
            State = value;
        }

        /// <summary>
        /// This initializes the states of the generator to the given seed, exactly.
        /// </summary>
        /// <remarks>
        /// All (2 to the 64) possible initial generator states can be produced here.
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed)
        {
            State = seed;
        }

        /// <summary>
        /// This initializes the states of the generator to the given seed, exactly.
        /// </summary>
        /// <remarks>
        /// All (2 to the 64) possible initial generator states can be produced here.
        /// This is the same as calling <see cref="Seed(ulong)">Seed(ulong)</see>.
        /// </remarks>
        /// <param name="state">The initial state value; may be any ulong.</param>
        public override void SetState(ulong state)
        {
            State = state;
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
    }
}
