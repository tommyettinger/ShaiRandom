using System;
using System.Collections.Generic;
using System.Linq;
using ShaiRandom.Generators;

namespace ShaiRandom.Collections
{
    /// <summary>
    /// A fixed-size probability table that pairs (potentially repeated) items with weights that affect how often they
    /// are returned by <code>NextItem()</code>.
    /// </summary>
    /// <remarks>
    /// You can call Reset() to change the table this uses. This uses Vose' Alias Method to get constant-time lookups.
    /// </remarks>
    /// <typeparam name="TItem">The type of item that this holds, other than weights; this can be an int or id to look up a table entry elsewhere.</typeparam>
    public class ProbabilityTable<TItem>
    {
        /// <summary>
        /// The IReadOnlyList of (item, weight) pairs this uses for what <see cref="NextItem()"/> can return.
        /// </summary>
        public IReadOnlyList<(TItem item, double weight)> Items { get; private set; } = null!; // Initialized by Reset (called from constructor)
        private uint[]? _mixed;

        /// <summary>
        /// How many item-weight pairs this stores (not necessarily how many unique items).
        /// </summary>
        public int Count => Items.Count;
        /// <summary>
        /// The IEnhancedRandom this uses to make its weighted random choices. This defaults to an unseeded <see cref="MizuchiRandom"/> if not specified.
        /// </summary>
        public IEnhancedRandom Random { get; set; }

        /// <summary>
        /// Constructs an empty ProbabilityTable. You must call Reset() with some items this can choose from before using this ProbabilityTable.
        /// </summary>
        public ProbabilityTable() : this(new MizuchiRandom(), Array.Empty<(TItem, double)>())
        {
        }

        /// <summary>
        /// Constructs a ProbabilityTable that will use the given (item, weight) pairs and an unseeded MizuchiRandom.
        /// </summary>
        /// <param name="items">
        /// A list of pairs, where the first item of a pair is the TItem to be potentially returned,
        /// and the second item is the weight for how much to favor returning that item.
        /// </param>
        public ProbabilityTable(IEnumerable<(TItem item, double weight)> items) : this(new MizuchiRandom(), items.ToArray())
        {
        }

        /// <summary>
        /// Constructs a ProbabilityTable that will use the given (item, weight) pairs and an unseeded MizuchiRandom.
        /// </summary>
        /// <param name="items">
        /// An IReadOnlyList of pairs, where the first item of a pair is the TItem to be potentially returned,
        /// and the second item is the weight for how much to favor returning that item.  The list will NOT be copied,
        /// and must not be modified after it is passed to this function.
        /// </param>
        public ProbabilityTable(ref IReadOnlyList<(TItem item, double weight)> items) : this(new MizuchiRandom(), items)
        {
        }


        /// <summary>
        /// Constructs a ProbabilityTable that will use the given (item, weight) pairs and the given IEnhancedRandom.
        /// </summary>
        /// <param name="random">Any IEnhancedRandom, such as a <see cref="TrimRandom"/> or <see cref="LaserRandom"/>.</param>
        /// <param name="items">
        /// A list of pairs, where the first item of a pair is the TItem to be potentially returned, and the
        /// second item is the weight for how much to favor returning that item.
        /// </param>
        public ProbabilityTable(IEnhancedRandom random, IEnumerable<(TItem item, double weight)> items) : this(random, items.ToArray())
        {
        }

        /// <summary>
        /// Constructs a ProbabilityTable that will use the given (item, weight) pairs and the given IEnhancedRandom.
        /// </summary>
        /// <param name="random">Any IEnhancedRandom, such as a <see cref="TrimRandom"/> or <see cref="LaserRandom"/>.</param>
        /// <param name="items">
        /// An IReadOnlyList of pairs, where the first item of a pair is the TItem to be potentially returned, and the
        /// second item is the weight for how much to favor returning that item.  The list will NOT be copied,
        /// and must not be modified after it is passed to this function.
        /// </param>
        public ProbabilityTable(IEnhancedRandom random, ref IReadOnlyList<(TItem item, double weight)> items) : this(random, items)
        {
        }

        /// <summary>
        /// Constructs a ProbabilityTable that will use the given items and weights as side-by-side sequences, and an unseeded MizuchiRandom.
        /// </summary>
        /// <remarks>
        /// If TItem is a reference type, the objects themselves are not copied.  If the two lists given are not the same size,
        /// items will be paired until the end of one of the lists is reached.
        /// </remarks>
        /// <param name="items">A list of TItem instances.</param>
        /// <param name="weights">A list of double weights.</param>
        public ProbabilityTable(IEnumerable<TItem> items, IEnumerable<double> weights) : this(new MizuchiRandom(), items, weights)
        {
        }
        /// <summary>
        /// Constructs a ProbabilityTable that will use the given items and weights as side-by-side sequences, and the given IEnhancedRandom.
        /// </summary>
        /// <remarks>
        /// If TItem is a reference type, the objects themselves are not copied.  If the two lists given are not the same size,
        /// items will be paired until the end of one of the lists is reached.
        /// </remarks>
        /// <param name="random">Any IEnhancedRandom, such as a <see cref="TrimRandom"/> or <see cref="LaserRandom"/>.</param>
        /// <param name="items">A list of TItem instances.</param>
        /// <param name="weights">A list of double weights.</param>
        public ProbabilityTable(IEnhancedRandom random, IEnumerable<TItem> items, IEnumerable<double> weights)
        {
            Random = random;
            Reset(items, weights);
        }

        // No deep copy
        private ProbabilityTable(IEnhancedRandom random, IReadOnlyList<(TItem item, double weight)> items)
        {
            Random = random;
            Reset(items);
        }

        /// <summary>
        /// Resets the ProbabilityTable to use a different group of items and weights, with the items and weights specified in side-by-side lists.
        /// </summary>
        /// <remarks>
        /// If TItem is a reference type, the objects themselves will not be copied.  If the two lists given are not the same size,
        /// items will be paired until the end of one of the lists is reached.
        /// </remarks>
        /// <param name="items">A list of TItem instances.</param>
        /// <param name="weights">An IReadOnlyList of double weights.</param>
        public void Reset(IEnumerable<TItem> items, IEnumerable<double> weights)
        {
            var list = new List<(TItem item, double weight)>();

            using (IEnumerator<TItem> itemsIt = items.GetEnumerator())
            using (IEnumerator<double> weightsIt = weights.GetEnumerator())
            {
                while (itemsIt.MoveNext() && weightsIt.MoveNext())
                    list.Add((itemsIt.Current, weightsIt.Current));
            }

            Reset(list);
        }

        /// <summary>
        /// Resets the ProbabilityTable to use a different group of items and weights, with the items and weights specified as pairs in one list.
        /// </summary>
        /// <param name="items">A list of pairs, where the first item of a pair is the TItem to be potentially returned, and the second item is the weight for how much to favor returning that item.</param>
        public void Reset(IEnumerable<(TItem item, double weight)> items) => Reset(items.ToArray());

        /// <summary>
        /// Resets the ProbabilityTable to use a different group of items and weights, with the items and weights specified as pairs in one list.
        /// </summary>
        /// <remarks>
        /// This does NOT copy the given list, and the list must not be changed after it is passed to this function.
        /// </remarks>
        /// <param name="items">An IReadOnlyList of pairs, where the first item of a pair is the TItem to be potentially returned, and the second item is the weight for how much to favor returning that item.</param>
        public void Reset(ref IReadOnlyList<(TItem item, double weight)> items) => Reset(items);

        // No defensive copy
        private void Reset(IReadOnlyList<(TItem item, double weight)> items)
        {
            Items = items;

            if (_mixed is null || _mixed.Length != (items.Count << 1))
                _mixed = new uint[items.Count << 1];

            int size = Count;
            double sum = 0.0;
            double[] probs = new double[size];
            for (int idx = 0; idx < Items.Count; idx++) {
                double weight = Items[idx].weight;
                if (weight <= 0.0) continue;
                sum += (probs[idx] = weight);
            }
            double average = sum / Count, invAverage = 1.0 / average;

            /* Create two stacks to act as worklists as we populate the tables. */
            List<uint> small = new List<uint>(size);
            List<uint> large = new List<uint>(size);

            /* Populate the stacks with the input probabilities. */
            for (uint i = 0; i < size; ++i)
            {
                /* If the probability is below the average probability, then we add
                 * it to the small list; otherwise we add it to the large list.
                 */
                if (probs[i] >= average)
                    large.Add(i);
                else
                    small.Add(i);
            }

            /* As a note: in the mathematical specification of the algorithm, we
             * will always exhaust the small list before the big list.  However,
             * due to floating point inaccuracies, this is not necessarily true.
             * Consequently, this inner loop (which tries to pair small and large
             * elements) will have to check that both lists aren't empty.
             */
            while (small.Count > 0 && large.Count > 0)
            {
                /* Get the index of the small and the large probabilities. */
                uint less = small[^1], less2 = less << 1;
                small.RemoveAt(small.Count - 1);
                uint more = large[^1];
                large.RemoveAt(large.Count - 1);

                /* These probabilities have not yet been scaled up to be such that
                 * sum/n is given weight 1.0.  We do this here instead.
                 */
                _mixed[less2] = (uint)(0xFFFFFFFF * (probs[less] * invAverage));
                _mixed[less2 | 1] = more;

                probs[more] += probs[less] - average;

                if (probs[more] >= average)
                    large.Add(more);
                else
                    small.Add(more);
            }

            while (small.Count > 0)
            {
                _mixed[small[^1] << 1] = 0xFFFFFFFF;
                small.RemoveAt(small.Count - 1);
            }
            while (large.Count > 0)
            {
                _mixed[large[^1] << 1] = 0xFFFFFFFF;
                large.RemoveAt(large.Count - 1);
            }
        }

        /// <summary>
        /// Gets a randomly-chosen TItem item (obeying the given weights) from the data this stores.
        /// </summary>
        /// <returns>A randomly-chosen TItem item.</returns>
        /// <exception cref="InvalidOperationException">If this was reset or initialized with an empty list of items.</exception>
        public TItem NextItem()
        {
            if (Items.Count == 0) throw new InvalidOperationException("NextItem() cannot be used if there are no items; use Reset() before calling.");
            ulong state = Random.NextULong();
            // get a random int (using half the bits of our previously-calculated state) that is less than size
            uint column = (uint)(((ulong)Count * (state & 0xFFFFFFFFUL)) >> 32);
            // use the other half of the bits of state to get a 31-bit int, compare to probability and choose either the
            // current column or the alias for that column based on that probability
            return Items[(int)(((state >> 32) <= _mixed![column << 1]) ? column : _mixed[column << 1 | 1])].item;

        }
    }
}
