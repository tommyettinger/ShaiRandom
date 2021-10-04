using Troschuetz.Random.Generators;
namespace ShaiRandom
{
    /// <summary>
    /// Wraps a ShaiRandom IEnhancedRandom object so it can also be used as a Troschuetz.Random AbstractGenerator.
    /// </summary>
    class TRWrapper : AbstractGenerator, IEnhancedRandom
    {
        /// <summary>
        /// The wrapped IEnhancedRandom, which must never be null.
        /// </summary>
        public IEnhancedRandom Wrapped { get; set; }

        public TRWrapper(uint seed) : base(seed) => Wrapped = new FourWheelRandom(seed);

        public TRWrapper(ulong seed) : base((uint)seed) => Wrapped = new FourWheelRandom(seed);

        public TRWrapper(IEnhancedRandom wrapped) : base((uint)wrapped.SelectState(0)) => Wrapped = wrapped;

        public int StateCount => Wrapped.StateCount;
        public override bool CanReset => false;
        public IEnhancedRandom Copy() => new TRWrapper(Wrapped.Copy());
        public override double NextDouble() => Wrapped.NextDouble();
        public override int NextInclusiveMaxValue() => (int)Wrapped.NextBits(31);
        public override uint NextUIntInclusiveMaxValue() => Wrapped.NextBits(31);
        public ulong NextUlong() => Wrapped.NextUlong();
        public ulong SelectState(int selection) => Wrapped.SelectState(selection);
        void IEnhancedRandom.Seed(ulong seed) => Wrapped.Seed(seed);
        public void SetState(ulong stateA) => Wrapped.SetState(stateA);
        public void SetState(ulong stateA, ulong stateB) => Wrapped.SetState(stateA, stateB);
        public void SetState(ulong stateA, ulong stateB, ulong stateC) => Wrapped.SetState(stateA, stateB, stateC);
        public void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD) => Wrapped.SetState(stateA, stateB, stateC, stateD);
        public void SetState(params ulong[] states) => Wrapped.SetState(states);
        public ulong Skip(ulong distance) => Wrapped.Skip(distance);
    }
}
