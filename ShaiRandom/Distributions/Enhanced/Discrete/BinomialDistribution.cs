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

namespace ShaiRandom.Enhanced.Distributions.Discrete
{
    using System;

    /// <summary>
    ///   Provides generation of Binomial distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="BinomialDistribution"/> type bases upon
    ///     information presented on
    ///     <a href="https://en.wikipedia.org/wiki/Binomial_distribution">Wikipedia - Binomial distribution</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    public sealed class BinomialDistribution : IEnhancedDiscreteDistribution
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="ParameterAlpha"/> if none is specified.
        /// </summary>
        public const double DefaultAlpha = 0.5;

        /// <summary>
        ///   The default value assigned to <see cref="ParameterBeta"/> if none is specified.
        /// </summary>
        public const int DefaultBeta = 1;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the parameter alpha which is used for generation of Binomial distributed
        ///   random numbers.
        /// </summary>
        private double _alpha;

        /// <summary>
        ///   Stores the parameter beta which is used for generation of Binomial distributed
        ///   random numbers.
        /// </summary>
        private int _beta;

        /// <summary>
        ///   Gets or sets the parameter alpha which is used for generation of Binomial
        ///   distributed random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidAlpha"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterAlpha
        {
            get { return _alpha; }
            set
            {
                if (!IsValidAlpha(value)) throw new ArgumentOutOfRangeException(nameof(ParameterAlpha), "Parameter 0 (alpha) must be >= 0.0 and <= 1.0.");
                _alpha = value;
            }
        }

        /// <summary>
        ///   Gets or sets the parameter alpha which is used for generation of Binomial
        ///   distributed random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidBeta"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public int ParameterBeta
        {
            get { return _beta; }
            set
            {
                if (!IsValidBeta(value)) throw new ArgumentOutOfRangeException(nameof(ParameterBeta), "Parameter 1 (beta) must be >= 0 .");
                _beta = value;
            }
        }

        /// <inheritdoc />
        public IEnhancedRandom Generator { get; set; }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="BinomialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        public BinomialDistribution() : this(new LaserRandom())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BinomialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public BinomialDistribution(ulong seed) : this(new LaserRandom(seed))
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="BinomialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seedA">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="seedB">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence. Usually an odd number; the last bit isn't used.
        /// </param>
        public BinomialDistribution(ulong seedA, ulong seedB) : this(new LaserRandom(seedA, seedB))
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BinomialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of Binomial distributed random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of Binomial distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is less than or equal to zero.
        /// </exception>
        public BinomialDistribution(double alpha, int beta) : this(new LaserRandom(), alpha, beta)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BinomialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of Binomial distributed random numbers.
        /// </param>
        /// <param name="beta">
        ///   The parameter beta which is used for generation of Binomial distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> or <paramref name="beta"/> is less than or equal to zero.
        /// </exception>
        public BinomialDistribution(ulong seed, double alpha, int beta) : this(new LaserRandom(seed), alpha, beta)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="BinomialDistribution"/> class, using
        ///   the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="alpha">
        ///   The parameter alpha which is used for generation of Binomial distributed random numbers.
        /// </param>
        /// <param name="beta">
        /// The parameter beta which is used for generation of Binomial distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="alpha"/> is less than or equal to zero.
        /// </exception>
        public BinomialDistribution(IEnhancedRandom generator, double alpha = DefaultAlpha, int beta = DefaultBeta)
        {
            Generator = generator;
            ParameterAlpha = alpha;
            ParameterBeta = beta;
        }

        #endregion Construction

        #region IContinuousDistribution Members

        /// <inheritdoc />
        public double Maximum => _beta;

        /// <inheritdoc />
        public double Minimum => 0.0;

        /// <inheritdoc />
        public double Mean => _alpha * _beta;

        /// <inheritdoc />
        public double Median => throw new NotSupportedException("Median is not supported by BinomialDistribution.");

        /// <inheritdoc />
        public double[] Mode => new[] { Math.Floor(_alpha * (_beta + 1.0)) };

        /// <inheritdoc />
        public double Variance => _alpha * (1.0 - _alpha) * _beta;

        /// <inheritdoc />
        public double NextDouble() => Sample(Generator, _alpha, _beta);

        /// <inheritdoc />
        public int NextInt() => Sample(Generator, _alpha, _beta);

        /// <inheritdoc />
        public int Steps => _beta;

        /// <inheritdoc />
        public int ParameterCount => 2;

        /// <inheritdoc />
        public string ParameterName(int index) => index == 0 ? "Alpha" : index == 1 ? "Beta" : "";

        /// <inheritdoc />
        public double ParameterValue(int index)
        {
            if (index == 0) return ParameterAlpha;
            if (index == 1) return ParameterBeta;
            throw new NotSupportedException($"The requested index does not exist in this BinomialDistribution.");
        }

        /// <inheritdoc />
        public void SetParameterValue(int index, double value)
        {
            if (index == 0)
            {
                if (IsValidAlpha(value))
                    ParameterAlpha = value;
                else
                    throw new NotSupportedException("The given value is invalid for Alpha.");
            }
            else if (index == 1)
            {
                if (IsValidBeta((int)value))
                    ParameterBeta = (int)value;
                else
                    throw new NotSupportedException("The given value is invalid for Beta.");
            }
            else
            {
                throw new NotSupportedException($"The requested index does not exist in this BinomialDistribution.");
            }
        }



        #endregion IContinuousDistribution Members

        #region Helpers

        /// <summary>
        ///   Determines whether Binomial distribution is defined under given parameter. The
        ///   default definition returns true if alpha is greater than or equal to zero and less than or equal to one; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="BinomialDistribution"/> class.
        /// </remarks>
        public static Func<double, bool> IsValidAlpha { get; set; } = alpha => alpha >= 0.0 && alpha <= 1.0;

        /// <summary>
        ///   Determines whether Binomial distribution is defined under given parameter. The
        ///   default definition returns true if beta is greater than or equal to zero; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="BinomialDistribution"/> class.
        /// </remarks>
        public static Func<int, bool> IsValidBeta { get; set; } = beta => beta >= 0;

        /// <summary>
        ///   Declares a function returning an Binomial distributed floating point random number.
        ///   The implementation here is only meant for smaller alpha values.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="BinomialDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, int, int> Sample { get; set; } = (generator, alpha, beta) =>
        {
            int successes = 0;
            for (int i = 0; i < beta; i++)
            {
                if (generator.NextDouble() < alpha)
                {
                    successes++;
                }
            }
            return successes;
        };


        #endregion Helpers
    }
}
