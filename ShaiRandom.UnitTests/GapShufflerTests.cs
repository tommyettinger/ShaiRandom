﻿using System;
using System.Linq;
using ShaiRandom.Generators;
using Xunit;

namespace ShaiRandom.UnitTests
{
    public class GapShufflerTests
    {
        [Fact]
        public void GapShuffleIEnumerable()
        {
            var rng = new MizuchiRandom(1);

            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = rng.GapShuffler(list);

            var shuffled = shuffler.Take(10).ToArray();
            CheckResult(originalList, list, shuffled, false);

            shuffled = GetShuffledWithNext(shuffler);
            CheckResult(originalList, list, shuffled, false);
        }

        [Fact]
        public void GapShuffleSpan()
        {
            var rng = new MizuchiRandom(1);

            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = rng.GapShuffler<int>(list.AsSpan());

            var shuffled = shuffler.Take(10).ToArray();
            CheckResult(originalList, list, shuffled, false);

            shuffled = GetShuffledWithNext(shuffler);
            CheckResult(originalList, list, shuffled, false);
        }

        [Fact]
        public void GapShuffleMemory()
        {
            var rng = new MizuchiRandom(1);

            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = rng.GapShuffler<int>(list.AsMemory());

            var shuffled = shuffler.Take(10).ToArray();
            CheckResult(originalList, list, shuffled, false);

            shuffled = GetShuffledWithNext(shuffler);
            CheckResult(originalList, list, shuffled, false);
        }

        [Fact]
        public void GapShuffleInPlaceIList()
        {
            var rng = new MizuchiRandom(1);

            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = rng.InPlaceGapShuffler(list);

            var shuffled = shuffler.Take(10).ToArray();
            CheckResult(originalList, list, shuffled, true);
        }

        [Fact]
        public void GapShuffleInPlaceNextIList()
        {
            var rng = new MizuchiRandom(1);

            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = rng.InPlaceGapShuffler(list);

            var shuffled = GetShuffledWithNext(shuffler);
            CheckResult(originalList, list, shuffled, true);
        }

        [Fact]
        public void GapShuffleInPlaceMemory()
        {
            var rng = new MizuchiRandom(1);

            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = rng.InPlaceGapShuffler(list.AsMemory());

            var shuffled = shuffler.Take(10).ToArray();
            CheckResult(originalList, list, shuffled, true);
        }

        [Fact]
        public void GapShuffleInPlaceNextMemory()
        {
            var rng = new MizuchiRandom(1);

            var originalList = new[] { 1, 2, 3, 4, 5 };
            var list = new[] { 1, 2, 3, 4, 5 };
            var shuffler = rng.InPlaceGapShuffler(list.AsMemory());

            var shuffled = GetShuffledWithNext(shuffler);
            CheckResult(originalList, list, shuffled, true);
        }

        private static int[] GetShuffledWithNext(GapShufflerEnumerator<int> shuffler)
        {
            var array = new int[10];
            for (int i = 0; i < array.Length; i++)
                array[i] = shuffler.Next();

            return array;
        }

        private static int[] GetShuffledWithNext(GapShufflerInPlaceMemoryEnumerator<int> shuffler)
        {
            var array = new int[10];
            for (int i = 0; i < array.Length; i++)
                array[i] = shuffler.Next();

            return array;
        }

        private static void CheckResult(int[] originalList, int[] currentList, int[] enumerableToArray, bool inPlace)
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
