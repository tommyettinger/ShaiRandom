using System.Collections.Generic;
using ShaiRandom.Collections;
using ShaiRandom.Generators;
using Xunit;

namespace ShaiRandom.UnitTests
{
    public class ProbabilityTableTest
    {
        [Fact]
        public void BasicTableTest()
        {
            FourWheelRandom fwr = new FourWheelRandom(1);
            ProbabilityTable<string> table = new ProbabilityTable<string>(fwr,
                new List<(string item, double weight)>() { ("goblin ear", 1.5), ("orc fang", 2.5), ("kobold tail", 0.5), ("cockatrice wattle", 3.5) });
            Dictionary<string, int> counts = new Dictionary<string, int>()
            {
                { "goblin ear", 0 }, {"orc fang", 0}, {"kobold tail", 0}, {"cockatrice wattle", 0}
            };
            for (int i = 0; i < 1024; i++)
            {
                string n = table.NextItem();
                ++counts[n];
            }
            Assert.InRange(counts["goblin ear"] / 128.0, 1.5 - 0.25, 1.5 + 0.25);
            Assert.InRange(counts["orc fang"] / 128.0, 2.5 - 0.25, 2.5 + 0.25);
            Assert.InRange(counts["kobold tail"] / 128.0, 0.5 - 0.25, 0.5 + 0.25);
            Assert.InRange(counts["cockatrice wattle"] / 128.0, 3.5 - 0.25, 3.5 + 0.25);
        }

    }
}
