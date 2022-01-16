using System;
using System.Collections.Generic;
using ShaiRandom.Generators;

namespace ShaiRandom.Wrappers
{

    /// <summary>
    /// Wraps another IEnhancedRandom without copying it, recording all results of individual calls to the generator in a <see cref="KnownSeriesRandom">KnownSeriesRandom</see>
    /// that can be used to replay the same results even if, for any reason, the results of the wrapped IEnhancedRandom change.
    ///</summary>
    ///<remarks>
    /// Version updates can sometimes change the series an IEnhancedRandom produces, so storing the KnownSeriesRandom is a surefire way to make the same values get produced.
    /// </remarks>
    public class ArchivalWrapper : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "A" , which is an invalid length to indicate the tag is not meant to be registered or used on its own.
        /// </summary>
        public override string Tag => "A";

        /// <summary>
        /// The ShaiRandom generator being wrapped, which must never be null.
        /// </summary>
        public IEnhancedRandom Wrapped { get; set; }

        /// <summary>
        /// The KnownSeriesRandom this builds up as this records calls to the various generation methods.
        /// </summary>
        public KnownSeriesRandom Series { get; set; }

        private readonly List<bool> _boolSeries;
        private readonly List<byte> _byteSeries;
        private readonly List<double> _doubleSeries;
        private readonly List<float> _floatSeries;
        private readonly List<int> _intSeries;
        private readonly List<uint> _uintSeries;
        private readonly List<long> _longSeries;
        private readonly List<ulong> _ulongSeries;

        /// <summary>
        /// Creates a new ArchivalWrapper around a new <see cref="FourWheelRandom"/> generator with a random initial
        /// state.
        /// </summary>
        public ArchivalWrapper(IEnhancedRandom random)
        {
            Wrapped = random;

            _boolSeries = new List<bool>();
            _byteSeries = new List<byte>();
            _doubleSeries = new List<double>();
            _floatSeries = new List<float>();
            _intSeries = new List<int>();
            _uintSeries = new List<uint>();
            _longSeries = new List<long>();
            _ulongSeries = new List<ulong>();
            Series = new KnownSeriesRandom(_intSeries, _uintSeries, _doubleSeries, _boolSeries, _byteSeries, _floatSeries, _longSeries, _ulongSeries);
        }

        /// <summary>
        /// Creates a new ArchivalWrapper around a new <see cref="FourWheelRandom"/> generator seeded with the given
        /// value.
        /// </summary>
        public ArchivalWrapper(ulong seed) : this(new FourWheelRandom(seed))
        {
        }

        /// <summary>
        /// Creates a new ArchivalWrapper around a new <see cref="FourWheelRandom"/> generator seeded randomly.
        /// </summary>
        public ArchivalWrapper() : this(new FourWheelRandom())
        {
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
        public override IEnhancedRandom Copy() => new ArchivalWrapper(Wrapped.Copy());

        /// <inheritdoc />
        public override ulong NextULong() => Wrapped.NextULong();

        /// <inheritdoc />
        public override void Seed(ulong seed) => Wrapped.Seed(seed);

        /// <inheritdoc />
        public override string StringSerialize() => "A" + Wrapped.StringSerialize().Substring(1);

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
        public override ulong SelectState(int selection) => Wrapped.SelectState(selection);
        /// <inheritdoc />
        public override void SetSelectedState(int selection, ulong value) => Wrapped.SetSelectedState(selection, value);
    }
}
