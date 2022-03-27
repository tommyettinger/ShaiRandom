// Copyright (c) Alessio Parma <alessio.parma@gmail.com>. All rights reserved.
//
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using ShaiRandom.Generators;

namespace ShaiRandom.Distributions.Discrete
{
    /// <summary>
    ///   Provides generation of poisson distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The poisson distribution generates only discrete numbers. <br/> The implementation of
    ///     the <see cref="PoissonDistribution"/> type bases upon information presented on
    ///     <a href="http://en.wikipedia.org/wiki/Poisson_distribution">Wikipedia - Poisson
    ///     distribution</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class PoissonDistribution : AbstractDistribution, IDiscreteDistribution, ILambdaDistribution<double>
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="Lambda"/> if none is specified.
        /// </summary>
        public const double DefaultLambda = 1;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the the parameter lambda which is used for generation of poisson distributed
        ///   random numbers.
        /// </summary>
        private double _lambda;

        /// <summary>
        ///   Gets or sets the parameter lambda which is used for generation of poisson distributed
        ///   random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidLambda"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double Lambda
        {
            get { return _lambda; }
            set
            {
                if (!IsValidLambda(value)) throw new ArgumentOutOfRangeException(nameof(Lambda), ErrorMessages.InvalidParams);
                _lambda = value;
            }
        }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        public PoissonDistribution() : this(new TrimRandom(), DefaultLambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Lambda, DefaultLambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public PoissonDistribution(uint seed) : this(new TrimRandom(seed), DefaultLambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Lambda, DefaultLambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public PoissonDistribution(IEnhancedRandom generator) : this(generator, DefaultLambda)
        {
            Debug.Assert(ReferenceEquals(Generator, generator));
            Debug.Assert(Equals(Lambda, DefaultLambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of poisson distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public PoissonDistribution(double lambda) : this(new TrimRandom(), lambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Lambda, lambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of poisson distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public PoissonDistribution(uint seed, double lambda) : this(new TrimRandom(seed), lambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Lambda, lambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of poisson distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public PoissonDistribution(IEnhancedRandom generator, double lambda) : base(generator)
        {
            var vp = IsValidParam;
            if (!vp(lambda)) throw new ArgumentOutOfRangeException(nameof(lambda), ErrorMessages.InvalidParams);
            _lambda = lambda;
        }

        #endregion Construction

        #region Instance Methods

        /// <summary>
        ///   Determines whether the specified value is valid for parameter <see cref="Lambda"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.</returns>
        public bool IsValidLambda(double value) => IsValidParam(value);

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
        public double Mean => _lambda;

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
        public double Minimum => 0.0;

        /// <summary>
        ///   Gets the mode of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mode is not defined for given distribution with some parameters.
        /// </exception>
        public double[] Mode => MathUtils.AreEqual(_lambda, Math.Floor(_lambda)) ? new[] { _lambda - 1.0, _lambda } : new[] { Math.Floor(_lambda) };

        /// <summary>
        ///   Gets the variance of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if variance is not defined for given distribution with some parameters.
        /// </exception>
        public double Variance => _lambda;

        /// <summary>
        ///   Returns a distributed random number.
        /// </summary>
        /// <returns>A distributed 32-bit signed integer.</returns>
        public int NextInt() => Sample(Generator, _lambda);

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public double NextDouble() => Sample(Generator, _lambda);

        #endregion IDiscreteDistribution Members

        #region TRandom Helpers

        /// <summary>
        ///   Determines whether poisson distribution is defined under given parameter. The default
        ///   definition returns true if lambda is greater than zero; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="PoissonDistribution"/> class.
        /// </remarks>
        public static Func<double, bool> IsValidParam { get; set; } = lambda => lambda > 0.0;

        /// <summary>
        ///   Declares a function returning an Poisson distributed floating point random number.
        ///   The implementation here is only meant for smaller lambda values.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="PoissonDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, int> Sample { get; set; } = (generator, lambda) =>
        {
            int x = 0;
            double p = Math.Exp(-lambda), s = p, u = generator.NextDouble();
            while (u > s)
            {
                ++x;
                p *= lambda / x;
                s += p;
            }
            return x;
        };

        #endregion TRandom Helpers
    }
}
