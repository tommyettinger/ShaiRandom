// Copyright 2020 Mark A. Overton
// Copyright 2020 Bradley Grainger
// Copyright 2021 Tommy Ettinger
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Derived from https://github.com/bgrainger/RomuRandom , which is an
// implementation of https://romu-random.org/ .

namespace ShaiRandom.Generators
{
    /// <summary>
    /// It's an AbstractRandom with 3 states, more here later. Implements the RomuTrio algorithm for fast ulongs.
    /// TricycleRandom or FourWheelRandom may be about the same speed or faster.
    /// </summary>
    public /*sealed*/ class RomuTrioRandom : AbstractRandom
    {
        /// <summary>
        /// The identifying tag here is "RTrR" .
        /// </summary>
        public override string Tag => "RTrR";

        static RomuTrioRandom()
        {
            RegisterTag(new RomuTrioRandom(1UL, 1UL, 1UL));
        }
        /// <summary>
        /// The first state; can be any ulong except that the whole state must not all be 0.
        /// </summary>
        /// <remarks>
        /// If this has just been set to some value, then the next call to <see cref="NextULong">NextULong</see> will return that value as-is. Later calls will be more random.</remarks>
        public ulong StateA { get; set; }

        private ulong _b;
        /// <summary>
        /// The second state; can be any ulong except that the whole state must not all be 0.
        /// </summary>
        public ulong StateB
        {
            get => _b;
            set => _b = value;
        }


        private ulong _c;
        /// <summary>
        /// The third state; can be any ulong except that the whole state must not all be 0.
        /// </summary>
        /// <remarks>If all other states are 0, and this would be set to 0, then this is instead set to 0xFFFFFFFFFFFFFFFFUL.</remarks>
        public ulong StateC
        {
            get => _c;
            set => _c = (StateA | StateB | value) == 0UL ? 0xFFFFFFFFFFFFFFFFUL : value;
        }

        /// <summary>
        /// Creates a new RomuTrioRandom with a random state.
        /// </summary>
        public RomuTrioRandom()
        {
            StateA = MakeSeed();
            StateB = MakeSeed();
            StateC = MakeSeed();
        }

        /// <summary>
        /// Creates a new RomuTrioRandom with the given seed; any ulong value is permitted.
        /// </summary>
        /// <remarks>
        /// The seed will be passed to <see cref="Seed(ulong)">Seed(ulong)</see> to attempt to adequately distribute the seed randomly.
        /// </remarks>
        /// <param name="seed">Any ulong.</param>
        public RomuTrioRandom(ulong seed)
        {
            SetSeed(this, seed);
        }

        /// <summary>
        /// Creates a new RomuTrioRandom with the given three states; all ulong values are permitted except for all 0s.
        /// </summary>
        /// <remarks>
        /// The states will be used verbatim unless all states are 0, in which case stateC is considered <see cref="ulong.MaxValue">ulong.MaxValue</see>.
        /// </remarks>
        /// <param name="stateA">Any ulong.</param>
        /// <param name="stateB">Any ulong.</param>
        /// <param name="stateC">Any ulong.</param>
        public RomuTrioRandom(ulong stateA, ulong stateB, ulong stateC)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
        }

        /// <summary>
        /// This generator has 3 ulong states, so this returns 3.
        /// </summary>
        public override int StateCount => 3;
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
        /// This does not support <see cref="IEnhancedRandom.PreviousULong()"/>.
        /// </summary>
        public override bool SupportsPrevious => false;

        /// <summary>
        /// Gets the state determined by selection, as-is.
        /// </summary>
        /// <remarks>The value for selection should be between 0 and 2, inclusive; if it is any other value this gets state C as if 2 was given.</remarks>
        /// <param name="selection">used to select which state variable to get; generally 0, 1, or 2.</param>
        /// <returns>The value of the selected state.</returns>
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 0:
                    return StateA;
                case 1:
                    return StateB;
                default:
                    return StateC;
            }
        }

        /// <summary>
        /// Sets one of the states, determined by selection, to value, as-is.
        /// </summary>
        /// <remarks>
        /// Selections 0, 1, and 2 refer to states A, B, and C,  and if the selection is anything else, this treats it as 2 and sets stateC.
        /// </remarks>
        /// <param name="selection">Used to select which state variable to set; generally 0, 1, or 2.</param>
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
                default:
                    StateC = value;
                    break;
            }
        }

        /// <summary>
        /// This initializes all states of the generator to different pseudo-random values based on the given seed.
        /// </summary>
        /// <remarks>
        /// (2 to the 64) possible initial generator states can be produced here, all with a different
        /// first value returned by <see cref="NextULong()">NextULong()</see> (because stateA is guaranteed to be
        /// different for every different seed).
        /// </remarks>
        /// <param name="seed">The initial seed; may be any ulong.</param>
        public override void Seed(ulong seed) => SetSeed(this, seed);

        private static void SetSeed(RomuTrioRandom rng, ulong seed)
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
                x = (seed + 0x9E3779B97F4A7C15UL);
                x ^= x >> 27;
                x *= 0x3C79AC492BA7B653UL;
                x ^= x >> 33;
                x *= 0x1C69B3F74AC4AE35UL;
                rng.StateC = x ^ x >> 27;
            }
        }

        /// <summary>
        /// Sets the state completely to the given three state variables.
        /// </summary>
        /// <remarks>
        /// This is the same as setting StateA, setStateB, and StateC as a group.
        /// </remarks>
        /// <param name="stateA">The first state; can be any ulong.</param>
        /// <param name="stateB">The second state; can be any ulong.</param>
        /// <param name="stateC">The third state; can be any ulong</param>
        public override void SetState(ulong stateA, ulong stateB, ulong stateC)
        {
            StateA = stateA;
            StateB = stateB;
            StateC = stateC;
        }

        /// <inheritdoc />
        public override ulong NextULong()
        {
            unchecked
            {
                ulong fa = StateA;
                StateA = 15241094284759029579UL * _c;
                _c -= _b;
                _b -= fa;
                _b.RotateLeftInPlace(12);
                _c.RotateLeftInPlace(44);
                return fa;
            }
        }

        /// <inheritdoc />
        public override IEnhancedRandom Copy() => new RomuTrioRandom(StateA, StateB, StateC);
    }
}
