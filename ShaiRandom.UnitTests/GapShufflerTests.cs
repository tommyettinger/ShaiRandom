using System.Linq;
using ShaiRandom.Generators;
using Xunit;
using Xunit.Abstractions;

namespace ShaiRandom.UnitTests
{
    public class GapShufflerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IEnhancedRandom _rng;

        public GapShufflerTests(ITestOutputHelper output)
        {
            _output = output;
            _rng = new MizuchiRandom(1);
        }
        [Fact]
        public void GapShuffle()
        {
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = _rng.GapShuffler(list);

            var shuffled = shuffler.Take(10).ToArray();
            Assert.NotEqual(list, shuffled[..5]);
            Assert.NotEqual(list, shuffled[5..]);
        }

        [Fact]
        public void GapShuffleInPlace()
        {
            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = _rng.InPlaceGapShuffler(list);

            var shuffled = shuffler.Take(10).ToArray();
            Assert.NotEqual(originalList, shuffled[..5]);
            Assert.NotEqual(originalList, shuffled[5..]);

            // Original list was modified
            Assert.NotEqual(originalList, list);
            Assert.NotEqual(originalList, shuffled);
        }
    }
}
