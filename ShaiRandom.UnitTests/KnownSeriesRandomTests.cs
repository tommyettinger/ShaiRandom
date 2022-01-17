using System;
using System.Diagnostics.CodeAnalysis;
using ShaiRandom.Generators;
using Xunit;

namespace ShaiRandom.UnitTests
{
    [SuppressMessage("ReSharper", "UselessBinaryOperation")]
    public class KnownSeriesRandomTests
    {
        private const int ReturnedValue = 10;

        private static readonly float s_floatAdjust = MathF.Pow(2f, -24f);
        private static readonly double s_doubleCloseTo1 = 1 - Math.Pow(2, -53);

        private readonly KnownSeriesRandom _boundedRNG = new KnownSeriesRandom(
            new []{ReturnedValue},
            new []{(uint)ReturnedValue},
            new []{(double)ReturnedValue},
            byteSeries: new []{(byte)ReturnedValue},
            floatSeries: new []{(float)ReturnedValue},
            longSeries: new []{(long)ReturnedValue},
            ulongSeries: new []{(ulong)ReturnedValue}
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

        #region Template Tests

        private static void TestIntFunctionBounds<T>(Func<T> unbounded, Func<T, T> outerBound, Func<T, T, T> dualBound)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(ReturnedValue, typeof(T));

            // Find min/max for unbounded functions, which should be the min/max values for the type itself
            T minValue = (T)typeof(T).GetField("MinValue")!.GetValue(null)!;
            T maxValue = (T)typeof(T).GetField("MaxValue")!.GetValue(null)!;

            // Check that unbounded generation function allows anything in type's range
            Assert.Equal(minValue, unbounded());
            Assert.Equal(maxValue,unbounded());

            // Check that bounded generation functions treat inner bounds as inclusive
            Assert.Equal(value, outerBound(value + 1)); // TODO: Fix; currently duplicate of outerBound check below
            Assert.Equal(value, dualBound(value, value + 1));

            Assert.Throws<ArgumentException>(() => dualBound(value + 1, value + 2));

            // Check that behavior is appropriate when the inner and outer bounds are crossed on bounded generation
            // functions (ie. outer <= inner)
            Assert.Equal(value, dualBound(value, value)); // Allowed range: value
            Assert.Equal(value, dualBound(value, value - 1)); // Allowed range: value
            Assert.Equal(value, dualBound(value, value - 2)); // Allowed range: [value - 1, value]

            Assert.Throws<ArgumentException>(() => dualBound(value - 1, value - 2));
            Assert.Throws<ArgumentException>(() => dualBound(value + 1, value));

            // Check that bounded generation functions treat outer bounds as exclusive
            Assert.Equal(value, outerBound(value + 1));
            Assert.Equal(value, dualBound(value, value + 1));

            Assert.Throws<ArgumentException>(() => outerBound(value));
            Assert.Throws<ArgumentException>(() => dualBound(value - 1, value));
        }

        // TODO: Combine
        private static void TestUnboundedFloatingFunction<T>(Func<T> unboundedGenFunc, T maxValueLessThanOne)
            where T : IConvertible
        {
            T zero = (T)Convert.ChangeType(0.0, typeof(T));
            Assert.Equal(zero, unboundedGenFunc());
            Assert.Equal(maxValueLessThanOne, unboundedGenFunc());
        }

        private static void TestLowerBoundFloatingFunction<T>(Func<T, T> upperBoundFunc, Func<T, T, T> dualBoundFunc)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(ReturnedValue, typeof(T));

            Assert.Equal(value, upperBoundFunc(value + 0.1));
            Assert.Equal(value, dualBoundFunc(value, value + 0.1));

            Assert.Throws<ArgumentException>(() => dualBoundFunc(value + 0.1, value + 0.2));
        }

        private static void TestCrossedBoundFloatingFunction<T>(Func<T, T, T> generatorFunc)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(ReturnedValue, typeof(T));

            // Allowed range: value
            Assert.Equal(value, generatorFunc(value, value));
            // Allowed range: Allowed range: (value - 0.1, value]
            Assert.Equal(value, generatorFunc(value, value - 0.1));

            Assert.Throws<ArgumentException>(() => generatorFunc(value - 0.1, value - 0.2));
            Assert.Throws<ArgumentException>(() => generatorFunc(value + 0.1, value));
        }

        private static void TestUpperBoundFloatingFunction<T>(Func<T, T> upperBoundFunc, Func<T, T, T> dualBoundFunc)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(ReturnedValue, typeof(T));

            Assert.Equal(value, upperBoundFunc(value + 0.1));
            Assert.Equal(value, dualBoundFunc(value, value + 0.1));

            Assert.Throws<ArgumentException>(() => upperBoundFunc(value));
            Assert.Throws<ArgumentException>(() => dualBoundFunc(value - 0.1, value));
        }
        #endregion

        [Fact]
        public void NextIntBounds()
            => TestIntFunctionBounds(_unboundedRNG.NextInt, _boundedRNG.NextInt, _boundedRNG.NextInt);


        #region Test Double

        [Fact]
        public void NextDoubleUnbounded()
            => TestUnboundedFloatingFunction(_unboundedRNG.NextDouble, 1 - s_doubleCloseTo1);

        [Fact]
        public void NextDoubleLowerBound()
            => TestLowerBoundFloatingFunction<double>(_boundedRNG.NextDouble, _boundedRNG.NextDouble);

        [Fact]
        public void NextDoubleCrossedBounds() => TestCrossedBoundFloatingFunction<double>(_boundedRNG.NextDouble);

        [Fact]
        public void NextDoubleUpperBound()
            => TestUpperBoundFloatingFunction<double>(_boundedRNG.NextDouble, _boundedRNG.NextDouble);
        #endregion
    }
}
