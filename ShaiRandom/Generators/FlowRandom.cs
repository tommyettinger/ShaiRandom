using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ShaiRandom.Generators
{
    //TODO: class docs

    /// <summary>
    /// It's an AbstractRandom with 2 states, more here later. This one supports <see cref="Skip(ulong)"/>.
    /// Both states allow all ulong values.
    /// </summary>
    public sealed class FlowRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "FloR" .
        /// </summary>
        public override string DefaultTag => "FloR";

        /// <summary>
        /// The first state; can be any ulong.
        /// </summary>
        public ulong StateA { get; set; }

        /// <summary>
        /// The second state; can be any ulong.
        /// </summary>
        public ulong StateB { get; set; }

        /// <summary>
        /// Creates a new FlowRandom with a random state.
        /// </summary>
        public FlowRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
        }

        /// <summary>
        /// Creates a new FlowRandom with the given seed; any ulong value is permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="Seed(ulong)">Seed(ulong)</see> to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public FlowRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new FlowRandom with the given two states; all ulong values are permitted, but stateB will always be made odd.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim, except if stateB is even (then 1 is added to it).
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong; if even, 1 will be added.</param>
        public FlowRandom(ulong stateA, ulong stateB)
        {
            StateA = stateA;
            StateB = stateB;
        }

        /// <summary>
        /// This generator has 2 ulong states, so this returns 2.
        /// </summary>
        public override int StateCount => 2;
        /// <summary>
        /// This supports <see cref="SelectState(int)"/>.
        /// </summary>
        public override bool SupportsReadAccess => true;
        /// <summary>
        /// This supports <see cref="SetSelectedState(int, ulong)"/>.
        /// </summary>
        public override bool SupportsWriteAccess => true;
        /// <summary>
        /// This supports <see cref="IEnhancedRandom.Skip(ulong)"/>.
        /// </summary>
        public override bool SupportsSkip => true;
        /// <summary>
        /// This does not support <see cref="IEnhancedRandom.Leap()"/>.
        /// </summary>
        public override bool SupportsLeap => false;
        /// <summary>
        /// This supports <see cref="PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => true;

        /// <summary>
        /// Gets the state determined by selection, as-is.
        /// </summary>
        /// <remarks>The value for selection should be 0 or 1; if it is any other value this gets state A as if 0 was given.</remarks>
        /// <param name="selection">used to select which state variable to get; generally 0 or 1.</param>
        /// <returns>The value of the selected state.</returns>
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 1:
                    return StateB;
                default:
                    return StateA;
            }
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value, as-is.
        /// </summary>
        /// <remarks>
        /// Selections 0 and 1 refer to states A and B, and if the selection is anything else, this treats it as 0 and sets stateA.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0 or 1.</param>
        /// <param name="value">The exact value to use for the selected state, if valid.</param>
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 1:
                    StateB = value;
                    break;
                default:
                    StateA = value;
                    break;
            }
        }

        /// <summary>
        /// This initializes all states of the generator to different pseudo-random values based on the given seed.
        /// </summary>
        /// <remarks>
        /// (2 to the 64) possible initial generator states can be produced here.
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(FlowRandom rng, ulong seed)
        {
            unchecked
            {
                rng.StateA = seed;
                rng.StateB = ~seed;
            }
        }

        /// <summary>
        /// Sets the state completely to the given two state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as setting StateA and StateB as a group.
        /// You may want to call <see cref="NextULong()">NextUlong()</see> a few times after setting the states like this, unless
        /// the states have very little or no correlation, because certain very-similar combinations of seeds produce correlated sequences.
        /// </remarks>
        /// <param name="stateA">The first state; can be any ulong.</param>
        /// <param name="stateB">The second state; can be any odd ulong.</param>
        public override void SetState(ulong stateA, ulong stateB)
        {
            StateA = stateA;
            StateB = stateB;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong NextULong()
        {
            unchecked
            {
                ulong x = (StateA += 0xD1B54A32D192ED03UL);
                ulong y = (StateB += 0x8CB92BA72F3D8DD7UL);
                x = (x ^ BitOperations.RotateLeft(y, 37)) * 0x3C79AC492BA7B653UL;
                x = (x ^ x >> 33) * 0x1C69B3F74AC4AE35UL;
                return x ^ x >> 27;
            }
        }

        ///// <inheritdoc/>
        //public override uint NextUInt()
        //{
        //    unchecked
        //    {
        //        //ulong z = (StateA += 0xC6BC279692B5C323UL);
        //        //z ^= z >> 31;
        //        //z *= (_b += 0x9E3779B97F4A7C16UL);
        //        ulong s = (StateA += 0xC6BC279692B5C323UL);
        //        ulong z = (s ^ s >> 31) * (_b += 0x9E3779B97F4A7C16UL);
        //        return (uint)(z ^ z >> 26 ^ z >> 6);
        //    }

        //}

        /// <inheritdoc />
        public override ulong Skip(ulong distance)
        {
            unchecked
            {
                ulong x = (StateA += 0xD1B54A32D192ED03UL * distance);
                ulong y = (StateB += 0x8CB92BA72F3D8DD7UL * distance);
                x = (x ^ BitOperations.RotateLeft(y, 37)) * 0x3C79AC492BA7B653UL;
                x = (x ^ x >> 33) * 0x1C69B3F74AC4AE35UL;
                return x ^ x >> 27;
            }
        }

        /// <inheritdoc />
        public override ulong PreviousULong()
        {
            unchecked
            {
                ulong x = StateA;
                ulong y = StateB;
                StateA -= 0xD1B54A32D192ED03UL;
                StateB -= 0x8CB92BA72F3D8DD7UL;
                x = (x ^ BitOperations.RotateLeft(y, 37)) * 0x3C79AC492BA7B653UL;
                x = (x ^ x >> 33) * 0x1C69B3F74AC4AE35UL;
                return x ^ x >> 27;
            }
        }

        /// <inheritdoc />
        public override float NextSparseFloat()
        {
            unchecked
            {
                ulong x = (StateA += 0xD1B54A32D192ED03UL);
                ulong y = (StateB += 0x8CB92BA72F3D8DD7UL);
                x = (x ^ BitOperations.RotateLeft(y, 37)) * 0x3C79AC492BA7B653UL;
                x = (x ^ x >> 33) * 0x1C69B3F74AC4AE35UL;
                return BitConverter.Int32BitsToSingle((int)(x >> 41) | 0x3F800000) - 1f;
            }
        }

        /// <inheritdoc />
        public override double NextSparseDouble()
        {
            unchecked
            {
                ulong x = (StateA += 0xD1B54A32D192ED03UL);
                ulong y = (StateB += 0x8CB92BA72F3D8DD7UL);
                x = (x ^ BitOperations.RotateLeft(y, 37)) * 0x3C79AC492BA7B653UL;
                x = (x ^ x >> 33) * 0x1C69B3F74AC4AE35UL;
                return BitConverter.Int64BitsToDouble((long)(x >> 12 ^ x >> 37) | 0x3FF0000000000000L) - 1.0;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new FlowRandom(StateA, StateB);

        /// <summary>
        /// Gets an ulong that identifies which of the 2 to the 64 possible streams this is on.
        /// This takes constant time.
        /// </summary>
        /// <returns>
        /// An ulong that identifies which stream the main state of the generator is on.
        /// </returns>
        public ulong GetStream()
        {
            unchecked
            {
                return StateB * 0xC83D0A80F9B4B5E7UL - StateA * 0x06106CCFA448E5ABUL;
            }
        }

        /// <summary>
        /// Changes the generator's stream to any of the 2 to the 64 possible streams this can be on.
        /// </summary>
        /// <remarks>
        /// The <c>stream</c> this takes uses the same numbering convention used by GetStream() and ShiftStream().
        /// This makes an absolute change to the stream, while ShiftStream() is relative.
        /// This takes constant time.
        /// </remarks>
        /// <param name="stream">The identifying number of the stream to change to; may be any ulong.</param>
        public void SetStream(ulong stream)
        {
            unchecked
            {
                StateB += 0x8CB92BA72F3D8DD7L *
                          (stream - (StateB * 0xC83D0A80F9B4B5E7L - StateA * 0x06106CCFA448E5ABL));
            }
        }

        /// <summary>
        /// Adjusts the generator's stream "up" or "down" to any of the 2 to the 64 possible streams this can be on.
        /// </summary>
        /// <remarks>
        /// The <c>difference</c> this takes will be the difference between the result of GetStream() before the
        /// shift, and after the shift. This makes a relative change to the stream, while SetStream() is absolute.
        /// This takes constant time.
        /// </remarks>
        /// <param name="difference">How much to change the stream by; may be any ulong.</param>
        public void ShiftStream(ulong difference)
        {
            unchecked
            {
                StateB += 0x8CB92BA72F3D8DD7UL * difference;
            }
        }
    }
}
