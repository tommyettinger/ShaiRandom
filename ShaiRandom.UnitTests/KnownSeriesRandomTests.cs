using System;
using ShaiRandom.Generators;
using Xunit;

namespace ShaiRandom.UnitTests
{
    public class KnownSeriesRandomTests
    {
        private const int LowerInt = 0;
        private const int UpperInt = 10;

        [Fact]
        public void NextIntLowerBound()
        {
            var rng = new KnownSeriesRandom(new[] { LowerInt });

            Assert.Equal(LowerInt, rng.NextInt(LowerInt + 1));
            Assert.Equal(LowerInt, rng.NextInt(LowerInt, LowerInt + 1));

            Assert.Throws<ArgumentException>(() => rng.NextInt(LowerInt + 1, LowerInt + 2));
        }
        
        [Fact]
        public void NextIntUpperBound()
        {
            var rng = new KnownSeriesRandom(new[] { UpperInt });

            Assert.Equal(UpperInt, rng.NextInt(UpperInt + 1));
            Assert.Equal(UpperInt, rng.NextInt(UpperInt, UpperInt + 1));

            Assert.Throws<ArgumentException>(() => rng.NextInt(UpperInt));
            Assert.Throws<ArgumentException>(() => rng.NextInt(UpperInt - 1, UpperInt));
        }
    }
}
