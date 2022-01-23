using System;

namespace ShaiRandom
{
    /// <summary>
    /// Some helper methods that can be useful when parsing span data.
    /// </summary>
    public static class SpanHelpers
    {
        /// <summary>
        /// IndexOf function which takes a starting point (like the similar overload for string).
        /// </summary>
        /// <param name="span"/>
        /// <param name="value">Value to search for.</param>
        /// <param name="start">Index at which to start searching.</param>
        /// <typeparam name="T">Type of elements in the span.</typeparam>
        /// <returns>
        /// Returns the index of first occurence of <paramref name="value"/> in <paramref name="span"/> which occurs on
        /// or after <paramref name="start"/>; or -1 if no such occurence exists.
        /// </returns>
        public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, int start)
            where T : IEquatable<T>
        {
            for (int i = start; i < span.Length; i++)
                if (span[i].Equals(value))
                    return i;

            return -1;
        }
    }
}
