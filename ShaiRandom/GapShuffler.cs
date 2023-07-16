using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ShaiRandom.Generators;

namespace ShaiRandom
{
    public struct GapShufflerEnumerator<TItem> : IEnumerator<TItem>, IEnumerable<TItem>
    {
        private readonly IList<TItem> _items;
        private readonly IEnhancedRandom _rng;
        private int _index;

        // Suppress warning stating to use auto-property because we want to guarantee micro-performance
        // characteristics.
#pragma warning disable IDE0032 // Use auto property
        private TItem _current;
#pragma warning restore IDE0032 // Use auto property

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public TItem Current => _current;

        object? IEnumerator.Current => _current;

        public GapShufflerEnumerator(IEnhancedRandom rng, IEnumerable<TItem> items)
        {
            _items = items.ToArray();
            _rng = rng;

            rng.Shuffle(_items);

            _index = 0;
            _current = default!;
        }

        /// <summary>
        /// Creates an enumerator that implements the "gap shuffle" algorithm.  Instead of copying the list like the
        /// other constructor or the IList.GapShuffler extension method, this takes a reference to the list and uses it
        /// without making a copy.
        ///
        /// This means that the list you give it will change pseudo-randomly as you advance the enumerator.  This is
        /// useful if you do not care about the order of the collection after you are done with the enumerator and wish
        /// to avoid a copy being made.
        /// </summary>
        /// <param name="rng">The RNG to use.</param>
        /// <param name="items">The list of items to shuffle.</param>
        public GapShufflerEnumerator(IEnhancedRandom rng, ref IList<TItem> items)
        {
            _items = items;
            _rng = rng;

            rng.Shuffle(_items);

            _index = 0;
            _current = default!;
        }

        public bool MoveNext()
        {
            int size = _items.Count; // In case size changes while we're iterating
            if(size == 1)
            {
                _current = _items[0];
                return true;
            }
            if(_index >= size)
            {
                int n = size - 1;
                int swapWith;
                for (int i = n; i > 1; i--) {
                    swapWith = _rng.NextInt(i);
                    (_items[i - 1], _items[swapWith]) = (_items[swapWith], _items[i - 1]);
                }
                swapWith = 1 + _rng.NextInt(n);
                (_items[n], _items[swapWith]) = (_items[swapWith], _items[n]);
                _index = 0;
            }
            _current = _items[_index++];
            return true;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GapShufflerEnumerator<TItem> GetEnumerator() => this;

        // Explicitly implemented to ensure we prefer the non-boxing versions where possible
        #region Explicit Interface Implementations
        /// <summary>
        /// This iterator does not support resetting.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        void IEnumerator.Reset() => throw new NotSupportedException();
        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        void IDisposable.Dispose()
        { }
        #endregion
    }
}
