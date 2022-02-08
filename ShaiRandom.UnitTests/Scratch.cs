using System;
using System.Collections.Generic;
using System.Linq;
using ShaiRandom.Generators;

namespace ShaiRandom.UnitTests
{
    class Scratch
    {
        private static FourWheelRandom fwr = new FourWheelRandom(1);
        private static void InRange(int checking, int lower, int upper)
        {
            if (checking < lower || checking > upper)
                Console.WriteLine(checking + " was outside of bounds");
        }
        private static void InRange(uint checking, uint lower, uint upper)
        {
            if (checking < lower || checking > upper)
                Console.WriteLine(checking + " was outside of bounds");
        }
        private static void InRange(long checking, long lower, long upper)
        {
            if (checking < lower || checking > upper)
                Console.WriteLine(checking + " was outside of bounds");
        }
        private static void InRange(ulong checking, ulong lower, ulong upper)
        {
            if (checking < lower || checking > upper)
                Console.WriteLine(checking + " was outside of bounds");
        }
        private static void InRange(double checking, double lower, double upper)
        {
            if (checking < lower || checking > upper)
                Console.WriteLine(checking + " was outside of bounds");
        }
        private static void InRange(float checking, float lower, float upper)
        {
            if(checking < lower || checking > upper)
            {
                Console.WriteLine(checking + " was outside of bounds");
                fwr.PreviousULong();
                Console.WriteLine("previous returned long was {0:X}", fwr.StateD);
                Console.WriteLine("(int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) >> 52) : {0:D}", (int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | (long)fwr.StateD) >> 52));
                Console.WriteLine("(127 + 962 + (int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) >> 52) << 23 : {0:D}", 127 + 962 + (int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | (long)fwr.StateD) >> 52) << 23);
                //BitConverter.Int32BitsToSingle((127 + 962 + (int)(BitConverter.DoubleToInt64Bits(-0x7FFFFFFFFFFFF001L | bits) >> 52) << 23) | ((int)~bits & 0x007FFFFF));
            }
        }

        public static void Main(string[] args)
        {
            Serializer.RegisterShaiRandomDefaultTags();
            float fsum = 0f;
            for (int i = 0; i < 100; i++)
            {
                float f = fwr.NextExclusiveFloat();
                Console.WriteLine(f);
                fsum += f;
            }
            Console.WriteLine();
            Console.WriteLine(fsum);

            Console.WriteLine("Starting InRange checks:");
            fwr.Seed(1);
            for (int i = 0; i < 100; i++)
            {
                InRange(fwr.NextInt(2), 0, 1);
                InRange(fwr.NextInt(-2), -1, 0);
                InRange(fwr.NextInt(-100, 101), -100, 100);
                InRange(fwr.NextInt(100, -101), -100, 100);
                InRange(fwr.NextUInt(100U, 301U), 100U, 300U);
                InRange(fwr.NextLong(-100L, 101L), -100L, 100L);
                InRange(fwr.NextLong(100L, -101L), -100L, 100L);
                InRange(fwr.NextULong(100UL, 301UL), 100UL, 300UL);
                InRange(fwr.NextExclusiveDouble(), 1.0842021724855044E-19, 0.9999999999999999);
                InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);
            }
            fwr.StateD = 1UL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

            fwr.StateD = 0UL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

            fwr.StateD = 0xFFFFFFFFFFFFFFFFUL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

            fwr.StateD = 0x8000000000000000UL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

            //int[] buckets = new int[256];
            //for(int i = 0; i < 0x4000000; i++)
            //{
            //    buckets[(int)(fwr.NextDecimal() * 256)]++;
            //}
            //List<int> bs = buckets.OrderBy(b => b).ToList();
            //foreach(int i in bs)
            //{
            //    Console.WriteLine(i);
            //}
            {
                IEnhancedRandom r = new MizuchiRandom(1);

                int[] buckets = new int[256];
                for (int i = 0; i < 0x100000; i++)
                {
                    buckets[(int)(r.NextDecimal() * 256)]++;
                }
                IOrderedEnumerable<int> ob = buckets.OrderBy(b => b);
                int smallest = ob.First();
                int biggest = ob.Last();
                Console.WriteLine(smallest);
                Console.WriteLine(biggest);
                Console.WriteLine((biggest - smallest) / (biggest + 0.001));
            }
            Console.WriteLine();
            {
                IEnhancedRandom r = new MizuchiRandom(1);

                int[] buckets = new int[256];
                for (int i = 0; i < 0x100000; i++)
                {
                    buckets[(int)(r.NextDouble() * 256)]++;
                }
                IOrderedEnumerable<int> ob = buckets.OrderBy(b => b);
                int smallest = ob.First();
                int biggest = ob.Last();
                Console.WriteLine(smallest);
                Console.WriteLine(biggest);
                Console.WriteLine((biggest - smallest) / (biggest + 0.001));
            }
            Console.WriteLine();
            {
                IEnhancedRandom r = new MizuchiRandom(1);

                int[] buckets = new int[256];
                for (int i = 0; i < 0x100000; i++)
                {
                    buckets[(int)(r.NextFloat() * 256)]++;
                }
                IOrderedEnumerable<int> ob = buckets.OrderBy(b => b);
                int smallest = ob.First();
                int biggest = ob.Last();
                Console.WriteLine(smallest);
                Console.WriteLine(biggest);
                Console.WriteLine((biggest - smallest) / (biggest + 0.001));
            }
            Console.WriteLine();
            {
                IEnhancedRandom r = new MizuchiRandom(1);

                int[] buckets = new int[256];
                for (int i = 0; i < 0x100000; i++)
                {
                    buckets[(int)(r.NextExclusiveDouble() * 256)]++;
                }
                IOrderedEnumerable<int> ob = buckets.OrderBy(b => b);
                int smallest = ob.First();
                int biggest = ob.Last();
                Console.WriteLine(smallest);
                Console.WriteLine(biggest);
                Console.WriteLine((biggest - smallest) / (biggest + 0.001));
            }
            Console.WriteLine();
            {
                IEnhancedRandom r = new MizuchiRandom(1);

                int[] buckets = new int[256];
                for (int i = 0; i < 0x100000; i++)
                {
                    buckets[(int)(r.NextExclusiveFloat() * 256)]++;
                }
                IOrderedEnumerable<int> ob = buckets.OrderBy(b => b);
                int smallest = ob.First();
                int biggest = ob.Last();
                Console.WriteLine(smallest);
                Console.WriteLine(biggest);
                Console.WriteLine((biggest - smallest) / (biggest + 0.001));
            }
            Console.WriteLine();
            // Create a KSR with unique series per type
            var ksr = new KnownSeriesRandom(
                new[] { 1, 2 }, new[] { 2U, 3U }, new[] { 3.3, 4.4 },
                new[] { true, false }, new[] { (byte)4, (byte)5 },
                new[] { 5.5f, 6.6f }, new[] { 6L, 7L }, null, new[] { 8.8M, 9.9M }); //new[] { 7UL, 8UL }

            // Advance all states (so the indices are not their starting value)
            ksr.SetState(1);

            // Serialize generator
            string ser = ksr.StringSerialize();
            Console.WriteLine(ser);
            Console.WriteLine("Round-Trip:");
            ksr.StringDeserialize(ser);
            Console.WriteLine(ksr.StringSerialize());


            IEnhancedRandom[] generators = DataGenerators.CreateGenerators(false).Where(g => g.SupportsPrevious).ToArray();
            foreach (var gen in generators)
            {
                List<ulong> forward = new List<ulong>(100);
                for (int i = 0; i < 100; i++)
                {
                    forward.Add(gen.NextULong());
                }
                gen.NextULong();
                forward.Reverse();
                for (int i = 0; i < 100; i++)
                {
                    if(forward[i] != gen.PreviousULong())
                    {
                        Console.WriteLine(gen.DefaultTag + " on " + i);
                    }
                }
            }
            //            Console.WriteLine(mr.NextExclusiveDouble());
            //            Console.WriteLine(mr.NextExclusiveDouble(-1e-9));

            MaxRandom mr = new MaxRandom();
            double d = mr.NextExclusiveDouble(-3e-9, -1e-9);
            Console.WriteLine(d);

        }
    }
}
