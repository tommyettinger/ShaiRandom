using System.Collections.Generic;
using System.Linq;
using ShaiRandom.Generators;
using Xunit;
using XUnit.ValueTuples;

namespace ShaiRandom.UnitTests
{
    public class SerializationTests
    {
        private static readonly IEnhancedRandom[] s_generators = DataGenerators.CreateGenerators(true).ToArray();
        public static IEnumerable<IEnhancedRandom> Generators => s_generators;

        // All should have unique values per type of series so they can be differentiated.  NextBool should always have
        // a value
        public static IEnumerable<KnownSeriesRandom> KSRGenerators = new[]
        {
            new KnownSeriesRandom(
            new[] { 1, 2 }, new[] { 2U, 3U }, new[] { 3.3, 4.4 },
            new[] { true, false }, new[] { (byte)4, (byte)5 },
            new[] { 5.5f, 6.6f }, new[] { 6L, 7L }, new[] { 7UL, 8UL }, new[] {8.8M, 9.9M}),
            new KnownSeriesRandom(
                new[] { 1, 2 }, null, new[] { 3.3, 4.4 },
                new[] { true, false }, new[] { (byte)4, (byte)5 },
                new[] { 5.5f, 6.6f }, new[] { 6L, 7L }, new[] { 7UL, 8UL }, new[] {8.8M, 9.9M}),
            new KnownSeriesRandom(
                new[] { 1, 2 }, null, null,
                new[] { true, false }, new[] { (byte)4, (byte)5 },
                new[] { 5.5f, 6.6f }, new[] { 6L, 7L }, new[] { 7UL, 8UL }, new[] {8.8M, 9.9M}),
            new KnownSeriesRandom(
                null, new[] { 2U, 3U }, new[] { 3.3, 4.4 },
                new[] { true, false }, new[] { (byte)4, (byte)5 },
                new[] { 5.5f, 6.6f }, new[] { 6L, 7L }, new[] { 7UL, 8UL }),
        };

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        public void BasicSerDeserTest(IEnhancedRandom gen)
        {
            // Advance state, just to make sure we have a valid generator
            gen.NextULong();

            // Serialize generator
            string ser = gen.StringSerialize();
            Assert.StartsWith(gen.Tag, ser);
            Assert.EndsWith("`", ser);

            // Deserialize generator
            var gen2 = AbstractRandom.Deserialize(ser);

            // Check that its state is equivalent and it generates identical numbers
            Assert.True(gen.Matches(gen2));
            Assert.Equal(gen.NextULong(), gen2.NextULong());
        }

        // Needs special serialization tests to ensure that not only its state is the same, but also the series
        // themselves, which is not represented in the state.
        [Theory]
        [MemberDataEnumerable(nameof(KSRGenerators))]
        public void KnownSeriesRandomSerDeserTest(KnownSeriesRandom ksr)
        {
            // Advance all states (so the indices are not their starting value if they have items)
            ksr.SetState(1);

            // Serialize generator
            string ser = ksr.StringSerialize();
            Assert.StartsWith(ksr.Tag, ser);
            Assert.EndsWith("`", ser);

            // Deserialize generator
            var ksr2 = (KnownSeriesRandom)AbstractRandom.Deserialize(ser);
            // Check that its state (indices) are equivalent to the original
            Assert.True(ksr.Matches(ksr2));
            // Check that each list is identical
            Assert.Equal(ksr.IntSeries, ksr2.IntSeries);
            Assert.Equal(ksr.UIntSeries, ksr2.UIntSeries);
            Assert.Equal(ksr.DoubleSeries, ksr2.DoubleSeries);
            Assert.Equal(ksr.BoolSeries, ksr2.BoolSeries);
            Assert.Equal(ksr.ByteSeries, ksr2.ByteSeries);
            Assert.Equal(ksr.FloatSeries, ksr2.FloatSeries);
            Assert.Equal(ksr.LongSeries, ksr2.LongSeries);
            Assert.Equal(ksr.ULongSeries, ksr2.ULongSeries);
            Assert.Equal(ksr.DecimalSeries, ksr2.DecimalSeries);

            // Sanity check that we can generate a value.
            ksr2.NextBool();
        }
    }
}
