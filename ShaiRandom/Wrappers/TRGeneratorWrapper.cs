using ShaiRandom.Generators;
using Troschuetz.Random;

namespace ShaiRandom.Wrappers
{
    /// <summary>
    /// Wraps a ShaiRandom IEnhancedRandom object so it can also be used as a Troschuetz.Random IGenerator.
    /// </summary>
    /// <remarks>
    /// This class implements both IGenerator and IEnhancedRandom.  Any IEnhancedRandom member is implemented by
    /// simply forwarding to the ShaiRandom generator being wrapped;  all IGenerator methods are explicitly implemented
    /// and are implemented in terms of IEnhancedRandom methods.
    /// </remarks>
    [System.Serializable]
    public class TRGeneratorWrapper : AbstractRandom, IGenerator
    {
        /// <summary>
        /// The identifying tag here is "T" , which is an invalid length to indicate the tag is not meant to be registered or used on its own.
        /// </summary>
        public override string Tag => "T";

        /// <summary>
        /// The wrapped RNG, which must never be null.
        /// </summary>
        public IEnhancedRandom Wrapped { get; set; }

        /// <summary>
        /// Creates a wrapper around a new FourWheelRandom generator with a random state.
        /// </summary>
        public TRGeneratorWrapper() => Wrapped = new FourWheelRandom();

        /// <summary>
        /// Creates a wrapper around a new FourWheelRandom generator, whose state will be initialized with the given seed.
        /// </summary>
        /// <param name="seed">Seed to initialize the new generator with.</param>
        public TRGeneratorWrapper(ulong seed) => Wrapped = new FourWheelRandom(seed);

        /// <summary>
        /// Creates a wrapper around the given ShaiRandom generator.
        /// </summary>
        /// <param name="wrapped">The ShaiRandom generator to wrap.</param>
        public TRGeneratorWrapper(IEnhancedRandom wrapped) => Wrapped = wrapped;

        /// <inheritdoc />
        public override int StateCount => Wrapped.StateCount;
        /// <inheritdoc />
        public override bool SupportsReadAccess => Wrapped.SupportsReadAccess;
        /// <inheritdoc />
        public override bool SupportsWriteAccess => Wrapped.SupportsWriteAccess;
        /// <inheritdoc />
        public override bool SupportsSkip => Wrapped.SupportsSkip;
        /// <inheritdoc />
        public override bool SupportsPrevious => Wrapped.SupportsPrevious;

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new TRGeneratorWrapper(Wrapped.Copy());
        /// <inheritdoc />
        public override double NextDouble() => Wrapped.NextDouble();
        /// <inheritdoc />
        public override ulong NextULong() => Wrapped.NextULong();
        /// <inheritdoc />
        public override ulong SelectState(int selection) => Wrapped.SelectState(selection);
        /// <inheritdoc />
        public override void Seed(ulong seed) => Wrapped.Seed(seed);
        /// <inheritdoc />
        public override void SetState(ulong stateA) => Wrapped.SetState(stateA);
        /// <inheritdoc />
        public override void SetState(ulong stateA, ulong stateB) => Wrapped.SetState(stateA, stateB);
        /// <inheritdoc />
        public override void SetState(ulong stateA, ulong stateB, ulong stateC) => Wrapped.SetState(stateA, stateB, stateC);
        /// <inheritdoc />
        public override void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD) => Wrapped.SetState(stateA, stateB, stateC, stateD);
        /// <inheritdoc />
        public override void SetState(params ulong[] states) => Wrapped.SetState(states);
        /// <inheritdoc />
        public override ulong Skip(ulong distance) => Wrapped.Skip(distance);
        /// <inheritdoc />
        public override ulong PreviousULong() => Wrapped.PreviousULong();
        /// <inheritdoc />
        public override string StringSerialize() => "T"+ Wrapped.StringSerialize().Substring(1);
        /// <inheritdoc />
        public override IEnhancedRandom StringDeserialize(string data)
        {
            Wrapped.StringDeserialize(data);
            return this;
        }

        #region IGenerator explicit implementation
        bool IGenerator.CanReset => false;
        uint IGenerator.Seed => 1;
        int IGenerator.Next() => Wrapped.NextInt(int.MaxValue);
        int IGenerator.Next(int maxValue) => Wrapped.NextInt(maxValue);
        int IGenerator.Next(int minValue, int maxValue) => Wrapped.NextInt(minValue, maxValue);
        bool IGenerator.NextBoolean() => Wrapped.NextBool();
        void IGenerator.NextBytes(byte[] buffer) => Wrapped.NextBytes(buffer);
        double IGenerator.NextDouble() => Wrapped.NextDouble();
        double IGenerator.NextDouble(double maxValue) => Wrapped.NextDouble(maxValue);
        double IGenerator.NextDouble(double minValue, double maxValue) => Wrapped.NextDouble(minValue, maxValue);
        int IGenerator.NextInclusiveMaxValue() => (int)Wrapped.NextBits(31);
        uint IGenerator.NextUInt() => Wrapped.NextUInt(uint.MaxValue);
        uint IGenerator.NextUInt(uint maxValue) => Wrapped.NextUInt(maxValue);
        uint IGenerator.NextUInt(uint minValue, uint maxValue) => Wrapped.NextUInt(minValue, maxValue);
        uint IGenerator.NextUIntExclusiveMaxValue() => Wrapped.NextUInt(uint.MaxValue);
        uint IGenerator.NextUIntInclusiveMaxValue() => Wrapped.NextUInt();
        bool IGenerator.Reset() => false;
        bool IGenerator.Reset(uint seed) => false;
        #endregion
    }
}
