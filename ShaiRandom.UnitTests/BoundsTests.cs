using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShaiRandom.Generators;
using ShaiRandom.Wrappers;
using Xunit;
using XUnit.ValueTuples;

namespace ShaiRandom.UnitTests
{
    /// <summary>
    /// Tests which check to make sure each generator implementation's bounded generation functions return values
    /// consistent with IEnhancedRandom's definition of bounds.
    /// </summary>
    public class BoundsTests
    {
        private static readonly IEnhancedRandom[] s_generators = DataGenerators.CreateGenerators(true).ToArray();
        private static readonly (int inner, int outer)[] s_signedBounds = { (1, 3), (2, 2), (-3, -1), (-2, 1), (1, -2) };
        private static readonly (int inner, int outer)[] s_unsignedBounds = { (1, 3), (2, 2), (3, 1) };

        public static IEnumerable<(int inner, int outer, IEnhancedRandom rng)> SignedTestData =
            s_signedBounds.Combinate(s_generators);

        public static IEnumerable<(int inner, int outer, IEnhancedRandom rng)> UnsignedTestData =
            s_unsignedBounds.Combinate(s_generators);

        #region Template Tests
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

        private void TestIntegerFunc<T>(IEnhancedRandom rng, string funcName, (int inner, int outer) bounds)
            where T : IComparable<T>
        {
            // Create dynamic variable for outer so we can add/subtract when checking the bound extents
            dynamic inner = (T)Convert.ChangeType(bounds.inner, typeof(T));
            dynamic outer = (T)Convert.ChangeType(bounds.outer, typeof(T));

            // Generate a bunch of integers, and record them in a known-series random.
            var wrapper = new ArchivalWrapper(rng);
            var (outerBound, dualBound) = GetGenerationFunctions<T>(wrapper, funcName);
            for (int i = 0; i < 100; i++)
            {
                outerBound(outer);
                dualBound(inner, outer);
            }

            // Generate each of those numbers from a KSR recording what the actual generator just generated.
            // Since KSR throws exception if any value is outside the allowable bounds, this validates that the bounds
            // stay within inner (inclusive) and outer (exclusive).  In the process, we'll also build a frequency
            // dictionary for the next step.
            var frequencyDual = new Dictionary<T, int>();
            var frequencyOuter = new Dictionary<T, int>();
            var ksr = wrapper.MakeArchivedSeries();
            (outerBound, dualBound) = GetGenerationFunctions<T>(ksr, funcName);
            for (int i = 0; i < 100; i++)
            {
                T value = outerBound(outer);
                if (!frequencyOuter.ContainsKey(value))
                    frequencyOuter[value] = 0;
                frequencyOuter[value] += 1;

                value = dualBound(inner, outer);
                if (!frequencyDual.ContainsKey(value))
                    frequencyDual[value] = 0;
                frequencyDual[value] += 1;
            }

            // KSR validates that the numbers don't _exceed_ the bound, but not that all numbers within the bounds
            // are generated.  Therefore, we'll check to ensure the inclusive bounds themselves were both returned
            // (which should happen w/ 100 iterations over a small range).
            dynamic outerInclusive = bounds.inner < bounds.outer ? outer - 1 :
                bounds.inner == bounds.outer ? outer : outer + 1;
            Assert.True(frequencyDual.ContainsKey(inner));
            Assert.True(frequencyDual.ContainsKey(outerInclusive));
            Assert.True(frequencyOuter.ContainsKey((T)Convert.ChangeType(0, typeof(T))));
            Assert.True(frequencyDual.ContainsKey(outerInclusive));
        }
        #endregion


        #region Integer Function Tests
        [Theory]
        [MemberDataTuple(nameof(UnsignedTestData))]
        void NextULong(int inner, int outer, IEnhancedRandom rng)
            => TestIntegerFunc<ulong>(rng, "NextULong", (inner, outer));

        [Theory]
        [MemberDataTuple(nameof(SignedTestData))]
        void NextLong(int inner, int outer, IEnhancedRandom rng)
            => TestIntegerFunc<long>(rng, "NextLong", (inner, outer));
        #endregion
    }
}
