using System;
using Troschuetz.Random;

namespace ShaiRandom
{
    /// <summary>
    /// Wraps a ShaiRandom ARandom object so it can also be used as a Troschuetz.Random IGenerator.
    /// </summary>
    [System.Serializable]
    public class TRWrapper : ARandom, IGenerator
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

        #region IGenerator explicit implementation
        bool IGenerator.CanReset => false;
        uint IGenerator.Seed => 1;
        int IGenerator.Next() => NextInt(int.MaxValue);
        int IGenerator.Next(int maxValue) => NextInt(maxValue);
        int IGenerator.Next(int minValue, int maxValue) => NextInt(minValue, maxValue);
        bool IGenerator.NextBoolean() => NextBool();
        void IGenerator.NextBytes(byte[] buffer) => NextBytes(buffer);
        double IGenerator.NextDouble() => NextDouble();
        double IGenerator.NextDouble(double maxValue) => NextDouble(maxValue);
        double IGenerator.NextDouble(double minValue, double maxValue) => NextDouble(minValue, maxValue);
        int IGenerator.NextInclusiveMaxValue() => (int)NextBits(31);
        uint IGenerator.NextUInt() => NextUint(uint.MaxValue);
        uint IGenerator.NextUInt(uint maxValue) => NextUint(maxValue);
        uint IGenerator.NextUInt(uint minValue, uint maxValue) => NextUint(minValue, maxValue);
        uint IGenerator.NextUIntExclusiveMaxValue() => NextUint(uint.MaxValue);
        uint IGenerator.NextUIntInclusiveMaxValue() => NextUint();
        bool IGenerator.Reset() => throw new NotImplementedException();
        bool IGenerator.Reset(uint seed) => throw new NotImplementedException();
        #endregion
    }
}
