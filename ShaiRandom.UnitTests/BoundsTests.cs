using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using ShaiRandom.Generators;
using ShaiRandom.Wrappers;
using Xunit;

namespace ShaiRandom.UnitTests
{
    public class BoundsTests
    {
        private (int inner, int outer) _signedBounds = (1, -2);
        private (int inner, int outer) _unsignedBounds = (3, 1);

        private (Func<T, T> outerBound, Func<T, T, T> dualBound) GetGenerationFunctions<T>(IEnhancedRandom rng, string name)
            where T : notnull
        {
            // Find info for the functions
            var outerBoundInfo = typeof(IEnhancedRandom).GetMethod(name, new []{typeof(T)})
                                 ?? throw new Exception("Couldn't generation method with the name given that takes a single (outer) bound.");
            var dualBoundInfo = typeof(IEnhancedRandom).GetMethod(name, new []{typeof(T), typeof(T)})
                                ?? throw new Exception("Couldn't find generation method with the name given which takes both an inner and outer bound.");

            // Create convenient Func wrappers from which we can call them.
            T OuterBound(T outer)
            {
                try
                {
                    return (T)outerBoundInfo.Invoke(rng, new object[] { outer })!;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }
            }

            T DualBound(T inner, T outer)
            {
                try
                {
                    return (T)dualBoundInfo.Invoke(rng, new object[] { inner, outer })!;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }
            }

            // Return functions
            return (OuterBound, DualBound);
        }

        private void TestIntegerFunc<T>(IEnhancedRandom rng, string funcName, (T inner, T outer) bounds)
            where T : IComparable<T>
        {
            // Create dynamic variable for outer so we can add/subtract when checking the bound extents
            dynamic outerDynamic = bounds.outer;

            // Generate a bunch of integers, and record them in a known-series random.
            var wrapper = new ArchivalWrapper(rng);
            var (_, dualBound) = GetGenerationFunctions<T>(wrapper, funcName);
            for (int i = 0; i < 100; i++)
                dualBound(bounds.inner, bounds.outer);

            // Generate each of those numbers from a KSR recording what the actual generator just generated.
            // Since KSR throws exception if any value is outside the allowable bounds, this validates that the bounds
            // stay within inner (inclusive) and outer (exclusive).  In the process, we'll also build a frequency
            // dictionary for the next step.
            var frequencyDict = new Dictionary<T, int>();
            var ksr = wrapper.MakeArchivedSeries();
            (_, dualBound) = GetGenerationFunctions<T>(ksr, funcName);
            for (int i = 0; i < 100; i++)
            {
                T value = dualBound(bounds.inner, bounds.outer);
                if (!frequencyDict.ContainsKey(value))
                    frequencyDict[value] = 0;
                frequencyDict[value] += 1;
            }

            // KSR validates that the numbers don't _exceed_ the bound, but not that all numbers within the bounds
            // are generated.  Therefore, we'll check to ensure the inclusive bounds themselves were both returned
            // (which should happen w/ 100 iterations over a small range).
            Assert.True(frequencyDict.ContainsKey(bounds.inner));
            Assert.True(frequencyDict.ContainsKey(bounds.inner.CompareTo(bounds.outer) < 0 ? outerDynamic - 1 : outerDynamic + 1));

        }

        // TODO: Bounds

        [Fact]
        void NextULong() => TestIntegerFunc<ulong>(new MizuchiRandom(), "NextULong", (3, 1));

        [Fact]
        void NextLong() => TestIntegerFunc<long>(new MizuchiRandom(), "NextLong", (1, -2));
    }
}
