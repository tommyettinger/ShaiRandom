﻿/*
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
    ///   Provides generation of Poisson distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="PoissonDistribution"/> type bases upon
    ///     information presented on
    ///     <a href="https://en.wikipedia.org/wiki/Poisson_distribution">Wikipedia - Poisson distribution</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    [Serializable]
    public sealed class PoissonDistribution : IDiscreteDistribution
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="ParameterLambda"/> if none is specified.
        /// </summary>
        public const double DefaultLambda = 1;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the parameter lambda which is used for generation of Poisson distributed
        ///   random numbers.
        /// </summary>
        private double _lambda;

        /// <summary>
        ///   Gets or sets the parameter lambda which is used for generation of Poisson
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
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        public PoissonDistribution() : this(new LaserRandom(), DefaultLambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public PoissonDistribution(ulong seed) : this(new LaserRandom(seed), DefaultLambda)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seedA">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="seedB">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence. Usually an odd number; the last bit isn't used.
        /// </param>
        public PoissonDistribution(ulong seedA, ulong seedB) : this(new LaserRandom(seedA, seedB), DefaultLambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using
        ///   the specified <see cref="IRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public PoissonDistribution(IRandom generator) : this(generator, DefaultLambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of Poisson distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public PoissonDistribution(double lambda) : this(new LaserRandom(), lambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of Poisson distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public PoissonDistribution(ulong seed, double lambda) : this(new LaserRandom(seed), lambda)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PoissonDistribution"/> class, using
        ///   the specified <see cref="IRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IRandom"/> object.</param>
        /// <param name="lambda">
        ///   The parameter lambda which is used for generation of Poisson distributed random numbers.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="lambda"/> is less than or equal to zero.
        /// </exception>
        public PoissonDistribution(IRandom generator, double lambda)
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
        public double Median => Math.Floor(_lambda + 0.333333333333333333 - 0.02 / _lambda);

        /// <summary>
        ///   Gets the mode of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mode is not defined for given distribution with some parameters.
        /// </exception>
        public double[] Mode => new[] { Math.Ceiling(_lambda) - 1, Math.Floor(_lambda) };

        /// <summary>
        ///   Gets the variance of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if variance is not defined for given distribution with some parameters.
        /// </exception>
        public double Variance => _lambda;

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        public double NextDouble() => Sample(Generator, _lambda);
        /// <summary>
        ///   Returns a distributed integer random number.
        /// </summary>
        /// <returns>A distributed integer number.</returns>
        public int NextInt() => Sample(Generator, _lambda);

        public int Steps => 1;

        public int ParameterCount => 1;

        public string ParameterName(int index) => index == 0 ? "Lambda" : "";
        public double ParameterValue(int index)
        {
            if (index == 0) return ParameterLambda;
            throw new NotSupportedException($"The requested index does not exist in this PoissonDistribution.");
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
            throw new NotSupportedException($"The requested index does not exist in this PoissonDistribution.");
        }



        #endregion IContinuousDistribution Members

        #region Helpers

        /// <summary>
        ///   Determines whether Poisson distribution is defined under given parameter. The
        ///   default definition returns true if lambda is greater than zero; otherwise, it returns false.
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
        public static Func<IRandom, double, int> Sample { get; set; } = (generator, lambda) =>
        {
            int x = 0;
            double p = Math.Exp(-lambda), s = p, u = generator.NextDouble();
            while(u > s)
            {
                ++x;
                p *= lambda / x;
                s += p;
            }
            return x;
        };

        #endregion Helpers
    }
}