using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ArchivalWrapper : IEnhancedRandom
    {
        private readonly List<bool> _boolSeries;
        private readonly List<byte> _byteSeries;
        private readonly List<double> _doubleSeries;
        private readonly List<float> _floatSeries;
        private readonly List<int> _intSeries;
        private readonly List<uint> _uintSeries;
        private readonly List<long> _longSeries;
        private readonly List<ulong> _ulongSeries;
        private readonly List<decimal> _decimalSeries;

        /// <summary>
        /// The identifying tag here is "ArW", which is a different length to indicate the tag is a wrapper.
        /// </summary>
        public string DefaultTag => "ArW";

        /// <summary>
        /// The ShaiRandom IEnhancedRandom being wrapped, which must never be null.
        /// </summary>
        public IEnhancedRandom Wrapped { get; set; }

        /// <summary>
        /// Creates a new KnownSeriesRandom that has the full sequence of archived results this has stored so far.
        /// </summary>
        /// <remarks>
        /// The result of this method does not share any state with this ArchivalWrapper, so if more method calls are archived, the KnownSeriesRandom won't know about them.
        /// You can call this again at any point to get another snapshot of the archive, from start to present, and can serialize the KnownSeriesRandom for permanent safe-keeping.
        /// </remarks>
        public KnownSeriesRandom MakeArchivedSeries()
        {
            return new KnownSeriesRandom(_intSeries, _uintSeries, _doubleSeries, _boolSeries, _byteSeries, _floatSeries, _longSeries, _ulongSeries, _decimalSeries);
        }

        /// <summary>
        /// Creates a ArchivalWrapper that is a copy of the given one.
        /// </summary>
        /// <param name="other">Generator to copy state from.</param>
        public ArchivalWrapper(ArchivalWrapper other)
            : this(other.Wrapped.Copy(), other._intSeries, other._uintSeries, other._doubleSeries, other._boolSeries, other._byteSeries, other._floatSeries, other._longSeries, other._ulongSeries, other._decimalSeries)
        {
        }

        /// <summary>
        /// Creates a ArchivalWrapper that wraps a FourWheelRandom with a random state.
        /// </summary>
        public ArchivalWrapper()
            : this(new FourWheelRandom())
        {
        }


        /// <summary>
        /// Creates a new known series generator, with parameters to indicate which series to use for
        /// the integer, unsigned integer, double, bool, and byte-based RNG functions. If null is
        /// specified, no values of that type may be returned, and functions that try to return a
        /// value of that type will throw an exception.
        /// </summary>
        /// <remarks>
        /// The values given for each series are looped over repeatedly as the appropriate function is called, so the
        /// RNG functions can be called an arbitrary number of times; doing so will simply result in values from the
        /// sequence being reused.
        /// </remarks>
        /// <param name="random">The IEnhancedRandom to wrap and record.</param>
        /// <param name="intSeries">Series of values to return via <see cref="NextInt()"/>.</param>
        /// <param name="uintSeries">Series of values to return via <see cref="NextUInt()"/>.</param>
        /// <param name="doubleSeries">Series of values to return via <see cref="NextDouble()"/>.</param>
        /// <param name="boolSeries">Series of values to return via <see cref="NextBool()"/>.</param>
        /// <param name="byteSeries">Series of values to return via <see cref="NextBytes(Span&lt;byte&gt;)"/>.</param>
        /// <param name="floatSeries">Series of values to return via <see cref="NextFloat()"/>.</param>
        /// <param name="longSeries">Series of values to return via <see cref="NextLong()"/>.</param>
        /// <param name="ulongSeries">Series of values to return via <see cref="NextULong()"/>.</param>
        /// <param name="decimalSeries">Series of values to return via <see cref="NextDecimal()"/>.</param>
        public ArchivalWrapper(IEnhancedRandom random, IEnumerable<int>? intSeries = null, IEnumerable<uint>? uintSeries = null,
                                 IEnumerable<double>? doubleSeries = null, IEnumerable<bool>? boolSeries = null,
                                 IEnumerable<byte>? byteSeries = null, IEnumerable<float>? floatSeries = null,
                                 IEnumerable<long>? longSeries = null,IEnumerable<ulong>? ulongSeries = null,
                                 IEnumerable<decimal>? decimalSeries = null)
        {
            Wrapped = random;
            _intSeries = intSeries == null ? new List<int>() : intSeries.ToList();
            _uintSeries = uintSeries == null ? new List<uint>() : uintSeries.ToList();
            _longSeries = longSeries == null ? new List<long>() : longSeries.ToList();
            _ulongSeries = ulongSeries == null ? new List<ulong>() : ulongSeries.ToList();
            _decimalSeries = decimalSeries == null ? new List<decimal>() : decimalSeries.ToList();
            _doubleSeries = doubleSeries == null ? new List<double>() : doubleSeries.ToList();
            _floatSeries = floatSeries == null ? new List<float>() : floatSeries.ToList();
            _boolSeries = boolSeries == null ? new List<bool>() : boolSeries.ToList();
            _byteSeries = byteSeries == null ? new List<byte>() : byteSeries.ToList();
        }

        /// <summary>
        /// This generator has the same number of states as the wrapped generator; the recorded values are not considered state.
        /// They can be considered state for the <see cref="KnownSeriesRandom"/> this creates.
        /// </summary>
        public int StateCount => Wrapped.StateCount;

        /// <summary>
        /// This supports <see cref="SelectState(int)"/> if the wrapped generator does.
        /// </summary>
        public bool SupportsReadAccess => Wrapped.SupportsReadAccess;

        /// <summary>
        /// This supports <see cref="SetSelectedState(int, ulong)"/> if the wrapped generator does.
        /// </summary>
        public bool SupportsWriteAccess => Wrapped.SupportsWriteAccess;

        /// <summary>
        /// This supports <see cref="IEnhancedRandom.Skip(ulong)"/> if the wrapped generator does.
        /// </summary>
        public bool SupportsSkip => Wrapped.SupportsSkip;

        /// <summary>
        /// This supports <see cref="PreviousULong()"/> if the wrapped generator does.
        /// </summary>
        public bool SupportsPrevious => false;

        /// <inheritdoc />
        public IEnhancedRandom Copy() => new ArchivalWrapper(Wrapped.Copy(), _intSeries, _uintSeries, _doubleSeries, _boolSeries, _byteSeries, _floatSeries, _longSeries, _ulongSeries, _decimalSeries);

        /// <inheritdoc/>
        public bool NextBool() {
            bool v = Wrapped.NextBool();
            _boolSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public int NextInt()
        {
            int v = Wrapped.NextInt();
            _intSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public int NextInt(int upperBound)
        {
            int v = Wrapped.NextInt(upperBound);
            _intSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public int NextInt(int minValue, int maxValue)
        {
            int v = Wrapped.NextInt(minValue, maxValue);
            _intSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public uint NextUInt()
        {
            uint v = Wrapped.NextUInt();
            _uintSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public uint NextUInt(uint upperBound)
        {
            uint v = Wrapped.NextUInt(upperBound);
            _uintSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public uint NextBits(int bits)
        {
            uint v = Wrapped.NextBits(bits);
            _uintSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public uint NextUInt(uint minValue, uint maxValue)
        {
            uint v = Wrapped.NextUInt(minValue, maxValue);
            _uintSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public double NextDouble()
        {
            double v = Wrapped.NextDouble();
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextDouble(double outerBound)
        {
            double v = Wrapped.NextDouble(outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextDouble(double innerBound, double outerBound)
        {
            double v = Wrapped.NextDouble(innerBound, outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextSparseDouble()
        {
            double v = Wrapped.NextSparseDouble();
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextSparseDouble(double outerBound)
        {
            double v = Wrapped.NextSparseDouble(outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextSparseDouble(double innerBound, double outerBound)
        {
            double v = Wrapped.NextSparseDouble(innerBound, outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextInclusiveDouble()
        {
            double v = Wrapped.NextInclusiveDouble();
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextInclusiveDouble(double outerBound)
        {
            double v = Wrapped.NextInclusiveDouble(outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextInclusiveDouble(double innerBound, double outerBound)
        {
            double v = Wrapped.NextInclusiveDouble(innerBound, outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextExclusiveDouble()
        {
            double v = Wrapped.NextExclusiveDouble();
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextExclusiveDouble(double outerBound)
        {
            double v = Wrapped.NextExclusiveDouble(outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public double NextExclusiveDouble(double innerBound, double outerBound)
        {
            double v = Wrapped.NextExclusiveDouble(innerBound, outerBound);
            _doubleSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public float NextFloat()
        {
            float v = Wrapped.NextFloat();
            _floatSeries.Add(v);
            return v;
        }


        /// <inheritdoc/>
        public float NextFloat(float outerBound)
        {
            float v = Wrapped.NextFloat(outerBound);
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public float NextFloat(float innerBound, float outerBound)
        {
            float v = Wrapped.NextFloat(innerBound, outerBound);
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public float NextInclusiveFloat()
        {
            float v = Wrapped.NextInclusiveFloat();
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public float NextInclusiveFloat(float outerBound)
        {
            float v = Wrapped.NextInclusiveFloat(outerBound);
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public float NextInclusiveFloat(float innerBound, float outerBound)
        {
            float v = Wrapped.NextInclusiveFloat(innerBound, outerBound);
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public float NextExclusiveFloat()
        {
            float v = Wrapped.NextExclusiveFloat();
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public float NextExclusiveFloat(float outerBound)
        {
            float v = Wrapped.NextExclusiveFloat(outerBound);
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public float NextExclusiveFloat(float innerBound, float outerBound)
        {
            float v = Wrapped.NextExclusiveFloat(innerBound, outerBound);
            _floatSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextDecimal()
        {
            decimal v = Wrapped.NextDecimal();
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextDecimal(decimal outerBound)
        {
            decimal v = Wrapped.NextDecimal(outerBound);
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextDecimal(decimal innerBound, decimal outerBound)
        {
            decimal v = Wrapped.NextDecimal(innerBound, outerBound);
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextInclusiveDecimal()
        {
            decimal v = Wrapped.NextInclusiveDecimal();
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextInclusiveDecimal(decimal outerBound)
        {
            decimal v = Wrapped.NextInclusiveDecimal(outerBound);
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextInclusiveDecimal(decimal innerBound, decimal outerBound)
        {
            decimal v = Wrapped.NextInclusiveDecimal(innerBound, outerBound);
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextExclusiveDecimal()
        {
            decimal v = Wrapped.NextExclusiveDecimal();
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextExclusiveDecimal(decimal outerBound)
        {
            decimal v = Wrapped.NextExclusiveDecimal(outerBound);
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public decimal NextExclusiveDecimal(decimal innerBound, decimal outerBound)
        {
            decimal v = Wrapped.NextExclusiveDecimal(innerBound, outerBound);
            _decimalSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public long NextLong()
        {
            long v = Wrapped.NextLong();
            _longSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public long NextLong(long outerBound)
        {
            long v = Wrapped.NextLong(outerBound);
            _longSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public long NextLong(long innerBound, long outerBound)
        {
            long v = Wrapped.NextLong(innerBound, outerBound);
            _longSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public ulong NextULong()
        {
            ulong v = Wrapped.NextULong();
            _ulongSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public ulong NextULong(ulong outerBound)
        {
            ulong v = Wrapped.NextULong(outerBound);
            _ulongSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public ulong NextULong(ulong innerBound, ulong outerBound)
        {
            ulong v = Wrapped.NextULong(innerBound, outerBound);
            _ulongSeries.Add(v);
            return v;
        }

        /// <inheritdoc/>
        public void NextBytes(Span<byte> bytes)
        {
            Wrapped.NextBytes(bytes);
            for(int i = 0; i < bytes.Length; i++)
            {
                _byteSeries.Add(bytes[i]);
            }
        }

        /// <inheritdoc/>
        public ulong PreviousULong()
        {
            ulong v = Wrapped.PreviousULong();
            _ulongSeries.Add(v);
            return v;
        }

        /// <inheritdoc />
        public void Seed(ulong seed) => Wrapped.Seed(seed);
        /// <inheritdoc />
        public ulong SelectState(int selection) => Wrapped.SelectState(selection);
        /// <inheritdoc />
        public void SetSelectedState(int selection, ulong value) => Wrapped.SetSelectedState(selection, value);
        /// <inheritdoc />
        public void SetState(ulong state) => Wrapped.SetState(state);

        /// <inheritdoc />
        public void SetState(ulong stateA, ulong stateB) => Wrapped.SetState(stateA, stateB);

        /// <inheritdoc />
        public void SetState(ulong stateA, ulong stateB, ulong stateC) => Wrapped.SetState(stateA, stateB, stateC);

        /// <inheritdoc />
        public void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD) => Wrapped.SetState(stateA, stateB, stateC, stateD);

        /// <inheritdoc />
        public void SetState(params ulong[] states) => Wrapped.SetState(states);

        /// <inheritdoc />
        public ulong Skip(ulong distance)
        {
            ulong v = Wrapped.Skip(distance);
            _ulongSeries.Add(v);
            return v;
        }

        /// <inheritdoc />
        public string StringSerialize()
        {
            var ser = new StringBuilder(Serializer.GetTag(this));
            ser.Append('`');
            ser.Append(MakeArchivedSeries().StringSerialize());
            ser.Append('~');
            ser.Append(Wrapped.StringSerialize());
            ser.Append('`');
            return ser.ToString();
        }

        /// <inheritdoc />
        public IEnhancedRandom StringDeserialize(ReadOnlySpan<char> data)
        {
            int breakPoint = data.IndexOf("`~") + 2;
            KnownSeriesRandom ksr = new KnownSeriesRandom();
            ksr.StringDeserialize(data[1..]);
            Wrapped = Serializer.Deserialize(data[breakPoint..]);
            _boolSeries.Clear();
            _byteSeries.Clear();
            _doubleSeries.Clear();
            _floatSeries.Clear();
            _intSeries.Clear();
            _uintSeries.Clear();
            _longSeries.Clear();
            _ulongSeries.Clear();
            _decimalSeries.Clear();

            _boolSeries.AddRange(ksr._boolSeries);
            _byteSeries.AddRange(ksr._byteSeries);
            _doubleSeries.AddRange(ksr._doubleSeries);
            _floatSeries.AddRange(ksr._floatSeries);
            _intSeries.AddRange(ksr._intSeries);
            _uintSeries.AddRange(ksr._uintSeries);
            _longSeries.AddRange(ksr._longSeries);
            _ulongSeries.AddRange(ksr._ulongSeries);
            _decimalSeries.AddRange(ksr._decimalSeries);
            return this;
        }
    }
}
