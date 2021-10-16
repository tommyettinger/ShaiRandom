using System;
using System.Collections.Generic;

namespace ShaiRandom
{

    /// <summary>
    /// Wraps another ARandom without copying it, making all calls to "NextSomething()" on this ReversingWrapper move the state in reverse
    /// instead of forward in the wrapped ARandom. This only works for ARandom implementations that support <see cref="PreviousUlong"/>,
    /// which can be checked with <see cref="SupportsPrevious"/>. If the wrapped ARandom supports <see cref="Skip(ulong)"/>, then this
    /// ReversingWrapper will permit calls to its Skip, and they will also go in reverse. The most common use for this would be to run
    /// the wrapped generator forward and track the number of calls made, then run the ReversingWrapper by the same number of calls
    /// (potentially using Skip(), if supported) to revert the state to its original value. This is more convenient for the usage where
    /// you use <see cref="ARandom.Shuffle{T}(T[])"/> with both the wrapped generator and wrapper, since they will use the same amount of
    /// calls, and this will even un-shuffle the array (restoring it to its order before the shuffle).
    /// </summary>
    [Serializable]
    public class ReversingWrapper : ARandom, IEquatable<ReversingWrapper?>
    {
        /// <summary>
        /// The identifying tag here is "R" , which is an invalid length to indicate the tag is not meant to be registered or used on its own.
        /// </summary>
        public override string Tag => "R";
        public IRandom Wrapped { get; set; }

        public ReversingWrapper()
        {
            Wrapped = new FourWheelRandom();
        }

        public ReversingWrapper(ulong seed)
        {
            Wrapped = new FourWheelRandom(seed);
        }

        public ReversingWrapper(IRandom wrapping)
        {
            if (!wrapping.SupportsPrevious)
                throw new NotSupportedException($"The ARandom to wrap must support PreviousUlong(), and {nameof(wrapping)} does not.");
            Wrapped = wrapping;
        }

        public override int StateCount => Wrapped.StateCount;

        public override bool SupportsReadAccess => Wrapped.SupportsReadAccess;

        public override bool SupportsWriteAccess => Wrapped.SupportsWriteAccess;

        public override bool SupportsSkip => Wrapped.SupportsSkip;

        public override bool SupportsPrevious => Wrapped.SupportsPrevious;

        public override IRandom Copy() => new ReversingWrapper(Wrapped.Copy());
        public override ulong NextUlong() => Wrapped.PreviousUlong();

        public override void Seed(ulong seed) => Wrapped.Seed(seed);
        public override string StringSerialize() => "R" + Wrapped.StringSerialize().Substring(1);
        public override IRandom StringDeserialize(string data)
        {
            Wrapped.StringDeserialize(data);
            return this;
        }
        public override ulong Skip(ulong distance) => Wrapped.Skip(0UL - distance);
        public override ulong PreviousUlong() => Wrapped.NextUlong();
        public override bool Equals(object? obj) => Equals(obj as ReversingWrapper);
        public bool Equals(ReversingWrapper? other) => other != null && EqualityComparer<IRandom>.Default.Equals(Wrapped, other.Wrapped);

        public static bool operator ==(ReversingWrapper? left, ReversingWrapper? right) => EqualityComparer<ReversingWrapper>.Default.Equals(left, right);
        public static bool operator !=(ReversingWrapper? left, ReversingWrapper? right) => !(left == right);
    }
}
