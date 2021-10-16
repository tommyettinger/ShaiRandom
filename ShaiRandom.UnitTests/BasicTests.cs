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
                Assert.InRange(fwr.NextUint(100U, 301U), 100U, 300U);
                Assert.InRange(fwr.NextLong(-100L, 101L), -100L, 100L);
                Assert.InRange(fwr.NextLong(100L, -101L), -100L, 100L);
                Assert.InRange(fwr.NextUlong(100UL, 301UL), 100UL, 300UL);
            }
        }
        [Fact]
        public void BoundedUnsignedTest()
        {
            FourWheelRandom fwr = new FourWheelRandom(1);
            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(100U, fwr.NextUint(100U, 3U));
                Assert.Equal(100UL, fwr.NextUlong(100UL, 3UL));
            }
        }
    }

    public class SerializationTests
    {
        [Fact]
        public void FourWheelSerDeserTest()
        {
            FourWheelRandom random = new FourWheelRandom(123456789UL, 0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL);
            random.NextUlong();
            string data = random.StringSerialize();
            Assert.StartsWith("#FoWR`", data);
            IRandom random2 = ARandom.Deserialize(data);
            Assert.Equal(random.NextUlong(), random2.NextUlong());
            Assert.Equal(random, random2);
        }
        [Fact]
        public void StrangerSerDeserTest()
        {
            StrangerRandom random = new StrangerRandom(0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL);
            random.NextUlong();
            string data = random.StringSerialize();
            Assert.StartsWith("#StrR`", data);
            IRandom random2 = ARandom.Deserialize(data);
            Assert.Equal(random.NextUlong(), random2.NextUlong());
            Assert.Equal(random, random2);
        }

        [Fact]
        public void TRWrapperSerDeserTest()
        {
            TRWrapper random = new TRWrapper(new FourWheelRandom(123456789UL, 0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL));
            random.NextUlong();
            string data = random.StringSerialize();
            Assert.StartsWith("TFoWR`", data);
            IRandom random2 = ARandom.Deserialize(data);
            Assert.Equal(random.NextUlong(), random2.NextUlong());
            Assert.Equal(random, random2);
        }

        [Fact]
        public void ReversingWrapperSerDeserTest()
        {
            ReversingWrapper random = new ReversingWrapper(new FourWheelRandom(123456789UL, 0xFA7BAB1E5UL, 0xB0BAFE77UL, 0x1234123412341234UL));
            random.NextUlong();
            string data = random.StringSerialize();
            Assert.StartsWith("RFoWR`", data);
            IRandom random2 = ARandom.Deserialize(data);
            Assert.Equal(random.NextUlong(), random2.NextUlong());
            Assert.Equal(random, random2);
        }
    }
}
