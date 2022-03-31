using System;
using System.Collections.Generic;
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
        public List<(TItem item, double weight)> Items { get; private set; }
        private int[] mixed;
        private IEnhancedRandom _random;

        public int Count => Items.Count;

        public IEnhancedRandom Random
        {
            get => _random; set => _random = value?? new MizuchiRandom();
        }

        public ProbabilityTable()
        {
            Random = new MizuchiRandom();
        }

        public ProbabilityTable(List<(TItem item, double weight)> items) : this(new MizuchiRandom(), items)
        {
        }

        public ProbabilityTable(IEnhancedRandom random, List<(TItem item, double weight)> items)
        {
            Random = random;
            Reset(items);
        }

        public ProbabilityTable(List<TItem> items, List<double> weights) : this(new MizuchiRandom(), items, weights)
        {
        }

        public ProbabilityTable(IEnhancedRandom random, List<TItem> items, List<double> weights)
        {
            Random = random;
            Reset(items, weights);
        }
        public void Reset(List<TItem> items, List<double> weights)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));
            if (weights is null) throw new ArgumentNullException(nameof(weights));

            int used = Math.Min(items.Count, weights.Count);
            List<(TItem item, double weight)> newItems = new List<(TItem item, double weight)>(used);
            for (int i = 0; i < used; i++)
            {
                newItems.Add((items[i], weights[i]));
            }
            Reset(newItems);
        }
        public void Reset(List<(TItem item, double weight)> items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            Items = new List<(TItem item, double weight)>(items);
            int size = Count;
            mixed = new int[size << 1];
            double sum = 0.0;
            double[] probs = new double[size];
            for (int idx = 0; idx < Items.Count; idx++) {
                double weight = Items[idx].weight;
                if (weight <= 0.0) continue;
                sum += (probs[idx] = weight);
            }
            double average = sum / Count, invAverage = 1.0 / average;

            /* Create two stacks to act as worklists as we populate the tables. */
            List<int> small = new List<int>(size);
            List<int> large = new List<int>(size);

            /* Populate the stacks with the input probabilities. */
            for (int i = 0; i < size; ++i)
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
                int less = small[small.Count - 1], less2 = less << 1;
                small.RemoveAt(small.Count - 1);
                int more = large[large.Count - 1];
                large.RemoveAt(large.Count - 1);

                /* These probabilities have not yet been scaled up to be such that
                 * sum/n is given weight 1.0.  We do this here instead.
                 */
                mixed[less2] = (int)(0x7FFFFFFF * (probs[less] * invAverage));
                mixed[less2 | 1] = more;

                probs[more] += probs[less] - average;

                if (probs[more] >= average)
                    large.Add(more);
                else
                    small.Add(more);
            }

            while (small.Count > 0)
            {
                mixed[small[small.Count - 1] << 1] = 0x7FFFFFFF;
                small.RemoveAt(small.Count - 1);
            }
            while (large.Count > 0)
            {
                mixed[large[large.Count - 1] << 1] = 0x7FFFFFFF;
                large.RemoveAt(large.Count - 1);
            }
        }

        public TItem NextItem()
        {
            long state = Random.NextLong();
            // get a random int (using half the bits of our previously-calculated state) that is less than size
            int column = (int)((Count * (state & 0xFFFFFFFFL)) >> 32);
            // use the other half of the bits of state to get a 31-bit int, compare to probability and choose either the
            // current column or the alias for that column based on that probability
            return Items[((state >> 32 & 0x7FFFFFFFL) <= mixed[column << 1]) ? column : mixed[column << 1 | 1]].item;

        }
    }
}
