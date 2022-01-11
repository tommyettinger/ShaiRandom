using ShaiRandom.Generators;
using ShaiRandom.Wrappers;
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
                Assert.Equal(100U, fwr.NextUInt(100U, 3U));
                Assert.Equal(100UL, fwr.NextULong(100UL, 3UL));
            }
        }
    }

    public class SerializationTests
    {
        [Fact]
        public void FourWheelSerDeserTest()
        {
            FourWheelRandom random = new FourWheelRandom(123456789UL, 0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL);
            random.NextULong();
            string data = random.StringSerialize();
            Assert.StartsWith("#FoWR`", data);
            IEnhancedRandom random2 = AbstractRandom.Deserialize(data);
            Assert.Equal(random.NextULong(), random2.NextULong());
            Assert.True(random.Matches(random2));
        }
        [Fact]
        public void StrangerSerDeserTest()
        {
            StrangerRandom random = new StrangerRandom(0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL);
            random.NextULong();
            string data = random.StringSerialize();
            Assert.StartsWith("#StrR`", data);
            IEnhancedRandom random2 = AbstractRandom.Deserialize(data);
            Assert.Equal(random.NextULong(), random2.NextULong());
            Assert.True(random.Matches(random2));
        }

        [Fact]
        public void TRWrapperSerDeserTest()
        {
            TRGeneratorWrapper random = new TRGeneratorWrapper(new FourWheelRandom(123456789UL, 0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL));
            random.NextULong();
            string data = random.StringSerialize();
            Assert.StartsWith("TFoWR`", data);
            IEnhancedRandom random2 = AbstractRandom.Deserialize(data);
            Assert.Equal(random.NextULong(), random2.NextULong());
            Assert.True(random.Matches(random2));
        }

        [Fact]
        public void ReversingWrapperSerDeserTest()
        {
            ReversingWrapper random = new ReversingWrapper(new FourWheelRandom(123456789UL, 0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL));
            random.NextULong();
            string data = random.StringSerialize();
            Assert.StartsWith("RFoWR`", data);
            IEnhancedRandom random2 = AbstractRandom.Deserialize(data);
            Assert.Equal(random.NextULong(), random2.NextULong());
            Assert.True(random.Matches(random2));

            random.Wrapped = new StrangerRandom(123456789UL, 0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL);
            random.NextULong();
            data = random.StringSerialize();
            Assert.StartsWith("RStrR`", data);
            random2 = AbstractRandom.Deserialize(data);
            Assert.Equal(random.NextULong(), random2.NextULong());
            Assert.True(random.Matches(random2));
        }
        [Fact]
        public void KnownSeriesSerDeserTest()
        {
            KnownSeriesRandom random = new KnownSeriesRandom(new int[] { 1, 3, -7 }, new uint[] { 2, 8, 12}, new double[] { -1.1, 2.0, 1e30}, null);
            string data = random.StringSerialize();
            Assert.StartsWith("#KnSR`", data);
            //IEnhancedRandom random2 = AbstractRandom.Deserialize(data);
            //Assert.Equal(random.NextUInt(), random2.NextUInt());
            //Assert.True(random.Matches(random2));
        }

    }
}
