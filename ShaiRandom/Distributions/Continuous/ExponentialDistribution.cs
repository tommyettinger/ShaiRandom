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
    ///   Provides generation of exponential distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="ExponentialDistribution"/> type bases upon
    ///     information presented on
    ///     <a href="http://en.wikipedia.org/wiki/Exponential_distribution">Wikipedia - Exponential distribution</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class ExponentialDistribution : IContinuousDistribution
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="ParameterLambda"/> if none is specified.
        /// </summary>
        public const double DefaultLambda = 1;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the parameter lambda which is used for generation of exponential distributed
        ///   random numbers.
        /// </summary>
        private double _lambda;

        /// <summary>
        ///   Gets or sets the parameter lambda which is used for generation of exponential
        ///   distributed random numbers.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidParam"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterLambda
        {
            get { return 1.0 / _lambda; }
            set
            {
                if (!IsValidParam(value)) throw new ArgumentOutOfRangeException(nameof(ParameterLambda), "Parameter 0 (Lambda) must be > 0.0 .");
                _lambda = 1.0 / value;
            }
        }

        public IRandom Generator { get; set; }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        public ExponentialDistribution() : this(new LaserRandom(), DefaultLambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public ExponentialDistribution(ulong seed) : this(new LaserRandom(seed), DefaultLambda)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seedA">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="seedB">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence; should be odd.
        /// </param>
        public ExponentialDistribution(ulong seedA, ulong seedB) : this(new LaserRandom(seedA, seedB), DefaultLambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using
        ///   the specified <see cref="IRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public ExponentialDistribution(IRandom generator) : this(generator, DefaultLambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of exponential distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public ExponentialDistribution(double lambda) : this(new LaserRandom(), lambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of exponential distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public ExponentialDistribution(ulong seed, double lambda) : this(new LaserRandom(seed), lambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using
        ///   the specified <see cref="IRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IRandom"/> object.</param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of exponential distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public ExponentialDistribution(IRandom generator, double lambda)
        {
            Generator = generator;
            ParameterLambda = lambda;
        }

        #endregion Construction

        #region IContinuousDistribution Members

        /// <summary>
        ///   Gets the maximum possible value of distributed random numbers.
        /// </summary>
        public double Maximum => double.PositiveInfinity;

        /// <summary>
        ///   Gets the minimum possible value of distributed random numbers.
        /// </summary>
        public double Minimum => 0.0;

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
        public double Median => Math.Log(2.0) * _lambda;

        /// <summary>
        ///   Gets the mode of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mode is not defined for given distribution with some parameters.
        /// </exception>
        public double[] Mode => new[] { 0.0 };

        /// <summary>
        ///   Gets the variance of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if variance is not defined for given distribution with some parameters.
        /// </exception>
        public double Variance => Math.Pow(1.0 / _lambda, -2.0);

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public double NextDouble() => Sample(Generator, _lambda);

        public int Steps => 1;

        public int ParameterCount => 1;

        public string ParameterName(int index) => index == 0 ? "Lambda" : "";
        public double ParameterValue(int index)
        {
            if (index == 0) return ParameterLambda;
            throw new NotSupportedException($"The requested index does not exist in this ExponentialDistribution.");
        }
        public void SetParameterValue(int index, double value)
        {
            if (index == 0)
            {
                if (IsValidParam(value))
                    ParameterLambda = value;
                else
                    throw new NotSupportedException("The given value is invalid for Lambda.");
            }
            else
            {
                throw new NotSupportedException($"The requested index does not exist in this ExponentialDistribution.");
            }
        }



        #endregion IContinuousDistribution Members

        #region Helpers

        /// <summary>
        ///   Determines whether exponential distribution is defined under given parameter. The
        ///   default definition returns true if lambda is greater than zero; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="ExponentialDistribution"/> class.
        /// </remarks>
        public static Func<double, bool> IsValidParam { get; set; } = lambda => lambda > 0.0;

        /// <summary>
        ///   Declares a function returning an exponential distributed floating point random number.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="ExponentialDistribution"/> class.
        /// </remarks>
        public static Func<IRandom, double, double> Sample { get; set; } = (generator, lambda) =>
        {
            return -Math.Log(1.0 - generator.NextDouble()) * lambda;
        };

        #endregion Helpers
    }
}
