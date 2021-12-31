using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// "Random number generator" that takes in a series of values, and simply returns them
    /// sequentially when RNG functions are called.
    /// </summary>
    /// <remarks>
    /// This class may be useful for testing, when you want to specify the numbers returned by an RNG
    /// without drastically modifying any code using the RNG.
    ///
    /// This class is mostly from GoRogue, with some modifications for ShaiRandom's API.
    /// </remarks>
    public class KnownSeriesRandom : AbstractRandom, IEquatable<KnownSeriesRandom?>
    {
        private int _boolIndex;
        private readonly List<bool> _boolSeries;
        private int _byteIndex;
        private readonly List<byte> _byteSeries;
        private int _doubleIndex;
        private readonly List<double> _doubleSeries;
        private int _floatIndex;
        private readonly List<float> _floatSeries;
        private int _intIndex;
        private readonly List<int> _intSeries;
        private int _uintIndex;
        private readonly List<uint> _uintSeries;
        private int _longIndex;
        private readonly List<long> _longSeries;
        private int _ulongIndex;
        private readonly List<ulong> _ulongSeries;

        public KnownSeriesRandom() : this(0UL)
        {
        }

        public KnownSeriesRandom(ulong seed) : this(null, null, null, null, null, null, null, null)
        {
            Seed(seed);
        }

        public KnownSeriesRandom(KnownSeriesRandom other) : this(other._intSeries, other._uintSeries, other._doubleSeries, other._boolSeries, other._byteSeries, other._floatSeries, other._longSeries, other._ulongSeries)
        {
            _intIndex = other._intIndex;
            _uintIndex = other._uintIndex;
            _doubleIndex = other._doubleIndex;
            _boolIndex = other._boolIndex;
            _byteIndex = other._byteIndex;
            _floatIndex = other._floatIndex;
            _longIndex = other._longIndex;
            _ulongIndex = other._ulongIndex;
        }

        /// <summary>
        /// Creates a new known series generator, with parameters to indicate which series to use for
        /// the integer, unsigned integer, double, bool, and byte-based RNG functions. If null is
        /// specified, no values of that type may be returned, and functions that try to return a
        /// value of that type will throw an exception.
        /// </summary>
        public KnownSeriesRandom(IEnumerable<int>? intSeries = null, IEnumerable<uint>? uintSeries = null, IEnumerable<double>? doubleSeries = null, IEnumerable<bool>? boolSeries = null, IEnumerable<byte>? byteSeries = null,
            IEnumerable<float>? floatSeries = null, IEnumerable<long>? longSeries = null, IEnumerable<ulong>? ulongSeries = null) : base(0UL)
        {
            if (intSeries == null)
                _intSeries = new List<int>();
            else
                _intSeries = intSeries.ToList();

            if (uintSeries == null)
                _uintSeries = new List<uint>();
            else
                _uintSeries = uintSeries.ToList();

            if (longSeries == null)
                _longSeries = new List<long>();
            else
                _longSeries = longSeries.ToList();

            if (ulongSeries == null)
                _ulongSeries = new List<ulong>();
            else
                _ulongSeries = ulongSeries.ToList();

            if (doubleSeries == null)
                _doubleSeries = new List<double>();
            else
                _doubleSeries = doubleSeries.ToList();

            if (floatSeries == null)
                _floatSeries = new List<float>();
            else
                _floatSeries = floatSeries.ToList();

            if (boolSeries == null)
                _boolSeries = new List<bool>();
            else
                _boolSeries = boolSeries.ToList();

            if (byteSeries == null)
                _byteSeries = new List<byte>();
            else
                _byteSeries = byteSeries.ToList();
        }

        /// <summary>
        /// This generator has 8 states; one for each type of IEnumerable taken in the constructor.
        /// </summary>
        public override int StateCount => 8;

        /// <summary>
        /// This supports <see cref="SelectState(int)"/>.
        /// </summary>
        public override bool SupportsReadAccess => true;

        /// <summary>
        /// This supports <see cref="SetSelectedState(int, ulong)"/>.
        /// </summary>
        public override bool SupportsWriteAccess => true;

        /// <summary>
        /// This does NOT support <see cref="IEnhancedRandom.Skip(ulong)"/>.
        /// </summary>
        public override bool SupportsSkip => false;

        /// <summary>
        /// This does NOT support <see cref="PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => false;

        /// <summary>
        /// Generator is not serializable, and thus has no tag.
        /// </summary>
        public override string Tag => "";

        private static T ReturnIfRange<T>(T minValue, T maxValue, List<T> series, ref int seriesIndex) where T : IComparable<T>
        {
            T value = ReturnValueFrom(series, ref seriesIndex);

            if (minValue.CompareTo(value) < 0)
                throw new ArgumentException("Value returned is less than minimum value.");

            if (maxValue.CompareTo(value) >= 0)
                throw new ArgumentException("Value returned is greater than/equal to maximum value.");

            return value;
        }

        private static T ReturnIfRangeBothExclusive<T>(T minValue, T maxValue, List<T> series, ref int seriesIndex) where T : IComparable<T>
        {
            T value = ReturnValueFrom(series, ref seriesIndex);

            if (minValue.CompareTo(value) <= 0)
                throw new ArgumentException("Value returned is less than/equal to minimum value.");

            if (maxValue.CompareTo(value) >= 0)
                throw new ArgumentException("Value returned is greater than/equal to maximum value.");

            return value;
        }

        private static T ReturnIfRangeInclusive<T>(T minValue, T maxValue, List<T> series, ref int seriesIndex) where T : IComparable<T>
        {
            T value = ReturnValueFrom(series, ref seriesIndex);

            if (minValue.CompareTo(value) < 0)
                throw new ArgumentException("Value returned is less than minimum value.");

            if (maxValue.CompareTo(value) > 0)
                throw new ArgumentException("Value returned is greater than/equal to maximum value.");

            return value;
        }

        private static T ReturnValueFrom<T>(IReadOnlyList<T> series, ref int seriesIndex)
        {
            if (series.Count == 0)
                throw new NotSupportedException("Tried to get value of type " + typeof(T).Name + ", but the KnownSeriesGenerator was not given any values of that type.");

            T value = series[seriesIndex];
            seriesIndex = MathUtils.WrapAround(seriesIndex + 1, series.Count);

            return value;
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new KnownSeriesRandom(this);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is KnownSeriesRandom random && StateCount == random.StateCount && _boolIndex == random._boolIndex && EqualityComparer<List<bool>>.Default.Equals(_boolSeries, random._boolSeries) && _byteIndex == random._byteIndex && EqualityComparer<List<byte>>.Default.Equals(_byteSeries, random._byteSeries) && _doubleIndex == random._doubleIndex && EqualityComparer<List<double>>.Default.Equals(_doubleSeries, random._doubleSeries) && _floatIndex == random._floatIndex && EqualityComparer<List<float>>.Default.Equals(_floatSeries, random._floatSeries) && _intIndex == random._intIndex && EqualityComparer<List<int>>.Default.Equals(_intSeries, random._intSeries) && _uintIndex == random._uintIndex && EqualityComparer<List<uint>>.Default.Equals(_uintSeries, random._uintSeries) && _longIndex == random._longIndex && EqualityComparer<List<long>>.Default.Equals(_longSeries, random._longSeries) && _ulongIndex == random._ulongIndex && EqualityComparer<List<ulong>>.Default.Equals(_ulongSeries, random._ulongSeries);

        /// <inheritdoc />
        public bool Equals(KnownSeriesRandom? random) => random != null && StateCount == random.StateCount && _boolIndex == random._boolIndex && EqualityComparer<List<bool>>.Default.Equals(_boolSeries, random._boolSeries) && _byteIndex == random._byteIndex && EqualityComparer<List<byte>>.Default.Equals(_byteSeries, random._byteSeries) && _doubleIndex == random._doubleIndex && EqualityComparer<List<double>>.Default.Equals(_doubleSeries, random._doubleSeries) && _floatIndex == random._floatIndex && EqualityComparer<List<float>>.Default.Equals(_floatSeries, random._floatSeries) && _intIndex == random._intIndex && EqualityComparer<List<int>>.Default.Equals(_intSeries, random._intSeries) && _uintIndex == random._uintIndex && EqualityComparer<List<uint>>.Default.Equals(_uintSeries, random._uintSeries) && _longIndex == random._longIndex && EqualityComparer<List<long>>.Default.Equals(_longSeries, random._longSeries) && _ulongIndex == random._ulongIndex && EqualityComparer<List<ulong>>.Default.Equals(_ulongSeries, random._ulongSeries);

        public override bool NextBool() => ReturnValueFrom(_boolSeries, ref _boolIndex);

        public new bool NextBool(float chance) => NextBool();

        public new int NextInt() => ReturnValueFrom(_intSeries, ref _intIndex);
        public new int NextInt(int outerBound) => NextInt(0, outerBound);

        /// <summary>
        /// Returns the next integer in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next integer in the underlying series.</returns>
        public new int NextInt(int minValue, int maxValue) => ReturnIfRange(minValue, maxValue, _intSeries, ref _intIndex);
        public new uint NextUInt() => ReturnValueFrom(_uintSeries, ref _uintIndex);
        public new uint NextUInt(uint outerBound) => NextUInt(0, outerBound);
        public new uint NextBits(int bits) => (bits & 31) == 0 ? NextUInt() : NextUInt(0, 1U << bits);

        /// <summary>
        /// Returns the next unsigned integer in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next unsigned integer in the underlying series.</returns>
        public new uint NextUInt(uint minValue, uint maxValue) => ReturnIfRange(minValue, maxValue, _uintSeries, ref _uintIndex);

        public override double NextDouble() => ReturnValueFrom(_doubleSeries, ref _doubleIndex);
        public new double NextDouble(double outerBound) => NextDouble(0.0, outerBound);
        public new double NextDouble(double minBound, double maxBound) => ReturnIfRange(minBound, maxBound, _doubleSeries, ref _doubleIndex);
        public new double NextInclusiveDouble() => NextInclusiveDouble(0f, 1f);
        public new double NextInclusiveDouble(double outerBound) => NextInclusiveDouble(0f, outerBound);
        public new double NextInclusiveDouble(double minBound, double maxBound) => ReturnIfRangeInclusive(minBound, maxBound, _doubleSeries, ref _doubleIndex);
        public new double NextExclusiveDouble() => NextExclusiveDouble(0f, 1f);
        public new double NextExclusiveDouble(double outerBound) => NextExclusiveDouble(0f, outerBound);
        public new double NextExclusiveDouble(double minBound, double maxBound) => ReturnIfRangeBothExclusive(minBound, maxBound, _doubleSeries, ref _doubleIndex);
        public override float NextFloat() => ReturnValueFrom(_floatSeries, ref _floatIndex);
        public new float NextFloat(float outerBound) => NextFloat(0f, outerBound);
        public new float NextFloat(float minBound, float maxBound) => ReturnIfRange(minBound, maxBound, _floatSeries, ref _floatIndex);
        public new float NextInclusiveFloat() => NextInclusiveFloat(0f, 1f);
        public new float NextInclusiveFloat(float outerBound) => NextInclusiveFloat(0f, outerBound);
        public new float NextInclusiveFloat(float minBound, float maxBound) => ReturnIfRangeInclusive(minBound, maxBound, _floatSeries, ref _floatIndex);
        public new float NextExclusiveFloat() => NextExclusiveFloat(0f, 1f);
        public new float NextExclusiveFloat(float outerBound) => NextExclusiveFloat(0f, outerBound);
        public new float NextExclusiveFloat(float minBound, float maxBound) => ReturnIfRangeBothExclusive(minBound, maxBound, _floatSeries, ref _floatIndex);
        public new float NextTriangular()
        {
            NextFloat(); // Used only to advance state the same number of times.
            return NextExclusiveFloat(-1f, 1f);
        }
        public new float NextTriangular(float min, float max) => NextExclusiveFloat(min, max);
        public new float NextTriangular(float min, float max, float mode) => NextExclusiveFloat(min, max);
        public new long NextLong() => ReturnValueFrom(_longSeries, ref _longIndex);
        public new long NextLong(long outerBound) => NextLong(0, outerBound);
        /// <summary>
        /// Returns the next long in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next long in the underlying series.</returns>
        public new long NextLong(long minValue, long maxValue) => ReturnIfRange(minValue, maxValue, _longSeries, ref _longIndex);

        public override ulong NextULong() => ReturnValueFrom(_ulongSeries, ref _ulongIndex);
        public new ulong NextULong(ulong outerBound) => NextULong(0, outerBound);
        /// <summary>
        /// Returns the next ulong in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next ulong in the underlying series.</returns>
        public new ulong NextULong(ulong minValue, ulong maxValue) => ReturnIfRange(minValue, maxValue, _ulongSeries, ref _ulongIndex);


        /// <summary>
        /// Fills the specified buffer with values from the underlying byte series.
        /// </summary>
        /// <param name="buffer">Buffer to fill.</param>
        public new void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = ReturnValueFrom(_byteSeries, ref _byteIndex);
        }
        public override ulong PreviousULong() => throw new NotImplementedException();
        public override void Seed(ulong seed)
        {
            int idx = (int)seed;
            _intIndex = idx;
            _uintIndex = idx;
            _doubleIndex = idx;
            _boolIndex = idx;
            _byteIndex = idx;
            _floatIndex = idx;
            _longIndex = idx;
            _ulongIndex = idx;
        }
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 0: return (ulong)_intIndex;
                case 1: return (ulong)_uintIndex;
                case 2: return (ulong)_doubleIndex;
                case 3: return (ulong)_boolIndex;
                case 4: return (ulong)_byteIndex;
                case 5: return (ulong)_floatIndex;
                case 6: return (ulong)_longIndex;
                default: return (ulong)_ulongIndex;
            }
        }
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 0: _intIndex = (int)value; break;
                case 1: _uintIndex = (int)value; break;
                case 2: _doubleIndex = (int)value; break;
                case 3: _boolIndex = (int)value; break;
                case 4: _byteIndex = (int)value; break;
                case 5: _floatIndex = (int)value; break;
                case 6: _longIndex = (int)value; break;
                default: _ulongIndex = (int)value; break;
            }
        }
        public override void SetState(ulong state) => Seed(state);
        public override void SetState(ulong stateA, ulong stateB) => Seed(stateA);
        public override void SetState(ulong stateA, ulong stateB, ulong stateC) => Seed(stateA);
        public override void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD) => Seed(stateA);
        public override void SetState(params ulong[] states) => base.SetState(states);
        public override ulong Skip(ulong distance) => throw new NotImplementedException();
        public override IEnhancedRandom StringDeserialize(string data) => throw new NotImplementedException();
        public override string StringSerialize() => throw new NotImplementedException();

        public static bool operator ==(KnownSeriesRandom? left, KnownSeriesRandom? right) => EqualityComparer<KnownSeriesRandom>.Default.Equals(left, right);
        public static bool operator !=(KnownSeriesRandom? left, KnownSeriesRandom? right) => !(left == right);
    }
}
