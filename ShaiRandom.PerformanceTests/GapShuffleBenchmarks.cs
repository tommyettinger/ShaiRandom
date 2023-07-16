using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using ShaiRandom.Generators;

namespace ShaiRandom.PerformanceTests
{
    public static class GapShuffleAlternateImplementations
    {
        public static IEnumerable<int> GapShuffleYieldReturn(this IEnhancedRandom rng, IEnumerable<int> items)
        {
            IList<int> list = items.ToArray();
            rng.Shuffle(list);

            int index = 0;

            while (true)
            {
                int size = list.Count; // In case size changes while we're iterating
                if(size == 1)
                    yield return list[0];

                if(index >= size)
                {
                    int n = size - 1;
                    int swapWith;
                    for (int i = n; i > 1; i--) {
                        swapWith = rng.NextInt(i);
                        (list[i - 1], list[swapWith]) = (list[swapWith], list[i - 1]);
                    }
                    swapWith = 1 + rng.NextInt(n);
                    (list[n], list[swapWith]) = (list[swapWith], list[n]);
                    index = 0;
                }

                yield return list[index++];
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public static IEnumerable<int> GapShuffleInPlaceYieldReturn(this IEnhancedRandom rng, IList<int> list)
        {
            rng.Shuffle(list);

            int index = 0;

            while (true)
            {
                int size = list.Count; // In case size changes while we're iterating
                if(size == 1)
                    yield return list[0];

                if(index >= size)
                {
                    int n = size - 1;
                    int swapWith;
                    for (int i = n; i > 1; i--) {
                        swapWith = rng.NextInt(i);
                        (list[i - 1], list[swapWith]) = (list[swapWith], list[i - 1]);
                    }
                    swapWith = 1 + rng.NextInt(n);
                    (list[n], list[swapWith]) = (list[swapWith], list[n]);
                    index = 0;
                }

                yield return list[index++];
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
    public class GapShuffleBenchmarks
    {
        [UsedImplicitly]
        [Params(10, 50, 100, 500)]
        public int Size;

        private int[] _numbers = null!;
        private int[] _inPlaceNumbers = null!;
        private IEnhancedRandom _rng = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _numbers = new int[Size];
            _inPlaceNumbers = new int[Size];

            for (int i = 0; i < Size; i++)
            {
                _numbers[i] = i + 1;
                _inPlaceNumbers[i] = i + 1;
            }

            _rng = new MizuchiRandom(1);
            _rng.Shuffle(_numbers);
            _rng.Shuffle(_inPlaceNumbers);
        }

        [Benchmark]
        public int ShaiRandomForeach()
        {
            int sum = 0;
            int take = 0;
            // Don't use .Take(Size) here to avoid boxing
            foreach (var num in _rng.GapShuffle(_numbers))
            {
                sum += num;
                take++;
                if (take >= Size) break;
            }

            return sum;
        }

        [Benchmark]
        public int ShaiRandomForeachInPlace()
        {
            int sum = 0;
            int take = 0;
            // Don't use .Take(Size) here to avoid boxing
            foreach (var num in _rng.GapShuffleInPlace(_inPlaceNumbers))
            {
                sum += num;
                take++;
                if (take >= Size) break;
            }

            return sum;
        }

        // Include these because .Take would be a very common use case, and would require boxing
        [Benchmark]
        public int ShaiRandomForeachTake()
        {
            int sum = 0;
            foreach (var num in _rng.GapShuffle(_numbers).Take(Size))
                sum += num;

            return sum;
        }

        [Benchmark]
        public int ShaiRandomForeachInPlaceTake()
        {
            int sum = 0;
            foreach (var num in _rng.GapShuffleInPlace(_inPlaceNumbers).Take(Size))
                sum += num;

            return sum;
        }

        [Benchmark]
        public int YieldReturnForeach()
        {
            int sum = 0;
            int take = 0;
            foreach (var num in _rng.GapShuffleYieldReturn(_numbers))
            {
                sum += num;
                take++;
                if (take >= Size) break;
            }

            return sum;
        }

        [Benchmark]
        public int YieldReturnForeachInPlace()
        {
            int sum = 0;
            int take = 0;
            foreach (var num in _rng.GapShuffleInPlaceYieldReturn(_inPlaceNumbers))
            {
                sum += num;
                take++;
                if (take >= Size) break;
            }

            return sum;
        }

        [Benchmark]
        public int YieldReturnForeachTake()
        {
            int sum = 0;
            foreach (var num in _rng.GapShuffleYieldReturn(_numbers).Take(Size))
                sum += num;

            return sum;
        }

        [Benchmark]
        public int YieldReturnForeachInPlaceTake()
        {
            int sum = 0;
            foreach (var num in _rng.GapShuffleInPlaceYieldReturn(_inPlaceNumbers).Take(Size))
                sum += num;

            return sum;
        }

        [Benchmark]
        public int ShaiRandomLinq()
            => _rng.GapShuffle(_numbers).Take(Size).Sum();

        [Benchmark]
        public int ShaiRandomLinqInPlace()
            => _rng.GapShuffleInPlace(_inPlaceNumbers).Take(Size).Sum();

        [Benchmark]
        public int YieldReturnLinq()
            => _rng.GapShuffleYieldReturn(_numbers).Take(Size).Sum();

        [Benchmark]
        public int YieldReturnLinqInPlace()
            => _rng.GapShuffleInPlaceYieldReturn(_inPlaceNumbers).Take(Size).Sum();

        // We could also just use .Take(x) here since it's already boxed, but we'll avoid it just for grins
        [Benchmark]
        public int ShaiRandomIEnumerable()
        {
            int sum = 0;
            IEnumerable<int> enumerable = _rng.GapShuffle(_numbers);
            int take = 0;
            foreach (var num in enumerable)
            {
                sum += num;
                take++;
                if (take >= Size) break;
            }

            return sum;
        }

        [Benchmark]
        public int ShaiRandomIEnumerableInPlace()
        {
            int sum = 0;
            IEnumerable<int> enumerable = _rng.GapShuffleInPlace(_inPlaceNumbers);
            int take = 0;
            foreach (var num in enumerable)
            {
                sum += num;
                take++;
                if (take >= Size) break;
            }

            return sum;
        }
    }
}
