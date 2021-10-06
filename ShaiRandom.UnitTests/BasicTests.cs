using System;
using System.Collections.Generic;
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

    }
}
