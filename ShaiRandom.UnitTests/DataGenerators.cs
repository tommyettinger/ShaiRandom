using System.Collections.Generic;
using ShaiRandom.Generators;
using ShaiRandom.Wrappers;

namespace ShaiRandom.UnitTests
{
    /// <summary>
    /// Some static methods that generate test data, which are used across multiple tests.
    /// </summary>
    public static class DataGenerators
    {
        /// <summary>
        /// Creates every generator in the library which qualifies under the given parameters.  It will never
        /// return a <see cref="KnownSeriesRandom"/>, since that generator is a special case.
        /// </summary>
        /// <remarks>
        /// New generator implementations should be added here to ensure they're unit tested.
        /// </remarks>
        /// <param name="includeWrappers">Whether or not to include things in the ShaiRandom.Wrappers namespace.</param>
        /// <returns>All qualifying generator types in the library.</returns>
        public static IEnumerable<IEnhancedRandom> CreateGenerators(bool includeWrappers)
        {
            yield return new DistinctRandom();
            yield return new FourWheelRandom();
            yield return new LaserRandom();
            yield return new MizuchiRandom();
            yield return new RomuTrioRandom();
            yield return new StrangerRandom();
            yield return new TricycleRandom();
            yield return new Xoshiro256StarStarRandom();
            yield return new TrimRandom();
            yield return new MinRandom();
            yield return new MaxRandom();

            if (includeWrappers)
            {
                yield return new ArchivalWrapper();
                yield return new ReversingWrapper();
                yield return new TRGeneratorWrapper();
            }
        }
    }
}
