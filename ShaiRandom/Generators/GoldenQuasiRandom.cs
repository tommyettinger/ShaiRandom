using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// A not-especially-random number generator in the form of a counter with a large increment.
    /// </summary>
    /// <remarks>
    /// This generator supports <see cref="Skip(ulong)"/>, along with all other optional operations except
    /// <see cref="IEnhancedRandom.Leap()"/>. It is nearly identical to <see cref="DistinctRandom"/> with all
    /// randomization steps (mixing) removed, leaving only the counter with a large increment. The increment is almost
    /// exactly 2 to the 64 divided by the Golden Ratio, only changed to make it an odd number. This is mainly useful
    /// when you want a quasi-random sequence of numbers instead of a pseudo-random sequence. Also called
    /// low-discrepancy sequences, quasi-random sequences tend to converge very quickly in Monte Carlo code.
    /// </remarks>
    public sealed class GoldenQuasiRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "GoQR" .
        /// </summary>
        public override string DefaultTag => "GoQR";

        /// <summary>
        /// The first and only state; can be any ulong.
        /// </summary>
        public ulong State { get; set; }

        /// <summary>
        /// Creates a new GoldenQuasiRandom with a random state.
        /// </summary>
        public GoldenQuasiRandom()
        {
            State = MakeSeed();
        }

        /// <summary>
        /// Creates a new GoldenQuasiRandom with the given state; any ulong value is permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be used verbatim as the State.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public GoldenQuasiRandom(ulong seed)
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
        /// This does not support <see cref="IEnhancedRandom.Leap()"/>.
        /// </summary>
        public override bool SupportsLeap => false;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            unchecked
            {
                return (State += 0x9E3779B97F4A7C15UL);
            }
        }

        /// <inheritdoc />
        public override float NextSparseFloat()
        {
            unchecked
            {
                return BitConverter.Int32BitsToSingle((int)((State += 0x9E3779B97F4A7C15UL) >> 41) | 0x3F800000) - 1f;
            }
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            unchecked
            {
                return BitConverter.Int64BitsToDouble((long)((State += 0x9E3779B97F4A7C15UL) >> 12) | 0x3FF0000000000000L) - 1.0;
            }
        }

        /// <inheritdoc />
        public override ulong Skip(ulong distance)
        {
            unchecked
            {
                return (State += 0x9E3779B97F4A7C15UL * distance);
            }
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            unchecked
            {
                ulong x = State;
                State -= 0x9E3779B97F4A7C15UL;
                return x;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new GoldenQuasiRandom(State);
    }
}
