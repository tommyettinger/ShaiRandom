﻿// Copyright (c) Alessio Parma <alessio.parma@gmail.com>. All rights reserved.
//
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using ShaiRandom.Generators;

namespace ShaiRandom.Distributions.Continuous
{
    /// <summary>
    ///   Provides generation of beta distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="BetaDistribution"/> type bases upon information
    ///     presented on <a href="http://en.wikipedia.org/wiki/Beta_distribution">Wikipedia - Beta
    ///     distribution</a> and <a href="http://www.xycoon.com/beta_randomnumbers.htm">Xycoon -
    ///     Beta Distribution</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class BetaDistribution : AbstractDistribution, IContinuousDistribution, IAlphaDistribution<double>, IBetaDistribution<double>
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="Alpha"/> if none is specified.
        /// </summary>
        public const double DefaultAlpha = 1;

        /// <summary>
        ///   The default value assigned to <see cref="Beta"/> if none is specified.
        /// </summary>
        public const double DefaultBeta = 1;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the parameter alpha which is used for generation of beta distributed random numbers.
        /// </summary>
        private double _alpha;

        /// <summary>
        ///   Stores the parameter beta which is used for generation of beta distributed random numbers.
        /// </summary>
        private double _beta;

        /// <summary>
        ///   Gets or sets the parameter alpha which is used for generation of beta distributed
        ///   random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="AreValidParams"/> to determine whether a value is valid and therefore assignable.
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
        ///   Gets or sets the parameter beta which is used for generation of beta distributed
        ///   random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="AreValidParams"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double Beta
        {
            get { return _beta; }
            set
            {
                if (!IsValidBeta(value)) throw new ArgumentOutOfRangeException(nameof(Beta), ErrorMessages.InvalidParams);
                _beta = value;
            }
        }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="BetaDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        public BetaDistribution()
            : this(new TrimRandom(), DefaultAlpha, DefaultBeta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Beta, DefaultBeta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BetaDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public BetaDistribution(uint seed)
            : this(new TrimRandom(seed), DefaultAlpha, DefaultBeta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Beta, DefaultBeta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BetaDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public BetaDistribution(IEnhancedRandom generator)
            : this(generator, DefaultAlpha, DefaultBeta)
        {
            Debug.Assert(ReferenceEquals(Generator, generator));
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Beta, DefaultBeta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BetaDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of beta distributed random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of beta distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> or <paramref name="beta"/> are less than or equal to zero.
        /// </exception>
        public BetaDistribution(double alpha, double beta)
            : this(new TrimRandom(), alpha, beta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
            Debug.Assert(Equals(Beta, beta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BetaDistribution"/> class, using a
        ///   <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of beta distributed random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of beta distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> or <paramref name="beta"/> are less than or equal to zero.
        /// </exception>
        public BetaDistribution(uint seed, double alpha, double beta)
            : this(new TrimRandom(seed), alpha, beta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
            Debug.Assert(Equals(Beta, beta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BetaDistribution"/> class, using the
        ///   specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of beta distributed random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of beta distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> or <paramref name="beta"/> are less than or equal to zero.
        /// </exception>
        public BetaDistribution(IEnhancedRandom generator, double alpha, double beta) : base(generator)
        {
            var vp = AreValidParams;
            if (!vp(alpha, beta)) throw new ArgumentOutOfRangeException($"{nameof(alpha)}/{nameof(beta)}", ErrorMessages.InvalidParams);
            _alpha = alpha;
            _beta = beta;
        }

        #endregion Construction

        #region Instance Methods

        /// <summary>
        ///   Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.</returns>
        public bool IsValidAlpha(double value) => AreValidParams(value, _beta);

        /// <summary>
        ///   Determines whether the specified value is valid for parameter <see cref="Beta"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.</returns>
        public bool IsValidBeta(double value) => AreValidParams(_alpha, value);

        #endregion Instance Methods

        #region IContinuousDistribution Members

        /// <summary>
        ///   Gets the maximum possible value of distributed random numbers.
        /// </summary>
        public double Maximum => 1.0;

        /// <summary>
        ///   Gets the mean of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mean is not defined for given distribution with some parameters.
        /// </exception>
        public double Mean => _alpha / (_alpha + _beta);

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
        public double[] Mode
        {
            get
            {
                if ((_alpha > 1) && (_beta > 1))
                {
                    return new[] { (_alpha - 1.0) / (_alpha + _beta - 2.0) };
                }
                if ((_alpha < 1) && (_beta < 1))
                {
                    return new[] { 0.0, 1.0 };
                }
                if (((_alpha < 1) && (_beta >= 1)) || (MathUtils.AreEqual(_alpha, 1) && (_beta > 1)))
                {
                    return new[] { 0.0 };
                }
                if (((_alpha >= 1) && (_beta < 1)) || ((_alpha > 1) && MathUtils.AreEqual(_beta, 1)))
                {
                    return new[] { 1.0 };
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
        public double Variance => (_alpha * _beta) / (MathUtils.Square(_alpha + _beta) * (_alpha + _beta + 1.0));

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public double NextDouble() => Sample(Generator, _alpha, _beta);

        #endregion IContinuousDistribution Members

        #region TRandom Helpers

        /// <summary>
        ///   Determines whether beta distribution is defined under given parameters. The default
        ///   definition returns true if alpha and beta are greater than zero; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="BetaDistribution"/> class.
        /// </remarks>
        public static Func<double, double, bool> AreValidParams { get; set; } = (alpha, beta) =>
        {
            return alpha > 0.0 && beta > 0.0;
        };

        /// <summary>
        ///   Declares a function returning a beta distributed floating point random number.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="BetaDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, double, double> Sample { get; set; } = (generator, alpha, beta) =>
        {
            // Formula: Gamma(a,1) / (Gamma(a,1) + Gamma(b,1)) ~ Beta(a,b)
            var x = GammaDistribution.Sample(generator, alpha, GammaDistribution.DefaultBeta);
            double t;
            do t = (x + GammaDistribution.Sample(generator, beta, GammaDistribution.DefaultBeta)); while (MathUtils.IsZero(t));
            return x / t;
        };

        #endregion TRandom Helpers
    }
}
