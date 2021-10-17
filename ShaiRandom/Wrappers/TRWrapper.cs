using System;
using System.Collections.Generic;
using Troschuetz.Random;

namespace ShaiRandom
{
    /// <summary>
    /// Wraps a ShaiRandom AbstractRandom object so it can also be used as a Troschuetz.Random IGenerator.
    /// </summary>
    [System.Serializable]
    public class TRWrapper : AbstractRandom, IGenerator, IEquatable<TRWrapper?>
    {
        /// <summary>
        /// The identifying tag here is "T" , which is an invalid length to indicate the tag is not meant to be registered or used on its own.
        /// </summary>
        public override string Tag => "T";

        /// <summary>
        /// The wrapped AbstractRandom, which must never be null.
        /// </summary>
        public IRandom Wrapped { get; set; }

        public TRWrapper() => Wrapped = new FourWheelRandom();

        public TRWrapper(ulong seed) => Wrapped = new FourWheelRandom(seed);

        public TRWrapper(IRandom wrapped) => Wrapped = wrapped;

        public override int StateCount => Wrapped.StateCount;
        public override bool SupportsReadAccess => Wrapped.SupportsReadAccess;
        public override bool SupportsWriteAccess => Wrapped.SupportsWriteAccess;
        public override bool SupportsSkip => Wrapped.SupportsSkip;
        public override bool SupportsPrevious => Wrapped.SupportsPrevious;

        public override IRandom Copy() => new TRWrapper(Wrapped.Copy());
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
        public override ulong PreviousUlong() => Wrapped.PreviousUlong();
        public override string StringSerialize() => "T"+ Wrapped.StringSerialize().Substring(1);
        public override IRandom StringDeserialize(string data)
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
        uint IGenerator.NextUInt() => Wrapped.NextUint(uint.MaxValue);
        uint IGenerator.NextUInt(uint maxValue) => Wrapped.NextUint(maxValue);
        uint IGenerator.NextUInt(uint minValue, uint maxValue) => Wrapped.NextUint(minValue, maxValue);
        uint IGenerator.NextUIntExclusiveMaxValue() => Wrapped.NextUint(uint.MaxValue);
        uint IGenerator.NextUIntInclusiveMaxValue() => Wrapped.NextUint();
        bool IGenerator.Reset() => false;
        bool IGenerator.Reset(uint seed) => false;
        #endregion

        public override bool Equals(object? obj) => Equals(obj as TRWrapper);
        public bool Equals(TRWrapper? other) => other != null && EqualityComparer<IRandom>.Default.Equals(Wrapped, other.Wrapped);

        public static bool operator ==(TRWrapper? left, TRWrapper? right) => EqualityComparer<TRWrapper>.Default.Equals(left, right);
        public static bool operator !=(TRWrapper? left, TRWrapper? right) => !(left == right);


    }
}
