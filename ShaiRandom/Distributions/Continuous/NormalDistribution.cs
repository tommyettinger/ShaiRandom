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

namespace ShaiRandom.Distributions
{
    using System;
    using ShaiRandom;

    /// <summary>
    ///   Provides generation of normal distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="NormalDistribution"/> type bases upon
    ///     information presented on <a href="http://en.wikipedia.org/wiki/Normal_distribution">Wikipedia -
    ///     Normal distribution</a> and the probit function from
    ///     <a href="https://web.archive.org/web/20151030215612/http://home.online.no/~pjacklam/notes/invnorm/">Peter John Acklam's page (archived)</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class NormalDistribution : IContinuousDistribution
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="ParameterMu"/> if none is specified.
        /// </summary>
        public const double DefaultMu = 0.0;

        /// <summary>
        ///   The default value assigned to <see cref="ParameterSigma"/> if none is specified.
        /// </summary>
        public const double DefaultSigma = 1.0;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the mu parameter, affecting mean.
        /// </summary>
        private double _mu;

        /// <summary>
        ///   Stores the sigma parameter, affecting variance.
        /// </summary>
        private double _sigma;

        /// <summary>
        ///   Gets or sets the mu parameter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is NaN.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidMu"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterMu
        {
            get { return _mu; }
            set
            {
                if (!IsValidMu(value)) throw new ArgumentOutOfRangeException(nameof(ParameterMu), "Parameter 0 (mu) must not be NaN .");
                _mu = value;
            }
        }

        /// <summary>
        ///   Gets or sets the sigma parameter.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidSigma"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterSigma
        {
            get { return _sigma; }
            set
            {
                if (!IsValidSigma(value)) throw new ArgumentOutOfRangeException(nameof(ParameterSigma), "Parameter 1 (sigma) must be > 0.0 .");
                _sigma = value;
            }
        }

        /// <inheritdoc />
        public IEnhancedRandom Generator { get; set; }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        public NormalDistribution() : this(new LaserRandom(), DefaultMu, DefaultSigma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public NormalDistribution(ulong seed) : this(new LaserRandom(seed), DefaultMu, DefaultSigma)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seedA">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="seedB">
        ///   Another unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public NormalDistribution(ulong seedA, ulong seedB) : this(new LaserRandom(seedA, seedB), DefaultMu, DefaultSigma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalDistribution"/> class, using
        ///   the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public NormalDistribution(IEnhancedRandom generator) : this(generator, DefaultMu, DefaultSigma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="mu">
        ///   The mu parameter.
        /// </param>
        /// <param name="sigma">
        ///   The sigma parameter.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="mu"/> is NaN, or
        ///   <paramref name="sigma"/> is less than or equal to zero.
        /// </exception>
        public NormalDistribution(double mu, double sigma) : this(new LaserRandom(), mu, sigma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="mu">
        ///   The mu parameter.
        /// </param>
        /// <param name="sigma">
        ///   The sigma parameter.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="mu"/> is NaN, or
        ///   <paramref name="sigma"/> is less than or equal to zero.
        /// </exception>
        public NormalDistribution(ulong seed, double mu, double sigma) : this(new LaserRandom(seed), mu, sigma)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NormalDistribution"/> class, using
        ///   the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="mu">
        ///   The mu parameter.
        /// </param>
        /// <param name="sigma">
        ///   The sigma parameter.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="mu"/> is NaN, or
        ///   <paramref name="sigma"/> is less than or equal to zero.
        /// </exception>
        public NormalDistribution(IEnhancedRandom generator, double mu, double sigma)
        {
            Generator = generator;
            ParameterMu = mu;
            ParameterSigma = sigma;
        }

        #endregion Construction

        #region IContinuousDistribution Members

        /// <inheritdoc />
        public double Maximum => double.PositiveInfinity;

        /// <inheritdoc />
        public double Mean => _mu;

        /// <inheritdoc />
        public double Median => _mu;

        /// <inheritdoc />
        public double Minimum => double.NegativeInfinity;

        /// <inheritdoc />
        public double[] Mode => new[] { _mu };

        /// <inheritdoc />
        public double Variance => _sigma * _sigma;

        /// <inheritdoc />
        public double NextDouble() => Sample(Generator, _mu, _sigma);

        /// <inheritdoc />
        public int Steps => 1;

        /// <inheritdoc />
        public int ParameterCount => 2;

        /// <inheritdoc />
        public string ParameterName(int index)
        {
            switch (index)
            {
                case 0: return "mu";
                case 1: return "sigma";
                default: return "";
            }
        }

        /// <inheritdoc />
        public double ParameterValue(int index)
        {
            switch (index)
            {
                case 0: return ParameterMu;
                case 1: return ParameterSigma;
                default: throw new NotSupportedException($"The requested index does not exist in this NormalDistribution.");
            }
        }

        /// <inheritdoc />
        public void SetParameterValue(int index, double value)
        {
            switch (index)
            {
                case 0: ParameterMu = value;
                    break;
                case 1: ParameterSigma = value;
                    break;
                default: throw new NotSupportedException($"The requested index does not exist in this NormalDistribution.");
            }
        }

        #endregion IContinuousDistribution Members

        #region Helpers

        /// <summary>
        ///   Determines whether the normal distribution is defined under the given mu parameter. The
        ///   default definition returns false if the parameter is NaN; otherwise, it returns true.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="NormalDistribution"/> class.
        /// </remarks>
        public static Func<double, bool> IsValidMu { get; set; } = p => !double.IsNaN(p);

        /// <summary>
        ///   Determines whether the normal distribution is defined under the given sigma parameter. The
        ///   default definition returns true if the parameter is greater than zero; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="NormalDistribution"/> class.
        /// </remarks>
        public static Func<double, bool> IsValidSigma { get; set; } = p => p > 0.0;

        /// <summary>
        ///   Declares a function returning a normal distributed floating point random number.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="NormalDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, double, double> Sample { get; set; } = (generator, mu, sigma) =>
        {
            return generator.NextNormal(mu, sigma);
        };

        #endregion Helpers
    }
}
