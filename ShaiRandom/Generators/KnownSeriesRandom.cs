using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Toolkit.HighPerformance;

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
    public class KnownSeriesRandom : IEnhancedRandom
    {
        private int _boolIndex;
        internal readonly List<bool> _boolSeries;
        /// <summary>
        /// Series of booleans returned by this generator.
        /// </summary>
        public IReadOnlyList<bool> BoolSeries => _boolSeries;

        private int _byteIndex;
        internal readonly List<byte> _byteSeries;
        /// <summary>
        /// Series of bytes returned by this generator.
        /// </summary>
        public IReadOnlyList<byte> ByteSeries => _byteSeries;

        private int _doubleIndex;
        internal readonly List<double> _doubleSeries;
        /// <summary>
        /// Series of doubles returned by this generator.
        /// </summary>
        public IReadOnlyList<double> DoubleSeries => _doubleSeries;

        private int _floatIndex;
        internal readonly List<float> _floatSeries;
        /// <summary>
        /// Series of floats returned by this generator.
        /// </summary>
        public IReadOnlyList<float> FloatSeries => _floatSeries;

        private int _intIndex;
        internal readonly List<int> _intSeries;
        /// <summary>
        /// Series of integers returned by this generator.
        /// </summary>
        public IReadOnlyList<int> IntSeries => _intSeries;

        private int _uintIndex;
        internal readonly List<uint> _uintSeries;
        /// <summary>
        /// Series of uints returned by this generator.
        /// </summary>
        public IReadOnlyList<uint> UIntSeries => _uintSeries;

        private int _longIndex;
        internal readonly List<long> _longSeries;
        /// <summary>
        /// Series of longs returned by this generator.
        /// </summary>
        public IReadOnlyList<long> LongSeries => _longSeries;

        private int _ulongIndex;
        internal readonly List<ulong> _ulongSeries;
        /// <summary>
        /// Series of ulong values returned by this generator.
        /// </summary>
        public IReadOnlyList<ulong> ULongSeries => _ulongSeries;

        private int _decimalIndex;
        internal readonly List<decimal> _decimalSeries;
        /// <summary>
        /// Series of decimal values returned by this generator.
        /// </summary>
        public IReadOnlyList<decimal> DecimalSeries => _decimalSeries;

        /// <summary>
        /// Creates a KnownSeriesRandom that is a copy of the given one.
        /// </summary>
        /// <param name="other">Generator to copy state from.</param>
        public KnownSeriesRandom(KnownSeriesRandom other)
            : this(other._intSeries, other._uintSeries, other._doubleSeries, other._boolSeries, other._byteSeries, other._floatSeries, other._longSeries, other._ulongSeries, other._decimalSeries)
        {
            _intIndex = other._intIndex;
            _uintIndex = other._uintIndex;
            _doubleIndex = other._doubleIndex;
            _boolIndex = other._boolIndex;
            _byteIndex = other._byteIndex;
            _floatIndex = other._floatIndex;
            _longIndex = other._longIndex;
            _ulongIndex = other._ulongIndex;
            _decimalIndex = other._decimalIndex;
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
        /// <param name="decimalSeries">Series of values to return via <see cref="NextDecimal()"/>.</param>
        public KnownSeriesRandom(IEnumerable<int>? intSeries = null, IEnumerable<uint>? uintSeries = null,
                                 IEnumerable<double>? doubleSeries = null, IEnumerable<bool>? boolSeries = null,
                                 IEnumerable<byte>? byteSeries = null, IEnumerable<float>? floatSeries = null,
                                 IEnumerable<long>? longSeries = null, IEnumerable<ulong>? ulongSeries = null,
                                 IEnumerable<decimal>? decimalSeries = null)
        {
            _intSeries = intSeries?.ToList() ?? new List<int>();
            _uintSeries = uintSeries?.ToList() ?? new List<uint>();
            _longSeries = longSeries?.ToList() ?? new List<long>();
            _ulongSeries = ulongSeries?.ToList() ?? new List<ulong>();
            _decimalSeries = decimalSeries?.ToList() ?? new List<decimal>();
            _doubleSeries = doubleSeries?.ToList() ?? new List<double>();
            _floatSeries = floatSeries?.ToList() ?? new List<float>();
            _boolSeries = boolSeries?.ToList() ?? new List<bool>();
            _byteSeries = byteSeries?.ToList() ?? new List<byte>();

            Seed(0L);
        }

        /// <summary>
        /// This generator has 9 states; one for each type of IEnumerable taken in the constructor.
        /// </summary>
        public int StateCount => 9;

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
        /// The identifying tag here is "KnSR" .
        /// </summary>
        public string Tag => "KnSR";
        static KnownSeriesRandom()
        {
            AbstractRandom.RegisterTag(new KnownSeriesRandom());
        }

        private static T ReturnIfBetweenBounds<T>(T innerValue, T outerValue, List<T> series, ref int seriesIndex)
            where T : IComparable<T>
        {
            T value = ReturnValueFrom(series, ref seriesIndex);

            // inner < outer; inner inclusive, outer exclusive
            int compareResult = outerValue.CompareTo(innerValue);
            if (compareResult > 0)
            {
                if (value.CompareTo(innerValue) < 0)
                    throw new ArgumentException("Value returned is below the bounds of the generator function call.");

                if (value.CompareTo(outerValue) >= 0)
                    throw new ArgumentException("Value returned is above the bounds of the generator function call.");
            }
            // inner == outer; there is only one valid value
            else if (compareResult == 0)
            {
                if (value.CompareTo(innerValue) != 0)
                    throw new ArgumentException("Value returned is below the bounds of the generator function call.");
            }
            // outer < inner; but outer is still _exclusive_, inner is _inclusive_
            else
            {
                if (value.CompareTo(outerValue) <= 0)
                    throw new ArgumentException("Value returned is below the bounds of the generator function call.");

                if (value.CompareTo(innerValue) > 0)
                    throw new ArgumentException("Value returned is above the bounds of the generator function call.");
            }

            return value;
        }

        private static T ReturnIfBetweenBoundsExclusive<T>(T innerValue, T outerValue, List<T> series, ref int seriesIndex)
            where T : IComparable<T>
        {
            T value = ReturnValueFrom(series, ref seriesIndex);
            var (min, max) = GetMinAndMax(innerValue, outerValue);

            if (min.CompareTo(max) == 0)
            {
                if (value.CompareTo(min) != 0)
                    throw new ArgumentException("Value returned is below the bounds of the generator function call.");

                return value; // If bounds are the same, the bound is returned.
            }

            if (value.CompareTo(min) <= 0)
                throw new ArgumentException("Value returned is below the bounds of the generator function call.");

            if (value.CompareTo(max) >= 0)
                throw new ArgumentException("Value returned is above the bounds of the generator function call.");

            return value;
        }

        private static T ReturnIfBetweenBoundsInclusive<T>(T innerValue, T outerValue, List<T> series, ref int seriesIndex)
            where T : IComparable<T>
        {
            T value = ReturnValueFrom(series, ref seriesIndex);
            var (min, max) = GetMinAndMax(innerValue, outerValue);

            if (min.CompareTo(max) == 0 && value.CompareTo(min) != 0)
                throw new ArgumentException("Value returned is below the bounds of the generator function call.");

            if (value.CompareTo(min) < 0)
                throw new ArgumentException("Value returned is below the bounds of the generator function call.");

            if (value.CompareTo(max) > 0)
                throw new ArgumentException("Value returned is above the bounds of the generator function call.");

            return value;
        }

        private static (T min, T max) GetMinAndMax<T>(T inner, T outer)
            where T : IComparable<T> => inner.CompareTo(outer) <= 0 ? (inner, outer) : (outer, inner);

        private static T ReturnValueFrom<T>(IReadOnlyList<T> series, ref int seriesIndex)
        {
            if (series.Count == 0)
                throw new NotSupportedException($"Tried to get value of type {typeof(T).Name}, but the {nameof(KnownSeriesRandom)} was not given any values of that type.");

            T value = series[seriesIndex];
            seriesIndex = MathUtils.WrapAround(seriesIndex + 1, series.Count);

            return value;
        }

        /// <inheritdoc />
        public IEnhancedRandom Copy() => new KnownSeriesRandom(this);

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
        public int NextInt(int minValue, int maxValue) => ReturnIfBetweenBounds(minValue, maxValue, _intSeries, ref _intIndex);

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
        public uint NextUInt(uint minValue, uint maxValue) => ReturnIfBetweenBounds(minValue, maxValue, _uintSeries, ref _uintIndex);

        /// <summary>
        /// Returns the next double in the underlying series.  If it is outside of the bound [0, 1), throws
        /// an exception.
        /// </summary>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextDouble() => NextDouble(0.0, 1.0);

        /// <summary>
        /// Returns the next double in the underlying series. The inner bound is always 0.0. If the value from the series is outside of the bound specified, throws an exception.
        /// </summary>
        /// <param name="outerBound">The outer bound for the returned double, exclusive.</param>
        /// <returns>The next double in the underlying series, if it is within the bound.</returns>
        public double NextDouble(double outerBound) => NextDouble(0.0, outerBound);

        /// <summary>
        /// Returns the next double in the underlying series. If the value is not between <paramref name="innerBound"/>
        /// (inclusive), and <paramref name="outerBound"/> (exclusive), throws an exception.
        /// </summary>
        /// <param name="innerBound">The inner bound (usually the minimum) for the returned number, inclusive.</param>
        /// <param name="outerBound">The outer bound (usually the maximum) for the returned number, exclusive.</param>
        /// <returns>The next double in the underlying series.</returns>
        public double NextDouble(double innerBound, double outerBound) => ReturnIfBetweenBounds(innerBound, outerBound, _doubleSeries, ref _doubleIndex);

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
        public double NextInclusiveDouble(double minBound, double maxBound) => ReturnIfBetweenBoundsInclusive(minBound, maxBound, _doubleSeries, ref _doubleIndex);

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
        public double NextExclusiveDouble(double minBound, double maxBound) => ReturnIfBetweenBoundsExclusive(minBound, maxBound, _doubleSeries, ref _doubleIndex);

        /// <summary>
        /// Returns the next float in the underlying series.  If it is outside of the bound [0, 1), throws
        /// an exception.
        /// </summary>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextFloat() => NextFloat(0f, 1f);

        /// <summary>
        /// Returns the next float in the underlying series. The inner bound is always 0. If it is outside of the bound specified, throws an exception.
        /// </summary>
        /// <param name="outerBound">The louter bound for the returned float, exclusive.</param>
        /// <returns>The next float in the underlying series, if it is within the bound.</returns>
        public float NextFloat(float outerBound) => NextFloat(0f, outerBound);

        /// <summary>
        /// Returns the next float in the underlying series. If the value is not between <paramref name="innerBound"/>
        /// (inclusive), and <paramref name="outerBound"/> (exclusive), throws an exception.
        /// </summary>
        /// <param name="innerBound">The inner bound (usually the minimum) for the returned number, inclusive.</param>
        /// <param name="outerBound">The outer bound (usually the maximum) for the returned number, exclusive.</param>
        /// <returns>The next float in the underlying series.</returns>
        public float NextFloat(float innerBound, float outerBound) => ReturnIfBetweenBounds(innerBound, outerBound, _floatSeries, ref _floatIndex);

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
        public float NextInclusiveFloat(float minBound, float maxBound) => ReturnIfBetweenBoundsInclusive(minBound, maxBound, _floatSeries, ref _floatIndex);

        /// <summary>
        /// Returns the next decimal in the underlying series.  If it is outside of the bound [0, 1], throws
        /// an exception.
        /// </summary>
        /// <returns>The next decimal in the underlying series, if it is within the bound.</returns>
        public decimal NextInclusiveDecimal() => NextInclusiveDecimal(decimal.Zero, decimal.One);

        /// <summary>
        /// Returns the next decimal in the underlying series.  If it is outside of the bound [0, <paramref name="outerBound"/>], throws
        /// an exception.
        /// </summary>
        /// <param name="outerBound">The maximum value of the returned number, inclusive.</param>
        /// <returns>The next decimal in the underlying series, if it is within the bound.</returns>
        public decimal NextInclusiveDecimal(decimal outerBound) => NextInclusiveDecimal(decimal.Zero, outerBound);

        /// <summary>
        /// Returns the next decimal in the underlying series.  If it is outside of the bound [<paramref name="minBound"/>, <paramref name="maxBound"/>], throws
        /// an exception.
        /// </summary>
        /// <param name="minBound">The minimum value of the returned number, inclusive.</param>
        /// <param name="maxBound">The maximum value of the returned number, inclusive.</param>
        /// <returns>The next decimal in the underlying series, if it is within the bounds.</returns>
        public decimal NextInclusiveDecimal(decimal minBound, decimal maxBound) => ReturnIfBetweenBoundsInclusive(minBound, maxBound, _decimalSeries, ref _decimalIndex);

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
        public float NextExclusiveFloat(float minBound, float maxBound) => ReturnIfBetweenBoundsExclusive(minBound, maxBound, _floatSeries, ref _floatIndex);

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
        public long NextLong(long minValue, long maxValue) => ReturnIfBetweenBounds(minValue, maxValue, _longSeries, ref _longIndex);

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
        public ulong NextULong(ulong minValue, ulong maxValue) => ReturnIfBetweenBounds(minValue, maxValue, _ulongSeries, ref _ulongIndex);

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
        /// Returns the next decimal in the underlying series.  If it is outside of the bound [0, 1), throws an exception.
        /// </summary>
        /// <returns>The next decimal in the underlying series, if it is within the bound.</returns>
        public decimal NextDecimal() => NextDecimal(decimal.Zero, decimal.One);
        /// <summary>
        /// Returns the next decimal in the underlying series.  If it is outside of the bound specified, throws an exception.
        /// </summary>
        /// <returns>The next decimal in the underlying series, if it is within the bound.</returns>
        public decimal NextDecimal(decimal outerBound) => NextDecimal(decimal.Zero, outerBound);

        /// <summary>
        /// Returns the next decimal in the underlying series. If the value is not between <paramref name="innerBound"/>
        /// (inclusive), and <paramref name="outerBound"/> (exclusive), throws an exception.
        /// </summary>
        /// <param name="innerBound">The inner bound (usually the minimum) for the returned number, inclusive.</param>
        /// <param name="outerBound">The outer bound (usually the maximum) for the returned number, exclusive.</param>
        /// <returns>The next decimal in the underlying series.</returns>
        public decimal NextDecimal(decimal innerBound, decimal outerBound) => ReturnIfBetweenBounds(innerBound, outerBound, _decimalSeries, ref _decimalIndex);

        /// <summary>
        /// Returns the next decimal in the underlying series.  If it is outside of the bound (0, 1), throws
        /// an exception.
        /// </summary>
        /// <returns>The next decimal in the underlying series, if it is within the bound.</returns>
        public decimal NextExclusiveDecimal() => NextExclusiveDecimal(decimal.Zero, decimal.One);

        /// <summary>
        /// Returns the next decimal in the underlying series.  If it is outside of the bound ([)0, <paramref name="outerBound"/>), throws
        /// an exception.
        /// </summary>
        /// <param name="outerBound">The maximum value of the returned number, exclusive.</param>
        /// <returns>The next decimal in the underlying series, if it is within the bound.</returns>
        public decimal NextExclusiveDecimal(decimal outerBound) => NextExclusiveDecimal(decimal.Zero, outerBound);

        /// <summary>
        /// Returns the next decimal in the underlying series.  If it is outside of the bound (<paramref name="minBound"/>, <paramref name="maxBound"/>), throws
        /// an exception.
        /// </summary>
        /// <param name="minBound">The minimum value of the returned number, exclusive.</param>
        /// <param name="maxBound">The maximum value of the returned number, exclusive.</param>
        /// <returns>The next decimal in the underlying series, if it is within the bounds.</returns>
        public decimal NextExclusiveDecimal(decimal minBound, decimal maxBound) => ReturnIfBetweenBoundsExclusive(minBound, maxBound, _decimalSeries, ref _decimalIndex);

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
            for (int i = 0; i < StateCount; i++)
                SetSelectedState(i, 0);
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
                8 => (ulong)_decimalIndex,
                _ => throw new ArgumentException($"Invalid selector given to {nameof(SelectState)}.", nameof(selection))
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
            int state = (int)value;
            switch (selection)
            {
                case 0:
                    _intIndex = _intSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _intSeries.Count);
                    break;
                case 1:
                    _uintIndex = _uintSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _uintSeries.Count);
                    break;
                case 2:
                    _doubleIndex = _doubleSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _doubleSeries.Count);
                    break;
                case 3:
                    _boolIndex = _boolSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _boolSeries.Count);
                    break;
                case 4:
                    _byteIndex = _byteSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _byteSeries.Count);
                    break;
                case 5:
                    _floatIndex = _floatSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _floatSeries.Count);
                    break;
                case 6:
                    _longIndex = _longSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _longSeries.Count);
                    break;
                case 7:
                    _ulongIndex = _ulongSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _ulongSeries.Count);
                    break;
                case 8:
                    _decimalIndex = _decimalSeries.Count == 0 ? 0 : MathUtils.WrapAround(state, _decimalSeries.Count);
                    break;
                default:
                    throw new ArgumentException("Invalid selector given to SetSelectedState.", nameof(selection));
            }
        }
        /// <summary>
        /// Sets all the number series to the current index value.
        /// </summary>
        /// <param name="state">Value to set to all of the series indices.</param>
        public void SetState(ulong state) => Seed(state);

        /// <summary>
        /// Sets the current indices in sequences as follows:
        ///     - intSeries, doubleSeries, byteSeries, longSeries, decimalSeries : stateA
        ///     - uintSeries, boolSeries, floatSeries, ulongSeries: stateB
        /// </summary>
        /// <param name="stateA">Index value to set for intSeries, doubleSeries, byteSeries, and longSeries.</param>
        /// <param name="stateB">Index value to set for uintSeries, boolSeries, floatSeries, ulongSeries.</param>
        public void SetState(ulong stateA, ulong stateB) => ((IEnhancedRandom)this).SetState(stateA, stateB);

        /// <summary>
        /// Sets the current indices in sequences as follows:
        ///     - intSeries, boolSeries, longSeries       : stateA
        ///     - uintSeries, byteSeries, ulongSeries     : stateB
        ///     - doubleSeries, floatSeries, decimalSeries: stateC
        /// </summary>
        /// <param name="stateA">Index value to set for intSeries, boolSeries, and longSeries.</param>
        /// <param name="stateB">Index value to set for uintSeries, byteSeries, ulongSeries.</param>
        /// <param name="stateC">Index value to set for doubleSeries, floatSeries, and decimalSeries.</param>
        public void SetState(ulong stateA, ulong stateB, ulong stateC) => ((IEnhancedRandom)this).SetState(stateA, stateB, stateC);

        /// <summary>
        /// Sets the current indices in sequences as follows:
        ///     - intSeries, byteSeries, decimalSeries: stateA
        ///     - uintSeries, floatSeries             : stateB
        ///     - doubleSeries, longSeries            : stateC
        ///     - boolSeries, ulongSeries             : stateC
        /// </summary>
        /// <param name="stateA">Index value to set for intSeries, byteSeries, and decimalSeries.</param>
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

        /// <inheritdoc />
        public IEnhancedRandom StringDeserialize(ReadOnlySpan<char> data)
        {
            int idx = data.IndexOf('`');

            // Int
            _intIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _intSeries.Clear();
            var seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _intSeries.Add(int.Parse(numData));

            // UInt
            _uintIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _uintSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _uintSeries.Add(uint.Parse(numData));

            // Double
            _doubleIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _doubleSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _doubleSeries.Add(double.Parse(numData));

            // Bool
            _boolIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _boolSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _boolSeries.Add(bool.Parse(numData));

            // Byte
            _byteIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _byteSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _byteSeries.Add(byte.Parse(numData));

            // Float
            _floatIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _floatSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _floatSeries.Add(float.Parse(numData));

            // Long
            _longIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _longSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _longSeries.Add(long.Parse(numData));

            // ULong
            _ulongIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _ulongSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1)));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _ulongSeries.Add(ulong.Parse(numData));

            // Decimal
            _decimalIndex = int.Parse(data.Slice(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))));
            _decimalSeries.Clear();
            seriesData = data.Slice(idx + 1, -1 - idx + data.IndexOf('`', idx + 1));
            foreach (var numData in seriesData.Tokenize('|'))
                if (!numData.IsEmpty)
                    _decimalSeries.Add(decimal.Parse(numData));

            return this;
        }

        private void SerializeList<T>(StringBuilder ser, IReadOnlyList<T> series, char lastChar = '~')
        {
            if (series.Count > 0)
            {
                foreach (var item in series)
                {
                    ser.Append(item); ser.Append('|');
                }
<<<<<<< HEAD
                ser.Remove(ser.Length - 1, 1);
            }
=======

                ser.Remove(ser.Length - 1, 1);
            }

>>>>>>> ae6bf48 (Cleanup of serialization code. Added unit test cases for KnownSeriesRandom w/ empty series.)
            ser.Append(lastChar);
        }

        /// <inheritdoc />
        public string StringSerialize()
        {
            StringBuilder ser = new StringBuilder("#KnSR`");
            ser.Append(_intIndex); ser.Append('~');
            SerializeList(ser, _intSeries);
            ser.Append(_uintIndex); ser.Append('~');
            SerializeList(ser, _uintSeries);
            ser.Append(_doubleIndex); ser.Append('~');
            SerializeList(ser, _doubleSeries);
            ser.Append(_boolIndex); ser.Append('~');
            SerializeList(ser, _boolSeries);
            ser.Append(_byteIndex); ser.Append('~');
            SerializeList(ser, _byteSeries);
            ser.Append(_floatIndex); ser.Append('~');
            SerializeList(ser, _floatSeries);
            ser.Append(_longIndex); ser.Append('~');
            SerializeList(ser, _longSeries);
            ser.Append(_ulongIndex); ser.Append('~');
            SerializeList(ser, _ulongSeries);
            ser.Append(_decimalIndex); ser.Append('~');
            SerializeList(ser, _decimalSeries, '`');

            return ser.ToString();
        }
    }
}
