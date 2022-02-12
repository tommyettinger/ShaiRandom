using ShaiRandom.Distributions;
using Troschuetz.Random;

namespace ShaiRandom.TroschuetzCompat.Distributions
{
    // /// <summary>
    // /// Wraps a ShaiRandom IEnhancedContinuousDistribution object so it can also be used as a Troschuetz.Random IContinuousDistribution.
    // /// </summary>
    // /// <remarks>
    // /// This class implements both IContinuousDistribution and IEnhancedContinuousDistribution.  Any IEnhancedDistribution member is
    // /// implemented by simply forwarding to the ShaiRandom distribution being wrapped;  all IDistribution methods are
    // /// explicitly implemented and are implemented in terms of IEnhancedDistribution methods.
    // /// </remarks>
    // public class TRContinuousDistributionWrapper : TRDistributionWrapper, IContinuousDistribution, IEnhancedContinuousDistribution
    // {
    //     /// <summary>
    //     /// The ShaiRandom distribution being wrapped.
    //     /// </summary>
    //     public new IEnhancedContinuousDistribution Wrapped => (IEnhancedContinuousDistribution)base.Wrapped;
    //
    //     /// <summary>
    //     /// Creates a new wrapper which wraps the given ShaiRandom distribution.
    //     /// </summary>
    //     /// <param name="wrapped">The ShaiRandom distribution to wrap.</param>
    //     public TRContinuousDistributionWrapper(IEnhancedContinuousDistribution wrapped)
    //         : base(wrapped)
    //     { }
    // }
}
