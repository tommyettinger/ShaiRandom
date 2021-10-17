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
using System;

namespace ShaiRandom.Distributions
{

    /// <summary>
    ///   Declares common functionality for all continuous random number distributions.
    /// </summary>
    public interface IContinuousDistribution : IDistribution
    {
    }

    /// <summary>
    ///   Declares common functionality for all discrete random number distributions.
    /// </summary>
    public interface IDiscreteDistribution : IDistribution
    {
        /// <summary>
        ///   Returns a distributed random number.
        /// </summary>
        /// <returns>A distributed 32-bit signed integer.</returns>
        int NextInt();
    }

    /// <summary>
    ///   Declares common functionality for all random number distributions.
    /// </summary>
    public interface IDistribution
    {
        /// <summary>
        ///   Gets the <see cref="IRandom"/> object that is used as underlying random number generator.
        /// </summary>
        IRandom Generator { get; }

        /// <summary>
        ///   Gets the maximum possible value of distributed random numbers.
        /// </summary>
        double Maximum { get; }

        /// <summary>
        ///   Gets the mean of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mean is not defined for given distribution with some parameters.
        /// </exception>
        double Mean { get; }

        /// <summary>
        ///   Gets the median of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if median is not defined for given distribution with some parameters.
        /// </exception>
        double Median { get; }

        /// <summary>
        ///   Gets the minimum possible value of distributed random numbers.
        /// </summary>
        double Minimum { get; }

        /// <summary>
        ///   Gets the mode of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if mode is not defined for given distribution with some parameters.
        /// </exception>
        double[] Mode { get; }

        /// <summary>
        ///   Gets the variance of distributed random numbers.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///   Thrown if variance is not defined for given distribution with some parameters.
        /// </exception>
        double Variance { get; }

        /// <summary>
        ///   Returns a distributed floating point random number.
        /// </summary>
        /// <returns>A distributed double-precision floating point number.</returns>
        double NextDouble();

        /// <summary>
        /// How many steps the IRandom generator advances each time it produces a distributed result, if a constant value.
        /// If the generator can advance a non-constant amount of steps, this should be a negative number.
        /// </summary>
        int Steps { get; }

        /// <summary>
        /// How many parameters this generator permits, or 0 if it is not parameterized.
        /// </summary>
        int ParameterCount { get; }

        /// <summary>
        /// Allows looking up the name or identifier used for a given parameter in formulas or other documentation for a distribution.
        /// </summary>
        /// <param name="index">At least 0 and less than <see cref="ParameterCount"/>.</param>
        /// <returns>The name or identifier for the given parameter, or the empty string if that parameter does not exist.</returns>
        string ParameterName(int index);

        /// <summary>
        /// Gets the current value of a given parameter by its index.
        /// </summary>
        /// <param name="index">At least 0 and less than <see cref="ParameterCount"/>.</param>
        /// <returns>The value of the given parameter, as a double (regardless of the parameter's actual type).</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if the parameter by the given index does not exist.
        /// </exception>
        double ParameterValue(int index);

        /// <summary>
        /// Sets the value of a given parameter by its index.
        /// </summary>
        /// <param name="index">At least 0 and less than <see cref="ParameterCount"/>.</param>
        /// <param name="value">The value to use for the given parameter, as a double.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if the parameter by the given index does not exist.
        /// </exception>
        void SetParameterValue(int index, double value);
    }
}
