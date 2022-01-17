using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ShaiRandom.Generators;
using Xunit;

namespace ShaiRandom.UnitTests
{
    [SuppressMessage("ReSharper", "UselessBinaryOperation")]
    public class KnownSeriesRandomTests
    {
        private const int ReturnedValue = 10;

        private static readonly float s_floatCloseTo1 = 1.0f - MathF.Pow(2f, -24f);
        private static readonly double s_doubleCloseTo1 = 1.0 - Math.Pow(2, -53);

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
            new []{0.0, s_doubleCloseTo1},
            byteSeries: new []{byte.MinValue, byte.MaxValue},
            floatSeries: new []{0.0f, s_floatCloseTo1},
            longSeries: new []{long.MinValue, long.MaxValue},
            ulongSeries: new []{ulong.MinValue, ulong.MaxValue}
        );

        #region Template Tests

        private (Func<T> unbounded, Func<T, T> outerBound, Func<T, T, T> dualBound) GetGenerationFunctions<T>(string name)
            where T : notnull
        {
            // Find info for the functions
            var unboundedInfo = typeof(KnownSeriesRandom).GetMethod(name, Array.Empty<Type>())
                            ?? throw new Exception("Couldn't find unbounded generation method with the name given.");
            var outerBoundInfo = typeof(KnownSeriesRandom).GetMethod(name, new []{typeof(T)})
                             ?? throw new Exception("Couldn't generation method with the name given that takes a single (outer) bound.");
            var dualBoundInfo = typeof(KnownSeriesRandom).GetMethod(name, new []{typeof(T), typeof(T)})
                            ?? throw new Exception("Couldn't find generation method with the name given which takes both an inner and outer bound.");

            // Create convenient Func wrappers from which we can call them.
            T Unbounded()
            {
                try
                {
                    return (T)unboundedInfo.Invoke(_unboundedRNG, null)!;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }

            }

            T OuterBound(T outer)
            {
                try
                {
                    return (T)outerBoundInfo.Invoke(_boundedRNG, new object[] { outer })!;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }
            }

            T DualBound(T inner, T outer)
            {
                try
                {
                    return (T)dualBoundInfo.Invoke(_boundedRNG, new object[] { inner, outer })!;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }
            }

            // Return functions
            return (Unbounded, OuterBound, DualBound);
        }

        private void TestIntFunctionBounds<T>(string nameOfFunctionToTest)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(ReturnedValue, typeof(T));

            // Get proxies to call the functions we are testing
            var (unbounded, outerBound, dualBound) = GetGenerationFunctions<T>(nameOfFunctionToTest);

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

        private void TestFloatingFunctionBounds<T>(string nameOfFunctionToTest, T maxValueLessThanOne)
            where T : IConvertible
        {
            // Duck-type the generic type so that we can add/subtract from it using the type's correct operators.
            dynamic value = (T)Convert.ChangeType(ReturnedValue, typeof(T));

            // Get the correct value types for type T for various constants we use
            T zero = (T)Convert.ChangeType(0.0, typeof(T));
            T pointOne = (T)Convert.ChangeType(0.1, typeof(T));
            T pointTwo = (T)Convert.ChangeType(0.2, typeof(T));

            // Get proxies to call the functions we are testing
            var (unbounded, outerBound, dualBound) = GetGenerationFunctions<T>(nameOfFunctionToTest);

            // Check that the unbounded functions allow everything within range [0, 1)
            Assert.Equal(zero, unbounded());
            Assert.Equal(maxValueLessThanOne, unbounded());

            // Check that bounded generation functions treat inner bounds as inclusive
            Assert.Equal(value, outerBound(value + pointOne));
            Assert.Equal(value, dualBound(value, value + pointOne));

            Assert.Throws<ArgumentException>(() => dualBound(value + pointOne, value + pointTwo));

            // Check that behavior is appropriate when the inner and outer bounds are crossed on bounded generation
            // functions (ie. outer <= inner)
            Assert.Equal(value, dualBound(value, value)); // Allowed range: value
            Assert.Equal(value, dualBound(value, value - pointOne)); // Allowed range: (value - 0.1, value]

            Assert.Throws<ArgumentException>(() => dualBound(value - pointOne, value - pointTwo));
            Assert.Throws<ArgumentException>(() => dualBound(value + pointOne, value));

            // Check that bounded generation functions treat outer bounds as exclusive
            Assert.Equal(value, outerBound(value + pointOne));
            Assert.Equal(value, dualBound(value, value + pointOne));

            Assert.Throws<ArgumentException>(() => outerBound(value));
            Assert.Throws<ArgumentException>(() => dualBound(value - pointOne, value));
        }
        #endregion

        #region Integer Function Tests
        [Fact]
        public void NextIntBounds()
            => TestIntFunctionBounds<int>(nameof(KnownSeriesRandom.NextInt));

        [Fact]
        public void NextUIntBounds()
            => TestIntFunctionBounds<uint>(nameof(KnownSeriesRandom.NextUInt));

        [Fact]
        public void NextLongBounds()
            => TestIntFunctionBounds<long>(nameof(KnownSeriesRandom.NextLong));

        [Fact]
        public void NextULongBounds()
            => TestIntFunctionBounds<ulong>(nameof(KnownSeriesRandom.NextULong));
        #endregion

        #region Floating-Point Function Tests
        [Fact]
        public void NextDecimalBounds()
            => TestFloatingFunctionBounds(nameof(KnownSeriesRandom.NextDecimal), (decimal)s_doubleCloseTo1);

        [Fact]
        public void NextDoubleBounds()
            => TestFloatingFunctionBounds(nameof(KnownSeriesRandom.NextDouble), s_doubleCloseTo1);

        [Fact]
        public void NextFloatBounds()
            => TestFloatingFunctionBounds(nameof(KnownSeriesRandom.NextFloat), s_floatCloseTo1);
        #endregion
    }
}
