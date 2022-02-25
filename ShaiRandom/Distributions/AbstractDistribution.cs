// Copyright (c) Alessio Parma <alessio.parma@gmail.com>. All rights reserved.
//
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace ShaiRandom.Distributions
{
    using System;
    using ShaiRandom;
    using ShaiRandom.Generators;

    /// <summary>
    ///   Abstract class which implements some features shared across all distributions.
    /// </summary>
    /// <remarks>The thread safety of this class depends on the one of the underlying generator.</remarks>
    [Serializable]
    public abstract class AbstractDistribution
    {
        /// <summary>
        ///   Builds a distribution using given generator.
        /// </summary>
        /// <param name="generator">The generator that will be used by the distribution.</param>
        /// <exception cref="ArgumentNullException">Given generator is null.</exception>
        protected AbstractDistribution(IEnhancedRandom generator)
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator), ErrorMessages.NullGenerator);
        }

        #region IDistribution members

        /// <summary>
        ///   Gets the <see cref="IEnhancedRandom"/> object that is used as underlying random number generator.
        /// </summary>
        public IEnhancedRandom Generator { get; }

        #endregion IDistribution members
    }
}
