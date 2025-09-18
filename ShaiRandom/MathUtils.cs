using System;

namespace ShaiRandom
{
    /// <summary>
    /// A collection of various math approximations and utility functions helpful for creating generators.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        ///   The delta used when comparing doubles.
        /// </summary>
        public const double Tolerance = 1E-6;

        private const double
            A0 = 0.195740115269792,
            A1 = -0.652871358365296,
            A2 = 1.246899760652504,
            B0 = 0.155331081623168,
            B1 = -0.839293158122257,
            C3 = -1.000182518730158122,
            C0 = 16.682320830719986527,
            C1 = 4.120411523939115059,
            C2 = 0.029814187308200211,
            D0 = 7.173787663925508066,
            D1 = 8.759693508958633869;


        /// <summary>
        ///   Safely checks if given doubles are equal.
        /// </summary>
        /// <param name="d1">A double.</param>
        /// <param name="d2">A double.</param>
        /// <returns>True if given doubles are safely equal, false otherwise.</returns>
        public static bool AreEqual(double d1, double d2) => IsZero(d1 - d2);

        /// <summary>
        ///   Safely checks if given double is zero.
        /// </summary>
        /// <param name="d">A double.</param>
        /// <returns>True if given double is near zero, false otherwise.</returns>
        public static bool IsZero(double d) => d > -Tolerance && d < Tolerance;

        /// <summary>
        ///   Fast square power.
        /// </summary>
        /// <param name="d">A double.</param>
        /// <returns>The square of given double.</returns>
        public static double Square(double d) => d * d;

        /// <summary>
        ///   Represents an approximation of the Gamma function, using an algorithm by T. J. Stieltjes.
        /// </summary>
        /// <remarks>
        /// This is exactly equivalent to <code>MathUtils.Factorial(x - 1.0)</code> .
        /// <br />
        /// The source for this function is here: http://www.luschny.de/math/factorial/approx/SimpleCases.html
        /// </remarks>
        /// <param name="x">A double-precision floating point number.</param>
        /// <returns>
        ///   A double-precision floating point number representing an approximation of Gamma( <paramref name="x"/>).
        /// </returns>
        public static double Gamma(double x)
        {
            return Factorial(x - 1.0);
        }

        /// <summary>
        /// A close approximation to the factorial function for real numbers, using an algorithm by T. J. Stieltjes.
        /// </summary>
        /// <remarks>
        /// This performs a variable number of multiplications that starts at 1 when x is between 5 and 6, and requires more
        /// multiplications the lower x goes (to potentially many if x is, for instance, -1000.0, which would need 1006
        /// multiplications per call). As such, you should try to call this mostly on x values that are positive or have a
        /// low magnitude.
        /// <br />
        /// The source for this function is here: http://www.luschny.de/math/factorial/approx/SimpleCases.html
        /// </remarks>
        /// <param name="x">A double; should not be both large and negative.</param>
        /// <returns>The generalized factorial of the given x, approximated.</returns>
        public static double Factorial(double x)
        {
            double y = x + 1.0, p = 1.0;
            for (; y < 7; y++)
                p *= y;
            double r = Math.Exp(y * Math.Log(y) - y + 1.0 / (12.0 * y + 2.0 / (5.0 * y + 53.0 / (42.0 * y))));
            if (x < 7.0) r /= p;
            return r * Math.Sqrt(6.283185307179586 / y);
        }

        /// <summary>
        /// A way of taking a double in the [0.0, 1.0] range and mapping it to a Gaussian or normal distribution, so high
        /// inputs correspond to high outputs, and similarly for the low range.
        /// </summary>
        /// <remarks>This is centered on 0.0 and its standard deviation seems to be 1.0 .
        /// If this is given an input of 0.0, it returns -38.467454509186325 . If given an input of 1.0, it returns
        /// 38.467454509186325 . If given <see cref="double.NaN"/>, it returns NaN.
        /// <a href="https://www.researchgate.net/publication/46462650_A_New_Approximation_to_the_Normal_Distribution_Quantile_Function">Uses this algorithm by Paul Voutier</a>.
        /// <br/>
        /// This can be used both as an optimization for generating Gaussian random values, and as a way of generating
        /// Gaussian values that match a pattern present in the inputs (which you could have by using a sub-random sequence
        /// as the input, such as those produced by a van der Corput, Halton, Sobol or R2 sequence). Most methods of generating
        /// Gaussian values (e.g. Box-Muller and Marsaglia polar) do not have any way to preserve a particular pattern.
        /// <br/>
        /// <a href="https://en.wikipedia.org/wiki/Probit_function">Wikipedia has a page on the probit function.</a>
        /// </remarks>
        /// <param name="p">Should be between 0 and 1, inclusive; other values are undefined but are not errors.</param>
        /// <returns>A normal-distributed double centered on 0.0; all results will be between -38.467454509186325 and 38.467454509186325, both inclusive.</returns>
        public static double Probit(double p)
        {
            if(0.0465 > p) {
                double r = Math.Sqrt(Math.Log(p + 4.9E-324) * -2.0);
                return C3 * r + C2 + (C1 * r + C0) / (r * (r + D1) + D0);
            }
            if(0.9535 < p) {
                double r = Math.Sqrt(Math.Log(1.0 - p + 4.9E-324) * -2.0);
                return -C3 * r - C2 - (C1 * r + C0) / (r * (r + D1) + D0);
            } else {
                double q = p - 0.5, r = q * q;
                return q * (A2 + (A1 * r + A0) / (r * (r + B1) + B0));
            }
        }

        /// <summary>
        /// A modified modulo operator, which practically differs from <paramref name="num"/> / <paramref name="wrapTo"/>
        /// in that it wraps from 0 to <paramref name="wrapTo"/> - 1, as well as from <paramref name="wrapTo"/> - 1 to 0.
        /// </summary>
        /// <remarks>
        /// A modified modulo operator. Returns the result of  the formula
        /// (<paramref name="num"/> % <paramref name="wrapTo"/> + <paramref name="wrapTo"/>) % <paramref name="wrapTo"/>.
        /// <br/>
        /// Practically it differs from regular modulo in that the values it returns when negative values for <paramref name="num"/>
        /// are wrapped around like one would want an array index to (if wrapTo is list.Length, -1 wraps to list.Length - 1). For example,
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
