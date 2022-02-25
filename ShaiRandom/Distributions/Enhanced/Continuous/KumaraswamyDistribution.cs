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
using System;

namespace ShaiRandom.Enhanced.Distributions.Continuous
{

    /// <summary>
    ///   Provides generation of Kumaraswamy distributed random numbers.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The implementation of the <see cref="KumaraswamyDistribution"/> type bases upon
    ///     information presented on
    ///     <a href="https://en.wikipedia.org/wiki/Kumaraswamy_distribution">Wikipedia - Kumaraswamy distribution</a>.
    ///   </para>
    ///   <para>The thread safety of this class depends on the one of the underlying generator.</para>
    /// </remarks>
    public sealed class KumaraswamyDistribution : IEnhancedContinuousDistribution
    {
        #region Constants

        /// <summary>
        ///   The default value assigned to <see cref="ParameterA"/> if none is specified.
        /// </summary>
        public const double DefaultA = 2.0;

        /// <summary>
        ///   The default value assigned to <see cref="ParameterB"/> if none is specified.
        /// </summary>
        public const double DefaultB = 2.0;

        #endregion Constants

        #region Fields

        /// <summary>
        ///   Stores the shape parameter a.
        /// </summary>
        private double _a;

        /// <summary>
        ///   Stores the shape parameter b.
        /// </summary>
        private double _b;

        /// <summary>
        ///   Gets or sets the shape parameter a.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidParam"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterA
        {
            get { return 1.0 / _a; }
            set
            {
                if (!IsValidParam(value)) throw new ArgumentOutOfRangeException(nameof(ParameterA), "Parameter 0 (a) must be > 0.0 .");
                _a = 1.0 / value;
            }
        }

        /// <summary>
        ///   Gets or sets the shape parameter b.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="value"/> is less than or equal to zero.
        /// </exception>
        /// <remarks>
        ///   Calls <see cref="IsValidParam"/> to determine whether a value is valid and therefore assignable.
        /// </remarks>
        public double ParameterB
        {
            get { return 1.0 / _b; }
            set
            {
                if (!IsValidParam(value)) throw new ArgumentOutOfRangeException(nameof(ParameterB), "Parameter 1 (b) must be > 0.0 .");
                _b = 1.0 / value;
            }
        }

        /// <inheritdoc />
        public IEnhancedRandom Generator { get; set; }

        #endregion Fields

        #region Construction

        /// <summary>
        ///   Initializes a new instance of the <see cref="KumaraswamyDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        public KumaraswamyDistribution() : this(new LaserRandom(), DefaultA, DefaultB)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KumaraswamyDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public KumaraswamyDistribution(ulong seed) : this(new LaserRandom(seed), DefaultA, DefaultB)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="KumaraswamyDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seedA">
        ///   An unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="seedB">
        ///   Another unsigned long used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        public KumaraswamyDistribution(ulong seedA, ulong seedB) : this(new LaserRandom(seedA, seedB), DefaultA, DefaultB)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KumaraswamyDistribution"/> class, using
        ///   the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        public KumaraswamyDistribution(IEnhancedRandom generator) : this(generator, DefaultA, DefaultB)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KumaraswamyDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="a">
        ///   The shape parameter a.
        /// </param>
        /// <param name="b">
        ///   The shape parameter b.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="a"/> is less than or equal to zero, or
        ///   <paramref name="b"/> is less than or equal to zero.
        /// </exception>
        public KumaraswamyDistribution(double a, double b) : this(new LaserRandom(), a, b)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KumaraswamyDistribution"/> class, using a
        ///   <see cref="LaserRandom"/> with the specified seed value.
        /// </summary>
        /// <param name="seed">
        ///   An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        /// <param name="a">
        ///   The shape parameter a.
        /// </param>
        /// <param name="b">
        ///   The shape parameter b.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="a"/> is less than or equal to zero, or
        ///   <paramref name="b"/> is less than or equal to zero.
        /// </exception>
        public KumaraswamyDistribution(ulong seed, double a, double b) : this(new LaserRandom(seed), a, b)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="KumaraswamyDistribution"/> class, using
        ///   the specified <see cref="IEnhancedRandom"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">An <see cref="IEnhancedRandom"/> object.</param>
        /// <param name="a">
        ///   The shape parameter a.
        /// </param>
        /// <param name="b">
        ///   The shape parameter b.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="generator"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="a"/> is less than or equal to zero, or
        ///   <paramref name="b"/> is less than or equal to zero.
        /// </exception>
        public KumaraswamyDistribution(IEnhancedRandom generator, double a, double b)
        {
            Generator = generator;
            ParameterA = a;
            ParameterB = b;
        }

        #endregion Construction

        #region IContinuousDistribution Members

        /// <inheritdoc />
        public double Maximum => 1.0;

        /// <inheritdoc />
        public double Mean => throw new NotSupportedException("I have no idea how to calculate this.");

        /// <inheritdoc />
        public double Median => Math.Pow(1.0 - Math.Pow(2.0, -_b), _a);

        /// <inheritdoc />
        public double Minimum => 0.0;

        /// <inheritdoc />
        public double[] Mode => throw new NotSupportedException("I have no idea how to calculate this, or if it is even defined for all valid parameters.");

        /// <inheritdoc />
        public double Variance => throw new NotSupportedException("I have no idea how to calculate this, or if it is even defined for all valid parameters.");

        /// <inheritdoc />
        public double NextDouble() => Sample(Generator, _a, _b);

        /// <inheritdoc />
        public int Steps => 1;

        /// <inheritdoc />
        public int ParameterCount => 2;

        /// <inheritdoc />
        public string ParameterName(int index)
        {
            switch (index)
            {
                case 0: return "a";
                case 1: return "b";
                default: return "";
            }
        }

        /// <inheritdoc />
        public double ParameterValue(int index)
        {
            switch (index)
            {
                case 0: return ParameterA;
                case 1: return ParameterB;
                default: throw new NotSupportedException($"The requested index does not exist in this KumaraswamyDistribution.");
            }
        }

        /// <inheritdoc />
        public void SetParameterValue(int index, double value)
        {
            switch (index)
            {
                case 0: ParameterA = value;
                    break;
                case 1: ParameterB = value;
                    break;
                default: throw new NotSupportedException($"The requested index does not exist in this KumaraswamyDistribution.");
            }
        }

        #endregion IContinuousDistribution Members

        #region Helpers

        /// <summary>
        ///   Determines whether the Kumaraswamy distribution is defined under a given parameter. The
        ///   default definition returns true if the parameter is greater than zero; otherwise, it returns false.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="KumaraswamyDistribution"/> class.
        /// </remarks>
        public static Func<double, bool> IsValidParam { get; set; } = p => p > 0.0;

        /// <summary>
        ///   Declares a function returning a Kumaraswamy distributed floating point random number.
        /// </summary>
        /// <remarks>
        ///   This is an extensibility point for the <see cref="KumaraswamyDistribution"/> class.
        /// </remarks>
        public static Func<IEnhancedRandom, double, double, double> Sample { get; set; } = (generator, a, b) =>
        {
            return Math.Pow(1.0 - Math.Pow(generator.NextExclusiveDouble(), b), a);
        };

        #endregion Helpers
    }
}
