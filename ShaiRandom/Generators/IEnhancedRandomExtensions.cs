using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// Thrown when a list-based random selection function can't find an item satisfying its selector within the
    /// number of attempts specified.
    /// </summary>
    public class MaxAttemptsReachedException : Exception
    {
        /// <inheritdoc />
        public MaxAttemptsReachedException()
            : base("Couldn't find random item satisfying selector within max attempts specified.")
        { }
    }

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
        /// Gets a randomly-chosen item from the given non-empty span.
        /// </summary>
        /// <exception cref="ArgumentException">An empty span was given.</exception>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the span.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <returns>A randomly-chosen item from the span.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, ReadOnlySpan<T> items)
        {
            if (items.Length == 0)
                throw new ArgumentException("Items for random selection must be non-empty.", nameof(items));

            return items[rng.NextInt(items.Length)];
        }

        /// <summary>
        /// Continuously selects random items from the given non-empty span, until one is found for which
        /// <paramref name="selector"/> returns true.
        /// </summary>
        /// <remarks>
        /// This function will never return if there is no object in the span meeting the criteria of the selector
        /// function, and may take an extremely long time if the list is very big and there are very few items in the
        /// list meeting the selector's criteria.  For more deterministic termination, consider using the overload
        /// of this function taking a maximum number of attempts:
        /// <see cref="RandomElement{T}(ShaiRandom.Generators.IEnhancedRandom,System.ReadOnlySpan{T}, Func{T, bool}, int)"/>
        /// </remarks>
        /// <exception cref="ArgumentException">An empty span was given.</exception>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the span.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid item to select.</param>
        /// <returns>A randomly-chosen item from the span for which <paramref name="selector"/> returns true.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, ReadOnlySpan<T> items, Func<T, bool> selector)
        {
            T item = rng.RandomElement(items);
            while (!selector(item))
                item = rng.RandomElement(items);

            return item;
        }

        /// <summary>
        /// Continuously selects random items from the given non-empty span, until either one is found for which
        /// <paramref name="selector"/> returns true, or the maximum number of attempts is reached.  An exception will
        /// be thrown if the function is unable to generate a valid item within the number of attempts specified.
        /// </summary>
        /// <exception cref="ArgumentException">An empty span was given.</exception>
        /// <exception cref="MaxAttemptsReachedException">The function could not find a valid element within the maximum number of tries.</exception>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the span.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid item to select.</param>
        /// <param name="maxTries">Maximum number of times to try generating a valid value before giving up and throwing an exception.</param>
        /// <returns>A randomly-chosen item from the span for which <paramref name="selector"/> returns true.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, ReadOnlySpan<T> items, Func<T, bool> selector, int maxTries)
        {
            if (maxTries <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxTries),
                    "Value must be > 0; for infinite retries, use the overload without a maxTries parameter.");

            int curTries = 0;
            while (curTries < maxTries)
            {
                T item = rng.RandomElement(items);
                if (selector(item))
                    return item;

                curTries++;
            }

            throw new MaxAttemptsReachedException();
        }

        /// <summary>
        /// Gets a randomly-chosen item from the given non-null, non-empty IReadOnlyList.
        /// </summary>
        /// <remarks>
        /// Note that this function can take arrays as well as the <paramref name="items"/> value, since arrays implement
        /// IReadOnlyList.
        /// </remarks>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="items">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen item from list.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, IReadOnlyList<T> items)
        {
            int count = items.Count;
            if (count == 0)
                throw new ArgumentException("Items for random selection must be non-empty.", nameof(items));

            return items[rng.NextInt(count)];
        }

        /// <summary>
        /// Continuously selects random items from the given non-empty IReadOnlyList, until one is found for which
        /// <paramref name="selector"/> returns true.
        /// </summary>
        /// <remarks>
        /// This function will never return if there is no object in the list meeting the criteria of the selector
        /// function, and may take an extremely long time if the list is very big and there are very few items in the
        /// list meeting the selector's criteria.  For more deterministic termination, consider using the overload
        /// of this function taking a maximum number of attempts:
        /// <see cref="RandomElement{T}(ShaiRandom.Generators.IEnhancedRandom, IReadOnlyList{T}, Func{T, bool}, int)"/>
        ///
        /// Note that this function can take arrays as well as the <paramref name="items"/> value, since arrays implement
        /// IReadOnlyList.
        /// </remarks>
        /// <exception cref="ArgumentException">An empty list was given.</exception>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid item to select.</param>
        /// <returns>A randomly-chosen item from the list for which <paramref name="selector"/> returns true.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, IReadOnlyList<T> items, Func<T, bool> selector)
        {
            T item = rng.RandomElement(items);
            while (!selector(item))
                item = rng.RandomElement(items);

            return item;
        }

        /// <summary>
        /// Continuously selects random items from the given non-empty IReadOnlyList, until either one is found for which
        /// <paramref name="selector"/> returns true, or the maximum number of attempts is reached.  An exception will
        /// be thrown if the function is unable to generate a valid item within the number of attempts specified.
        /// </summary>
        /// <remarks>
        /// Note that this function can take arrays as well as the <paramref name="items"/> value, since arrays implement
        /// IReadOnlyList.
        /// </remarks>
        /// <exception cref="ArgumentException">An empty list was given.</exception>
        /// <exception cref="MaxAttemptsReachedException">The function could not find a valid element within the maximum number of tries.</exception>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid item to select.</param>
        /// <param name="maxTries">Maximum number of times to try generating a valid value before giving up and throwing an exception.</param>
        /// <returns>A randomly-chosen item from the list for which <paramref name="selector"/> returns true.</returns>
        public static T RandomElement<T>(this IEnhancedRandom rng, IReadOnlyList<T> items, Func<T, bool> selector, int maxTries)
        {
            if (maxTries <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxTries),
                    "Value must be > 0; for infinite retries, use the overload without a maxTries parameter.");

            int curTries = 0;
            while (curTries < maxTries)
            {
                T item = rng.RandomElement(items);
                if (selector(item))
                    return item;

                curTries++;
            }

            throw new MaxAttemptsReachedException();
        }

        /// <summary>
        /// Gets a randomly-chosen value that is a valid index for an item from the given non-empty span.
        /// </summary>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the span.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <returns>A randomly-chosen value that is a valid index in the span.</returns>
        public static int RandomIndex<T>(this IEnhancedRandom rng, ReadOnlySpan<T> items)
            => rng.NextInt(items.Length);

        /// <summary>
        /// Continuously selects random indices from the given (non-empty) span, until one is found for which the
        /// given <paramref name="selector"/> returns true.
        /// </summary>
        /// <remarks>
        /// This function will never return if there is no object in the span meeting the criteria of the selector
        /// function, and may take an extremely long time if the list is very big and there are very few items in the
        /// list meeting the selector's criteria.  For more deterministic termination, consider using the overload
        /// of this function taking a maximum number of attempts:
        /// <see cref="RandomIndex{T}(ShaiRandom.Generators.IEnhancedRandom,System.ReadOnlySpan{T}, Func{int, bool}, int)"/>
        /// </remarks>
        /// <exception cref="ArgumentException">An empty span was given.</exception>
        /// <param name="rng" />
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid selection of index.</param>
        /// <typeparam name="T">The type of items in the span.</typeparam>
        /// <returns>A randomly-chosen value that is a valid index in the span, for which <paramref name="selector"/> returns true.</returns>
        public static int RandomIndex<T>(this IEnhancedRandom rng, ReadOnlySpan<T> items, Func<int, bool> selector)
        {
            int idx = rng.RandomIndex(items);
            while (!selector(idx))
                idx = rng.RandomIndex(items);

            return idx;
        }

        /// <summary>
        /// selects random indices from the given (non-empty) span, until either one is found for which
        /// <paramref name="selector"/> returns true, or the maximum number of attempts is reached.  An exception will
        /// be thrown if the function is unable to generate a valid index within the number of attempts specified.
        /// </summary>
        /// <exception cref="ArgumentException">An empty span was given.</exception>
        /// <exception cref="MaxAttemptsReachedException">The function could not find a valid index within the maximum number of tries.</exception>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the span.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid index to select.</param>
        /// <param name="maxTries">Maximum number of times to try generating a valid value before giving up and throwing an exception.</param>
        /// <returns>A randomly-chosen index from the span for which <paramref name="selector"/> returns true.</returns>
        public static int RandomIndex<T>(this IEnhancedRandom rng, ReadOnlySpan<T> items, Func<int, bool> selector, int maxTries)
        {
            if (maxTries <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxTries),
                    "Value must be > 0; for infinite retries, use the overload without a maxTries parameter.");

            int curTries = 0;
            while (curTries < maxTries)
            {
                int idx = rng.RandomIndex(items);
                if (selector(idx))
                    return idx;

                curTries++;
            }

            throw new MaxAttemptsReachedException();
        }

        /// <summary>
        /// Gets a randomly-chosen value that is a valid index for an item from the given non-null non-empty IReadOnlyList.
        /// </summary>
        /// <remarks>
        /// Note that this function can take arrays as well as the <paramref name="items"/> value, since arrays implement
        /// IReadOnlyList.
        /// </remarks>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="items">Must be non-null and non-empty.</param>
        /// <returns>A randomly-chosen value that is a valid index in the list.</returns>
        public static int RandomIndex<T>(this IEnhancedRandom rng, IReadOnlyList<T> items)
            => rng.NextInt(items.Count);

        /// <summary>
        /// Continuously selects random indices from the given (non-empty) IReadOnlyList, until one is found for which the
        /// given <paramref name="selector"/> returns true.
        /// </summary>
        /// <remarks>
        /// This function will never return if there is no object in the list meeting the criteria of the selector
        /// function, and may take an extremely long time if the list is very big and there are very few items in the
        /// list meeting the selector's criteria.  For more deterministic termination, consider using the overload
        /// of this function taking a maximum number of attempts:
        /// <see cref="RandomIndex{T}(ShaiRandom.Generators.IEnhancedRandom, IReadOnlyList{T}, Func{int, bool}, int)"/>
        /// </remarks>
        /// <exception cref="ArgumentException">An empty list was given.</exception>
        /// <param name="rng" />
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid selection of index.</param>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <returns>A randomly-chosen value that is a valid index in the list, for which <paramref name="selector"/> returns true.</returns>
        public static int RandomIndex<T>(this IEnhancedRandom rng, IReadOnlyList<T> items, Func<int, bool> selector)
        {
            int idx = rng.RandomIndex(items);
            while (!selector(idx))
                idx = rng.RandomIndex(items);

            return idx;
        }

        /// <summary>
        /// Continuously selects random indices from the given (non-empty) IReadOnlyList, until either one is found for which
        /// <paramref name="selector"/> returns true, or the maximum number of attempts is reached.  An exception will
        /// be thrown if the function is unable to generate a valid index within the number of attempts specified.
        /// </summary>
        /// <exception cref="ArgumentException">An empty list was given.</exception>
        /// <exception cref="MaxAttemptsReachedException">The function could not find a valid index within the maximum number of tries.</exception>
        /// <param name="rng" />
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="items">Must be non-empty.</param>
        /// <param name="selector">Function that should return true _only_ for a valid index to select.</param>
        /// <param name="maxTries">Maximum number of times to try generating a valid value before giving up and throwing an exception.</param>
        /// <returns>A randomly-chosen index from the list for which <paramref name="selector"/> returns true.</returns>
        public static int RandomIndex<T>(this IEnhancedRandom rng, IReadOnlyList<T> items, Func<int, bool> selector, int maxTries)
        {
            if (maxTries <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxTries),
                    "Value must be > 0; for infinite retries, use the overload without a maxTries parameter.");

            int curTries = 0;
            while (curTries < maxTries)
            {
                int idx = rng.RandomIndex(items);
                if (selector(idx))
                    return idx;

                curTries++;
            }

            throw new MaxAttemptsReachedException();
        }

        /// <summary>
        /// Shuffles the given Span in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <remarks>
        /// Note that this function can easily accept an array as well, or anything else that can convert to span either
        /// via either implicit or explicit conversion.  It can also shuffle only part of any such array or structure
        /// (see examples).
        /// <example>
        /// <code>
        /// // Shuffle whole array (either works)
        /// myRng.Shuffle&lt;TypeOfElementsInMyArray&gt;(myArray);
        /// myRng.Shuffle(myArray.AsSpan());
        /// </code>
        /// </example>
        ///
        /// <example>
        /// <code>
        /// // Shuffle the three elements starting at index 1
        /// myRng.Shuffle(myArray.AsSpan(1, 3));
        /// </code>
        /// </example>
        /// <example>
        /// <code>
        /// // Shuffle all elements from index 1 to (but not including) the last element
        /// myRng.Shuffle(myArray.AsSpan(1..^1));
        /// </code>
        /// </example>
        /// </remarks>
        /// <typeparam name="T">Type of elements in the span</typeparam>
        /// <param name="rng" />
        /// <param name="items">A span of some type; may contain null items</param>
        public static void Shuffle<T>(this IEnhancedRandom rng, Span<T> items)
        {
            for (int i = items.Length - 1; i > 0; i--)
            {
                int ii = rng.NextInt(i + 1);
                (items[i], items[ii]) = (items[ii], items[i]);
            }
        }

        /// <summary>
        /// Shuffles the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        public static void Shuffle<T>(this IEnhancedRandom rng, IList<T> items)
            => ShuffleUnchecked(rng, items, 0, items.Count);

        /// <summary>
        /// Shuffles a section of the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// Only items from the given index onward will be shuffled.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <param name="startIndex">Index of the first element in the list to shuffle</param>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        public static void Shuffle<T>(this IEnhancedRandom rng, IList<T> items, Index startIndex)
        {
            int length = items.Count;
            int start = startIndex.GetOffset(items.Count);
            length -= start;

            rng.Shuffle(items, start, length);
        }

        /// <summary>
        /// Shuffles a section of the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// Only items within the given range will be shuffled.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <param name="range">Range of items in the list to shuffle</param>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        public static void Shuffle<T>(this IEnhancedRandom rng, IList<T> items, Range range)
        {
            var (offset, length) = range.GetOffsetAndLength(items.Count);
            ShuffleUnchecked(rng, items, offset, length);
        }

        /// <summary>
        /// Shuffles a section of the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <param name="start">the index of the first element of the IList that can be shuffled</param>
        public static void Shuffle<T>(this IEnhancedRandom rng, IList<T> items, int start)
            => rng.Shuffle(items, start, items.Count - start);

        /// <summary>
        /// Shuffles a section of the given IList in-place pseudo-randomly, using the Fisher-Yates (also called Knuth) shuffle algorithm.
        /// </summary>
        /// <param name="rng" />
        /// <param name="items">an IList; must be non-null but may contain null items</param>
        /// <param name="start">the index of the first element of the IList that can be shuffled</param>
        /// <param name="length">the length of the section to shuffle</param>
        public static void Shuffle<T>(this IEnhancedRandom rng, IList<T> items, int start, int length)
        {
            if (start < 0 || start >= items.Count)
                throw new ArgumentOutOfRangeException(nameof(start), "Index given was out of range");

            if (length < 0 || length > items.Count - start)
                throw new ArgumentOutOfRangeException(nameof(length), "Index given was out of range");

            ShuffleUnchecked(rng, items, start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ShuffleUnchecked<T>(IEnhancedRandom rng, IList<T> items, int start, int length)
        {
            for (int i = start + length - 1; i > start; i--)
            {
                int ii = rng.NextInt(start, i + 1);
                (items[i], items[ii]) = (items[ii], items[i]);
            }
        }

        /// <summary>
        /// Implements an infinite "gap shuffle"; a shuffle which takes a fixed-size set of items and produce an
        /// infinite shuffled stream of them such that an element is never chosen in quick succession.
        /// </summary>
        /// <remarks>
        /// The "gap shuffle" algorithm the returned enumerator implements, is akin to an infinite-length IEnumerable
        /// that shuffles a sequence, iterates over the shuffled elements as they are requested, and when it runs out of
        /// elements it shuffles the sequence again. The Gap in the name refers to how it prevents the most-recently
        /// returned item in the sequence from being returned again immediately after the items are shuffled.
        ///
        /// One use case for the gap-shuffle algorithm is for text generation, where using the same word in rapid
        /// succession makes the writing look less "educated." It can also be useful for color selection; in pixel art,
        /// using the same colors for skin and for long hair will make a human look like they have tentacles on their
        /// head, so you'd almost always want different colors for those two.
        ///
        /// The returned enumerator is infinite (but lazily evaluated), so you will need to ensure you do not attempt to
        /// iterate it until its completion.  You can simply call the <see cref="GapShufflerEnumerator{TItem}.Next"/>
        /// function a fixed number of times, or if you need to use it in a loop, you will need to either ensure you
        /// call "break", or use LINQ's .Take(n) function, in order to avoid an infinite loop.
        ///
        /// The elements in the IEnumerable specified are copied internally into an array; if you need to avoid a copy,
        /// you can instead use <see cref="InPlaceGapShuffler{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.Collections.Generic.IList{TItem})"/>
        /// or one of its overloads.
        ///
        /// Many use cases may simply involve calling the Next function when you need to advance; but the returned struct
        /// is also a custom IEnumerator, so you can use it directly in a foreach loop (and it is faster than a typical
        /// IEnumerable when used this way).  If you need an IEnumerable to use with LINQ or other code, the returned struct
        /// also implements IEnumerable; however note that iterating over it this way will not perform as well as
        /// iterating directly over this object.
        /// </remarks>
        /// <param name="rng">RNG to use for shuffling.</param>
        /// <param name="items">Items to shuffle, which are copied into an array internally.</param>
        /// <returns>An infinite stream of (mostly) random shuffles of the given items, one item at a time.</returns>
        public static GapShufflerEnumerator<TItem> GapShuffler<TItem>(this IEnhancedRandom rng, IEnumerable<TItem> items)
            => new GapShufflerEnumerator<TItem>(rng, items);

        /// <summary>
        /// Implements an infinite "gap shuffle"; a shuffle which takes a fixed-size set of items and produce an
        /// infinite shuffled stream of them such that an element is never chosen in quick succession.
        /// </summary>
        /// <remarks>
        /// The "gap shuffle" algorithm the returned enumerator implements, is akin to an infinite-length IEnumerable
        /// that shuffles a sequence, iterates over the shuffled elements as they are requested, and when it runs out of
        /// elements it shuffles the sequence again. The Gap in the name refers to how it prevents the most-recently
        /// returned item in the sequence from being returned again immediately after the items are shuffled.
        ///
        /// One use case for the gap-shuffle algorithm is for text generation, where using the same word in rapid
        /// succession makes the writing look less "educated." It can also be useful for color selection; in pixel art,
        /// using the same colors for skin and for long hair will make a human look like they have tentacles on their
        /// head, so you'd almost always want different colors for those two.
        ///
        /// The returned enumerator is infinite (but lazily evaluated), so you will need to ensure you do not attempt to
        /// iterate it until its completion.  You can simply call the <see cref="GapShufflerEnumerator{TItem}.Next"/>
        /// function a fixed number of times, or if you need to use it in a loop, you will need to either ensure you
        /// call "break", or use LINQ's .Take(n) function, in order to avoid an infinite loop.
        ///
        /// The elements in the Span specified are copied internally into an array; if you need to avoid a copy,
        /// you can instead use <see cref="InPlaceGapShuffler{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.Collections.Generic.IList{TItem})"/>
        /// or one of its overloads; however you'll need a Memory&lt;T&gt; object or some other non-ref struct.
        ///
        /// Many use cases may simply involve calling the Next function when you need to advance; but the returned struct
        /// is also a custom IEnumerator, so you can use it directly in a foreach loop (and it is faster than a typical
        /// IEnumerable when used this way).  If you need an IEnumerable to use with LINQ or other code, the returned struct
        /// also implements IEnumerable; however note that iterating over it this way will not perform as well as
        /// iterating directly over this object.
        /// </remarks>
        /// <param name="rng">RNG to use for shuffling.</param>
        /// <param name="items">Items to shuffle, which are copied into an array internally.</param>
        /// <returns>An infinite stream of (mostly) random shuffles of the given items, one item at a time.</returns>
        public static GapShufflerEnumerator<TItem> GapShuffler<TItem>(this IEnhancedRandom rng, ReadOnlySpan<TItem> items)
            => new GapShufflerEnumerator<TItem>(rng, items);

        /// <summary>
        /// Implements an infinite "gap shuffle"; a shuffle which takes a fixed-size set of items and produce an
        /// infinite shuffled stream of them such that an element is never chosen in quick succession.
        /// </summary>
        /// <remarks>
        /// The "gap shuffle" algorithm the returned enumerator implements, is akin to an infinite-length IEnumerable
        /// that shuffles a sequence, iterates over the shuffled elements as they are requested, and when it runs out of
        /// elements it shuffles the sequence again. The Gap in the name refers to how it prevents the most-recently
        /// returned item in the sequence from being returned again immediately after the items are shuffled.
        ///
        /// One use case for the gap-shuffle algorithm is for text generation, where using the same word in rapid
        /// succession makes the writing look less "educated." It can also be useful for color selection; in pixel art,
        /// using the same colors for skin and for long hair will make a human look like they have tentacles on their
        /// head, so you'd almost always want different colors for those two.
        ///
        /// The returned enumerator is infinite (but lazily evaluated), so you will need to ensure you do not attempt to
        /// iterate it until its completion.  You can simply call the <see cref="GapShufflerEnumerator{TItem}.Next"/>
        /// function a fixed number of times, or if you need to use it in a loop, you will need to either ensure you
        /// call "break", or use LINQ's .Take(n) function, in order to avoid an infinite loop.
        ///
        /// The elements in the Memory object specified are copied internally into an array; if you need to avoid a copy,
        /// you can instead use <see cref="InPlaceGapShuffler{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.Memory{TItem})"/>
        /// or one of its overloads instead.
        ///
        /// Many use cases may simply involve calling the Next function when you need to advance; but the returned struct
        /// is also a custom IEnumerator, so you can use it directly in a foreach loop (and it is faster than a typical
        /// IEnumerable when used this way).  If you need an IEnumerable to use with LINQ or other code, the returned struct
        /// also implements IEnumerable; however note that iterating over it this way will not perform as well as
        /// iterating directly over this object.
        /// </remarks>
        /// <param name="rng">RNG to use for shuffling.</param>
        /// <param name="items">Items to shuffle, which are copied into an array internally.</param>
        /// <returns>An infinite stream of (mostly) random shuffles of the given items, one item at a time.</returns>
        public static GapShufflerEnumerator<TItem> GapShuffler<TItem>(this IEnhancedRandom rng, ReadOnlyMemory<TItem> items)
            => new GapShufflerEnumerator<TItem>(rng, items.Span);

        /// <summary>
        /// Implements an infinite "gap shuffle"; a shuffle which takes a fixed-size set of items and produce an
        /// infinite shuffled stream of them such that an element is never chosen in quick succession.
        /// </summary>
        /// <remarks>
        /// The "gap shuffle" algorithm the returned enumerator implements, is akin to an infinite-length IEnumerable
        /// that shuffles a sequence, iterates over the shuffled elements as they are requested, and when it runs out of
        /// elements it shuffles the sequence again. The Gap in the name refers to how it prevents the most-recently
        /// returned item in the sequence from being returned again immediately after the items are shuffled.
        ///
        /// One use case for the gap-shuffle algorithm is for text generation, where using the same word in rapid
        /// succession makes the writing look less "educated." It can also be useful for color selection; in pixel art,
        /// using the same colors for skin and for long hair will make a human look like they have tentacles on their
        /// head, so you'd almost always want different colors for those two.
        ///
        /// The returned enumerator is infinite (but lazily evaluated), so you will need to ensure you do not attempt to
        /// iterate it until its completion.  You can simply call the <see cref="GapShufflerEnumerator{TItem}.Next"/>
        /// function a fixed number of times, or if you need to use it in a loop, you will need to either ensure you
        /// call "break", or use LINQ's .Take(n) function, in order to avoid an infinite loop.
        ///
        /// The given list is stored directly in the enumerator, so elements will be shuffled in-place as needed as the
        /// enumerator is advanced.  Therefore, you should not make any changes to the list while the enumerator is active.
        /// If you want to instead make a copy, you can use one of the overloads of
        /// <see cref="GapShuffler{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.Collections.Generic.IEnumerable{TItem})"/>
        /// instead.
        ///
        /// Many use cases may simply involve calling the Next function when you need to advance; but the returned struct
        /// is also a custom IEnumerator, so you can use it directly in a foreach loop (and it is faster than a typical
        /// IEnumerable when used this way).  If you need an IEnumerable to use with LINQ or other code, the returned struct
        /// also implements IEnumerable; however note that iterating over it this way will not perform as well as
        /// iterating directly over this object.
        /// </remarks>
        /// <param name="rng">RNG to use for shuffling.</param>
        /// <param name="items">List of items to shuffle, which will be repeatedly shuffled in-place as the iterator advances.</param>
        /// <returns>An infinite stream of (mostly) random shuffles of the given items, one item at a time.</returns>
        public static GapShufflerEnumerator<TItem> InPlaceGapShuffler<TItem>(this IEnhancedRandom rng, IList<TItem> items)
            => new GapShufflerEnumerator<TItem>(rng, ref items);

        /// <summary>
        /// Implements an infinite "gap shuffle"; a shuffle which takes a fixed-size set of items and produce an
        /// infinite shuffled stream of them such that an element is never chosen in quick succession.
        /// </summary>
        /// <remarks>
        /// The "gap shuffle" algorithm the returned enumerator implements, is akin to an infinite-length IEnumerable
        /// that shuffles a sequence, iterates over the shuffled elements as they are requested, and when it runs out of
        /// elements it shuffles the sequence again. The Gap in the name refers to how it prevents the most-recently
        /// returned item in the sequence from being returned again immediately after the items are shuffled.
        ///
        /// One use case for the gap-shuffle algorithm is for text generation, where using the same word in rapid
        /// succession makes the writing look less "educated." It can also be useful for color selection; in pixel art,
        /// using the same colors for skin and for long hair will make a human look like they have tentacles on their
        /// head, so you'd almost always want different colors for those two.
        ///
        /// The returned enumerator is infinite (but lazily evaluated), so you will need to ensure you do not attempt to
        /// iterate it until its completion.  You can simply call the <see cref="GapShufflerInPlaceMemoryEnumerator{TItem}.Next"/>
        /// function a fixed number of times, or if you need to use it in a loop, you will need to either ensure you
        /// call "break", or use LINQ's .Take(n) function, in order to avoid an infinite loop.
        ///
        /// The given Memory object is stored directly in the enumerator, so elements will be shuffled in-place as needed as the
        /// enumerator is advanced.  Therefore, you should not make any changes to the memory while the enumerator is active.
        /// If you want to instead make a copy, you can use
        /// <see cref="GapShuffler{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.ReadOnlyMemory{TItem})"/> or one of
        /// its overloads instead.
        ///
        /// Many use cases may simply involve calling the Next function when you need to advance; but the returned struct
        /// is also a custom IEnumerator, so you can use it directly in a foreach loop (and it is faster than a typical
        /// IEnumerable when used this way).  If you need an IEnumerable to use with LINQ or other code, the returned struct
        /// also implements IEnumerable; however note that iterating over it this way will not perform as well as
        /// iterating directly over this object.
        /// </remarks>
        /// <param name="rng">RNG to use for shuffling.</param>
        /// <param name="items">List of items to shuffle, which will be repeatedly shuffled in-place as the iterator advances.</param>
        /// <returns>An infinite stream of (mostly) random shuffles of the given items, one item at a time.</returns>
        public static GapShufflerInPlaceMemoryEnumerator<TItem> InPlaceGapShuffler<TItem>(this IEnhancedRandom rng, Memory<TItem> items)
            => new GapShufflerInPlaceMemoryEnumerator<TItem>(rng, items);

        /// <summary>
        /// Gets a normally-distributed (Gaussian) double, with the specified mean (default 0.0) and standard deviation (default 1.0).
        /// If the standard deviation is 1.0 and the mean is 0.0, then this can produce results between -26.48372928592822 and 26.48372928592822 (both extremely rarely).
        /// </summary>
        /// <param name="rng" />
        /// <param name="mean">Mean for normal distribution.</param>
        /// <param name="stdDev">Standard deviation for normal distribution.</param>
        /// <returns>A double from the normal distribution with the specified mean (default 0.0) and standard deviation (default 1.0).</returns>
        public static double NextNormal(this IEnhancedRandom rng, double mean = 0.0, double stdDev = 1.0)
        {
            return MathUtils.Probit(rng.NextInclusiveDouble()) * stdDev + mean;
        }

        /// <summary>
        /// Returns true if a random value between 0 and 1 is less than the specified value.
        /// </summary>
        /// <param name="rng" />
        /// <param name="chance">a float between 0.0 and 1.0; higher values are more likely to result in true</param>
        /// <returns>a bool selected with the given chance of being true</returns>
        public static bool NextBool(this IEnhancedRandom rng, float chance)
        {
            return rng.NextFloat() < chance;
        }

        /// <summary>
        /// Returns -1 or 1, randomly.
        /// </summary>
        /// <param name="rng" />
        /// <returns>-1 or 1, selected with approximately equal likelihood</returns>
        public static int NextSign(this IEnhancedRandom rng)
        {
            return 1 | rng.NextInt() >> 31;
        }

        /// <summary>
        /// Returns a triangularly distributed random number between -1.0 (exclusive) and 1.0 (exclusive), where values around zero are
        /// more likely. Advances the state twice.
        /// </summary>
        /// <remarks>
        /// This can be an optimized version of <see cref="NextTriangular(IEnhancedRandom, float, float, float)"/>, or: <code> NextTriangular(-1, 1, 0)</code>
        /// </remarks>
        /// <param name="rng" />
        public static float NextTriangular(this IEnhancedRandom rng)
        {
            return rng.NextFloat() - rng.NextFloat();
        }

        /// <summary>
        /// Returns a triangularly distributed random number between <code>-max</code> (exclusive) and <code>max</code> (exclusive), where values
        /// around zero are more likely. Advances the state twice.
        /// </summary>
        /// <remarks>
        /// This is an optimized version of <see cref="NextTriangular(IEnhancedRandom, float, float, float)"/>, or: <code> NextTriangular(-max, max, 0)</code>
        /// </remarks>
        /// <param name="rng" />
        /// <param name="max">the outer exclusive limit</param>
        public static float NextTriangular(this IEnhancedRandom rng, float max)
        {
            return (rng.NextFloat() - rng.NextFloat()) * max;
        }

        /// <summary>
        /// Returns a triangularly distributed random number between min (inclusive) and max (exclusive), where the
        /// mode argument defaults to the midpoint between the bounds, giving a symmetric distribution. Advances the state once.
        /// </summary>
        /// <remarks>
        /// This is an optimized version of <see cref="NextTriangular(IEnhancedRandom, float, float, float)"/>, or: <code> NextTriangular(min, max, (min + max) * 0.5f)</code>
        /// </remarks>
        /// <param name="rng" />
        /// <param name="min">the lower limit</param>
        /// <param name="max">the upper limit</param>
        public static float NextTriangular(this IEnhancedRandom rng, float min, float max)
        {
            return rng.NextTriangular(min, max, (min + max) * 0.5f);
        }

        /// <summary>
        /// Returns a triangularly distributed random number between min (inclusive) and max (exclusive), where values
        /// around mode are more likely. Advances the state once.
        /// </summary>
        /// <param name="rng" />
        /// <param name="min"> the lower limit</param>
        /// <param name="max"> the upper limit</param>
        /// <param name="mode">the point around which the values are more likely</param>
        public static float NextTriangular(this IEnhancedRandom rng, float min, float max, float mode)
        {
            float u = rng.NextFloat();
            float d = max - min;
            if (u <= (mode - min) / d) { return min + MathF.Sqrt(u * d * (mode - min)); }
            return max - MathF.Sqrt((1 - u) * d * (max - mode));
        }

        /// <summary>
        /// Sets each state in this IEnhancedRandom to the corresponding state in the other IEnhancedRandom.
        /// This generally only works correctly if both objects have the same class.
        /// </summary>
        /// <param name="rng">Any IEnhancedRandom that supports <see cref="IEnhancedRandom.SetSelectedState(int, ulong)"/>.</param>
        /// <param name="other">Another IEnhancedRandom that almost always should have the same class as this one.</param>
        public static void SetWith(this IEnhancedRandom rng, IEnhancedRandom other)
        {
            int myCount = rng.StateCount, otherCount = other.StateCount;
            int i = 0;
            for (; i < myCount && i < otherCount; i++)
            {
                rng.SetSelectedState(i, other.SelectState(i));
            }
            for (; i < myCount; i++)
            {
                rng.SetSelectedState(i, 0xFFFFFFFFFFFFFFFFUL);
            }
        }

        /// <summary>
        /// Given two IEnhancedRandom objects that could have the same or different classes,
        /// this returns true if they have the same class and same state, or false otherwise.
        /// </summary>
        /// <remarks>
        /// Both of the arguments should implement <see cref="IEnhancedRandom.SelectState(int)"/>, or this
        /// will throw an exception. This can be useful for comparing IEnhancedRandom classes.
        /// </remarks>
        /// <returns>true if the two objects have the same class and state, or false otherwise</returns>
        public static bool Matches(this IEnhancedRandom left, IEnhancedRandom right)
        {
            if (left == right)
                return true;
            if (left.GetType() != right.GetType())
                return false;

            int count = left.StateCount;
            for (int i = 0; i < count; i++)
            {
                if (left.SelectState(i) != right.SelectState(i))
                    return false;
            }
            return true;
        }
    }
}
