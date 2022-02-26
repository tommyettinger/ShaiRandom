// Copyright (c) Alessio Parma <alessio.parma@gmail.com>. All rights reserved.
//
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace ShaiRandom.Distributions.Continuous
{
    using System;
    using System.Diagnostics;
    using ShaiRandom;
    using ShaiRandom.Generators;

    /// <summary>
    ///   Provides generation of weibull distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="WeibullDistribution"/> type bases upon information
    ///     presented on <a href="http://en.wikipedia.org/wiki/Weibull_distribution">Wikipedia -
    ///     Weibull distribution</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class WeibullDistribution : AbstractDistribution, IContinuousDistribution, IAlphaDistribution<double>, ILambdaDistribution<double>
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="Alpha"/> if none is specified.
        /// </summary>
        public const double DefaultAlpha = 1;

        /// <summary>
        ///   The default value assigned to <see cref="Lambda"/> if none is specified.
        /// </summary>
        public const double DefaultLambda = 1;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the parameter alpha which is used for generation of weibull distributed random numbers.
        /// </summary>
        private double _alpha;

        /// <summary>
        ///   Stores the parameter lambda which is used for generation of weibull distributed random numbers.
        /// </summary>
        private double _lambda;

        /// <summary>
        ///   Gets or sets the parameter alpha which is used for generation of weibull distributed
        ///   random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidAlpha"/> to determine whether a value is valid and therefore assignable.
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

        /// <summary>
        ///   Gets or sets the parameter lambda which is used for generation of erlang distributed
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
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        public WeibullDistribution() : this(new TrimRandom(), DefaultAlpha, DefaultLambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Lambda, DefaultLambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public WeibullDistribution(uint seed) : this(new TrimRandom(seed), DefaultAlpha, DefaultLambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Lambda, DefaultLambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public WeibullDistribution(IEnhancedRandom generator) : this(generator, DefaultAlpha, DefaultLambda)
        {
            Debug.Assert(ReferenceEquals(Generator, generator));
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Lambda, DefaultLambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of weibull distributed random numbers.
        /// </param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of weibull distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> or <paramref name="lambda"/> are less than or equal to zero.
        /// </exception>
        public WeibullDistribution(double alpha, double lambda) : this(new TrimRandom(), alpha, lambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
            Debug.Assert(Equals(Lambda, lambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of weibull distributed random numbers.
        /// </param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of weibull distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> or <paramref name="lambda"/> are less than or equal to zero.
        /// </exception>
        public WeibullDistribution(uint seed, double alpha, double lambda)
            : this(new TrimRandom(seed), alpha, lambda)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
            Debug.Assert(Equals(Lambda, lambda));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeibullDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of weibull distributed random numbers.
        /// </param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of weibull distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> or <paramref name="lambda"/> are less than or equal to zero.
        /// </exception>
        public WeibullDistribution(IEnhancedRandom generator, double alpha, double lambda) : base(generator)
        {
            var vp = AreValidParams;
            if (!vp(alpha, lambda)) throw new ArgumentOutOfRangeException($"{nameof(alpha)}/{nameof(lambda)}", ErrorMessages.InvalidParams);
            _alpha = alpha;
            _lambda = lambda;
        }

        #endregion Construction

        #region Instance Methods

        /// <summary>
        ///   Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.</returns>
        public bool IsValidAlpha(double value) => AreValidParams(value, Lambda);

        /// <summary>
        ///   Determines whether the specified value is valid for parameter <see cref="Lambda"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.</returns>
        public bool IsValidLambda(double value) => AreValidParams(_alpha, value);

        #endregion Instance Methods

        #region IContinuousDistribution Members

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
        public double Mean => Lambda * MathUtils.Gamma(1.0 + 1.0 / _alpha);

        /// <summary>
        ///   Gets the median of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if median is not defined for given distribution with some parameters.
        /// </exception>
        public double Median => Lambda * Math.Pow(Math.Log(2.0), 1.0 / _alpha);

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
        public double[] Mode
        {
            get
            {
                if (_alpha >= 1.0)
                {
                    return new[] { Lambda * Math.Pow(1.0 - 1.0 / _alpha, 1.0 / _alpha) };
                }
                throw new NotSupportedException(ErrorMessages.UndefinedModeForParams);
            }
        }

        /// <summary>
        ///   Gets the variance of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if variance is not defined for given distribution with some parameters.
        /// </exception>
        public double Variance => MathUtils.Square(Lambda) * MathUtils.Gamma(1.0 + 2.0 / _alpha) - MathUtils.Square(Mean);

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public double NextDouble() => Sample(Generator, _alpha, _lambda);

        #endregion IContinuousDistribution Members

        #region TRandom Helpers

        /// <summary>
        ///   Determines whether weibull distribution is defined under given parameters. The default
        ///   definition returns true if alpha and lambda are greater than zero; otherwise, it
        ///   returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="WeibullDistribution"/> class.
        /// </remarks>
        public static Func<double, double, bool> AreValidParams { get; set; } = (alpha, lambda) =>
        {
            return alpha > 0.0 && lambda > 0.0;
        };

        /// <summary>
        ///   Declares a function returning a weibull distributed floating point random number.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="WeibullDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, double, double> Sample { get; set; } = (generator, alpha, lambda) =>
        {
            var helper1 = 1.0 / alpha;
            // Subtracts a random number from 1.0 to avoid Math.Log(0.0).
            return lambda * Math.Pow(-Math.Log(1.0 - generator.NextDouble()), helper1);
        };

        #endregion TRandom Helpers
    }
}
