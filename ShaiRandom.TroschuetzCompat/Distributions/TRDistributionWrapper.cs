using ShaiRandom.Generators;
using ShaiRandom.TroschuetzCompat.Generators;
using Troschuetz.Random;
using IDistribution = Troschuetz.Random.IDistribution;
using IShaiDistribution = ShaiRandom.Distributions.IDistribution;

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
    public class TRDistributionWrapper : IDistribution, IShaiDistribution
    {
        /// <summary>
        /// The ShaiRandom distribution being wrapped.
        /// </summary>
        public IShaiDistribution Wrapped { get; }

        /// <summary>
        /// Creates a new wrapper which wraps the given ShaiRandom distribution.
        /// </summary>
        /// <param name="wrapped">The ShaiRandom distribution to wrap.</param>
        public TRDistributionWrapper(IShaiDistribution wrapped) => Wrapped = wrapped;

        /// <inheritdoc />
        public IEnhancedRandom Generator => Wrapped.Generator;

        /// <inheritdoc cref="IDistribution.Maximum" />
        public double Maximum => Wrapped.Maximum;

        /// <inheritdoc cref="IDistribution.Mean" />
        public double Mean => Wrapped.Mean;

        /// <inheritdoc cref="IDistribution.Median" />
        public double Median => Wrapped.Median;

        /// <inheritdoc cref="IDistribution.Minimum" />
        public double Minimum => Wrapped.Minimum;

        /// <inheritdoc cref="IDistribution.Mode" />
        public double[] Mode => Wrapped.Mode;

        /// <inheritdoc cref="IDistribution.Variance" />
        public double Variance => Wrapped.Variance;

        /// <inheritdoc cref="IDistribution.NextDouble" />
        public double NextDouble() => Wrapped.NextDouble();

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
