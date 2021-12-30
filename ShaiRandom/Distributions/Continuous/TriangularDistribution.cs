/*
 * MIT License
 *
 * Copyright (c) 2006-2007 Stefan Troschuetz <stefan@troschuetz.de>
 * Copyright (c) 2012-2021 Alessio Parma <alessio.parma@gmail.com>
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using ShaiRandom.Generators;

namespace ShaiRandom.Distributions.Continuous
{
    using System;
    using ShaiRandom;

    /// <summary>
    ///   Provides generation of normal distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="TriangularDistribution"/> type bases upon
    ///     information presented on
    ///     <a href="http://en.wikipedia.org/wiki/Triangular_distribution">Wikipedia - Triangular
    ///     distribution</a> and the implementation in the
    ///     <a href="http://www.boost.org/libs/random/index.html">Boost Random Number Library</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class TriangularDistribution : IContinuousDistribution
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="ParameterAlpha"/> if none is specified.
        /// </summary>
        public const double DefaultAlpha = 0.0;

        /// <summary>
        ///   The default value assigned to <see cref="ParameterBeta"/> if none is specified.
        /// </summary>
        public const double DefaultBeta = 1.0;

        /// <summary>
        ///   The default value assigned to <see cref="ParameterGamma"/> if none is specified.
        /// </summary>
        public const double DefaultGamma = 0.5;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the alpha parameter, affecting minimum.
        /// </summary>
        private double _alpha;

        /// <summary>
        ///   Stores the beta parameter, affecting maximum.
        /// </summary>
        private double _beta;

        /// <summary>
        ///   Stores the gamma parameter, affecting median.
        /// </summary>
        private double _gamma;

        /// <summary>
        ///   Gets or sets the alpha parameter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is greater than or equal to <see cref="ParameterBeta"/>, or
        ///   <paramref name="value"/> is greater than <see cref="ParameterGamma"/>.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="AreValidParameters"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterAlpha
        {
            get { return _alpha; }
            set
            {
                if (!AreValidParameters(value, _beta, _gamma)) throw new ArgumentOutOfRangeException(nameof(ParameterAlpha), "Parameter 0 (alpha) is too high.");
                _alpha = value;
            }
        }

        /// <summary>
        ///   Gets or sets the beta parameter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <see cref="ParameterAlpha"/> is greater than or equal to <paramref name="value"/>, or
        ///   <paramref name="value"/> is less than <see cref="ParameterGamma"/>.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="AreValidParameters"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterBeta
        {
            get { return _beta; }
            set
            {
                if (!AreValidParameters(_alpha, value, _gamma)) throw new ArgumentOutOfRangeException(nameof(ParameterBeta), "Parameter 1 (beta) must be > alpha and <= gamma .");
                _beta = value;
            }
        }

        /// <summary>
        ///   Gets or sets the gamma parameter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <see cref="ParameterAlpha"/> is greater than <paramref name="value"/>, or <see cref="ParameterBeta"/> is
        ///   less than <paramref name="value"/>.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="AreValidParameters"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterGamma
        {
            get { return _gamma; }
            set
            {
                if (!AreValidParameters(_alpha, _beta, value)) throw new ArgumentOutOfRangeException(nameof(ParameterBeta), "Parameter 2 (gamma) must be >= alpha and <= beta .");
                _gamma = value;
            }
        }

        /// <inheritdoc />
        public IEnhancedRandom Generator { get; set; }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        public TriangularDistribution() : this(new LaserRandom(), DefaultAlpha, DefaultBeta, DefaultGamma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public TriangularDistribution(ulong seed) : this(new LaserRandom(seed), DefaultAlpha, DefaultBeta, DefaultGamma)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seedA">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="seedB">
        ///   Another unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public TriangularDistribution(ulong seedA, ulong seedB) : this(new LaserRandom(seedA, seedB), DefaultAlpha, DefaultBeta, DefaultGamma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TriangularDistribution"/> class, using
        ///   the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public TriangularDistribution(IEnhancedRandom generator) : this(generator, DefaultAlpha, DefaultBeta, DefaultGamma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="alpha">
        ///   The alpha parameter.
        /// </param>
        /// <param name="beta">
        ///   The beta parameter.
        /// </param>
        /// <param name="gamma">
        ///   The gamma parameter.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is NaN, or
        ///   <paramref name="beta"/> is less than or equal to zero.
        /// </exception>
        public TriangularDistribution(double alpha, double beta, double gamma) : this(new LaserRandom(), alpha, beta, gamma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TriangularDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="alpha">
        ///   The alpha parameter.
        /// </param>
        /// <param name="beta">
        ///   The beta parameter.
        /// </param>
        /// <param name="gamma">
        ///   The gamma parameter.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is NaN, or
        ///   <paramref name="beta"/> is less than or equal to zero.
        /// </exception>
        public TriangularDistribution(ulong seed, double alpha, double beta, double gamma) : this(new LaserRandom(seed), alpha, beta, gamma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TriangularDistribution"/> class, using
        ///   the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="alpha">
        ///   The alpha parameter.
        /// </param>
        /// <param name="beta">
        ///   The beta parameter.
        /// </param>
        /// <param name="gamma">
        ///   The gamma parameter.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is NaN, or
        ///   <paramref name="beta"/> is less than or equal to zero.
        /// </exception>
        public TriangularDistribution(IEnhancedRandom generator, double alpha, double beta, double gamma)
        {
            Generator = generator;
            ParameterAlpha = alpha;
            ParameterBeta = beta;
            ParameterGamma = gamma;
        }

        #endregion Construction

        #region IContinuousDistribution Members

        /// <inheritdoc />
        public double Maximum => _beta;

        /// <inheritdoc />
        public double Mean => (_alpha + _beta + _gamma) / 3.0;

        /// <inheritdoc />
        public double Median
        {
            get
            {
                if (_gamma >= (_beta - _alpha) / 2.0)
                {
                    return _alpha + (Math.Sqrt((_beta - _alpha) * (_gamma - _alpha)) / Math.Sqrt(2.0));
                }
                return _beta - (Math.Sqrt((_beta - _alpha) * (_beta - _gamma)) / Math.Sqrt(2.0));
            }
        }

        /// <inheritdoc />
        public double Minimum => _alpha;

        /// <inheritdoc />
        public double[] Mode => new[] { _gamma };

        /// <inheritdoc />
        public double Variance => ((_alpha * _alpha) + (_beta * _beta) + (_gamma * _gamma) - _alpha * _beta -
                                   (_alpha + _beta) * _gamma) / 18.0;

        /// <inheritdoc />
        public double NextDouble() => Sample(Generator, _alpha, _beta, _gamma);

        /// <inheritdoc />
        public int Steps => 1;

        /// <inheritdoc />
        public int ParameterCount => 3;

        /// <inheritdoc />
        public string ParameterName(int index)
        {
            switch (index)
            {
                case 0: return "alpha";
                case 1: return "beta";
                case 2: return "gamma";
                default: return "";
            }
        }

        /// <inheritdoc />
        public double ParameterValue(int index)
        {
            switch (index)
            {
                case 0: return ParameterAlpha;
                case 1: return ParameterBeta;
                case 2: return ParameterGamma;
                default: throw new NotSupportedException($"The requested index does not exist in this TriangularDistribution.");
            }
        }

        /// <inheritdoc />
        public void SetParameterValue(int index, double value)
        {
            switch (index)
            {
                case 0: ParameterAlpha = value;
                    break;
                case 1: ParameterBeta = value;
                    break;
                case 2: ParameterGamma = value;
                    break;
                default: throw new NotSupportedException($"The requested index does not exist in this TriangularDistribution.");
            }
        }

        #endregion IContinuousDistribution Members

        #region Helpers

        /// <summary>
        ///   Determines whether triangular distribution is defined under given parameters. The
        ///   default definition returns true if alpha is less than beta, and if alpha is less than
        ///   or equal to gamma, and if beta is greater than or equal to gamma; otherwise, it
        ///   returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="TriangularDistribution"/> class.
        /// </remarks>
        public static Func<double, double, double, bool> AreValidParameters { get; set; } = (alpha, beta, gamma) =>
        {
            return alpha < beta && alpha <= gamma && beta >= gamma;
        };

        /// <summary>
        ///   Declares a function returning a normal distributed floating point random number.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="TriangularDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, double, double, double> Sample { get; set; } = (generator, alpha, beta, gamma) =>
        {
            double helper1 = gamma - alpha;
            double helper2 = beta - alpha;
            double helper3 = Math.Sqrt(helper1 * helper2);
            double helper4 = Math.Sqrt(beta - gamma);
            double genNum = generator.NextDouble();
            if (genNum <= helper1 / helper2)
            {
                return alpha + Math.Sqrt(genNum) * helper3;
            }
            return beta - Math.Sqrt(genNum * helper2 - helper1) * helper4;

        };

        #endregion Helpers
    }
}
