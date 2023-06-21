using System.Collections.Generic;
using System.Linq;
using ShaiRandom.Generators;
using Xunit;
using XUnit.ValueTuples;

namespace ShaiRandom.UnitTests
{
    /// <summary>
    /// Tests which check to make sure the generator implementations that support PreviousULong() do so correctly.
    /// </summary>
    public class PreviousTests
    {
        public static readonly IEnumerable<IEnhancedRandom> Generators = DataGenerators.CreateGenerators(true).Where(g => g.SupportsPrevious);

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        public void PreviousMatchesTest(IEnhancedRandom gen)
        {
            List<ulong> forward = new List<ulong>(100);
            for (int i = 0; i < 100; i++)
                forward.Add(gen.NextULong());

            forward.Reverse();

            for (int i = 0; i < 100; i++)
                Assert.Equal(forward[i], gen.PreviousULong());
        }
    }
}
