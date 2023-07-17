using System;
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
        public void GapShuffleIEnumerable()
        {
            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = _rng.GapShuffler(list);

            var shuffled = shuffler.Take(10).ToArray();
            var halves = new[] { shuffled[..5], shuffled[5..] };
            foreach (var half in halves)
            {
                // Each set of 5 contains each element of the original precisely once
                Assert.Equal(list.ToHashSet(), half.ToHashSet());

                // No set is the original list.  This is not _technically_ enforced by the algorithm, but it's
                // highly unlikely to happen by chance (and won't happen with the particular RNG seed we're using).
                // It is useful to check this to ensure all streams are randomized, to avoid bugs like the list reverting
                // to the original one after one set of all of its elements or some such.
                Assert.NotEqual(list, half);
            }

            // Ensure the original list was not modified (non in-place versions should copy)
            Assert.Equal(originalList, list);
        }

        [Fact]
        public void GapShuffleSpan()
        {
            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = _rng.GapShuffler<int>(list.AsSpan());

            var shuffled = shuffler.Take(10).ToArray();
            var halves = new[] { shuffled[..5], shuffled[5..] };
            foreach (var half in halves)
            {
                // Each set of 5 contains each element of the original precisely once
                Assert.Equal(list.ToHashSet(), half.ToHashSet());

                // No set is the original list.  This is not _technically_ enforced by the algorithm, but it's
                // highly unlikely to happen by chance (and won't happen with the particular RNG seed we're using).
                // It is useful to check this to ensure all streams are randomized, to avoid bugs like the list reverting
                // to the original one after one set of all of its elements or some such.
                Assert.NotEqual(list, half);
            }

            // Ensure the original list was not modified (non in-place versions should copy)
            Assert.Equal(originalList, list);
        }

        [Fact]
        public void GapShuffleMemory()
        {
            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = _rng.GapShuffler<int>(list.AsMemory());

            var shuffled = shuffler.Take(10).ToArray();
            var halves = new[] { shuffled[..5], shuffled[5..] };
            foreach (var half in halves)
            {
                // Each set of 5 contains each element of the original precisely once
                Assert.Equal(list.ToHashSet(), half.ToHashSet());

                // No set is the original list.  This is not _technically_ enforced by the algorithm, but it's
                // highly unlikely to happen by chance (and won't happen with the particular RNG seed we're using).
                // It is useful to check this to ensure all streams are randomized, to avoid bugs like the list reverting
                // to the original one after one set of all of its elements or some such.
                Assert.NotEqual(list, half);
            }

            // Ensure the original list was not modified (non in-place versions should copy)
            Assert.Equal(originalList, list);
        }

        [Fact]
        public void GapShuffleInPlace()
        {
            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = _rng.InPlaceGapShuffler(list);

            var shuffled = shuffler.Take(10).ToArray();
            var halves = new[] { shuffled[..5], shuffled[5..] };
            foreach (var half in halves)
            {
                // Each set of 5 contains each element of the original precisely once
                Assert.Equal(originalList.ToHashSet(), half.ToHashSet());

                // No set is the original list.  This is not _technically_ enforced by the algorithm, but it's
                // highly unlikely to happen by chance (and won't happen with the particular RNG seed we're using).
                // It is useful to check this to ensure all streams are randomized, to avoid bugs like the list reverting
                // to the original one after one set of all of its elements or some such.
                Assert.NotEqual(originalList, half);
            }

            // We'll also check that the original list is modified.  Again, this is not _technically_ enforced by the algorithm,
            // but is guaranteed to happen with the seed we chose for this test, and ensures that the implementation
            // is shuffling in-place.
            Assert.NotEqual(originalList, list);
        }

        private void CheckResult(int[] originalList, int[] currentList, int[] enumerableToArray, bool inPlace)
        {
            // Only required for how we test, not for the algorithm itself
            Assert.Equal(originalList.Length * 2, enumerableToArray.Length);

            // Split into two halves, each of which should contain each element of the original list exactly once
            var halves = new[] { enumerableToArray[..5], enumerableToArray[5..] };
            foreach (var half in halves)
            {
                // Each set of 5 contains each element of the original precisely once
                Assert.Equal(originalList.ToHashSet(), half.ToHashSet());

                // No set is the original list.  This is not _technically_ enforced by the algorithm, but it's
                // highly unlikely to happen by chance (and won't happen with the particular RNG seed we're using).
                // It is useful to check this to ensure all streams are randomized, to avoid bugs like the list reverting
                // to the original one after one set of all of its elements or some such.
                Assert.NotEqual(originalList, half);
            }

            // Ensure the list was modified in place if an in-place version was used.  Again, note that the list being
            // modified is not _technically_ enforced by the algorithm for an in-place shuffle, but is guaranteed to
            // happen with the seed we chose for this test, and ensures that the implementation is shuffling in-place.
            if (inPlace)
                Assert.NotEqual(originalList, currentList);
            else
                Assert.Equal(originalList, currentList);
        }
    }
}
