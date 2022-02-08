using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ShaiRandom.Generators;
using ShaiRandom.Wrappers;

namespace ShaiRandom
{
    /// <summary>
    /// Static class containing logic to take a string produced by <see cref="IEnhancedRandom.StringDeserialize"/>
    /// and produce an appropriate generator.
    /// </summary>
    /// <remarks>
    /// Before serializing/deserializing, you must register "tags" with the Serializer, which will be used to identify
    /// the type as it is deserialized.  No tags are registered by default.
    ///
    /// Each IEnhancedRandom implementation has a <see cref="IEnhancedRandom.DefaultTag"/> field, which constitutes the default
    /// tag that will be used for the generator type, if one is not specified when the registration is performed.  Therefore,
    /// if the default tag works for your use case, registration requires nothing more than calling <see cref="RegisterTag(IEnhancedRandom)"/>.
    /// If you need to specify a different tag, there is an overload available which takes a tag.  Other functions are also provided
    /// which fit various use cases, including overwriting registered tags, removing tags, etc.
    ///
    /// You may also just call the <see cref="RegisterShaiRandomDefaultTags()"/> function, to register the default tags for every generator
    /// implementation in ShaiRandom.
    ///
    /// If you have custom <see cref="IEnhancedRandom"/> implementations, provided they implement <see cref="IEnhancedRandom.StringSerialize"/>,
    /// <see cref="IEnhancedRandom.StringDeserialize"/>, and <see cref="IEnhancedRandom.Copy"/>, you may register them to function with this
    /// serializer by calling RegisterTag or any of the other tag registration functions.
    /// </remarks>
    public static class Serializer
    {
        private static readonly Dictionary<string, IEnhancedRandom> s_tagsToGenerators = new Dictionary<string, IEnhancedRandom>();
        private static readonly Dictionary<Type, string> s_typesToTags = new Dictionary<Type, string>();

        #region Tag Registration/Management
        /// <summary>
        /// Registers an instance of an IEnhancedRandom implementation by its string <see cref="IEnhancedRandom.DefaultTag"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The type of generator given already had a tag, the generator's DefaultTag was already registered to a generator, or the generator's
        /// DefaultTag contained invalid characters.
        /// </exception>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a valid value for <see cref="IEnhancedRandom.DefaultTag"/>.</param>
        public static void RegisterTag(IEnhancedRandom instance) => RegisterTag(instance.DefaultTag, instance);

        /// <summary>
        /// Registers an instance of an IEnhancedRandom implementation to the tag specified.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The type of generator given already had a tag, the given tag was already registered to a generator, or the generator's
        /// DefaultTag contained invalid characters.
        /// </exception>
        /// <param name="tag">The tag to register to the given instance's type.</param>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a valid value for <see cref="IEnhancedRandom.DefaultTag"/>.</param>
        public static void RegisterTag(string tag, IEnhancedRandom instance)
        {
            var type = instance.GetType();
            if (s_typesToTags.ContainsKey(type))
                throw new ArgumentException(
                    $"Tried to register a generator of type {type.Name} with tag {tag}, but that generator type was already associated with a tag: {s_typesToTags[type]}",
                    nameof(instance));
            if (s_tagsToGenerators.ContainsKey(tag))
                throw new ArgumentException(
                    $"Tried to register a generator of type {type.Name} with tag {tag}, but that tag was already associated with a different generator type: {s_tagsToGenerators[tag].GetType().Name}",
                    nameof(tag));
            if (tag.Contains('`'))
                throw new ArgumentException(
                    $"Tried to register a generator of type {type.Name} with tag {tag}, but tags cannot not contain the '`' character.");

            s_tagsToGenerators.Add(tag, instance);
            s_typesToTags.Add(type, tag);
        }

        /// <summary>
        /// Tries to register an instance of an IEnhancedRandom implementation by its string <see cref="IEnhancedRandom.DefaultTag"/>.
        /// </summary>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a valid value for <see cref="IEnhancedRandom.DefaultTag"/>.</param>
        /// <returns>Returns true if the tag was successfully registered for the first time, or false if the tags are unchanged.</returns>
        public static bool TryRegisterTag(IEnhancedRandom instance) => TryRegisterTag(instance.DefaultTag, instance);

        /// <summary>
        /// Tries to register an instance of an IEnhancedRandom implementation to the tag specified.
        /// </summary>
        /// <param name="tag">The tag to register for generators of the given instance's type.</param>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null.</param>
        /// <returns>Returns true if the tag was successfully registered for the first time, or false if the tags are unchanged.</returns>
        public static bool TryRegisterTag(string tag, IEnhancedRandom instance)
        {
            var type = instance.GetType();
            if (s_tagsToGenerators.ContainsKey(tag) || s_typesToTags.ContainsKey(type) || tag.Contains('`')) return false;
            s_tagsToGenerators.Add(tag, instance);
            s_typesToTags.Add(type, tag);
            return true;
        }

        /// <summary>
        /// Registers an instance of an IEnhancedRandom implementation by its string <see cref="IEnhancedRandom.DefaultTag"/>,
        /// overwriting any conflicting tag or type registrations.
        /// </summary>
        /// <remarks>
        /// This function will overwrite _both_ conflicting tags AND conflicting types; eg. if the instance's DefaultTag
        /// is registered to a different type, that type's registration will be replaced, AND if the type of the instance
        /// given is already registered to a different tag, that registration will be replaced with this one.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The generator's DefaultTag contained invalid characters.
        /// </exception>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a valid value for <see cref="IEnhancedRandom.DefaultTag"/>.</param>
        public static void ForceRegisterTag(IEnhancedRandom instance)
            => ForceRegisterTag(instance.DefaultTag, instance);

        /// <summary>
        /// Registers an instance of an IEnhancedRandom implementation to the tag specified,
        /// overwriting any conflicting tag or type registrations.
        /// </summary>
        /// <remarks>
        /// This function will overwrite _both_ conflicting tags AND conflicting types; eg. if the given tag is registered
        /// to a different type, that type's registration will be replaced, AND if the type of the instance given is already
        /// registered to a different tag, that registration will be replaced with this one.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The given tag contained invalid characters.
        /// </exception>
        /// <param name="tag">The tag to register for generators of the given instance's type.</param>
        /// <param name="instance">An instance of an IEnhancedRandom implementation, which will be copied as needed; its state does not matter,
        /// as long as it is non-null and has a valid value for <see cref="IEnhancedRandom.DefaultTag"/>.</param>
        public static void ForceRegisterTag(string tag, IEnhancedRandom instance)
        {
            UnregisterTag(tag);
            UnregisterTag(instance.GetType());
            RegisterTag(tag, instance);
        }

        /// <summary>
        /// Unregisters any generator type that is registered to the given tag.
        /// </summary>
        /// <param name="tag">The tag to unregister</param>
        /// <returns>True if the tag was found and unregistered; false if the tag given isn't found.</returns>
        public static bool UnregisterTag(string tag)
        {
            if (!s_tagsToGenerators.TryGetValue(tag, out var instance))
                return false;

            s_typesToTags.Remove(instance.GetType());
            s_tagsToGenerators.Remove(tag);

            return true;
        }

        /// <summary>
        /// Unregisters the given type's tag, whatever that tag may be.
        /// </summary>
        /// <param name="generatorType">The type to unregister.</param>
        /// <returns>True if the type was found and its tag was unregistered; false if the generator type given was never registered.</returns>
        public static bool UnregisterTag(Type generatorType)
        {
            if (!s_typesToTags.TryGetValue(generatorType, out var tag))
                return false;

            s_tagsToGenerators.Remove(tag);
            s_typesToTags.Remove(generatorType);

            return true;
        }

        /// <summary>
        /// Unregisters the given type's tag, whatever that tag may be.
        /// </summary>
        /// <typeparam name="T">Type of the generator to unregister.</typeparam>
        /// <returns>True if the type was found and its tag was unregistered; false if the generator type given was never registered.</returns>
        public static bool UnregisterTag<T>() where T : IEnhancedRandom
            => UnregisterTag(typeof(T));

        /// <summary>
        /// Gets the tag registered for the given type.
        /// </summary>
        /// <exception cref="KeyNotFoundException">No tag was registered for the given type.</exception>
        /// <param name="generatorType">Type to retrieve the registered tag for.</param>
        /// <returns>The tag registered for the given type</returns>
        public static string GetTag(Type generatorType) => s_typesToTags[generatorType];

        /// <summary>
        /// Gets the tag registered for the given type.
        /// </summary>
        /// <exception cref="KeyNotFoundException">No tag was registered for the given type.</exception>
        /// <typeparam name="T">Type to retrieve the registered tag for.</typeparam>
        /// <returns>The tag registered for the given type</returns>
        public static string GetTag<T>() where T : IEnhancedRandom
            => GetTag(typeof(T));

        /// <summary>
        /// Gets the tag registered for the given instance's type.
        /// </summary>
        /// <exception cref="KeyNotFoundException">No tag was registered for the type of the given instance.</exception>
        /// <param name="instance">Instance to retrieve the tag for.</param>
        /// <returns>The tag registered for the runtime type of the given instance.</returns>
        public static string GetTag(IEnhancedRandom instance) => GetTag(instance.GetType());

        /// <summary>
        /// Tries to get the tag registered for the given type.
        /// </summary>
        /// <param name="generatorType">Type to retrieve the registered tag for.</param>
        /// <param name="tag">Out-variable in which to place the retrieved tag, or null if the type had no registered tag.</param>
        /// <returns>True if a tag was found; false otherwise.</returns>
        public static bool TryGetTag(Type generatorType, [MaybeNullWhen(false)] out string tag)
            => s_typesToTags.TryGetValue(generatorType, out tag);

        /// <summary>
        /// Tries to get the tag registered for the given type.
        /// </summary>
        /// <typeparam name="T">Type to retrieve the registered tag for.</typeparam>
        /// <param name="tag">Out-variable in which to place the retrieved tag, or null if the type had no registered tag.</param>
        /// <returns>True if a tag was found; false otherwise.</returns>
        public static bool TryGetTag<T>([MaybeNullWhen(false)] out string tag) where T : IEnhancedRandom
            => TryGetTag(typeof(T), out tag);

        /// <summary>
        /// Tries to get the tag registered for the type of the given generator instance.
        /// </summary>
        /// <param name="instance">The instance for which to retrieve the registered tag.</param>
        /// <param name="tag">Out-variable in which to place the retrieved tag, or null if the instance's runtime type had no registered tag.</param>
        /// <returns>True if a tag was found; false otherwise.</returns>
        public static bool TryGetTag(IEnhancedRandom instance, [MaybeNullWhen(false)] out string tag)
            => TryGetTag(instance.GetType(), out tag);

        /// <summary>
        /// Gets the tag registered for the given type, or null if no tag is registered for that type.
        /// </summary>
        /// <param name="generatorType">Type to retrieve the registered tag for.</param>
        /// <returns>The tag registered for the given type, or null if no tag was registered for the given type.</returns>
        public static string? GetTagOrNull(Type generatorType) => s_typesToTags.GetValueOrDefault(generatorType);

        /// <summary>
        /// Gets the tag registered for the given type, or null if no tag is registered for that type.
        /// </summary>
        /// <typeparam name="T">Type to retrieve the registered tag for.</typeparam>
        /// <returns>The tag registered for the given type, or null if no tag was registered for the given type.</returns>
        public static string? GetTagOrNull<T>() where T : IEnhancedRandom
            => GetTagOrNull(typeof(T));

        /// <summary>
        /// Gets the tag registered for the type of the given instance, or null if no tag is registered for that type.
        /// </summary>
        /// <param name="instance">Generator for which to retrieve the tag.</param>
        /// <returns>The tag registered for the runtime type of the given instance, or null if no tag was registered for that type.</returns>
        public static string? GetTagOrNull(IEnhancedRandom instance) => GetTagOrNull(instance.GetType());
        #endregion

        #region Standard Generator Tag Registration
        /// <summary>
        /// Registers the DefaultTag for all of the IEnhancedRandom implementations in ShaiRandom.
        /// </summary>
        /// <exception cref="ArgumentException">One of the tags failed to register.</exception>
        /// <remarks>
        /// This function will throw an exception if any of the tags are already registered to their type,
        /// or if any of the tags have already been registered to something else.  For a more tolerant way of setting
        /// tags, see the <see cref="TryRegisterShaiRandomDefaultTags"/> function.
        /// </remarks>
        public static void RegisterShaiRandomDefaultTags()
        {
            foreach (var instance in GetSerializerInstancesForShaiRandomGens())
                RegisterTag(instance);
        }

        /// <summary>
        /// Tries to register the DefaultTag for all of the IEnhancedRandom implementations in ShaiRandom.
        /// </summary>
        /// <remarks>
        /// This function will return false if ANY of the tags failed to set; however it will attempt to set tags for
        /// ALL the generators in ShaiRandom, even if one fails.  If you wish to forcibly set all the built-in ShaiRandom
        /// generators to be registered to their tag, see the <see cref="ForceRegisterShaiRandomDefaultTags"/> function.
        /// </remarks>
        public static bool TryRegisterShaiRandomDefaultTags()
        {
            bool allSucceeded = true;
            foreach (var instance in GetSerializerInstancesForShaiRandomGens())
                allSucceeded &= TryRegisterTag(instance);

            return allSucceeded;
        }

        /// <summary>
        /// Registers the DefaultTag for all of the IEnhancedRandom implementations in ShaiRandom, overwriting any
        /// existing registrations for those types and tags.
        /// </summary>
        /// <remarks>
        /// This function will unregister any conflicting tags, and replace them with the default tags, for all
        /// ShaiRandom implementations.  For a way of setting tags that is more tolerate to existing registrations,
        /// see <see cref="TryRegisterShaiRandomDefaultTags"/>.
        /// </remarks>
        public static void ForceRegisterShaiRandomDefaultTags()
        {
            foreach (var instance in GetSerializerInstancesForShaiRandomGens())
                ForceRegisterTag(instance);
        }

        private static IEnumerable<IEnhancedRandom> GetSerializerInstancesForShaiRandomGens()
        {
            // Generators
            yield return new DistinctRandom(1UL);
            yield return new FourWheelRandom(1UL, 1UL, 1UL, 1UL);
            yield return new KnownSeriesRandom();
            yield return new LaserRandom(1UL, 1UL);
            yield return MaxRandom.Instance;
            yield return MinRandom.Instance;
            yield return new MizuchiRandom(1UL, 1UL);
            yield return new RomuTrioRandom(1UL, 1UL, 1UL);
            yield return new StrangerRandom(1UL, 1UL, 1UL, 1UL);
            yield return new TricycleRandom(1UL, 1UL, 1UL);
            yield return new TrimRandom(1UL, 1UL, 1UL, 1UL);
            yield return new Xoshiro256StarStarRandom(1UL, 1UL, 1UL, 1UL);

            // Wrappers
            yield return new ArchivalWrapper(new DistinctRandom(1UL));
            yield return new ReversingWrapper(new DistinctRandom(1UL));
            yield return new TRGeneratorWrapper(new DistinctRandom(1UL));
        }
        #endregion

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
        /// This function will support any IEnhancedRandom implementation that has been registered with the serializer,
        /// provided that it had the same tag registered to it when it was serialized that it does when this function is called.
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
            return s_tagsToGenerators[tagData].Copy().StringDeserialize(data[idx..]);
        }
    }
}
