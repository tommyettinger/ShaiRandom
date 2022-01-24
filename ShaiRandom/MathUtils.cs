using System;

namespace ShaiRandom
{
    /// <summary>
    /// A collection of various math approximations and utility functions helpful for creating generators.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// A way of taking a double in the (0.0, 1.0) range and mapping it to a Gaussian or normal distribution, so high
        /// inputs correspond to high outputs, and similarly for the low range.
        /// </summary>
        /// <remarks>This is centered on 0.0 and its standard
        /// deviation seems to be 1.0 . If this is given an input of 0.0
        /// or less, it returns -38.5, which is slightly less than the result when given <see cref="double.MinValue"/>. If it is
        /// given an input of 1.0 or more, it returns 38.5, which is significantly larger than the result when given the
        /// largest double less than 1.0 (this value is further from 1.0 than <see cref="double.MinValue"/> is from 0.0). If
        /// given <see cref="double.NaN"/>, it returns NaN. It uses an algorithm by Peter John Acklam, as
        /// implemented by Sherali Karimov.
        /// <a href="https://web.archive.org/web/20150910002142/http://home.online.no/~pjacklam/notes/invnorm/impl/karimov/StatUtil.java">Original source</a>.
        /// <a href="https://web.archive.org/web/20151030215612/http://home.online.no/~pjacklam/notes/invnorm/">Information on the algorithm</a>.
        /// <a href="https://en.wikipedia.org/wiki/Probit_function">Wikipedia's page on the probit function</a> may help, but
        /// is more likely to just be confusing.
        /// <br/>
        /// Acklam's algorithm and Karimov's implementation are both quite fast. This appears faster than generating
        /// Gaussian-distributed numbers using either the Box-Muller Transform or Marsaglia's Polar Method, though it isn't
        /// as precise and can't produce as extreme min and max results in the extreme cases they should appear. If given
        /// a typical uniform random double that's exclusive on 1.0, it won't produce a result higher than
        /// 8.209536145151493, and will only produce results of at least -8.209536145151493 if 0.0 is
        /// excluded from the inputs (if 0.0 is an input, the result is -38.5).
        /// <br/>
        /// This can be used both as an optimization for generating Gaussian random values, and as a way of generating
        /// Gaussian values that match a pattern present in the inputs (which you could have by using a sub-random sequence
        /// as the input, such as those produced by a van der Corput, Halton, Sobol or R2 sequence). Most methods of generating
        /// Gaussian values (e.g. Box-Muller and Marsaglia polar) do not have any way to preserve a particular pattern.
        /// </remarks>
        /// <param name="d">should be between 0 and 1, exclusive, but other values are tolerated</param>
        /// <returns>a normal-distributed double centered on 0.0; all results will be between -38.5 and 38.5, both inclusive</returns>
        public static double Probit(double d)
        {
            if (d <= 0)
            {
                return -38.5;
            }
            else if (d >= 1)
            {
                return 38.5;
            }
            else if (d < 0.02425)
            {
                double q = Math.Sqrt(-2.0 * Math.Log(d));
                return (((((-7.784894002430293e-03 * q + -3.223964580411365e-01) * q + -2.400758277161838e+00) * q + -2.549732539343734e+00) * q + 4.374664141464968e+00) * q + 2.938163982698783e+00) / (
                    (((7.784695709041462e-03 * q + 3.224671290700398e-01) * q + 2.445134137142996e+00) * q + 3.754408661907416e+00) * q + 1.0);
            }
            else if (0.97575 < d)
            {
                double q = Math.Sqrt(-2.0 * Math.Log(1 - d));
                return -(((((-7.784894002430293e-03 * q + -3.223964580411365e-01) * q + -2.400758277161838e+00) * q + -2.549732539343734e+00) * q + 4.374664141464968e+00) * q + 2.938163982698783e+00) / (
                    (((7.784695709041462e-03 * q + 3.224671290700398e-01) * q + 2.445134137142996e+00) * q + 3.754408661907416e+00) * q + 1.0);
            }
            else
            {
                double q = d - 0.5;
                double r = q * q;
                return (((((-3.969683028665376e+01 * r + 2.209460984245205e+02) * r + -2.759285104469687e+02) * r + 1.383577518672690e+02) * r + -3.066479806614716e+01) * r + 2.506628277459239e+00) * q / (
                    ((((-5.447609879822406e+01 * r + 1.615858368580409e+02) * r + -1.556989798598866e+02) * r + 6.680131188771972e+01) * r + -1.328068155288572e+01) * r + 1.0);
            }
        }

        /// <summary>
        /// A modified modulo operator, which practically differs from <paramref name="num"/> / <paramref name="wrapTo"/>
        /// in that it wraps from 0 to <paramref name="wrapTo"/> - 1, as well as from <paramref name="wrapTo"/> - 1 to 0.
        /// </summary>
        /// <remarks>
        /// A modified modulo operator. Returns the result of  the formula
        /// (<paramref name="num"/> % <paramref name="wrapTo"/> + <paramref name="wrapTo"/>) % <paramref name="wrapTo"/>.
        ///
        /// Practically it differs from regular modulo in that the values it returns when negative values for <paramref name="num"/>
        /// are wrapped around like one would want an array index to (if wrapTo is list.length, -1 wraps to list.length - 1). For example,
        /// 0 % 3 = 0, -1 % 3 = -1, -2 % 3 = -2, -3 % 3 = 0, and so forth, but WrapTo(0, 3) = 0,
        /// WrapTo(-1, 3) = 2, WrapTo(-2, 3) = 1, WrapTo(-3, 3) = 0, and so forth. This can be useful if you're
        /// trying to "wrap" a number around at both ends, for example wrap to 3, such that 3 wraps
        /// to 0, and -1 wraps to 2. This is common if you are wrapping around an array index to the
        /// length of the array and need to ensure that positive numbers greater than or equal to the
        /// length of the array wrap to the beginning of the array (index 0), AND that negative
        /// numbers (under 0) wrap around to the end of the array (Length - 1).
        /// </remarks>
        /// <param name="num">The number to wrap.</param>
        /// <param name="wrapTo">
        /// The number to wrap to -- the result of the function is as outlined in function
        /// description, and guaranteed to be between [0, wrapTo - 1], inclusive.
        /// </param>
        /// <returns>
        /// The wrapped result, as outlined in function description. Guaranteed to lie in range [0,
        /// wrapTo - 1], inclusive.
        /// </returns>
        public static int WrapAround(int num, int wrapTo) => (num % wrapTo + wrapTo) % wrapTo;
    }
}
