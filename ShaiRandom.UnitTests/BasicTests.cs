using System;
using System.Collections.Generic;
using ShaiRandom.Generators;
using ShaiRandom.Wrappers;
using Xunit;
using XUnit.ValueTuples;

namespace ShaiRandom.UnitTests
{
    public class BasicTests
    {
        [Fact]
        public void InRangeTest()
        {
            FourWheelRandom fwr = new FourWheelRandom(1);
            for (int i = 0; i < 100; i++)
            {
                Assert.InRange(fwr.NextInt(-100, 101), -100, 100);
                Assert.InRange(fwr.NextInt(100, -101), -100, 100);
                Assert.InRange(fwr.NextUInt(100U, 301U), 100U, 300U);
                Assert.InRange(fwr.NextLong(-100L, 101L), -100L, 100L);
                Assert.InRange(fwr.NextLong(100L, -101L), -100L, 100L);
                Assert.InRange(fwr.NextULong(100UL, 301UL), 100UL, 300UL);
                Assert.InRange(fwr.NextExclusiveDouble(), 1.0842021724855044E-19, 0.9999999999999999);
                Assert.InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);
            }
            fwr.StateD = 1UL;
            Assert.InRange(fwr.NextExclusiveDouble(), 1.0842021724855044E-19, 0.9999999999999999);

            fwr.StateD = 0UL;
            Assert.InRange(fwr.NextExclusiveDouble(), 1.0842021724855044E-19, 0.9999999999999999);

            fwr.StateD = 0xFFFFFFFFFFFFFFFFUL;
            Assert.InRange(fwr.NextExclusiveDouble(), 1.0842021724855044E-19, 0.9999999999999999);

            fwr.StateD = 0x8000000000000000UL;
            Assert.InRange(fwr.NextExclusiveDouble(), 1.0842021724855044E-19, 0.9999999999999999);

            fwr.StateD = 1UL;
            Assert.InRange(fwr.NextExclusiveFloat(), 1.0842022E-19f, 0.99999994f);

            fwr.StateD = 0UL;
            Assert.InRange(fwr.NextExclusiveFloat(), 1.0842022E-19f, 0.99999994f);

            fwr.StateD = 0xFFFFFFFFFFFFFFFFUL;
            Assert.InRange(fwr.NextExclusiveFloat(), 1.0842022E-19f, 0.99999994f);

            fwr.StateD = 0x8000000000000000UL;
            Assert.InRange(fwr.NextExclusiveFloat(), 1.0842022E-19f, 0.99999994f);
        }
        [Fact]
        public void AverageValueTest()
        {
            unchecked
            {
                FourWheelRandom fwr = new FourWheelRandom(1);
                double sum = 0.0;
                for (int i = 0; i < 100; i++)
                {
                    sum += fwr.NextExclusiveDouble();
                }
                Assert.InRange(sum, 45.0, 55.0);
                fwr.Seed(1UL);
                float fsum = 0f;
                for (int i = 0; i < 100; i++)
                {
                    fsum += fwr.NextExclusiveFloat();
                }
                Assert.InRange(fsum, 45f, 55f);
                fwr.Seed(1);
                ulong usum = 0UL;
                for (int i = 0; i < 256; i++)
                {
                    usum += fwr.NextULong() & 0xFFFF;
                }
                Assert.InRange(usum, 0x7A0000UL, 0x860000UL);

                fwr.Seed(1);
                usum = 0UL;
                for (int i = 0; i < 256; i++)
                {
                    usum += fwr.NextULong() >> 48;
                }
                Assert.InRange(usum, 0x7A0000UL, 0x860000UL);
            }
        }
        [Fact]
        public void BoundedUnsignedTest()
        {
            FourWheelRandom fwr = new FourWheelRandom(1);
            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(100U, fwr.NextUInt(100U, 3U));
                Assert.Equal(100UL, fwr.NextULong(100UL, 3UL));
            }
        }

        [Fact]
        public void NextDecimalDistributionTest()
        {
            MizuchiRandom r = new MizuchiRandom(1);
            int[] buckets = new int[256];
            for (int i = 0; i < 0x100000; i++)
            {
                buckets[(int)(r.NextDecimal() * 256)]++;
            }
            Array.Sort(buckets);
            int smallest = buckets[0];
            int biggest = buckets[^1];
            Assert.True((biggest - smallest) / (biggest + 0.001) < 0.11);
        }

    }

    public class SerializationTests
    {
        private static IEnhancedRandom[] _generators =
        {
            new DistinctRandom(), new FourWheelRandom(), new LaserRandom(), new MizuchiRandom(),
            new RomuTrioRandom(), new StrangerRandom(), new TricycleRandom(), new Xoshiro256StarStarRandom(),
            new ArchivalWrapper(), new ReversingWrapper(), new TRGeneratorWrapper()
        };

        public static IEnumerable<IEnhancedRandom> Generators => _generators;

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        public void BasicSerDeserTest(IEnhancedRandom gen)
        {
            // Advance state, just to make sure we have a valid generator
            gen.NextULong();

            // Serialize generator; wrappers have a special-case starting sequence
            string ser = gen.StringSerialize();
            Assert.StartsWith(gen.Tag.Length == 1 ? $"{gen.Tag}" : $"#{gen.Tag}`", ser);
            Assert.EndsWith("`", ser);

            // Deserialize generator
            var gen2 = AbstractRandom.Deserialize(ser);

            // Check that its state is equivalent and it generates identical numbers
            Assert.True(gen.Matches(gen2));
            Assert.Equal(gen.NextULong(), gen2.NextULong());
        }

        // Needs special serialization tests to ensure that not only its state is the same, but also the series
        // themselves, which is not represented in the state.
        [Fact]
        public void KnownSeriesRandomSerDeserTest()
        {
            // Create a KSR with unique series per type
            var ksr = new KnownSeriesRandom(
                new[] { 1, 2 }, new[] { 2U, 3U }, new[] { 3.3, 4.4 },
                new[] { true, false }, new[] { (byte)4, (byte)5 },
                new[] { 5.5f, 6.6f }, new[] { 6L, 7L }, new[] { 7UL, 8UL }, new[] {8.8M, 9.9M});

            // Advance all states (so the indices are not their starting value)
            ksr.SetState(1);

            // Serialize generator
            string ser = ksr.StringSerialize();
            Assert.StartsWith($"#{ksr.Tag}", ser);
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
        }
    }
}
