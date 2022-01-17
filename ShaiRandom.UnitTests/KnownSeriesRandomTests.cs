using System;
using System.Diagnostics.CodeAnalysis;
using ShaiRandom.Generators;
using Xunit;

namespace ShaiRandom.UnitTests
{
    [SuppressMessage("ReSharper", "UselessBinaryOperation")]
    public class KnownSeriesRandomTests
    {
        private const int LowerValue = 0;
        private const int UpperValue = 10;

        private static readonly float s_floatAdjust = MathF.Pow(2f, -24f);
        private static readonly double s_doubleCloseTo1 = 1 - Math.Pow(2, -53);

        private readonly KnownSeriesRandom _lowerRNG = new KnownSeriesRandom(
            new []{LowerValue},
            new []{(uint)LowerValue},
            new []{(double)LowerValue},
            byteSeries: new []{(byte)LowerValue},
            floatSeries: new []{(float)LowerValue},
            longSeries: new []{(long)LowerValue},
            ulongSeries: new []{(ulong)LowerValue}
        );

        private readonly KnownSeriesRandom _upperRNG = new KnownSeriesRandom(
            new []{UpperValue},
            new []{(uint)UpperValue},
            new []{(double)UpperValue},
            byteSeries: new []{(byte)UpperValue},
            floatSeries: new []{(float)UpperValue},
            longSeries: new []{(long)UpperValue},
            ulongSeries: new []{(ulong)UpperValue}
        );

        private readonly KnownSeriesRandom _unboundedRNG = new KnownSeriesRandom(
            new []{int.MinValue, int.MaxValue},
            new []{uint.MinValue, uint.MaxValue},
            new []{0.0, 1.0 - s_doubleCloseTo1},
            byteSeries: new []{byte.MinValue, byte.MaxValue},
            floatSeries: new []{0.0f, 1.0f - s_floatAdjust},
            longSeries: new []{long.MinValue, long.MaxValue},
            ulongSeries: new []{ulong.MinValue, ulong.MaxValue}
        );

        #region Double

        [Fact]
        public void NextDoubleUnbounded()
        {
            var rng = new KnownSeriesRandom(doubleSeries: new[] { 0.0, s_doubleCloseTo1 });

            Assert.Equal(0.0, rng.NextDouble());
            Assert.Equal(s_doubleCloseTo1, rng.NextDouble());
        }

        [Fact]
        public void NextDoubleLowerBound()
        {
            double value = UpperValue;

            Assert.Equal(value, _upperRNG.NextDouble(value + 0.1));
            Assert.Equal(value, _upperRNG.NextDouble(value, value + 0.1));

            Assert.Throws<ArgumentException>(() => _upperRNG.NextDouble(value + 0.1, value + 0.2));
        }

        [Fact]
        public void NextDoubleCrossedBounds()
        {
            double value = LowerValue;

            // Allowed range: value
            Assert.Equal(value, _lowerRNG.NextDouble(value, value));
            // Allowed range: (value - 0.1, value]
            Assert.Equal(value, _lowerRNG.NextDouble(value, value - 0.1));
        }

        [Fact]
        public void NextDoubleUpperBound()
        {
            double value = UpperValue;

            Assert.Equal(value, _upperRNG.NextDouble(value + 0.1));
            Assert.Equal(value, _upperRNG.NextDouble(value, value + 0.1));

            Assert.Throws<ArgumentException>(() => _upperRNG.NextDouble(value));
            Assert.Throws<ArgumentException>(() => _upperRNG.NextDouble(value - 0.1, value));
        }
        #endregion

        #region Template Tests

        private void TestUnboundedIntFunction<T>(Func<T> unboundedGenFunc)
        {
            T minValue = (T)typeof(T).GetField("MinValue")!.GetValue(null)!;
            T maxValue = (T)typeof(T).GetField("MaxValue")!.GetValue(null)!;

            Assert.Equal(minValue, unboundedGenFunc());
            Assert.Equal(maxValue,unboundedGenFunc());
        }

        private static void TestLowerBoundIntFunction<T>(Func<T, T> upperBoundFunc, Func<T, T, T> dualBoundFunc)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(LowerValue, typeof(T));

            Assert.Equal(value, upperBoundFunc(value + 1));
            Assert.Equal(value, dualBoundFunc(value, value + 1));

            Assert.Throws<ArgumentException>(() => dualBoundFunc(value + 1, value + 2));
        }

        private static void TestCrossedBoundIntFunction<T>(Func<T, T, T> generatorFunc)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(UpperValue, typeof(T));

            // Allowed range: value
            Assert.Equal(value, generatorFunc(value, value));
            // Allowed range: value
            Assert.Equal(value, generatorFunc(value, value - 1));
            // Allowed range: [value - 1, value]
            Assert.Equal(value, generatorFunc(value, value - 2));
        }

        private static void TestUpperBoundIntFunction<T>(Func<T, T> upperBoundFunc, Func<T, T, T> dualBoundFunc)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(UpperValue, typeof(T));

            Assert.Equal(value, upperBoundFunc(value + 1));
            Assert.Equal(value, dualBoundFunc(value, value + 1));

            Assert.Throws<ArgumentException>(() => upperBoundFunc(value));
            Assert.Throws<ArgumentException>(() => dualBoundFunc(value - 1, value));
        }
        #endregion

        #region Int

        [Fact]
        public void NextIntUnbounded() => TestUnboundedIntFunction(_unboundedRNG.NextInt);

        [Fact]
        public void NextIntLowerBound() => TestLowerBoundIntFunction<int>(_lowerRNG.NextInt, _lowerRNG.NextInt);

        [Fact]
        public void NextIntCrossedBounds() => TestCrossedBoundIntFunction<int>(_upperRNG.NextInt);

        [Fact]
        public void NextIntUpperBound() => TestUpperBoundIntFunction<int>(_upperRNG.NextInt, _upperRNG.NextInt);

        #endregion

    }
}
