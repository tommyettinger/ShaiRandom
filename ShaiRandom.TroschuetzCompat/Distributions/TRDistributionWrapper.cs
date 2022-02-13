using ShaiRandom.Distributions;
using ShaiRandom.Generators;
using ShaiRandom.TroschuetzCompat.Generators;
using Troschuetz.Random;

namespace ShaiRandom.TroschuetzCompat.Distributions
{
    /// <summary>
    /// Wraps a ShaiRandom IEnhancedDistribution object so it can also be used as a Troschuetz.Random IDistribution.
    /// </summary>
    /// <remarks>
    /// This class implements both IDistribution and IEnhancedDistribution.  Any IEnhancedDistribution member is
    /// implemented by simply forwarding to the ShaiRandom distribution being wrapped;  all IDistribution methods are
    /// explicitly implemented and are implemented in terms of IEnhancedDistribution methods.
    /// </remarks>
    public class TRDistributionWrapper : IDistribution, IEnhancedDistribution
    {
        /// <summary>
        /// The ShaiRandom distribution being wrapped.
        /// </summary>
        public IEnhancedDistribution Wrapped { get; }

        /// <summary>
        /// Creates a new wrapper which wraps the given ShaiRandom distribution.
        /// </summary>
        /// <param name="wrapped">The ShaiRandom distribution to wrap.</param>
        public TRDistributionWrapper(IEnhancedDistribution wrapped) => Wrapped = wrapped;

        /// <inheritdoc />
        public IEnhancedRandom Generator => Wrapped.Generator;

        /// <inheritdoc cref="IEnhancedDistribution.Maximum" />
        public double Maximum => Wrapped.Maximum;

        /// <inheritdoc cref="IEnhancedDistribution.Mean" />
        public double Mean => Wrapped.Mean;

        /// <inheritdoc cref="IEnhancedDistribution.Median" />
        public double Median => Wrapped.Median;

        /// <inheritdoc cref="IEnhancedDistribution.Minimum" />
        public double Minimum => Wrapped.Minimum;

        /// <inheritdoc cref="IEnhancedDistribution.Mode" />
        public double[] Mode => Wrapped.Mode;

        /// <inheritdoc cref="IEnhancedDistribution.Variance" />
        public double Variance => Wrapped.Variance;

        /// <inheritdoc cref="IEnhancedDistribution.NextDouble" />
        public double NextDouble() => Wrapped.NextDouble();

        /// <inheritdoc />
        public int Steps => Wrapped.Steps;

        /// <inheritdoc />
        public int ParameterCount => Wrapped.ParameterCount;

        /// <inheritdoc />
        public string ParameterName(int index) => Wrapped.ParameterName(index);

        /// <inheritdoc />
        public double ParameterValue(int index) => Wrapped.ParameterValue(index);

        /// <inheritdoc />
        public void SetParameterValue(int index, double value) => Wrapped.SetParameterValue(index, value);

        #region IDistribution Explicit Implementations

        bool IDistribution.CanReset => false;
        bool IDistribution.Reset() => false;

        IGenerator IDistribution.Generator => Wrapped.Generator switch
        {
            IGenerator trGen => trGen,
            _ => new TRGeneratorWrapper(Wrapped.Generator)
        };

        #endregion
    }
}
