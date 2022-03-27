using System;
using ShaiRandom.Generators;

namespace ShaiRandom.Distributions.Wrappers
{
    /// <summary>
    /// Wraps an IContinuousDistribution to permit it to be used as an IDiscreteDistribution, by rounding the result of NextDouble().
    /// </summary>
    /// <typeparam name="T">Any IContinuousDistribution implementation.</typeparam>
    public class ToDiscreteWrapper<T> : IDiscreteDistribution where T : IContinuousDistribution
    {
        /// <summary>
        /// Creates a ToDiscreteWrapper that wraps the given continuous distribution.
        /// </summary>
        /// <param name="continuousDistribution">Any IContinuousDistribution implementation.</param>
        public ToDiscreteWrapper(T continuousDistribution) => ContinuousDistribution = continuousDistribution;
        /// <summary>
        /// The continuous distribution this wraps.
        /// </summary>
        public T ContinuousDistribution { get; private set; }
        /// <summary>
        /// The IEnhancedRandom generator the distribution uses.
        /// </summary>
        public IEnhancedRandom Generator => ContinuousDistribution.Generator;
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public double Maximum => ContinuousDistribution.Maximum;
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public double Mean => ContinuousDistribution.Mean;
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public double Median => ContinuousDistribution.Median;
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public double Minimum => ContinuousDistribution.Minimum;
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public double[] Mode => ContinuousDistribution.Mode;
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public double Variance => ContinuousDistribution.Variance;
        /// <summary>
        /// Returns the result of <see cref="NextDouble()"/>, rounded to the nearest int using banker's rounding (MidpointRounding.ToEven).
        /// </summary>
        /// <remarks>
        /// You generally should consider adjusting the parameters on the wrapped distribution so it can return values other than 0 and 1 (which
        /// would be the only possible results if a distribution produces doubles in the 0 to 1 range).
        /// </remarks>
        /// <returns>An int produced by rounding a call to <see cref="NextDouble()"/>.</returns>
        public int NextInt() => (int)Math.Round(ContinuousDistribution.NextDouble(), MidpointRounding.ToEven);
        /// <summary>
        /// Calls the wrapped distribution's NextDouble() method and returns its result verbatim.
        /// </summary>
        /// <returns>The wrapped distribution's NextDouble().</returns>
        public double NextDouble() => ContinuousDistribution.NextDouble();
    }

}
