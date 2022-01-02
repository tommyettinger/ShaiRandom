using System;
using System.Collections.Generic;
using ShaiRandom.Generators;

namespace ShaiRandom.Wrappers
{

    /// <summary>
    /// Wraps another AbstractRandom without copying it, making all calls to "NextSomething()" on this ReversingWrapper move the state in reverse
    /// instead of forward in the wrapped AbstractRandom. This only works for AbstractRandom implementations that support <see cref="PreviousULong"/>,
    /// which can be checked with <see cref="SupportsPrevious"/>. If the wrapped AbstractRandom supports <see cref="Skip(ulong)"/>, then this
    /// ReversingWrapper will permit calls to its Skip, and they will also go in reverse. The most common use for this would be to run
    /// the wrapped generator forward and track the number of calls made, then run the ReversingWrapper by the same number of calls
    /// (potentially using Skip(), if supported) to revert the state to its original value. This is more convenient for the usage where
    /// you use <see cref="EnhancedRandomExtensions.Shuffle{T}(IEnhancedRandom, Span&lt;T&gt;)"/> or other span variants with both the wrapped
    /// generator and wrapper, since they will use the same amount of calls, and this will even un-shuffle the array (restoring it to its order before the shuffle).
    /// </summary>
    [Serializable]
    public class ReversingWrapper : AbstractRandom, IEquatable<ReversingWrapper?>
    {
        /// <summary>
        /// The identifying tag here is "R" , which is an invalid length to indicate the tag is not meant to be registered or used on its own.
        /// </summary>
        public override string Tag => "R";
        public IEnhancedRandom Wrapped { get; set; }

        public ReversingWrapper()
        {
            Wrapped = new FourWheelRandom();
        }

        public ReversingWrapper(ulong seed)
        {
            Wrapped = new FourWheelRandom(seed);
        }

        public ReversingWrapper(IEnhancedRandom wrapping)
        {
            if (!wrapping.SupportsPrevious)
                throw new NotSupportedException($"The AbstractRandom to wrap must support PreviousULong(), and {nameof(wrapping)} does not.");
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
        public override string StringSerialize() => "R" + Wrapped.StringSerialize().Substring(1);

        /// <inheritdoc />
        public override IEnhancedRandom StringDeserialize(string data)
        {
            Wrapped.StringDeserialize(data);
            return this;
        }

        /// <inheritdoc />
        public override ulong Skip(ulong distance) => Wrapped.Skip(0UL - distance);

        /// <inheritdoc />
        public override ulong PreviousULong() => Wrapped.NextULong();

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as ReversingWrapper);

        /// <inheritdoc />
        public bool Equals(ReversingWrapper? other) => other != null && EqualityComparer<IEnhancedRandom>.Default.Equals(Wrapped, other.Wrapped);

        /// <inheritdoc />
        public override ulong SelectState(int selection) => Wrapped.SelectState(selection);
        /// <inheritdoc />
        public override void SetSelectedState(int selection, ulong value) => Wrapped.SetSelectedState(selection, value);

        public static bool operator ==(ReversingWrapper? left, ReversingWrapper? right) => EqualityComparer<ReversingWrapper>.Default.Equals(left, right);
        public static bool operator !=(ReversingWrapper? left, ReversingWrapper? right) => !(left == right);
    }
}
