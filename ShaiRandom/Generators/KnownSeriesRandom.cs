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
        private int boolIndex;
        private List<bool> boolSeries;
        private int byteIndex;
        private List<byte> byteSeries;
        private int doubleIndex;
        private List<double> doubleSeries;
        private int floatIndex;
        private List<float> floatSeries;
        private int intIndex;
        private List<int> intSeries;
        private int uintIndex;
        private List<uint> uintSeries;
        private int longIndex;
        private List<long> longSeries;
        private int ulongIndex;
        private List<ulong> ulongSeries;

        public KnownSeriesRandom() : this(0UL)
        {
        }

        public KnownSeriesRandom(ulong seed) : this(null, null, null, null, null, null, null, null)
        {
            Seed(seed);
        }

        public KnownSeriesRandom(KnownSeriesRandom other) : this(other.intSeries, other.uintSeries, other.doubleSeries, other.boolSeries, other.byteSeries, other.floatSeries, other.longSeries, other.ulongSeries)
        {
            intIndex = other.intIndex;
            uintIndex = other.uintIndex;
            doubleIndex = other.doubleIndex;
            boolIndex = other.boolIndex;
            byteIndex = other.byteIndex;
            floatIndex = other.floatIndex;
            longIndex = other.longIndex;
            ulongIndex = other.ulongIndex;
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
                this.intSeries = new List<int>();
            else
                this.intSeries = intSeries.ToList();

            if (uintSeries == null)
                this.uintSeries = new List<uint>();
            else
                this.uintSeries = uintSeries.ToList();

            if (longSeries == null)
                this.longSeries = new List<long>();
            else
                this.longSeries = longSeries.ToList();

            if (ulongSeries == null)
                this.ulongSeries = new List<ulong>();
            else
                this.ulongSeries = ulongSeries.ToList();

            if (doubleSeries == null)
                this.doubleSeries = new List<double>();
            else
                this.doubleSeries = doubleSeries.ToList();

            if (floatSeries == null)
                this.floatSeries = new List<float>();
            else
                this.floatSeries = floatSeries.ToList();

            if (boolSeries == null)
                this.boolSeries = new List<bool>();
            else
                this.boolSeries = boolSeries.ToList();

            if (byteSeries == null)
                this.byteSeries = new List<byte>();
            else
                this.byteSeries = byteSeries.ToList();
        }

        public override int StateCount => 8;

        public override bool SupportsReadAccess => true;

        public override bool SupportsWriteAccess => true;

        public override bool SupportsSkip => false;

        public override bool SupportsPrevious => false;

        public override string Tag => "";

        private static T returnIfRange<T>(T minValue, T maxValue, List<T> series, ref int seriesIndex) where T : IComparable<T>
        {
            T value = returnValueFrom(series, ref seriesIndex);

            if (minValue.CompareTo(value) < 0)
                throw new ArgumentException("Value returned is less than minimum value.");

            if (maxValue.CompareTo(value) >= 0)
                throw new ArgumentException("Value returned is greater than/equal to maximum value.");

            return value;
        }

        private static T returnIfRangeBothExclusive<T>(T minValue, T maxValue, List<T> series, ref int seriesIndex) where T : IComparable<T>
        {
            T value = returnValueFrom(series, ref seriesIndex);

            if (minValue.CompareTo(value) <= 0)
                throw new ArgumentException("Value returned is less than/equal to minimum value.");

            if (maxValue.CompareTo(value) >= 0)
                throw new ArgumentException("Value returned is greater than/equal to maximum value.");

            return value;
        }

        private static T returnIfRangeInclusive<T>(T minValue, T maxValue, List<T> series, ref int seriesIndex) where T : IComparable<T>
        {
            T value = returnValueFrom(series, ref seriesIndex);

            if (minValue.CompareTo(value) < 0)
                throw new ArgumentException("Value returned is less than minimum value.");

            if (maxValue.CompareTo(value) > 0)
                throw new ArgumentException("Value returned is greater than/equal to maximum value.");

            return value;
        }

        private static T returnValueFrom<T>(List<T> series, ref int seriesIndex)
        {
            if (series.Count == 0)
                throw new NotSupportedException("Tried to get value of type " + typeof(T).Name + ", but the KnownSeriesGenerator was not given any values of that type.");

            T value = series[seriesIndex];
            seriesIndex = WrapAround(seriesIndex + 1, series.Count);

            return value;
        }

        // Basically modulo for array indices, solves - num issues. (-1, 3) is 2.
        /// <summary>
        /// A modified modulo operator, which practically differs from <paramref name="num"/> / <paramref name="wrapTo"/>
        /// in that it wraps from 0 to <paramref name="wrapTo"/> - 1, as well as from <paramref name="wrapTo"/> - 1 to 0.
        /// </summary>
        /// <remarks>
        /// A modified modulo operator. Returns the result of  the formula
        /// (<paramref name="num"/> % <paramref name="wrapTo"/> + <paramref name="wrapTo"/>) % <paramref name="wrapTo"/>.
        /// 
        /// Practically it differs from regular modulo in that the values it returns when negative values for <paramref name="num"/>
        /// are wrapped around like one would want an array index to (if wrapTo is list.length, -1 wraps to list.length - 1). For example,
        /// 0 % 3 = 0, -1 % 3 = -1, -2 % 3 = -2, -3 % 3 = 0, and so forth, but WrapTo(0, 3) = 0,
        /// WrapTo(-1, 3) = 2, WrapTo(-2, 3) = 1, WrapTo(-3, 3) = 0, and so forth. This can be useful if you're
        /// trying to "wrap" a number around at both ends, for example wrap to 3, such that 3 wraps
        /// to 0, and -1 wraps to 2. This is common if you are wrapping around an array index to the
        /// length of the array and need to ensure that positive numbers greater than or equal to the
        /// length of the array wrap to the beginning of the array (index 0), AND that negative
        /// numbers (under 0) wrap around to the end of the array (Length - 1).
        /// </remarks>
        /// <param name="num">The number to wrap.</param>
        /// <param name="wrapTo">
        /// The number to wrap to -- the result of the function is as outlined in function
        /// description, and guaranteed to be between [0, wrapTo - 1], inclusive.
        /// </param>
        /// <returns>
        /// The wrapped result, as outlined in function description. Guaranteed to lie in range [0,
        /// wrapTo - 1], inclusive.
        /// </returns>
        private static int WrapAround(int num, int wrapTo) => (num % wrapTo + wrapTo) % wrapTo;
        public override IEnhancedRandom Copy() => new KnownSeriesRandom(this);
        public override bool Equals(object? obj) => obj is KnownSeriesRandom random && StateCount == random.StateCount && boolIndex == random.boolIndex && EqualityComparer<List<bool>>.Default.Equals(boolSeries, random.boolSeries) && byteIndex == random.byteIndex && EqualityComparer<List<byte>>.Default.Equals(byteSeries, random.byteSeries) && doubleIndex == random.doubleIndex && EqualityComparer<List<double>>.Default.Equals(doubleSeries, random.doubleSeries) && floatIndex == random.floatIndex && EqualityComparer<List<float>>.Default.Equals(floatSeries, random.floatSeries) && intIndex == random.intIndex && EqualityComparer<List<int>>.Default.Equals(intSeries, random.intSeries) && uintIndex == random.uintIndex && EqualityComparer<List<uint>>.Default.Equals(uintSeries, random.uintSeries) && longIndex == random.longIndex && EqualityComparer<List<long>>.Default.Equals(longSeries, random.longSeries) && ulongIndex == random.ulongIndex && EqualityComparer<List<ulong>>.Default.Equals(ulongSeries, random.ulongSeries);
        public bool Equals(KnownSeriesRandom? random) => random != null && StateCount == random.StateCount && boolIndex == random.boolIndex && EqualityComparer<List<bool>>.Default.Equals(boolSeries, random.boolSeries) && byteIndex == random.byteIndex && EqualityComparer<List<byte>>.Default.Equals(byteSeries, random.byteSeries) && doubleIndex == random.doubleIndex && EqualityComparer<List<double>>.Default.Equals(doubleSeries, random.doubleSeries) && floatIndex == random.floatIndex && EqualityComparer<List<float>>.Default.Equals(floatSeries, random.floatSeries) && intIndex == random.intIndex && EqualityComparer<List<int>>.Default.Equals(intSeries, random.intSeries) && uintIndex == random.uintIndex && EqualityComparer<List<uint>>.Default.Equals(uintSeries, random.uintSeries) && longIndex == random.longIndex && EqualityComparer<List<long>>.Default.Equals(longSeries, random.longSeries) && ulongIndex == random.ulongIndex && EqualityComparer<List<ulong>>.Default.Equals(ulongSeries, random.ulongSeries);
        public override bool NextBool() => returnValueFrom(boolSeries, ref boolIndex);
        public new bool NextBool(float chance) => NextBool();

        public new int NextInt() => returnValueFrom(intSeries, ref intIndex);
        public new int NextInt(int outerBound) => NextInt(0, outerBound);
        /// <summary>
        /// Returns the next integer in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next integer in the underlying series.</returns>
        public new int NextInt(int minValue, int maxValue) => returnIfRange(minValue, maxValue, intSeries, ref intIndex);
        public new uint NextUInt() => returnValueFrom(uintSeries, ref uintIndex);
        public new uint NextUInt(uint outerBound) => NextUInt(0, outerBound);
        public new uint NextBits(int bits) => (bits & 31) == 0 ? NextUInt() : NextUInt(0, 1U << bits);

        /// <summary>
        /// Returns the next unsigned integer in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next unsigned integer in the underlying series.</returns>
        public new uint NextUInt(uint minValue, uint maxValue) => returnIfRange(minValue, maxValue, uintSeries, ref uintIndex);
        public override double NextDouble() => returnValueFrom(doubleSeries, ref doubleIndex);
        public new double NextDouble(double outerBound) => NextDouble(0.0, outerBound);
        public new double NextDouble(double minBound, double maxBound) => returnIfRange(minBound, maxBound, doubleSeries, ref doubleIndex);
        public new double NextInclusiveDouble() => NextInclusiveDouble(0f, 1f);
        public new double NextInclusiveDouble(double outerBound) => NextInclusiveDouble(0f, outerBound);
        public new double NextInclusiveDouble(double minBound, double maxBound) => returnIfRangeInclusive(minBound, maxBound, doubleSeries, ref doubleIndex);
        public new double NextExclusiveDouble() => NextExclusiveDouble(0f, 1f);
        public new double NextExclusiveDouble(double outerBound) => NextExclusiveDouble(0f, outerBound);
        public new double NextExclusiveDouble(double minBound, double maxBound) => returnIfRangeBothExclusive(minBound, maxBound, doubleSeries, ref doubleIndex);
        public override float NextFloat() => returnValueFrom(floatSeries, ref floatIndex);
        public new float NextFloat(float outerBound) => NextFloat(0f, outerBound);
        public new float NextFloat(float minBound, float maxBound) => returnIfRange(minBound, maxBound, floatSeries, ref floatIndex);
        public new float NextInclusiveFloat() => NextInclusiveFloat(0f, 1f);
        public new float NextInclusiveFloat(float outerBound) => NextInclusiveFloat(0f, outerBound);
        public new float NextInclusiveFloat(float minBound, float maxBound) => returnIfRangeInclusive(minBound, maxBound, floatSeries, ref floatIndex);
        public new float NextExclusiveFloat() => NextExclusiveFloat(0f, 1f);
        public new float NextExclusiveFloat(float outerBound) => NextExclusiveFloat(0f, outerBound);
        public new float NextExclusiveFloat(float minBound, float maxBound) => returnIfRangeBothExclusive(minBound, maxBound, floatSeries, ref floatIndex);
        public new float NextTriangular()
        {
            NextFloat(); // Used only to advance state the same number of times.
            return NextExclusiveFloat(-1f, 1f);
        }
        public new float NextTriangular(float min, float max) => NextExclusiveFloat(min, max);
        public new float NextTriangular(float min, float max, float mode) => NextExclusiveFloat(min, max);
        public new long NextLong() => returnValueFrom(longSeries, ref longIndex);
        public new long NextLong(long outerBound) => NextLong(0, outerBound);
        /// <summary>
        /// Returns the next long in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next long in the underlying series.</returns>
        public new long NextLong(long minValue, long maxValue) => returnIfRange(minValue, maxValue, longSeries, ref longIndex);

        public override ulong NextULong() => returnValueFrom(ulongSeries, ref ulongIndex);
        public new ulong NextULong(ulong outerBound) => NextULong(0, outerBound);
        /// <summary>
        /// Returns the next ulong in the underlying series. If the value is less than
        /// <paramref name="minValue"/>, or greater than/equal to <paramref name="maxValue"/>, throws an exception.
        /// </summary>
        /// <param name="minValue">The minimum value for the returned number, inclusive.</param>
        /// <param name="maxValue">The maximum value for the returned number, exclusive.</param>
        /// <returns>The next ulong in the underlying series.</returns>
        public new ulong NextULong(ulong minValue, ulong maxValue) => returnIfRange(minValue, maxValue, ulongSeries, ref ulongIndex);


        /// <summary>
        /// Fills the specified buffer with values from the underlying byte series.
        /// </summary>
        /// <param name="buffer">Buffer to fill.</param>
        public new void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = returnValueFrom(byteSeries, ref byteIndex);
        }
        public override ulong PreviousULong() => throw new NotImplementedException();
        public override void Seed(ulong seed)
        {
            int idx = (int)seed;
            intIndex = idx;
            uintIndex = idx;
            doubleIndex = idx;
            boolIndex = idx;
            byteIndex = idx;
            floatIndex = idx;
            longIndex = idx;
            ulongIndex = idx;
        }
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 0: return (ulong)intIndex;
                case 1: return (ulong)uintIndex;
                case 2: return (ulong)doubleIndex;
                case 3: return (ulong)boolIndex;
                case 4: return (ulong)byteIndex;
                case 5: return (ulong)floatIndex;
                case 6: return (ulong)longIndex;
                default: return (ulong)ulongIndex;
            }
        }
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 0: intIndex = (int)value; break;
                case 1: uintIndex = (int)value; break;
                case 2: doubleIndex = (int)value; break;
                case 3: boolIndex = (int)value; break;
                case 4: byteIndex = (int)value; break;
                case 5: floatIndex = (int)value; break;
                case 6: longIndex = (int)value; break;
                default: ulongIndex = (int)value; break;
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
