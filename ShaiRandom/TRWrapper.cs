using System;
using Troschuetz.Random.Generators;

namespace ShaiRandom
{
    /// <summary>
    /// Wraps a ShaiRandom ARandom object so it can also be used as a Troschuetz.Random IGenerator.
    /// </summary>
    [Serializable]
    public class TRWrapper : ARandom
    {
        /// <summary>
        /// The wrapped ARandom, which must never be null.
        /// </summary>
        public ARandom Wrapped { get; set; }

        public TRWrapper() => Wrapped = new FourWheelRandom();

        public TRWrapper(ulong seed) => Wrapped = new FourWheelRandom(seed);

        public TRWrapper(ARandom wrapped) => Wrapped = wrapped.Copy();

        public override int StateCount => Wrapped.StateCount;
        public override ARandom Copy() => new TRWrapper(Wrapped);
        public override double NextDouble() => Wrapped.NextDouble();
        public override ulong NextUlong() => Wrapped.NextUlong();
        public override ulong SelectState(int selection) => Wrapped.SelectState(selection);
        public override void Seed(ulong seed) => Wrapped.Seed(seed);
        public override void SetState(ulong stateA) => Wrapped.SetState(stateA);
        public override void SetState(ulong stateA, ulong stateB) => Wrapped.SetState(stateA, stateB);
        public override void SetState(ulong stateA, ulong stateB, ulong stateC) => Wrapped.SetState(stateA, stateB, stateC);
        public override void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD) => Wrapped.SetState(stateA, stateB, stateC, stateD);
        public override void SetState(params ulong[] states) => Wrapped.SetState(states);
        public override ulong Skip(ulong distance) => Wrapped.Skip(distance);
    }
}
