using System;
using System.Text;
using ShaiRandom.Generators;

namespace ShaiRandom.Wrappers
{

    /// <summary>
    /// Wraps another IEnhancedRandom without copying it, making all calls to "NextSomething()" on this ReversingWrapper move the state in reverse
    /// instead of forward in the wrapped IEnhancedRandom. This only works for IEnhancedRandom implementations that support <see cref="PreviousULong"/>,
    /// which can be checked with <see cref="SupportsPrevious"/>. If the wrapped IEnhancedRandom supports <see cref="Skip(ulong)"/>, then this
    /// ReversingWrapper will permit calls to its Skip, and they will also go in reverse. The most common use for this would be to run
    /// the wrapped generator forward and track the number of calls made, then run the ReversingWrapper by the same number of calls
    /// (potentially using Skip(), if supported) to revert the state to its original value. This is more convenient for the usage where
    /// you use <see cref="EnhancedRandomExtensions.Shuffle{T}(IEnhancedRandom, Span&lt;T&gt;)"/> or other Shuffle variants with both the wrapped
    /// generator and wrapper, since they will use the same amount of calls, and this will even un-shuffle the array (restoring it to its order before the shuffle).
    /// </summary>
    public class ReversingWrapper : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "RvW", which is a different length to indicate the tag is a wrapper.
        /// </summary>
        public override string DefaultTag => "RvW";

        /// <summary>
        /// The ShaiRandom generator being wrapped, which must never be null.
        /// </summary>
        public IEnhancedRandom Wrapped { get; set; }

        /// <summary>
        /// Creates a new ReversingWrapper around a new <see cref="FourWheelRandom"/> generator with a random initial
        /// state.
        /// </summary>
        public ReversingWrapper()
        {
            Wrapped = new FourWheelRandom();
        }

        /// <summary>
        /// Creates a new ReversingWrapper around a new <see cref="FourWheelRandom"/> generator seeded with the given
        /// value.
        /// </summary>
        public ReversingWrapper(ulong seed)
        {
            Wrapped = new FourWheelRandom(seed);
        }

        /// <summary>
        /// Creates a new ReversingWrapper around the given generator, which must support <see cref="IEnhancedRandom.PreviousULong"/>.
        /// </summary>
        public ReversingWrapper(IEnhancedRandom wrapping)
        {
            if (!wrapping.SupportsPrevious)
                throw new ArgumentException($"The IEnhancedRandom to wrap must support PreviousULong().", nameof(wrapping));
            Wrapped = wrapping;
        }

        /// <summary>
        /// The <see cref="IEnhancedRandom.StateCount"/> of the wrapped generator.
        /// </summary>
        public override int StateCount => Wrapped.StateCount;

        /// <summary>
        /// The <see cref="IEnhancedRandom.SupportsReadAccess"/> value of the wrapped generator.
        /// </summary>
        public override bool SupportsReadAccess => Wrapped.SupportsReadAccess;

        /// <summary>
        /// The <see cref="IEnhancedRandom.SupportsWriteAccess"/> value of the wrapped generator.
        /// </summary>
        public override bool SupportsWriteAccess => Wrapped.SupportsWriteAccess;

        /// <summary>
        /// The <see cref="IEnhancedRandom.SupportsSkip"/> value of the wrapped generator.
        /// </summary>
        public override bool SupportsSkip => Wrapped.SupportsSkip;

        /// <summary>
        /// The <see cref="IEnhancedRandom.SupportsPrevious"/> value of the wrapped generator.
        /// </summary>
        public override bool SupportsPrevious => Wrapped.SupportsPrevious;

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new ReversingWrapper(Wrapped.Copy());

        /// <inheritdoc />
        public override ulong NextULong() => Wrapped.PreviousULong();

        /// <inheritdoc />
        public override void Seed(ulong seed) => Wrapped.Seed(seed);

        /// <inheritdoc />
        public override string StringSerialize()
        {
            var ser = new StringBuilder(Serializer.GetTag(this));
            ser.Append('`');
            ser.Append(Wrapped.StringSerialize());
            ser.Append('`');

            return ser.ToString();
        }

        /// <inheritdoc />
        public override IEnhancedRandom StringDeserialize(ReadOnlySpan<char> data)
        {
            Wrapped = Serializer.Deserialize(data[1..]);
            return this;
        }

        /// <inheritdoc />
        public override ulong Skip(ulong distance) => Wrapped.Skip(0UL - distance);

        /// <inheritdoc />
        public override ulong PreviousULong() => Wrapped.NextULong();

        /// <inheritdoc />
        public override ulong SelectState(int selection) => Wrapped.SelectState(selection);
        /// <inheritdoc />
        public override void SetSelectedState(int selection, ulong value) => Wrapped.SetSelectedState(selection, value);
    }
}
