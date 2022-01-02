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
    public class KnownSeriesRandom : IEnhancedRandom, IEquatable<KnownSeriesRandom?>
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

        /// <summary>
        /// Creates a KnownSeriesRandom that is a copy of the given one.
        /// </summary>
        /// <param name="other">Generator to copy state from.</param>
        public KnownSeriesRandom(KnownSeriesRandom other)
            : this(other._intSeries, other._uintSeries, other._doubleSeries, other._boolSeries, other._byteSeries, other._floatSeries, other._longSeries, other._ulongSeries)
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
        /// <remarks>
        /// The values given for each series are looped over repeatedly as the appropriate function is called, so the
        /// RNG functions can be called an arbitrary number of times; doing so will simply result in values from the
        /// sequence being reused.
        /// </remarks>
        /// <param name="intSeries">Series of values to return via <see cref="NextInt()"/>.</param>
        /// <param name="uintSeries">Series of values to return via <see cref="NextUInt()"/>.</param>
        /// <param name="doubleSeries">Series of values to return via <see cref="NextDouble()"/>.</param>
        /// <param name="boolSeries">Series of values to return via <see cref="NextBool()"/>.</param>
        /// <param name="byteSeries">Series of values to return via <see cref="NextBytes(Span&lt;byte&gt;)"/>.</param>
        /// <param name="floatSeries">Series of values to return via <see cref="NextFloat()"/>.</param>
        /// <param name="longSeries">Series of values to return via <see cref="NextLong()"/>.</param>
        /// <param name="ulongSeries">Series of values to return via <see cref="NextULong()"/>.</param>
        public KnownSeriesRandom(IEnumerable<int>? intSeries = null, IEnumerable<uint>? uintSeries = null,
                                 IEnumerable<double>? doubleSeries = null, IEnumerable<bool>? boolSeries = null,
                                 IEnumerable<byte>? byteSeries = null, IEnumerable<float>? floatSeries = null,
                                 IEnumerable<long>? longSeries = null,IEnumerable<ulong>? ulongSeries = null)
        {
            Seed(0L);

            _intSeries = intSeries == null ? new List<int>() : intSeries.ToList();
            _uintSeries = uintSeries == null ? new List<uint>() : uintSeries.ToList();
            _longSeries = longSeries == null ? new List<long>() : longSeries.ToList();
            _ulongSeries = ulongSeries == null ? new List<ulong>() : ulongSeries.ToList();
            _doubleSeries = doubleSeries == null ? new List<double>() : doubleSeries.ToList();
            _floatSeries = floatSeries == null ? new List<float>() : floatSeries.ToList();
            _boolSeries = boolSeries == null ? new List<bool>() : boolSeries.ToList();
            _byteSeries = byteSeries == null ? new List<byte>() : byteSeries.ToList();
        }

        /// <summary>
        /// This generator has 8 states; one for each type of IEnumerable taken in the constructor.
        /// </summary>
        public int StateCount => 8;

        /// <summary>
        /// This supports <see cref="SelectState(int)"/>.
        /// </summary>
        public bool SupportsReadAccess => true;

        /// <summary>
        /// This supports <see cref="SetSelectedState(int, ulong)"/>.
        /// </summary>
        public bool SupportsWriteAccess => true;

        /// <summary>
        /// This does NOT support <see cref="IEnhancedRandom.Skip(ulong)"/>.
        /// </summary>
        public bool SupportsSkip => false;

        /// <summary>
        /// This does NOT support <see cref="PreviousULong()"/>.
        /// </summary>
        public bool SupportsPrevious => false;

        /// <summary>
        /// Generator is not serializable, and thus has no tag.
        /// </summary>
        public string Tag => throw new NotSupportedException("KnownSeriesRandom generators are not serializable, and thus have no Tag.");

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
        public IEnhancedRandom Copy() => new KnownSeriesRandom(this);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is KnownSeriesRandom random && StateCount == random.StateCount && _boolIndex == random._boolIndex && EqualityComparer<List<bool>>.Default.Equals(_boolSeries, random._boolSeries) && _byteIndex == random._byteIndex && EqualityComparer<List<byte>>.Default.Equals(_byteSeries, random._byteSeries) && _doubleIndex == random._doubleIndex && EqualityComparer<List<double>>.Default.Equals(_doubleSeries, random._doubleSeries) && _floatIndex == random._floatIndex && EqualityComparer<List<float>>.Default.Equals(_floatSeries, random._floatSeries) && _intIndex == random._intIndex && EqualityComparer<List<int>>.Default.Equals(_intSeries, random._intSeries) && _uintIndex == random._uintIndex && EqualityComparer<List<uint>>.Default.Equals(_uintSeries, random._uintSeries) && _longIndex == random._longIndex && EqualityComparer<List<long>>.Default.Equals(_longSeries, random._longSeries) && _ulongIndex == random._ulongIndex && EqualityComparer<List<ulong>>.Default.Equals(_ulongSeries, random._ulongSeries);

        /// <inheritdoc />
        public bool Equals(KnownSeriesRandom? random) => random != null && StateCount == random.StateCount && _boolIndex == random._boolIndex && EqualityComparer<List<bool>>.Default.Equals(_boolSeries, random._boolSeries) && _byteIndex == random._byteIndex && EqualityComparer<List<byte>>.Default.Equals(_byteSeries, random._byteSeries) && _doubleIndex == random._doubleIndex && EqualityComparer<List<double>>.Default.Equals(_doubleSeries, random._doubleSeries) && _floatIndex == random._floatIndex && EqualityComparer<List<float>>.Default.Equals(_floatSeries, random._floatSeries) && _intIndex == random._intIndex && EqualityComparer<List<int>>.Default.Equals(_intSeries, random._intSeries) && _uintIndex == random._uintIndex && EqualityComparer<List<uint>>.Default.Equals(_uintSeries, random._uintSeries) && _longIndex == random._longIndex && EqualityComparer<List<long>>.Default.Equals(_longSeries, random._longSeries) && _ulongIndex == random._ulongIndex && EqualityComparer<List<ulong>>.Default.Equals(_ulongSeries, random._ulongSeries);

        /// <summary>
        /// Returns the next boolean value from the underlying series.
        /// </summary>
        /// <returns>The next boolean value from the underlying series.</returns>
        public bool NextBool() => ReturnValueFrom(_boolSeries, ref _boolIndex);

        /// <summary>
        /// Returns the next integer from the underlying series.
        /// </summary>
        /// <returns>The next integer from the underlying series.</returns>
        public int NextInt() => ReturnValueFrom(_intSeries, ref _intIndex);

        /// <summary>
        /// Returns the next integer from underlying series, if it is within the bound; if not,
        /// throws an exception.
        /// </summary>
        /// <param name="outerBound">The upper bound for the returned integer, exclusive.</param>
        /// <returns>The next integer from the underlying series, if it is within the bound.</returns>
        public int NextInt(int outerBound) => NextInt(0, outerBound);

        /// <summary>
        /// Returns the next integer in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next integer in the underlying series.</returns>
        public int NextInt(int minValue, int maxValue) => ReturnIfRange(minValue, maxValue, _intSeries, ref _intIndex);

        /// <summary>
        /// Returns the next uint in the underlying series.
        /// </summary>
        /// <returns>The next uint in the underlying series.</returns>
        public uint NextUInt() => ReturnValueFrom(_uintSeries, ref _uintIndex);

        /// <summary>
        /// Returns the next uint in the underlying series.  If it is outside of the bound specified, throws an exception.
        /// </summary>
        /// <param name="outerBound">The upper bound for the returned uint, exclusive.</param>
        /// <returns>The next uint in the underlying series, if it is within the bound.</returns>
        public uint NextUInt(uint outerBound) => NextUInt(0, outerBound);

        /// <summary>
        /// Uses the next unsigned integer from the underlying series to return the specified number of bits.
        /// </summary>
        /// <param name="bits">Number of bits to return</param>
        /// <returns>An integer containing the specified number of bits.</returns>
        public uint NextBits(int bits) => (bits & 31) == 0 ? NextUInt() : NextUInt(0, 1U << bits);

        /// <summary>
        /// Returns the next unsigned integer in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next unsigned integer in the underlying series.</returns>
        public uint NextUInt(uint minValue, uint maxValue) => ReturnIfRange(minValue, maxValue, _uintSeries, ref _uintIndex);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound [0, 1), throws
        /// an exception.
        /// </summary>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextDouble() => NextDouble(0.0, 1.0);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound specified, throws an exception.
        /// </summary>
        /// <param name="outerBound">The upper bound for the returned double, exclusive.</param>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextDouble(double outerBound) => NextDouble(0.0, outerBound);

        /// <summary>
        /// Returns the next double in the underlying series. If the value is less than
        /// <paramref name="minBound"/>, or greater than/equal to <paramref name="maxBound"/>, throws an exception.
        /// </summary>
        /// <param name="minBound">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxBound">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next double in the underlying series.</returns>
        public double NextDouble(double minBound, double maxBound) => ReturnIfRange(minBound, maxBound, _doubleSeries, ref _doubleIndex);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound [0, 1], throws
        /// an exception.
        /// </summary>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextInclusiveDouble() => NextInclusiveDouble(0f, 1f);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound [0, <paramref name="outerBound"/>], throws
        /// an exception.
        /// </summary>
        /// <param name="outerBound">The maximum value of the returned number, inclusive.</param>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextInclusiveDouble(double outerBound) => NextInclusiveDouble(0f, outerBound);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound [<paramref name="minBound"/>, <paramref name="maxBound"/>], throws
        /// an exception.
        /// </summary>
        /// <param name="minBound">The minimum value of the returned number, inclusive.</param>
        /// <param name="maxBound">The maximum value of the returned number, inclusive.</param>
        /// <returns>The next double in the underlying series, if it is within the bounds.</returns>
        public double NextInclusiveDouble(double minBound, double maxBound) => ReturnIfRangeInclusive(minBound, maxBound, _doubleSeries, ref _doubleIndex);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound (0, 1), throws
        /// an exception.
        /// </summary>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextExclusiveDouble() => NextExclusiveDouble(0f, 1f);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound ([)0, <paramref name="outerBound"/>), throws
        /// an exception.
        /// </summary>
        /// <param name="outerBound">The maximum value of the returned number, exclusive.</param>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextExclusiveDouble(double outerBound) => NextExclusiveDouble(0f, outerBound);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound (<paramref name="minBound"/>, <paramref name="maxBound"/>), throws
        /// an exception.
        /// </summary>
        /// <param name="minBound">The minimum value of the returned number, exclusive.</param>
        /// <param name="maxBound">The maximum value of the returned number, exclusive.</param>
        /// <returns>The next double in the underlying series, if it is within the bounds.</returns>
        public double NextExclusiveDouble(double minBound, double maxBound) => ReturnIfRangeBothExclusive(minBound, maxBound, _doubleSeries, ref _doubleIndex);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound [0, 1), throws
        /// an exception.
        /// </summary>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextFloat() => NextFloat(0f, 1f);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound specified, throws an exception.
        /// </summary>
        /// <param name="outerBound">The upper bound for the returned float, exclusive.</param>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextFloat(float outerBound) => NextFloat(0f, outerBound);

        /// <summary>
        /// Returns the next float in the underlying series. If the value is less than
        /// <paramref name="minBound"/>, or greater than/equal to <paramref name="maxBound"/>, throws an exception.
        /// </summary>
        /// <param name="minBound">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxBound">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next float in the underlying series.</returns>
        public float NextFloat(float minBound, float maxBound) => ReturnIfRange(minBound, maxBound, _floatSeries, ref _floatIndex);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound [0, 1], throws
        /// an exception.
        /// </summary>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextInclusiveFloat() => NextInclusiveFloat(0f, 1f);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound [0, <paramref name="outerBound"/>], throws
        /// an exception.
        /// </summary>
        /// <param name="outerBound">The maximum value of the returned number, inclusive.</param>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextInclusiveFloat(float outerBound) => NextInclusiveFloat(0f, outerBound);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound [<paramref name="minBound"/>, <paramref name="maxBound"/>], throws
        /// an exception.
        /// </summary>
        /// <param name="minBound">The minimum value of the returned number, inclusive.</param>
        /// <param name="maxBound">The maximum value of the returned number, inclusive.</param>
        /// <returns>The next float in the underlying series, if it is within the bounds.</returns>
        public float NextInclusiveFloat(float minBound, float maxBound) => ReturnIfRangeInclusive(minBound, maxBound, _floatSeries, ref _floatIndex);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound (0, 1), throws
        /// an exception.
        /// </summary>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextExclusiveFloat() => NextExclusiveFloat(0f, 1f);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound ([)0, <paramref name="outerBound"/>), throws
        /// an exception.
        /// </summary>
        /// <param name="outerBound">The maximum value of the returned number, exclusive.</param>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextExclusiveFloat(float outerBound) => NextExclusiveFloat(0f, outerBound);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound (<paramref name="minBound"/>, <paramref name="maxBound"/>), throws
        /// an exception.
        /// </summary>
        /// <param name="minBound">The minimum value of the returned number, exclusive.</param>
        /// <param name="maxBound">The maximum value of the returned number, exclusive.</param>
        /// <returns>The next float in the underlying series, if it is within the bounds.</returns>
        public float NextExclusiveFloat(float minBound, float maxBound) => ReturnIfRangeBothExclusive(minBound, maxBound, _floatSeries, ref _floatIndex);

        /// <summary>
        /// Returns the next long from the underlying series.
        /// </summary>
        /// <returns>The next long from the underlying series.</returns>
        public long NextLong() => ReturnValueFrom(_longSeries, ref _longIndex);

        /// <summary>
        /// Returns the next long from underlying series, if it is within the bound; if not,
        /// throws an exception.
        /// </summary>
        /// <param name="outerBound">The upper bound for the returned long, exclusive.</param>
        /// <returns>The next long from the underlying series, if it is within the bound.</returns>
        public long NextLong(long outerBound) => NextLong(0, outerBound);

        /// <summary>
        /// Returns the next long in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next long in the underlying series.</returns>
        public long NextLong(long minValue, long maxValue) => ReturnIfRange(minValue, maxValue, _longSeries, ref _longIndex);

        /// <summary>
        /// Returns the next ulong from the underlying series.
        /// </summary>
        /// <returns>The next ulong from the underlying series.</returns>
        public ulong NextULong() => ReturnValueFrom(_ulongSeries, ref _ulongIndex);

        /// <summary>
        /// Returns the next ulong from underlying series, if it is within the bound; if not,
        /// throws an exception.
        /// </summary>
        /// <param name="outerBound">The upper bound for the returned ulong, exclusive.</param>
        /// <returns>The next ulong from the underlying series, if it is within the bound.</returns>
        public ulong NextULong(ulong outerBound) => NextULong(0, outerBound);

        /// <summary>
        /// Returns the next ulong in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next ulong in the underlying series.</returns>
        public ulong NextULong(ulong minValue, ulong maxValue) => ReturnIfRange(minValue, maxValue, _ulongSeries, ref _ulongIndex);

        /// <summary>
        /// Fills the specified buffer with values from the underlying byte series.  See <see cref="IEnhancedRandom.NextBytes"/>
        /// for detailed examples on various uses.
        /// </summary>
        /// <param name="bytes">Buffer to fill.</param>
        public void NextBytes(Span<byte> bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = ReturnValueFrom(_byteSeries, ref _byteIndex);
        }

        /// <summary>
        /// Not supported by this generator.
        /// </summary>
        public ulong PreviousULong() => throw new NotSupportedException();

        /// <summary>
        /// Sets the current index of each given sequence to the given seed value.
        /// </summary>
        /// <param name="seed">Index for the sequences.</param>
        public void Seed(ulong seed)
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

        /// <summary>
        /// Retrieves the index of a given series based on the selection given. The selection values start at 0, and
        /// they correspond to the constructor sequences as follows:
        ///     - 0: intSeries
        ///     - 1: uintSeries
        ///     - 2: doubleSeries
        ///     - 3: boolSeries
        ///     - 4: byteSeries
        ///     - 5: floatSeries
        ///     - 6: longSeries
        ///     - 7: ulongSeries
        /// </summary>
        /// <param name="selection">Selection value.</param>
        /// <returns>The index of the selected series that will be returned next time that series is used.</returns>
        public ulong SelectState(int selection)
        {
            return selection switch
            {
                0 => (ulong)_intIndex,
                1 => (ulong)_uintIndex,
                2 => (ulong)_doubleIndex,
                3 => (ulong)_boolIndex,
                4 => (ulong)_byteIndex,
                5 => (ulong)_floatIndex,
                6 => (ulong)_longIndex,
                7 => (ulong)_ulongIndex,
                _ => throw new ArgumentException("Invalid selector given to SelectState.", nameof(selection))
            };
        }

        /// <summary>
        /// Sets the index for the given number series to the given value.
        /// </summary>
        /// <remarks>
        /// The selection values start at 0, and they correspond to the constructor sequences as follows:
        ///     - 0: intSeries
        ///     - 1: uintSeries
        ///     - 2: doubleSeries
        ///     - 3: boolSeries
        ///     - 4: byteSeries
        ///     - 5: floatSeries
        ///     - 6: longSeries
        ///     - 7: ulongSeries
        /// </remarks>
        /// <param name="selection">Selection value of index to set.</param>
        /// <param name="value">Value to set the index to.</param>
        public void SetSelectedState(int selection, ulong value)
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
        /// <summary>
        /// Sets all the number series to the current index value.
        /// </summary>
        /// <param name="state">Value to set to all of the series indices.</param>
        public void SetState(ulong state) => Seed(state);

        /// <summary>
        /// Sets the current indices in sequences as follows:
        ///     - intSeries, doubleSeries, byteSeries, longSeries : stateA
        ///     - uintSeries, boolSeries, floatSeries, ulongSeries: stateB
        /// </summary>
        /// <param name="stateA">Index value to set for intSeries, doubleSeries, byteSeries, and longSeries.</param>
        /// <param name="stateB">Index value to set for uintSeries, boolSeries, floatSeries, ulongSeries.</param>
        public void SetState(ulong stateA, ulong stateB) => ((IEnhancedRandom)this).SetState(stateA, stateB);

        /// <summary>
        /// Sets the current indices in sequences as follows:
        ///     - intSeries, boolSeries, longSeries  : stateA
        ///     - uintSeries, byteSeries, ulongSeries: stateB
        ///     - doubleSeries, floatSeries          : stateC
        /// </summary>
        /// <param name="stateA">Index value to set for intSeries, boolSeries, and longSeries.</param>
        /// <param name="stateB">Index value to set for uintSeries, byteSeries, ulongSeries.</param>
        /// <param name="stateC">Index value to set for doubleSeries and floatSeries.</param>
        public void SetState(ulong stateA, ulong stateB, ulong stateC) => ((IEnhancedRandom)this).SetState(stateA, stateB, stateC);

        /// <summary>
        /// Sets the current indices in sequences as follows:
        ///     - intSeries, byteSeries   : stateA
        ///     - uintSeries, floatSeries : stateB
        ///     - doubleSeries, longSeries: stateC
        ///     - boolSeries, ulongSeries : stateC
        /// </summary>
        /// <param name="stateA">Index value to set for intSeries and byteSeries.</param>
        /// <param name="stateB">Index value to set for uintSeries and floatSeries.</param>
        /// <param name="stateC">Index value to set for doubleSeries and longSeries.</param>
        /// <param name="stateD">Index value to set for boolSeries and ulongSeries.</param>
        public void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD) => ((IEnhancedRandom)this).SetState(stateA, stateB, stateC, stateD);

        /// <inheritdoc />
        public void SetState(params ulong[] states) => ((IEnhancedRandom)this).SetState(states);

        /// <summary>
        /// Not supported by this generator.
        /// </summary>
        public ulong Skip(ulong distance) => throw new NotSupportedException();

        /// <summary>
        /// Serialization is not supported by this generator.
        /// </summary>
        public IEnhancedRandom StringDeserialize(string data) => throw new NotSupportedException();

        /// <summary>
        /// Serialization is not supported by this generator.
        /// </summary>
        public string StringSerialize() => throw new NotSupportedException();

        public static bool operator ==(KnownSeriesRandom? left, KnownSeriesRandom? right) => EqualityComparer<KnownSeriesRandom>.Default.Equals(left, right);
        public static bool operator !=(KnownSeriesRandom? left, KnownSeriesRandom? right) => !(left == right);
    }
}
