using System.Collections.Generic;
using System.Linq;
using ShaiRandom.Generators;
using Xunit;

namespace ShaiRandom.UnitTests
{
    /// <summary>
    /// Tests which check to make sure the generator implementations that support PreviousULong() do so correctly.
    /// </summary>
    public class PreviousTests
    {
        [Fact]
        public void PreviousMatchesTest()
        {
            IEnhancedRandom[] generators = DataGenerators.CreateGenerators(true).Where(g => g.SupportsPrevious).ToArray();
            foreach (var gen in generators)
            {
                List<ulong> forward = new List<ulong>(100);
                for (int i = 0; i < 100; i++)
                {
                    forward.Add(gen.NextULong());
                }
                // Advancing one extra step is needed so when we go back, it gives us the last element added to forward.
                gen.NextULong();
                forward.Reverse();
                for (int i = 0; i < 100; i++)
                {
                    Assert.Equal(forward[i], gen.PreviousULong());
                }
            }
        }
    }
}
