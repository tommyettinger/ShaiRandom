// ReSharper disable CheckNamespace
using System;
using System.Collections.Generic;
using ShaiRandom.Generators;
using ShaiRandom.TroschuetzCompat.Generators;

namespace ShaiRandom.TroschuetzCompat
{
    /// <summary>
    /// Static class defining methods which allow convenient registration of the standard
    /// tags for all generators defined in TroschuetzCompat.
    /// </summary>
    /// <remarks>
    /// These are the same as the <see cref="Serializer.RegisterShaiRandomDefaultTags"/> and similar methods,
    /// however these register the generators defined in TroschuetzCompat, rather than the ones defined in
    /// ShaiRandom.
    /// </remarks>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Registers the DefaultTag for all of the IEnhancedRandom implementations in ShaiRandom.TroschuetzCompat.
        /// </summary>
        /// <exception cref="ArgumentException">One of the tags failed to register.</exception>
        /// <remarks>
        /// This function will throw an exception if any of the tags are already registered to their type,
        /// or if any of the tags have already been registered to something else.  For a more tolerant way of setting
        /// tags, see the <see cref="TryRegisterTroschuetzCompatDefaultTags"/> function.
        /// </remarks>
        public static void RegisterTroschuetzCompatDefaultTags()
        {
            foreach (var instance in GetSerializerInstancesForCompatRandomGens())
                Serializer.RegisterTag(instance);
        }

        /// <summary>
        /// Tries to register the DefaultTag for all of the IEnhancedRandom implementations in ShaiRandom.TroschuetzCompat.
        /// </summary>
        /// <remarks>
        /// This function will return false if ANY of the tags failed to set; however it will attempt to set tags for
        /// ALL the generators in ShaiRandom.TroschuetzCompat, even if one fails.  If you wish to forcibly set all the
        /// ShaiRandom.TroschuetzCompat generators to be registered to their tag, see the
        /// <see cref="ForceRegisterTroschuetzCompatDefaultTags"/> function.
        /// </remarks>
        public static bool TryRegisterTroschuetzCompatDefaultTags()
        {
            bool allSucceeded = true;
            foreach (var instance in GetSerializerInstancesForCompatRandomGens())
                allSucceeded &= Serializer.TryRegisterTag(instance);

            return allSucceeded;
        }

        /// <summary>
        /// Registers the DefaultTag for all of the IEnhancedRandom implementations in ShaiRandom.TroschuetzCompat, overwriting any
        /// existing registrations for those types and tags.
        /// </summary>
        /// <remarks>
        /// This function will unregister any conflicting tags, and replace them with the default tags, for all
        /// ShaiRandom.TroschuetzCompat implementations.  For a way of setting tags that is more tolerant to existing registrations,
        /// see <see cref="TryRegisterTroschuetzCompatDefaultTags"/>.
        /// </remarks>
        public static void ForceRegisterTroschuetzCompatDefaultTags()
        {
            foreach (var instance in GetSerializerInstancesForCompatRandomGens())
                Serializer.ForceRegisterTag(instance);
        }

        private static IEnumerable<IEnhancedRandom> GetSerializerInstancesForCompatRandomGens()
        {
            yield return new TRGeneratorWrapper(new DistinctRandom(1UL));
        }
    }
}
