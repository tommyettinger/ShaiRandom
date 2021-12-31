using System;
using System.Collections.Generic;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// A collection of useful extension methods for IEnhancedRandom implementations.
    /// </summary>
    public static class EnhancedRandomExtensions
    {
        /// <summary>
        /// Returns the minimum result of trials calls to <see cref="IEnhancedRandom.NextInt(int, int)"/> using the
        /// given innerBound and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static int MinIntOf(this IEnhancedRandom rng, int innerBound, int outerBound, int trials)
        {
            int v = rng.NextInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, rng.NextInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to <see cref="IEnhancedRandom.NextInt(int, int)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static int MaxIntOf(this IEnhancedRandom rng, int innerBound, int outerBound, int trials)
        {
            int v = rng.NextInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, rng.NextInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to <see cref="IEnhancedRandom.NextLong(long, long)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static long MinLongOf(this IEnhancedRandom rng, long innerBound, long outerBound, int trials)
        {
            long v = rng.NextLong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, rng.NextLong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to <see cref="IEnhancedRandom.NextLong(long, long)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static long MaxLongOf(this IEnhancedRandom rng, long innerBound, long outerBound, int trials)
        {
            long v = rng.NextLong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, rng.NextLong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to <see cref="IEnhancedRandom.NextUInt(uint, uint)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static uint MinUIntOf(this IEnhancedRandom rng, uint innerBound, uint outerBound, int trials)
        {
            uint v = rng.NextUInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, rng.NextUInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to <see cref="IEnhancedRandom.NextUInt(uint, uint)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static uint MaxUIntOf(this IEnhancedRandom rng, uint innerBound, uint outerBound, int trials)
        {
            uint v = rng.NextUInt(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, rng.NextUInt(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to <see cref="IEnhancedRandom.NextULong(ulong, ulong)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static ulong MinULongOf(this IEnhancedRandom rng, ulong innerBound, ulong outerBound, int trials)
        {
            ulong v = rng.NextULong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, rng.NextULong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to <see cref="IEnhancedRandom.NextULong(ulong, ulong)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static ulong MaxULongOf(this IEnhancedRandom rng, ulong innerBound, ulong outerBound, int trials)
        {
            ulong v = rng.NextULong(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, rng.NextULong(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to <see cref="IEnhancedRandom.NextDouble(double, double)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static double MinDoubleOf(this IEnhancedRandom rng, double innerBound, double outerBound, int trials)
        {
            double v = rng.NextDouble(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, rng.NextDouble(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to <see cref="IEnhancedRandom.NextDouble(double, double)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static double MaxDoubleOf(this IEnhancedRandom rng, double innerBound, double outerBound, int trials)
        {
            double v = rng.NextDouble(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, rng.NextDouble(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the minimum result of trials calls to <see cref="IEnhancedRandom.NextFloat(float, float)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the lower the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the lowest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static float MinFloatOf(this IEnhancedRandom rng, float innerBound, float outerBound, int trials)
        {
            float v = rng.NextFloat(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Min(v, rng.NextFloat(innerBound, outerBound));
            }
            return v;
        }

        /// <summary>
        /// Returns the maximum result of trials calls to <see cref="IEnhancedRandom.NextFloat(float, float)"/> using the given innerBound
        /// and outerBound. The innerBound is inclusive; the outerBound is exclusive.
        /// The higher trials is, the higher the average value this returns.
        /// </summary>
        /// <param name="rng" />
        /// <param name="innerBound">the inner inclusive bound; may be positive or negative</param>
        /// <param name="outerBound">the outer exclusive bound; may be positive or negative</param>
        /// <param name="trials">how many random numbers to acquire and compare</param>
        /// <returns>the highest random number between innerBound (inclusive) and outerBound (exclusive) this found</returns>
        public static float MaxFloatOf(this IEnhancedRandom rng, float innerBound, float outerBound, int trials)
        {
            float v = rng.NextFloat(innerBound, outerBound);
            for (int i = 1; i < trials; i++)
            {
                v = Math.Max(v, rng.NextFloat(innerBound, outerBound));
            }
            return v;
        }


        /// <summary>
        /// Gets a randomly-chosen item from the given non-null, non-empty array.
        /// </summary>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="array">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen item from array.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, T[] array)
            => array[rng.NextInt(array.Length)];

        /// <summary>
        /// Gets a randomly-chosen item from the given non-null, non-empty IList.
        /// </summary>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen item from list.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, IList<T> list)
            => list[rng.NextInt(list.Count)];

        /// <summary>
        /// Shuffles the given array in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an array of some reference type; must be non-null but may contain null items</param>
        public static void Shuffle<T>(this IEnhancedRandom rng, T[] items)
            => rng.Shuffle(items, 0, items.Length);

        /// <summary>
        /// Shuffles the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        public static void Shuffle<T>(this IEnhancedRandom rng, IList<T> items)
            => rng.Shuffle(items, 0, items.Count);

        /// <summary>
        /// Shuffles a section of the given array in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an array of some reference type; must be non-null but may contain null items</param>
        /// <param name="offset">the index of the first element of the array that can be shuffled</param>
        /// <param name="length">the length of the section to shuffle</param>
        public static void Shuffle<T>(this IEnhancedRandom rng, T[] items, int offset, int length)
        {
            offset = Math.Min(Math.Max(0, offset), items.Length);
            length = Math.Min(items.Length - offset, Math.Max(0, length));
            for (int i = offset + length - 1; i > offset; i--)
            {
                int ii = rng.NextInt(offset, i + 1);
                (items[i], items[ii]) = (items[ii], items[i]);
            }
        }

        /// <summary>
        /// Shuffles a section of the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <param name="offset">the index of the first element of the IList that can be shuffled</param>
        /// <param name="length">the length of the section to shuffle</param>
        public static void Shuffle<T>(this IEnhancedRandom rng, IList<T> items, int offset, int length)
        {
            offset = Math.Min(Math.Max(0, offset), items.Count);
            length = Math.Min(items.Count - offset, Math.Max(0, length));
            for (int i = offset + length - 1; i > offset; i--)
            {
                int ii = rng.NextInt(offset, i + 1);
                (items[i], items[ii]) = (items[ii], items[i]);
            }
        }

        /// <summary>
        /// A way of taking a double in the (0.0, 1.0) range and mapping it to a Gaussian or normal distribution, so high
        /// inputs correspond to high outputs, and similarly for the low range.
        /// </summary>
        /// <remarks>This is centered on 0.0 and its standard
        /// deviation seems to be 1.0 (the same as {@link java.util.Random#nextGaussian()}). If this is given an input of 0.0
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
        /// {@code 8.209536145151493}, and will only produce results of at least {@code -8.209536145151493} if 0.0 is
        /// excluded from the inputs (if 0.0 is an input, the result is {@code -38.5}). A chief advantage of using this with
        /// a random number generator is that it only requires one random double to obtain one Gaussian value;
        /// {@link java.util.Random#nextGaussian()} generates at least two random doubles for each two Gaussian values, but
        /// may rarely require much more random generation.
        /// <br/>
        /// This can be used both as an optimization for generating Gaussian random values, and as a way of generating
        /// Gaussian values that match a pattern present in the inputs (which you could have by using a sub-random sequence
        /// as the input, such as those produced by a van der Corput, Halton, Sobol or R2 sequence). Most methods of generating
        /// Gaussian values (e.g. Box-Muller and Marsaglia polar) do not have any way to preserve a particular pattern.
        /// </remarks>
        /// <param name="rng" />
        /// <param name="d">should be between 0 and 1, exclusive, but other values are tolerated</param>
        /// <returns>a normal-distributed double centered on 0.0; all results will be between -38.5 and 38.5, both inclusive</returns>
        public static double Probit(this IEnhancedRandom rng, double d)
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
        /// Gets a normally-distributed (Gaussian) double, with a the specified mean (default 0.0) and standard deviation (default 1.0).
        /// If the standard deviation is 1.0 and the mean is 0.0, then this can produce results between -8.209536145151493 and 8.209536145151493 (both extremely rarely).
        /// </summary>
        /// <param name="rng" />
        /// <param name="mean">Mean for normal distribution.</param>
        /// <param name="stdDev">Standard deviation for normal distribution.</param>
        /// <returns>A double from the normal distribution with the specified mean (default 0.0) and standard deviation (default 1.0).</returns>
        public static double NextNormal(this IEnhancedRandom rng, double mean = 0.0, double stdDev = 1.0)
        {
            return rng.Probit(rng.NextExclusiveDouble()) * stdDev + mean;
        }
    }
}
