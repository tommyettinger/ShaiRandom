using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ShaiRandom.Generators;
using ShaiRandom.Wrappers;

namespace ShaiRandom
{
    /// <summary>
    /// Static class containing deserialization logic to take a string produced by <see cref="IEnhancedRandom.StringDeserialize"/>
    /// and produce an appropriate generator.
    /// </summary>
    /// <remarks>
    /// If you have custom <see cref="IEnhancedRandom"/> implementations, provided they implement <see cref="IEnhancedRandom.StringSerialize"/>,
    /// <see cref="IEnhancedRandom.StringDeserialize"/>, and <see cref="IEnhancedRandom.Copy"/>, you may register them to function with this
    /// serializer by calling <see cref="RegisterTag"/>.
    ///
    /// This only needs to be done once per type, and is typically performed inside a static class constructor for your RNG type.  The generator
    /// implementations in ShaiRandom do this same thing; feel free to look at any of them for an example.
    /// </remarks>
    public static class Serializer
    {
        private static readonly Dictionary<string, IEnhancedRandom> s_tags = new Dictionary<string, IEnhancedRandom>();

        /// <summary>
        /// List of tags registered to the serializer.  Register a type to its tag with <see cref="RegisterTag"/>.
        /// </summary>
        public static IReadOnlyDictionary<string, IEnhancedRandom> Tags => s_tags;

        /// <summary>
        /// Registers an instance of an IEnhancedRandom implementation by its string <see cref="IEnhancedRandom.Tag"/>.
        /// </summary>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a four-character <see cref="IEnhancedRandom.Tag"/>.</param>
        /// <returns>Returns true if the tag was successfully registered for the first time, or false if the tags are unchanged.</returns>
        public static bool RegisterTag(IEnhancedRandom instance)
        {
            if (s_tags.ContainsKey(instance.Tag)) return false;
            if (instance.Tag.Length != 0)
            {
                s_tags.Add(instance.Tag, instance);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Produces a string representing the serialized form of the given RNG, by calling its <see cref="IEnhancedRandom.StringSerialize"/>
        /// method.
        /// </summary>
        /// <param name="rng">RNG to serialize.</param>
        /// <returns>A string representing the serialized form of the given RNG.</returns>
        public static string Serialize(IEnhancedRandom rng) => rng.StringSerialize();

        /// <summary>
        /// Given data from a string produced by <see cref="IEnhancedRandom.StringSerialize"/> on any registered IEnhancedRandom,
        /// this returns a new IEnhancedRandom with the same implementation and state it had when it was serialized.
        /// </summary>
        /// <remarks>
        /// This handles all IEnhancedRandom implementations in this library, including <see cref="TRGeneratorWrapper"/>,
        /// <see cref="ReversingWrapper"/>, and <see cref="ArchivalWrapper"/>.  It will also work with arbitrary custom
        /// IEnhancedRandom implementations, provided they implement StringSerialize, StringDeserialize, and Copy, and register
        /// their tag via <see cref="RegisterTag"/>.
        ///
        /// This function takes as input a ReadOnlySpan of char, which allows data to be any string or some more specialized types.
        /// </remarks>
        /// <param name="data">A string or ReadOnlySpan of char produced by an IEnhancedRandom's StringSerialize() method.</param>
        /// <returns>A newly-allocated IEnhancedRandom matching the implementation and state of the serialized IEnhancedRandom.</returns>
        public static IEnhancedRandom Deserialize(ReadOnlySpan<char> data)
        {
            int idx = data.IndexOf('`');
            if (idx == -1)
                throw new ArgumentException("String given cannot represent a valid generator.");

            // Can't use Span as the key in a dictionary, so we have to allocate a string to perform the lookup.
            // When the feature linked here is implemented, we could get around this:
            // https://github.com/dotnet/runtime/issues/27229
            string tagData = new string(data[..idx]);
            return s_tags[tagData].Copy().StringDeserialize(data[idx..]);
        }
    }
}
