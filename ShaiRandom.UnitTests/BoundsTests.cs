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

        // Dictionary which includes all types which should be excluded from distribution checks in the below tests
        private readonly HashSet<Type> _distributionCheckExclusions = new HashSet<Type>{ typeof(MinRandom) };

        private const int NumValuesToGenerate = 100;

        // MUST be different than values used in other sets.
        private const float EqualTestValue = 1.2f;
        private static readonly (float inner, float outer)[] s_floatingBounds =
        {
            (0.000000001f, 0.000000003f), (EqualTestValue, EqualTestValue), (0, 0), (-0.000000003f, -0.000000001f),
            (-0.000000001f, 0.000000001f), (0.000000001f, -0.000000001f)
        };

        public static IEnumerable<(int inner, int outer, IEnhancedRandom rng)> SignedTestData =
            s_signedBounds.Combinate(s_generators);

        public static IEnumerable<(int inner, int outer, IEnhancedRandom rng)> UnsignedTestData =
            s_unsignedBounds.Combinate(s_generators);

        public static IEnumerable<(float inner, float outer, IEnhancedRandom rng)> FloatingTestData =
            s_floatingBounds.Combinate(s_generators);

        public static IEnumerable<IEnhancedRandom> Generators = s_generators;

        #region Template Tests
        private (Func<T> unbounded, Func<T, T> outerBound, Func<T, T, T> dualBound) GetGenerationFunctions<T>(IEnhancedRandom rng, string name)
            where T : notnull
        {
            // Find info for the functions
            var unboundedInfo = typeof(IEnhancedRandom).GetMethod(name, Array.Empty<Type>())
                                        ?? throw new Exception("Couldn't find unbounded generation method with the name given.");
            var outerBoundInfo = typeof(IEnhancedRandom).GetMethod(name, new []{typeof(T)})
                                 ?? throw new Exception("Couldn't generation method with the name given that takes a single (outer) bound.");
            var dualBoundInfo = typeof(IEnhancedRandom).GetMethod(name, new []{typeof(T), typeof(T)})
                                ?? throw new Exception("Couldn't find generation method with the name given which takes both an inner and outer bound.");

            // Create convenient Func wrappers from which we can call them.
            T Unbounded()
            {
                try
                {
                    return (T)unboundedInfo.Invoke(rng, null)!;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }
            }

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
            return (Unbounded, OuterBound, DualBound);
        }

        private void TestIntFunctionBounds<T>(IEnhancedRandom rng, string funcName, (int inner, int outer) bounds)
            where T : IComparable<T>
        {
            // Create dynamic variable for outer so we can add/subtract when checking the bound extents
            dynamic inner = (T)Convert.ChangeType(bounds.inner, typeof(T));
            dynamic outer = (T)Convert.ChangeType(bounds.outer, typeof(T));

            // Generate a bunch of integers, and record them in a known-series random.
            var wrapper = new ArchivalWrapper(rng);
            var (_, outerBound, dualBound) = GetGenerationFunctions<T>(wrapper, funcName);
            for (int i = 0; i < NumValuesToGenerate; i++)
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
            (_, outerBound, dualBound) = GetGenerationFunctions<T>(ksr, funcName);
            for (int i = 0; i < NumValuesToGenerate; i++)
            {
                T value = outerBound(outer);
                if (!frequencyOuter.ContainsKey(value))
                    frequencyOuter[value] = 0;
                frequencyOuter[value]++;

                value = dualBound(inner, outer);
                if (!frequencyDual.ContainsKey(value))
                    frequencyDual[value] = 0;
                frequencyDual[value]++;
            }

            if (_distributionCheckExclusions.Contains(rng.GetType())) return;

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

        // NOTE: This function works for the regular floating-point functions, as well as both the inclusive and exclusive
        // bound functions (at least for now).  We can't really do any sanity checking to ensure the bounds are precise;
        // we can only check to ensure there is more than 1 unique value if the range values weren't equal, and also
        // that all values returned were at least within the bounds; and all that applies to our test data regardless
        // of the exclusivity or inclusivity of its bounds.
        private void TestFloatingFunctionBounds<T>(IEnhancedRandom rng, string funcName, (float inner, float outer) bounds)
            where T : IComparable<T>
        {
            // Create dynamic variable for outer so we can add/subtract when checking the bound extents
            dynamic inner = (T)Convert.ChangeType(bounds.inner, typeof(T));
            dynamic outer = (T)Convert.ChangeType(bounds.outer, typeof(T));

            // Check if we're explicitly comparing for equality, so we can avoid our sanity check for more than
            // one unique returned value in that case
            bool isEqualCase =  Math.Abs(bounds.inner - 1.2f) < 0.00000000001 || bounds.inner == 0 && bounds.outer == 0;

            // Sanity check to make sure we haven't made the test case data so precise that floating-point imprecision
            // bit us and considered the bounds equal
            if (!isEqualCase)
                Assert.True(inner != outer);

            // Generate a bunch of values, and record them in a known-series random.
            var wrapper = new ArchivalWrapper(rng);
            var (_, outerBound, dualBound) = GetGenerationFunctions<T>(wrapper, funcName);
            for (int i = 0; i < NumValuesToGenerate; i++)
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
            (_, outerBound, dualBound) = GetGenerationFunctions<T>(ksr, funcName);
            for (int i = 0; i < NumValuesToGenerate; i++)
            {
                T value = outerBound(outer);
                if (!frequencyOuter.ContainsKey(value))
                    frequencyOuter[value] = 0;
                frequencyOuter[value]++;

                value = dualBound(inner, outer);
                if (!frequencyDual.ContainsKey(value))
                    frequencyDual[value] = 0;
                frequencyDual[value]++;
            }

            if (_distributionCheckExclusions.Contains(rng.GetType())) return;

            // KSR validates that the numbers don't _exceed_ the bound, but not that all numbers within the bounds
            // are generated.  But, especially since some implementations of floating point RNG won't be able to
            // generate all values, there's no easy way to check this.  We'll at least sanity check that there are
            // multiple values returned though if we're not checking specifically for equality, to ensure bound-crossing
            // behavior is consistent.  Since the ranges we use are small, there should definitely be at least 2 unique
            // values out of 100 generated (if not, the generator probably has severe distribution problems).
            if (isEqualCase)
            {
                Assert.Single(frequencyDual);
                Assert.Equal(outer, frequencyDual.Keys.First());

                if (bounds.inner == 0 && bounds.outer == 0)
                {
                    Assert.Single(frequencyOuter);
                    Assert.Equal(outer, frequencyOuter.Keys.First());
                }
                else
                    Assert.True(frequencyOuter.Count > 1);
            }
            else
            {
                Assert.True(frequencyDual.Count > 1);
                Assert.True(frequencyOuter.Count > 1);
            }
        }

        // NOTE: This function works for the regular floating-point functions, as well as both the inclusive and exclusive
        // functions (at least for now).  We can't really do any sanity checking to ensure the bounds are precise;
        // we can only check to ensure there is more than 1 unique value if the range values weren't equal, and also
        // that all values returned were at least within the bounds; and all that applies to our test data regardless
        // of the exclusivity or inclusivity of its bounds.
        private void TestFloatingFunctionUnbounded<T>(IEnhancedRandom rng, string funcName)
            where T : IComparable<T>
        {
            // Generate a bunch of values, and record them in a known-series random.
            var wrapper = new ArchivalWrapper(rng);
            var (unbounded, _, _) = GetGenerationFunctions<T>(wrapper, funcName);
            for (int i = 0; i < NumValuesToGenerate; i++)
                unbounded();

            // Generate each of those numbers from a KSR recording what the actual generator just generated.
            // Since KSR throws exception if any value is outside the allowable bounds, this validates that the bounds
            // stay within inner (inclusive) and outer (exclusive).  In the process, we'll also build a frequency
            // dictionary for the next step.
            var frequency = new Dictionary<T, int>();
            var ksr = wrapper.MakeArchivedSeries();
            (unbounded, _, _) = GetGenerationFunctions<T>(ksr, funcName);
            for (int i = 0; i < NumValuesToGenerate; i++)
            {
                T value = unbounded();
                if (!frequency.ContainsKey(value))
                    frequency[value] = 0;
                frequency[value]++;
            }

            if (_distributionCheckExclusions.Contains(rng.GetType())) return;

            // KSR validates that the numbers don't _exceed_ the bound, but not that all numbers within the bounds
            // are generated.  But, especially since some implementations of floating point RNG won't be able to
            // generate all values, there's no easy way to check this.  We'll at least sanity check that there are
            // multiple values returned though if we're not checking specifically for equality, to ensure bound-crossing
            // behavior is consistent.  Since the ranges we use are small, there should definitely be at least 2 unique
            // values out of 100 generated (if not, the generator probably has severe distribution problems).
            Assert.True(frequency.Count > 1);
        }
        #endregion


        #region Integer Function Tests
        [Theory]
        [MemberDataTuple(nameof(UnsignedTestData))]
        void NextULong(int inner, int outer, IEnhancedRandom rng)
            => TestIntFunctionBounds<ulong>(rng, "NextULong", (inner, outer));

        [Theory]
        [MemberDataTuple(nameof(SignedTestData))]
        void NextLong(int inner, int outer, IEnhancedRandom rng)
            => TestIntFunctionBounds<long>(rng, "NextLong", (inner, outer));

        [Theory]
        [MemberDataTuple(nameof(UnsignedTestData))]
        void NextUInt(int inner, int outer, IEnhancedRandom rng)
            => TestIntFunctionBounds<uint>(rng, "NextUInt", (inner, outer));

        [Theory]
        [MemberDataTuple(nameof(SignedTestData))]
        void NextInt(int inner, int outer, IEnhancedRandom rng)
            => TestIntFunctionBounds<int>(rng, "NextInt", (inner, outer));

        #endregion

        #region Floating-Point Function Tests
        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextFloat(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<float>(rng, "NextFloat", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextFloatUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<float>(rng, "NextFloat");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextFloatInclusiveBounds(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<float>(rng, "NextInclusiveFloat", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextFloatInclusiveUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<float>(rng, "NextInclusiveFloat");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextFloatExclusiveBounds(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<float>(rng, "NextExclusiveFloat", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextFloatExclusiveUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<float>(rng, "NextExclusiveFloat");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextDouble(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<double>(rng, "NextDouble", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextDoubleUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<double>(rng, "NextDouble");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextDoubleInclusiveBounds(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<double>(rng, "NextInclusiveDouble", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextDoubleInclusiveUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<double>(rng, "NextInclusiveDouble");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextDoubleExclusiveBounds(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<double>(rng, "NextExclusiveDouble", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextDoubleExclusiveUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<double>(rng, "NextExclusiveDouble");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextDecimal(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<decimal>(rng, "NextDecimal", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextDecimalUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<decimal>(rng, "NextDecimal");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextDecimalInclusiveBounds(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<decimal>(rng, "NextInclusiveDecimal", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextDecimalInclusiveUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<decimal>(rng, "NextInclusiveDecimal");

        [Theory]
        [MemberDataTuple(nameof(FloatingTestData))]
        void NextDecimalExclusiveBounds(float inner, float outer, IEnhancedRandom rng)
            => TestFloatingFunctionBounds<decimal>(rng, "NextExclusiveDecimal", (inner, outer));

        [Theory]
        [MemberDataEnumerable(nameof(Generators))]
        void NextDecimalExclusiveUnbounded(IEnhancedRandom rng)
            => TestFloatingFunctionUnbounded<decimal>(rng, "NextExclusiveDecimal");
        #endregion
    }
}
