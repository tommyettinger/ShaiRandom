using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ShaiRandom.Generators;

namespace ShaiRandom
{
    /// <summary>
    /// A custom enumerator used to efficiently implement an infinite "gap shuffle"; a shuffle which takes a fixed-size
    /// set of items and produce a shuffled stream of them such that an element is never chosen in
    /// quick succession.
    ///
    /// Generally, you should use <see cref="EnhancedRandomExtensions.GapShuffle{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.Collections.Generic.IEnumerable{TItem})"/>
    /// or <see cref="EnhancedRandomExtensions.GapShuffleInPlace{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.Collections.Generic.IList{TItem})"/>
    /// to get an instance of this, rather than creating one yourself.
    /// </summary>
    /// <remarks>
    /// This enumerator is infinite, so you will need to use it in a foreach loop and break out of it, use something
    /// like LINQ's .Take(n) function, or use it in a while loop with a condition to break out of it in order to prevent
    /// an infinite loop.  Until you break out of the loop, it will continue to produce a mostly random shuffled stream
    /// of the items. These shuffles are spaced so that a single element should always have a large amount of "gap" in
    /// order between one appearance and the next. Specifically, all elements must be returned precisely once before
    /// elements are allowed to repeat.
    ///
    /// It supports in-place enumeration, where you give it a list and it performs its shuffles in place on the list or array
    /// you specify.  It also provides a constructor where you give it an IEnumerable, and it will copy elements into an
    /// array, as well a a similar constructor which copies a Span.  In the case of in-place enumeration, like most
    /// enumerators, modifying the collection while the shuffle is active (including adding or removing items) will cause
    /// undefined behavior.
    ///
    /// This type is a struct, and as such is much more efficient when used in a foreach loop than a function returning
    /// IEnumerable&lt;TItem&gt; by using "yield return".  This type does implement <see cref="IEnumerable{TItem}"/>,
    /// so you can pass it to functions which require one (for example, System.LINQ).  However, this will have reduced
    /// performance due to boxing of the iterator.
    /// </remarks>
    public struct GapShufflerEnumerator<TItem> : IEnumerator<TItem>, IEnumerable<TItem>
    {
        private readonly IList<TItem> _items;
        private readonly IEnhancedRandom _rng;
        private int _index;

        // We store the size instead of using _items.Count in MoveNext because changing the items in the middle of
        // an enumeration is not useful behavior in either case, and this is faster for many collections.
        private readonly int _size;

        // Suppress warning stating to use auto-property because we want to guarantee micro-performance
        // characteristics.
#pragma warning disable IDE0032 // Use auto property
        private TItem _current;
#pragma warning restore IDE0032 // Use auto property

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public TItem Current => _current;

        /// <inheritdoc />
        object? IEnumerator.Current => _current;

        /// <summary>
        /// Creates an enumerator that implements the "gap shuffle" algorithm.  This constructor copies the IEnumerable
        /// of items it receives into an array, which it uses internally for the shuffle.
        /// </summary>
        /// <param name="rng">RNG to use for shuffling.</param>
        /// <param name="items">Items to shuffle, which are copied into an array internally.</param>
        public GapShufflerEnumerator(IEnhancedRandom rng, IEnumerable<TItem> items)
        {
            _items = items.ToArray();
            _size = _items.Count;
            _rng = rng;

            rng.Shuffle(_items);

            _index = 0;
            _current = default!;
        }

        /// <summary>
        /// Creates an enumerator that implements the "gap shuffle" algorithm.  This constructor copies the span
        /// of items it receives into an array, which it uses internally for the shuffle.
        /// </summary>
        /// <param name="rng">RNG to use for shuffling.</param>
        /// <param name="items">Items to shuffle, which are copied into an array internally.</param>
        public GapShufflerEnumerator(IEnhancedRandom rng, ReadOnlySpan<TItem> items)
        {
            _items = items.ToArray();
            _size = _items.Count;
            _rng = rng;

            rng.Shuffle(_items);

            _index = 0;
            _current = default!;
        }

        /// <summary>
        /// Creates an enumerator that implements the "gap shuffle" algorithm.  Instead of copying the items into
        /// an array like the other constructor or the IList.GapShuffler extension method, this takes a reference to a
        /// list and uses it without making a copy.
        ///
        /// This means that the list you give it will change pseudo-randomly as you advance the enumerator.  This is
        /// useful if you do not care about the order of the collection after you are done with the enumerator and wish
        /// to avoid a copy being made.
        /// </summary>
        /// <param name="rng">The RNG to use.</param>
        /// <param name="items">The list of items to shuffle; the items will be shuffled in place, and no copy of this list is made.</param>
        public GapShufflerEnumerator(IEnhancedRandom rng, ref IList<TItem> items)
        {
            _items = items;
            _size = _items.Count;
            _rng = rng;

            rng.Shuffle(_items);

            _index = 0;
            _current = default!;
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if(_size == 1)
            {
                _current = _items[0];
                return true;
            }
            if(_index >= _size)
            {
                int n = _size - 1;
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
        /// Returns the next item in the sequence.
        /// </summary>
        /// <returns>The next (shuffled) item in the sequence.</returns>
        public TItem Next()
        {
            MoveNext();
            return _current;
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

    /// <summary>
    /// A custom enumerator used to efficiently implement an infinite "gap shuffle"; a shuffle which takes a fixed-size
    /// set of items and produce a shuffled stream of them such that an element is never chosen in
    /// quick succession.
    ///
    /// Generally, you should use <see cref="EnhancedRandomExtensions.GapShuffle{TItem}(ShaiRandom.Generators.IEnhancedRandom,ReadOnlyMemory{TItem})"/>
    /// or <see cref="EnhancedRandomExtensions.GapShuffleInPlace{TItem}(ShaiRandom.Generators.IEnhancedRandom,System.Memory{TItem})"/>to get an instance of this, rather than creating
    /// one yourself.
    /// </summary>
    /// <remarks>
    /// This enumerator is infinite, so you will need to use it in a foreach loop and break out of it, use something
    /// like LINQ's .Take(n) function, or use it in a while loop with a condition to break out of it in order to prevent
    /// an infinite loop.  Until you break out of the loop, it will continue to produce a mostly random shuffled stream
    /// of the items. These shuffles are spaced so that a single element should always have a large amount of "gap" in
    /// order between one appearance and the next. Specifically, all elements must be returned precisely once before
    /// elements are allowed to repeat.
    ///
    /// The Memory object is provided in the constructor, and it will store this object internally and repeatedly shuffle
    /// it as the iterator progresses.it will copy elements into an
    /// array, as well a a similar constructor which copies a Span.  Like most enumerators, modifying the collection
    /// while the shuffle is active (including adding or removing items) will cause undefined behavior.
    ///
    /// This type is a struct, and as such is much more efficient when used in a foreach loop than a function returning
    /// IEnumerable&lt;TItem&gt; by using "yield return".  This type does implement <see cref="IEnumerable{TItem}"/>,
    /// so you can pass it to functions which require one (for example, System.LINQ).  However, this will have reduced
    /// performance due to boxing of the iterator.
    /// </remarks>
    public struct GapShufflerInPlaceMemoryEnumerator<TItem> : IEnumerator<TItem>, IEnumerable<TItem>
    {
        private readonly Memory<TItem> _items;
        private readonly IEnhancedRandom _rng;
        private int _index;

        // We store the size instead of using _items.Count in MoveNext because changing the items in the middle of
        // an enumeration is not useful behavior in either case, and this is faster for many collections.
        private readonly int _size;

        // Suppress warning stating to use auto-property because we want to guarantee micro-performance
        // characteristics.
#pragma warning disable IDE0032 // Use auto property
        private TItem _current;
#pragma warning restore IDE0032 // Use auto property

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public TItem Current => _current;

        /// <inheritdoc />
        object? IEnumerator.Current => _current;

        /// <summary>
        /// Creates an enumerator that implements the "gap shuffle" algorithm.  The Memory&lt;TItem&gt; it receives
        /// is stored directly and will be shuffled in-place repeatedly as the iterator advances.
        /// </summary>
        /// <param name="rng">The RNG to use.</param>
        /// <param name="items">The list of items to shuffle; the items will be shuffled in place, and no copy of this list is made.</param>
        public GapShufflerInPlaceMemoryEnumerator(IEnhancedRandom rng, Memory<TItem> items)
        {
            _items = items;
            _size = _items.Length;
            _rng = rng;

            rng.Shuffle(_items.Span);

            _index = 0;
            _current = default!;
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            var span = _items.Span;

            if(_size == 1)
            {
                _current = span[0];
                return true;
            }
            if(_index >= _size)
            {
                int n = _size - 1;
                int swapWith;
                for (int i = n; i > 1; i--) {
                    swapWith = _rng.NextInt(i);
                    (span[i - 1], span[swapWith]) = (span[swapWith], span[i - 1]);
                }
                swapWith = 1 + _rng.NextInt(n);
                (span[n], span[swapWith]) = (span[swapWith], span[n]);
                _index = 0;
            }
            _current = span[_index++];
            return true;
        }

        /// <summary>
        /// Returns the next item in the sequence.
        /// </summary>
        /// <returns>The next (shuffled) item in the sequence.</returns>
        public TItem Next()
        {
            MoveNext();
            return _current;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GapShufflerInPlaceMemoryEnumerator<TItem> GetEnumerator() => this;

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
