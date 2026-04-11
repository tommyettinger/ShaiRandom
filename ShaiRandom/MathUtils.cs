using System;
using System.Numerics;

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

        private const float
            A0F = 0.195740115269792f,
            A1F = -0.652871358365296f,
            A2F = 1.246899760652504f,
            B0F = 0.155331081623168f,
            B1F = -0.839293158122257f,
            C3F = -1.000182518730158122f,
            C0F = 16.682320830719986527f,
            C1F = 4.120411523939115059f,
            C2F = 0.029814187308200211f,
            D0F = 7.173787663925508066f,
            D1F = 8.759693508958633869f;


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
        /// A single-precision probit() approximation that returns an approximately-Gaussian-distributed float.
        /// </summary>
        /// <remarks>
        /// The function maps the lowest inputs to the most negative outputs,
        /// the highest inputs to the most positive outputs, and inputs near
        /// 0.5 to outputs near 0. All possible results are between
        /// -13.003068 and 13.003068, inclusive, for valid inputs.
        /// <a href="https://www.researchgate.net/publication/46462650_A_New_Approximation_to_the_Normal_Distribution_Quantile_Function">Uses this algorithm by Paul Voutier</a>.
        /// <a href="https://en.wikipedia.org/wiki/Probit_function">Wikipedia has a page on the probit function.</a>
        /// </remarks>
        /// <param name="p">a non-NaN float between 0 and 1 inclusive.</param>
        /// <returns>an approximately-Gaussian-distributed float between -13.003068 and 13.003068 .</returns>
        public static float ProbitF(float p)
        {
            if(0.0465f > p){
                float r = MathF.Sqrt(LogRough(p) * -2f);
                return C3F * r + C2F + (C1F * r + C0F) / (r * (r + D1F) + D0F);
            }

            if(0.9535f < p) {
                float r = MathF.Sqrt(LogRough(1f - p) * -2f);
                return -C3F * r - C2F - (C1F * r + C0F) / (r * (r + D1F) + D0F);
            }

            float h = p - 0.5f, q = h * h;
            return h * (A2F + (A1F * q + A0F) / (q * (q + B1F) + B0F));
        }

        /// <summary>
        /// Approximates the natural logarithm of {@code x} (that is, with base E), using single-precision, somewhat roughly.
        /// </summary>
        /// <remarks>
        /// Ported from <a href="https://code.google.com/archive/p/fastapprox/">fastapprox</a>, which is open source
        /// under the New BSD License.
        /// </remarks>
        /// <param name="x">the argument to the logarithm; should be greater than 0, but 0 is tolerated.</param>
        /// <returns>an approximation of the logarithm of x with base E; can be any float.</returns>
        public static float LogRough (float x)
        {
            int vx = BitConverter.SingleToInt32Bits(x);
            float mx = BitConverter.Int32BitsToSingle((vx & 0x007FFFFF) | 0x3f000000);
            return vx * 8.262958E-8f - 86.10657f - 1.0383555f * mx - 1.1962888f / (0.3520887068f + mx);
        }

        /// <summary>
        /// Given uniformly distributed ulong input, this produces approximately normal-distributed float results.
        /// </summary>
        /// <remarks>
        /// This won't preserve input patterns when x has a pattern you might want to
        /// keep, such as being quasi-random. This differs from <see cref="Probit"/>
        /// in that regard.
        /// <br/>
        /// This can't produce as extreme results in extremely-rare cases as methods
        /// like Box-Muller and Marsaglia Polar can. All possible results are between
        /// {@code -7.92908} and {@code 7.92908}, inclusive. This method is fairly
	    /// accurate to the normal distribution; the center has a rounded top.
        /// <br/>
        /// <a href="https://marc-b-reynolds.github.io/distribution/2021/03/18/CheapGaussianApprox.html">Credit
        /// to Marc B. Reynolds</a> for coming up with this clever fusion of the
        /// already-bell-curved bit count and two triangular distributions to smooth
        /// it out. Using one random long split into four parts instead of two
        /// random longs being needed is the contribution here.
        /// Using x * x + x is another contribution; it's slightly faster.
        /// </remarks>
        /// <param name="x">a typically random ulong; input patterns will not be preserved.</param>
        /// <returns>a Gaussian-distributed ("normally distributed") float between -7.92908 and 7.92908 .</returns>
        public static float NormalRough (ulong x) {
            long c = BitOperations.PopCount(x) - 32L << 16;
            long u = (long)(x * x + x); /* Note, this is always even, but it is unlikely to matter. */
            return 3.78088E-6f * (c + (short)(u) - (u >> 48) - (short)(u >> 32) - (short)(u >> 16));
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

        /// <summary>
        /// Computes and returns the multiplicative inverse of <paramref name="a"/> modulo (2 to the 32).
        /// </summary>
        /// <remarks>
        /// The modular multiplicative inverse is only defined when <paramref name="a"/> is odd. Multiplying
        /// <paramref name="a"/> by its inverse will produce 1U.
        /// </remarks>
        /// <param name="a">Must be an odd uint.</param>
        /// <returns>The multiplicative inverse of <paramref name="a"/> modulo (2 to the 32).</returns>
        public static uint ModularMultiplicativeInverse(uint a)
        {
            uint x = 2u ^ a * 3u;
            x *= 2u - a * x;
            x *= 2u - a * x;
            x *= 2u - a * x;
            return x;
        }

        /// <summary>
        /// Computes and returns the multiplicative inverse of <paramref name="a"/> modulo (2 to the 64).
        /// </summary>
        /// <remarks>
        /// The modular multiplicative inverse is only defined when <paramref name="a"/> is odd. Multiplying
        /// <paramref name="a"/> by its inverse will produce 1UL.
        /// </remarks>
        /// <param name="a">Must be an odd ulong.</param>
        /// <returns>The multiplicative inverse of <paramref name="a"/> modulo (2 to the 64).</returns>
        public static ulong ModularMultiplicativeInverse(ulong a)
        {
            ulong x = 2ul ^ a * 3ul;
            x *= 2ul - a * x;
            x *= 2ul - a * x;
            x *= 2ul - a * x;
            x *= 2ul - a * x;
            return x;
        }

        /// <summary>
        /// Attempts to gauge the quality of a "gamma" increment for an additive (Weyl) sequence.
        /// </summary>
        /// <remarks>
        /// This is stricter than the checks in Java 8's SplittableRandom.
        /// The goal here is to see if the gamma is "sufficiently random" to avoid pattern when used as an increment.
        /// Examples of gamma values that aren't random enough include 1UL, 3UL,
        /// 0xFFFFFFFFFFFFFFFFUL, 0xAAAAAAAAAAAAAAABUL, and so on.
        /// This returns the "score" for any gamma value, where the score is the maximum difference of four bit counts
        /// from an ideal of 32. The values that have their bits counted are:
        /// <ul>
        ///     <li>The gamma itself,</li>
        ///     <li>The Gray code of the gamma, defined as (gamma ^ (gamma >>> 1)),</li>
        ///     <li>The <see cref="ModularMultiplicativeInverse(ulong)"/> of the gamma,</li>
        ///     <li>And the Gray code of the above inverse of the gamma.</li>
        /// </ul>
        /// A score of 9 can typically be considered "potentially problematic," and though it isn't necessarily a real
        /// problem, there are so many other possible gammas that it should be avoided. Scores higher than 9 can be
        /// considered more "problematic," and scores less than 9 are probably fine for SplitMix gammas.
        /// <br/>
        /// If the given gamma is even, it is not suitable as a SplitMix gamma automatically, and the maximum (worst)
        /// rating is returned, 32.
        /// <br/>
        /// This was informed by O'Neill's blog post about SplittableRandom's gamma,
        /// https://www.pcg-random.org/posts/bugs-in-splitmix.html
        /// </remarks>
        /// <param name="gamma">Any ulong, though almost always an odd number, that would be added as an increment in a sequence.</param>
        /// <returns>How far the given gamma is from an optimal score of 0.</returns>
        public static int RateGamma(ulong gamma)
        {
            if ((gamma & 1UL) == 0UL) return 32;
            ulong inverse = ModularMultiplicativeInverse(gamma);
            return Math.Max(Math.Max(Math.Max(
                        Math.Abs(BitOperations.PopCount(gamma) - 32),
                        Math.Abs(BitOperations.PopCount(gamma ^ gamma >> 1) - 32)),
                    Math.Abs(BitOperations.PopCount(inverse) - 32)),
                Math.Abs(BitOperations.PopCount(inverse ^ inverse >> 1) - 32));
        }


        /// <summary>
        /// Attempts to improve the quality of a "gamma" increment for an additive (Weyl) sequence.
        /// </summary>
        /// <remarks>
        /// This is stricter than the checks in Java 8's SplittableRandom.
        /// The goal here is to see if the gamma is "sufficiently random" to avoid pattern when used as an increment.
        /// Examples of gamma values that aren't random enough include 1UL, 3UL,
        /// 0xFFFFFFFFFFFFFFFFUL, 0xAAAAAAAAAAAAAAABUL, and so on.
        /// This rejects any gamma value where any of four bit counts are less than 32 - threshold or greater
        /// than 32 + threshold. The values that have their bits counted are:
        /// <ul>
        ///     <li>The gamma itself,</li>
        ///     <li>The Gray code of the gamma, defined as {@code (gamma ^ (gamma >>> 1))},</li>
        ///     <li>The {@link MathTools#modularMultiplicativeInverse(long)} of the gamma,</li>
        ///     <li>And the Gray code of the above inverse of the gamma.</li>
        /// </ul>
        /// If a gamma is rejected, this multiplies it by an LCG constant, 0xD1342543DE82EF95UL, adds an increasing even
        /// number (first 2, then 4, then 6, and so on) and tries again repeatedly. It returns the first gamma that wasn't
        /// rejected, which could be the original gamma.
        /// <br/>
        /// This was informed by O'Neill's blog post about SplittableRandom's gamma,
        /// https://www.pcg-random.org/posts/bugs-in-splitmix.html
        /// </remarks>
        /// <param name="gamma">Any ulong, though almost always an odd number, that would be added as an increment in a sequence.</param>
        /// <param name="threshold">The maximum acceptable "score" as evaluated by <see cref="RateGamma(ulong)"/>; must be at least 1.</param>
        /// <returns>The gamma or a modification upon it such that its bits are "sufficiently random" to be a good increment.</returns>
        public static ulong FixGamma(ulong gamma, int threshold)
        {
            gamma |= 1UL;
            threshold = Math.Max(1, threshold);
            ulong inverse, add = 0UL;
            while (Math.Abs(BitOperations.PopCount(gamma) - 32) > threshold
                   || Math.Abs(BitOperations.PopCount(gamma ^ gamma >> 1) - 32) > threshold
                   || Math.Abs(BitOperations.PopCount(inverse = ModularMultiplicativeInverse(gamma)) - 32) > threshold
                   || Math.Abs(BitOperations.PopCount(inverse ^ inverse >> 1) - 32) > threshold)
            {
                gamma = gamma * 0xD1342543DE82EF95UL + (add += 2UL);
            }
            return gamma;
        }
    }
}
