// Copyright (c) Alessio Parma <alessio.parma@gmail.com>. All rights reserved.
//
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using ShaiRandom.Generators;

namespace ShaiRandom.Distributions.Discrete
{
    /// <summary>
    ///   Provides generation of geometric distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The geometric distribution generates only discrete numbers. <br/> The implementation of
    ///     the <see cref="GeometricDistribution"/> type bases upon information presented on
    ///     <a href="http://en.wikipedia.org/wiki/Geometric_distribution">Wikipedia - Geometric
    ///     distribution</a>. An earlier version used code from the Communication Networks Class Library, but nothing
    ///     here requires its implementation, so that code was removed.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class GeometricDistribution : AbstractDistribution, IDiscreteDistribution, IAlphaDistribution<double>
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="Alpha"/> if none is specified.
        /// </summary>
        public const double DefaultAlpha = 0.5;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the parameter beta which is used for generation of uniformly distributed random numbers.
        /// </summary>
        private double _alpha;

        /// <summary>
        ///   Gets or sets the parameter alpha which is used for generation of geometric distributed
        ///   random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero or it is greater than one.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidParam"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double Alpha
        {
            get { return _alpha; }
            set
            {
                if (!IsValidAlpha(value)) throw new ArgumentOutOfRangeException(nameof(Alpha), ErrorMessages.InvalidParams);
                _alpha = value;
            }
        }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeometricDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        public GeometricDistribution() : this(new TrimRandom(), DefaultAlpha)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeometricDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public GeometricDistribution(uint seed) : this(new TrimRandom(seed), DefaultAlpha)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeometricDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public GeometricDistribution(IEnhancedRandom generator) : this(generator, DefaultAlpha)
        {
            Debug.Assert(ReferenceEquals(Generator, generator));
            Debug.Assert(Equals(Alpha, DefaultAlpha));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeometricDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of geometric distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is less than or equal to zero or it is greater than one.
        /// </exception>
        public GeometricDistribution(double alpha) : this(new TrimRandom(), alpha)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeometricDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of geometric distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is less than or equal to zero or it is greater than one.
        /// </exception>
        public GeometricDistribution(uint seed, double alpha) : this(new TrimRandom(seed), alpha)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="GeometricDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of geometric distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is less than or equal to zero or it is greater than one.
        /// </exception>
        public GeometricDistribution(IEnhancedRandom generator, double alpha) : base(generator)
        {
            var vp = IsValidParam;
            if (!vp(alpha)) throw new ArgumentOutOfRangeException(nameof(alpha), ErrorMessages.InvalidParams);
            _alpha = alpha;
        }

        #endregion Construction

        #region Instance Methods

        /// <summary>
        ///   Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if value is greater than 0.0, and less than or equal to 1.0;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValidAlpha(double value) => IsValidParam(value);

        #endregion Instance Methods

        #region IDiscreteDistribution Members

        /// <summary>
        ///   Gets the maximum possible value of distributed random numbers.
        /// </summary>
        public double Maximum => double.PositiveInfinity;

        /// <summary>
        ///   Gets the mean of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mean is not defined for given distribution with some parameters.
        /// </exception>
        public double Mean => 1.0 / Alpha;

        /// <summary>
        ///   Gets the median of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if median is not defined for given distribution with some parameters.
        /// </exception>
        public double Median
        {
            get { throw new NotSupportedException(ErrorMessages.UndefinedMedian); }
        }

        /// <summary>
        ///   Gets the minimum possible value of distributed random numbers.
        /// </summary>
        public double Minimum => 1.0;

        /// <summary>
        ///   Gets the mode of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mode is not defined for given distribution with some parameters.
        /// </exception>
        public double[] Mode => new[] { 1.0 };

        /// <summary>
        ///   Gets the variance of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if variance is not defined for given distribution with some parameters.
        /// </exception>
        public double Variance => (1.0 - Alpha) / MathUtils.Square(Alpha);

        /// <summary>
        ///   Returns a distributed random number.
        /// </summary>
        /// <returns>A distributed 32-bit signed integer.</returns>
        public int NextInt() => Sample(Generator, _alpha);

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public double NextDouble() => Sample(Generator, _alpha);

        #endregion IDiscreteDistribution Members

        #region TRandom Helpers

        /// <summary>
        ///   Determines whether geometric distribution is defined under given parameter. The
        ///   default definition returns true if alpha is greater than zero and if it is less than
        ///   or equal to one; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="GeometricDistribution"/> class.
        /// </remarks>
        public static Func<double, bool> IsValidParam { get; set; } = alpha =>
        {
            return alpha > 0.0 && alpha <= 1.0;
        };

        /// <summary>
        ///   Declares a function returning a geometric distributed 32-bit signed integer.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="GeometricDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, int> Sample { get; set; } = (generator, alpha) =>
        {
            int samples = 1;
            for (; generator.NextDouble() >= alpha; samples++)
            {
                // Empty
            }
            return samples;
        };

        #endregion TRandom Helpers
    }
}
