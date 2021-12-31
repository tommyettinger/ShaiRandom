using ShaiRandom.Distributions;
using Troschuetz.Random;

namespace ShaiRandom.Wrappers
{
    /// <summary>
    /// Wraps a ShaiRandom IEnhancedDiscreteDistribution object so it can also be used as a Troschuetz.Random IDiscreteDistribution.
    /// </summary>
    /// <remarks>
    /// This class implements both IDiscreteDistribution and IEnhancedDiscreteDistribution.  Any ShaiRandom distribution member is
    /// implemented by simply forwarding to the ShaiRandom distribution being wrapped;  all Troschuetz methods are
    /// explicitly implemented and are implemented in terms of ShaiRandom distribution methods.
    /// </remarks>
    public class TRDiscreteDistributionWrapper : TRDistributionWrapper, IDiscreteDistribution, IEnhancedDiscreteDistribution
    {
        /// <summary>
        /// The ShaiRandom distribution being wrapped.
        /// </summary>
        public new IEnhancedDiscreteDistribution Wrapped => (IEnhancedDiscreteDistribution)base.Wrapped;

        /// <summary>
        /// Creates a new wrapper which wraps the given ShaiRandom distribution.
        /// </summary>
        /// <param name="wrapped">The ShaiRandom distribution to wrap.</param>
        public TRDiscreteDistributionWrapper(IEnhancedDiscreteDistribution wrapped)
            : base(wrapped)
        { }

        /// <inheritdoc />
        public int NextInt() => Wrapped.NextInt();

        #region IDiscreteDistribution Explicit Implementation
        int IDiscreteDistribution.Next() => NextInt();
        #endregion
    }
}
