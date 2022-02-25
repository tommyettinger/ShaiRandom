// Copyright (c) Alessio Parma <alessio.parma@gmail.com>. All rights reserved.
//
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace ShaiRandom.Distributions.Discrete
{
    using System;
    using System.Diagnostics;
    using ShaiRandom;
    using ShaiRandom.Generators;

    /// <summary>
    ///   Provides generation of discrete uniformly distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The discrete uniform distribution generates only discrete numbers. <br/> The
    ///     implementation of the <see cref="DiscreteUniformDistribution"/> type bases upon
    ///     information presented on
    ///     <a href="http://en.wikipedia.org/wiki/Uniform_distribution_%28discrete%29">Wikipedia -
    ///     Uniform distribution (discrete)</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class DiscreteUniformDistribution : AbstractDistribution, IDiscreteDistribution, IAlphaDistribution<int>, IBetaDistribution<int>
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="Alpha"/> if none is specified.
        /// </summary>
        public const int DefaultAlpha = 0;

        /// <summary>
        ///   The default value assigned to <see cref="Beta"/> if none is specified.
        /// </summary>
        public const int DefaultBeta = 1;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the parameter beta which is used for generation of uniformly distributed random numbers.
        /// </summary>
        private int _alpha;

        /// <summary>
        ///   Stores the parameter beta which is used for generation of uniformly distributed random numbers.
        /// </summary>
        private int _beta;

        /// <summary>
        ///   Gets or sets the parameter alpha which is used for generation of uniformly distributed
        ///   random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is greater than <see cref="Beta"/>.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="AreValidParams"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public int Alpha
        {
            get { return _alpha; }
            set
            {
                if (!IsValidAlpha(value)) throw new ArgumentOutOfRangeException(nameof(Alpha), ErrorMessages.InvalidParams);
                _alpha = value;
            }
        }

        /// <summary>
        ///   Gets or sets the parameter beta which is used for generation of uniformly distributed
        ///   random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <see cref="Alpha"/> is greater than <paramref name="value"/>, or
        ///   <paramref name="value"/> is equal to <see cref="int.MaxValue"/>.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="AreValidParams"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public int Beta
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
        ///   Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class,
        ///   using a <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        public DiscreteUniformDistribution() : this(new TrimRandom(), DefaultAlpha, DefaultBeta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Beta, DefaultBeta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class,
        ///   using a <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public DiscreteUniformDistribution(uint seed) : this(new TrimRandom(seed), DefaultAlpha, DefaultBeta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Beta, DefaultBeta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class,
        ///   using the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public DiscreteUniformDistribution(IEnhancedRandom generator) : this(generator, DefaultAlpha, DefaultBeta)
        {
            Debug.Assert(ReferenceEquals(Generator, generator));
            Debug.Assert(Equals(Alpha, DefaultAlpha));
            Debug.Assert(Equals(Beta, DefaultBeta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class,
        ///   using a <see cref="TrimRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of discrete uniform distributed
        ///   random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of discrete uniform distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is greater than <paramref name="beta"/>, or
        ///   <paramref name="beta"/> is equal to <see cref="int.MaxValue"/>.
        /// </exception>
        public DiscreteUniformDistribution(int alpha, int beta) : this(new TrimRandom(), alpha, beta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
            Debug.Assert(Equals(Beta, beta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class,
        ///   using a <see cref="TrimRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of discrete uniform distributed
        ///   random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of discrete uniform distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is greater than <paramref name="beta"/>, or
        ///   <paramref name="beta"/> is equal to <see cref="int.MaxValue"/>.
        /// </exception>
        public DiscreteUniformDistribution(uint seed, int alpha, int beta)
            : this(new TrimRandom(seed), alpha, beta)
        {
            Debug.Assert(Generator is TrimRandom);
            Debug.Assert(Equals(Alpha, alpha));
            Debug.Assert(Equals(Beta, beta));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DiscreteUniformDistribution"/> class,
        ///   using the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of discrete uniform distributed
        ///   random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of discrete uniform distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is greater than <paramref name="beta"/>, or
        ///   <paramref name="beta"/> is equal to <see cref="int.MaxValue"/>.
        /// </exception>
        public DiscreteUniformDistribution(IEnhancedRandom generator, int alpha, int beta) : base(generator)
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
        /// <returns>
        ///   <see langword="true"/> if value is less than or equal to <see cref="Beta"/>;
        ///   otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValidAlpha(int value) => AreValidParams(value, _beta);

        /// <summary>
        ///   Determines whether the specified value is valid for parameter <see cref="Beta"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if value is greater than or equal to <see cref="Alpha"/>, and
        ///   less than <see cref="int.MaxValue"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsValidBeta(int value) => AreValidParams(_alpha, value);

        #endregion Instance Methods

        #region IDiscreteDistribution Members

        /// <summary>
        ///   Gets the maximum possible value of distributed random numbers.
        /// </summary>
        public double Maximum => _beta;

        /// <summary>
        ///   Gets the mean of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mean is not defined for given distribution with some parameters.
        /// </exception>
        public double Mean => Alpha / 2.0 + _beta / 2.0;

        /// <summary>
        ///   Gets the median of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if median is not defined for given distribution with some parameters.
        /// </exception>
        public double Median => Alpha / 2.0 + _beta / 2.0;

        /// <summary>
        ///   Gets the minimum possible value of distributed random numbers.
        /// </summary>
        public double Minimum => Alpha;

        /// <summary>
        ///   Gets the mode of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mode is not defined for given distribution with some parameters.
        /// </exception>
        public double[] Mode
        {
            get { throw new NotSupportedException(ErrorMessages.UndefinedMode); }
        }

        /// <summary>
        ///   Gets the variance of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if variance is not defined for given distribution with some parameters.
        /// </exception>
        public double Variance => (MathUtils.Square(_beta - Alpha + 1.0) - 1.0) / 12.0;

        /// <summary>
        ///   Returns a distributed random number.
        /// </summary>
        /// <returns>A distributed 32-bit signed integer.</returns>
        public int Next() => Sample(Generator, _alpha, _beta);

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public double NextDouble() => Sample(Generator, _alpha, _beta);

        #endregion IDiscreteDistribution Members

        #region TRandom Helpers

        /// <summary>
        ///   Determines whether discrete uniform distribution is defined under given parameters.
        ///   The default definition returns true if alpha is less than or equal to beta, and if
        ///   beta is less than <see cref="int.MaxValue"/>; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="DiscreteUniformDistribution"/> class.
        /// </remarks>
        public static Func<int, int, bool> AreValidParams { get; set; } = (alpha, beta) =>
        {
            return alpha <= beta && beta < int.MaxValue;
        };

        /// <summary>
        ///   Declares a function returning a discrete uniform distributed 32-bit signed integer.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="DiscreteUniformDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, int, int, int> Sample { get; set; } = (generator, alpha, beta) =>
        {
            return generator.NextInt(alpha, beta + 1);
        };

        #endregion TRandom Helpers
    }
}
