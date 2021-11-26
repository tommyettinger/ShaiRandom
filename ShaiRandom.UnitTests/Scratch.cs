using System;

namespace ShaiRandom.UnitTests
{
    class Scratch
    {
        private static FourWheelRandom fwr = new FourWheelRandom(1);
        private static void InRange(int checking, int lower, int upper)
        {

        }
        private static void InRange(uint checking, uint lower, uint upper)
        {

        }
        private static void InRange(long checking, long lower, long upper)
        {

        }
        private static void InRange(ulong checking, ulong lower, ulong upper)
        {

        }
        private static void InRange(double checking, double lower, double upper)
        {

        }
        private static void InRange(float checking, float lower, float upper)
        {
            if(checking < lower || checking > upper)
            {
                Console.WriteLine(checking + " was outside of bounds");
                fwr.PreviousUlong();
                Console.WriteLine("previous returned long was {0:X}", fwr.stateD);
            }
        }

        public static void Main(string[] args)
        {
            float fsum = 0f;
            for (int i = 0; i < 100; i++)
            {
                float f = fwr.NextExclusiveFloat();
                Console.WriteLine(f);
                fsum += f;
            }
            Console.WriteLine();
            Console.WriteLine(fsum);

            fwr.Seed(1);
            for (int i = 0; i < 100; i++)
            {
                InRange(fwr.NextInt(-100, 101), -100, 100);
                InRange(fwr.NextInt(100, -101), -100, 100);
                InRange(fwr.NextUint(100U, 301U), 100U, 300U);
                InRange(fwr.NextLong(-100L, 101L), -100L, 100L);
                InRange(fwr.NextLong(100L, -101L), -100L, 100L);
                InRange(fwr.NextUlong(100UL, 301UL), 100UL, 300UL);
                InRange(fwr.NextExclusiveDouble(), 1.0842021724855044E-19, 0.9999999999999999);
                InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);
            }
            fwr.stateD = 1UL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

            fwr.stateD = 0UL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

            fwr.stateD = 0xFFFFFFFFFFFFFFFFUL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

            fwr.stateD = 0x8000000000000000UL;
            InRange(fwr.NextExclusiveFloat(), 0f, 0.99999994f);

        }
    }
}
