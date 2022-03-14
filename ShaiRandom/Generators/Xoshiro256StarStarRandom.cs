using System;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 4 states, implementing a known-rather-good algorithm that is 4-dimensionally equidistributed.
    /// </summary>
    /// <remarks>
    /// Being 4-dimensionally equidistributed means this produces every group of 4 consecutive ulong values with equal likelihood
    /// (except for four 0s, which this never returns consecutively). This generator is not perfect (it isn't as fast as most others
    /// on .NET, and there's a known problem if all results are multiplied by a constant with a specific last byte in its 64 bits),
    /// but it's very hard to get this level of equidistribution and keep up competitive speed. The algorithm here is xoshiro256** ;
    /// see https://prng.di.unimi.it/ for more information on this family of algorithms.
    /// <br />
    /// This does not support <see cref="IEnhancedRandom.PreviousULong()"/> or <see cref="IEnhancedRandom.Skip(ulong)"/>.
    /// </remarks>
    public sealed class Xoshiro256StarStarRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "XSSR" .
        /// </summary>
        public override string DefaultTag => "XSSR";

        /// <summary>
        /// The first state; can be any ulong except that the whole state must not all be 0.
        /// </summary>
        public ulong StateA { get; set; }

        /// <summary>
        /// The second state; can be any ulong except that the whole state must not all be 0.
        /// </summary>
        /// <remarks>
        /// This is the state that is scrambled and returned; if it is 0 before a number is generated, then the next number will be 0.
        /// </remarks>
        public ulong StateB { get; set; }
        /// <summary>
        /// The third state; can be any ulong except that the whole state must not all be 0.
        /// </summary>
        public ulong StateC { get; set; }
        private ulong _d;
        /// <summary>
        /// The fourth state; can be any ulong except that the whole state must not all be 0.
        /// </summary>
        /// <remarks>If all other states are 0, and this would be set to 0, then this is instead set to 0xFFFFFFFFFFFFFFFFUL.</remarks>
        public ulong StateD
        {
            get => _d;
            set => _d = (StateA | StateB | StateC | value) == 0UL ? 0xFFFFFFFFFFFFFFFFUL : value;
        }


        /// <summary>
        /// Creates a new Xoshiro256StarStarRandom with a random state.
        /// </summary>
        public Xoshiro256StarStarRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
            StateD = MakeSeed();
        }


        /// <summary>
        /// Creates a new Xoshiro256StarStarRandom with the given seed; all ulong values are permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="SetSeed(Xoshiro256StarStarRandom, ulong)">SetSeed(Xoshiro256StarStarRandom, ulong)</see>, to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public Xoshiro256StarStarRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new Xoshiro256StarStarRandom with the given four states; all ulong values are permitted except for when all states are 0.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        /// <param name="stateD">Any ulong.</param>
        public Xoshiro256StarStarRandom(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
        }

        /// <summary>
        /// This generator has 4 ulong states, so this returns 4.
        /// </summary>
        public override int StateCount => 4;
        /// <summary>
        /// This supports <see cref="SelectState(int)"/>.
        /// </summary>
        public override bool SupportsReadAccess => true;
        /// <summary>
        /// This supports <see cref="SetSelectedState(int, ulong)"/>.
        /// </summary>
        public override bool SupportsWriteAccess => true;
        /// <summary>
        /// This does not support <see cref="IEnhancedRandom.Skip(ulong)"/>.
        /// </summary>
        public override bool SupportsSkip => false;
        /// <summary>
        /// This supports <see cref="IEnhancedRandom.Leap()"/>, with one call to Leap() equivalent to <code>Math.Pow(2, 192)</code> calls to <see cref="NextULong()"/>.
        /// </summary>
        public override bool SupportsLeap => true;
        /// <summary>
        /// This does not support <see cref="IEnhancedRandom.PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => false;

        /// <summary>
        /// Gets the state determined by selection, as-is.
        /// </summary>
        /// <remarks>The value for selection should be between 0 and 3, inclusive; if it is any other value this gets state D as if 3 was given.</remarks>
        /// <param name="selection">used to select which state variable to get; generally 0, 1, 2, or 3.</param>
        /// <returns>The value of the selected state.</returns>
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 0:
                    return StateA;
                case 1:
                    return StateB;
                case 2:
                    return StateC;
                default:
                    return StateD;
            }
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value, as-is.
        /// </summary>
        /// <remarks>
        /// Selections 0, 1, 2, and 3 refer to states A, B, C, and D,  and if the selection is anything else, this treats it as 3 and sets stateD.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0, 1, 2, or 3.</param>
        /// <param name="value">The exact value to use for the selected state, if valid.</param>
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 0:
                    StateA = value;
                    break;
                case 1:
                    StateB = value;
                    break;
                case 2:
                    StateC = value;
                    break;
                default:
                    StateD = value;
                    break;
            }
        }

        /// <summary>
        /// Initializes all 4 states of the generator to random values based on the given seed.
        /// </summary>
        /// <remarks>
        /// (2 to the 64) possible initial generator states can be produced here, all with a different first value returned
        /// by <see cref="NextULong()">NextUlong()</see> (because stateB is guaranteed to be different for every different seed).
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>

        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(Xoshiro256StarStarRandom rng, ulong seed)
        {
            unchecked
            {
                ulong x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateA = x ^ x >> 27;
                x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateB = x ^ x >> 27;
                x = (seed += 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateC = x ^ x >> 27;
                x = (seed + 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng._d = x ^ x >> 27;
            }
        }

        /// <summary>
        /// Sets the state completely to the given four state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as calling setStateA(ulong), setStateB(ulong), setStateC(ulong), and setStateD(ulong) as a group.
        /// You may want to call <see cref="NextULong()">NextUlong()</see> a few times after setting the states like this, unless
        /// the value for stateB (in particular) is already adequately random; the first call to NextULong(), if it is made immediately after calling this, will return stateB as-is.
        /// If all parameters are 0 here, this will assign 0xFFFFFFFFFFFFFFFFUL to stateD and 0 to the rest.
        /// </remarks>
        /// <param name="stateA">The first state; can be any long.</param>
        /// <param name="stateB">The second state; can be any long.</param>
        /// <param name="stateC">The third state; can be any long.</param>
        /// <param name="stateD">The fourth state; this will be returned as-is if the next call is to NextULong().</param>
        public override void SetState(ulong stateA, ulong stateB, ulong stateC, ulong stateD)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
            StateD = stateD;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            unchecked
            {
                ulong result = (StateB * 5UL).RotateLeft(7) * 9UL;
                ulong t = StateB << 17;
                StateC ^= StateA;
                _d ^= StateB;
                StateB ^= StateC;
                StateA ^= _d;
                StateC ^= t;
                _d = _d.RotateLeft(45);
                return result;
            }
        }

        /// <inheritdoc />
        public override float NextSparseFloat()
        {
            unchecked
            {
                ulong result = (StateB * 5UL).RotateLeft(7) * 9UL;
                ulong t = StateB << 17;
                StateC ^= StateA;
                _d ^= StateB;
                StateB ^= StateC;
                StateA ^= _d;
                StateC ^= t;
                _d = _d.RotateLeft(45);
                return BitConverter.Int32BitsToSingle((int)(result >> 41) | 0x3F800000) - 1f;
            }
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            unchecked
            {
                ulong result = (StateB * 5UL).RotateLeft(7) * 9UL;
                ulong t = StateB << 17;
                StateC ^= StateA;
                _d ^= StateB;
                StateB ^= StateC;
                StateA ^= _d;
                StateC ^= t;
                _d = _d.RotateLeft(45);
                return BitConverter.Int64BitsToDouble((long)(result >> 12) | 0x3FF0000000000000L) - 1.0;
            }
        }

        /// <summary>
        /// Jumps extremely far in the generator's sequence, such that it requires <code>Math.Pow(2, 64)</code> calls to Leap() to complete
        /// a cycle through the generator's entire sequence. This can be used to create over 18 quintillion substreams of this generator's
        /// sequence, each with a period of <code>Math.Pow(2, 192)</code>.
        /// </summary>
        /// <returns>The result of what NextULong() would return if it was called at the state this jumped to.</returns>
        public override ulong Leap()
        {
            ulong s0 = 0UL;
            ulong s1 = 0UL;
            ulong s2 = 0UL;
            ulong s3 = 0UL;
            for (ulong b = 0x76e15d3efefdcbbfUL; b != 0UL; b >>= 1)
            {
                if ((1UL & b) != 0UL)
                {
                    s0 ^= StateA;
                    s1 ^= StateB;
                    s2 ^= StateC;
                    s3 ^= StateD;
                }
                NextULong();
            }
            for (ulong b = 0xc5004e441c522fb3UL; b != 0UL; b >>= 1)
            {
                if ((1UL & b) != 0UL)
                {
                    s0 ^= StateA;
                    s1 ^= StateB;
                    s2 ^= StateC;
                    s3 ^= StateD;
                }
                NextULong();
            }
            for (ulong b = 0x77710069854ee241UL; b != 0UL; b >>= 1)
            {
                if ((1UL & b) != 0UL)
                {
                    s0 ^= StateA;
                    s1 ^= StateB;
                    s2 ^= StateC;
                    s3 ^= StateD;
                }
                NextULong();
            }
            for (ulong b = 0x39109bb02acbe635UL; b != 0UL; b >>= 1)
            {
                if ((1UL & b) != 0UL)
                {
                    s0 ^= StateA;
                    s1 ^= StateB;
                    s2 ^= StateC;
                    s3 ^= StateD;
                }
                NextULong();
            }

            StateA = s0;
            StateB = s1;
            StateC = s2;
            StateD = s3;
            return (s1 * 5UL).RotateLeft(7) * 9UL;
        }



        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new Xoshiro256StarStarRandom(StateA, StateB, StateC, StateD);
    }
}
