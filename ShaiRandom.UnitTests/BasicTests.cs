using System;
using ShaiRandom.Generators;
using Xunit;

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
                Assert.InRange(fwr.NextUInt(10U, 3U), 4U, 10U);
                Assert.InRange(fwr.NextULong(10UL, 3UL), 4UL, 10UL);
                Assert.NotInRange(fwr.NextUInt(10U, 3U), 0U, 3U);
                Assert.NotInRange(fwr.NextULong(10UL, 3UL), 0UL, 3UL);

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
}
