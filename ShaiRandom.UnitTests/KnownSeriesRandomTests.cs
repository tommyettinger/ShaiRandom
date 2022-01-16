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
            double value = LowerValue;

            Assert.Equal(value, _lowerRNG.NextDouble(value + 0.1));
            Assert.Equal(value, _lowerRNG.NextDouble(value, value + 0.1));

            Assert.Throws<ArgumentException>(() => _lowerRNG.NextDouble(value + 0.1, value + 0.2));
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

        #region Int

        [Fact]
        public void NextIntUnbounded()
        {
            var rng = new KnownSeriesRandom(new[] { int.MinValue, int.MaxValue });

            Assert.Equal(int.MinValue, rng.NextInt());
            Assert.Equal(int.MaxValue,rng.NextInt());
        }

        [Fact]
        public void NextIntLowerBound()
        {
            int value = LowerValue;

            Assert.Equal(value, _lowerRNG.NextInt(value + 1));
            Assert.Equal(value, _lowerRNG.NextInt(value, value + 1));

            Assert.Throws<ArgumentException>(() => _lowerRNG.NextInt(value + 1, value + 2));
        }

        [Fact]
        public void NextIntCrossedBounds()
        {
            int value = LowerValue;

            // Allowed range: value
            Assert.Equal(value, _lowerRNG.NextInt(value, value));
            // Allowed range: value
            Assert.Equal(value, _lowerRNG.NextInt(value, value - 1));
            // Allowed range: [value - 1, value]
            Assert.Equal(value, _lowerRNG.NextInt(value, value - 2));
        }

        [Fact]
        public void NextIntUpperBound()
        {
            int value = UpperValue;

            Assert.Equal(value, _upperRNG.NextInt(value + 1));
            Assert.Equal(value, _upperRNG.NextInt(value, value + 1));

            Assert.Throws<ArgumentException>(() => _upperRNG.NextInt(value));
            Assert.Throws<ArgumentException>(() => _upperRNG.NextInt(value - 1, value));
        }
        #endregion
    }
}
