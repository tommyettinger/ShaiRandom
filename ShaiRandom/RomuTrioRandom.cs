﻿// Copyright 2020 Mark A. Overton
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

using System;
using System.Collections.Generic;

namespace ShaiRandom
{
    /// <summary>
    /// It's an ARandom with 3 states, more here later. Implements the RomuTrio algorithm for fast ulongs.
    /// TricycleRandom or FourWheelRandom may be about the same speed or faster.
    /// </summary>
    [Serializable]
    public class RomuTrioRandom : ARandom, IEquatable<RomuTrioRandom?>
    {
        static RomuTrioRandom()
        {
            RegisterTag("RTrR", new RomuTrioRandom(1UL, 1UL, 1UL));
        }
        /**
         * The first state; can be any long except that the whole state must not all be 0.
         * If this has just been set to some value, then the next call to
         * {@link #nextUlong()} will return that value as-is. Later calls will be more random.
         */
        public ulong stateA { get; set; }

        private ulong _b;
        /**
         * The second state; can be any long except that the whole state must not all be 0.
         */
        public ulong stateB
        {
            get => _b;
            set => _b = value;
        }


        private ulong _c;
        /**
         * The third state; can be any long except that the whole state must not all be 0.
         * If all other states are 0, and this would be set to 0,
         * then this is instead set to 0xFFFFFFFFFFFFFFFFUL.
         */
        public ulong stateC
        {
            get => _c;
            set => _c = (stateA | stateB | value) == 0UL ? 0xFFFFFFFFFFFFFFFFUL : value;
        }

        /**
         * Creates a new RomuTrioRandom with a random state.
         */
        public RomuTrioRandom()
        {
            stateA = MakeSeed();
            stateB = MakeSeed();
            stateC = MakeSeed();
        }

        /**
         * Creates a new RomuTrioRandom with the given seed; all {@code long} values are permitted.
         * The seed will be passed to {@link #Seed(long)} to attempt to adequately distribute the seed randomly.
         * @param seed any {@code long} value
         */
        public RomuTrioRandom(ulong seed)
        {
            Seed(seed);
        }

        /**
         * Creates a new RomuTrioRandom with the given three states; all {@code long} values are permitted.
         * These states will be used verbatim.
         * @param stateA any {@code long} value
         * @param stateB any {@code long} value
         * @param stateC any {@code long} value
         */
        public RomuTrioRandom(ulong stateA, ulong stateB, ulong stateC)
        {
            this.stateA = stateA;
            this.stateB = stateB;
            this.stateC = stateC;
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
        /// This does not support <see cref="IRandom.Skip(ulong)"/>.
        /// </summary>
        public override bool SupportsSkip => false;
        /// <summary>
        /// This does not support <see cref="IRandom.PreviousUlong()"/>.
        /// </summary>
        public override bool SupportsPrevious => false;
        /**
         * Gets the state determined by {@code selection}, as-is. The value for selection should be
         * between 0 and 2, inclusive; if it is any other value this gets state C as if 2 was given.
         * @param selection used to select which state variable to get; generally 0, 1, or 2
         * @return the value of the selected state
         */
        public override ulong SelectState(int selection)
        {
            switch (selection)
            {
                case 0:
                    return stateA;
                case 1:
                    return stateB;
                default:
                    return stateC;
            }
        }

        /**
         * Sets one of the states, determined by {@code selection}, to {@code value}, as-is.
         * Selections 0, 1, and 2 refer to states A, B, and C, and if the selection is anything
         * else, this treats it as 2 and sets stateC.
         * @param selection used to select which state variable to set; generally 0, 1, or 2
         * @param value the exact value to use for the selected state, if valid
         */
        public override void SetSelectedState(int selection, ulong value)
        {
            switch (selection)
            {
                case 0:
                    stateA = value;
                    break;
                case 1:
                    stateB = value;
                    break;
                default:
                    stateC = value;
                    break;
            }
        }

        /**
         * This initializes all 3 states of the generator to different random values based on the given seed.
         * (2 to the 64) possible initial generator states can be produced here, all with a different
         * first value returned by {@link #nextLong()} (because {@code stateA} is guaranteed to be
         * different for every different {@code seed}).
         * @param seed the initial seed; may be any long
         */
        public override void Seed(ulong seed)
        {
            ulong x = (seed += 0x9E3779B97F4A7C15UL);
            x ^= x >> 27;
            x *= 0x3C79AC492BA7B653L;
            x ^= x >> 33;
            x *= 0x1C69B3F74AC4AE35L;
            stateA = x ^ x >> 27;
            x = (seed += 0x9E3779B97F4A7C15L);
            x ^= x >> 27;
            x *= 0x3C79AC492BA7B653L;
            x ^= x >> 33;
            x *= 0x1C69B3F74AC4AE35L;
            stateB = x ^ x >> 27;
            x = (seed + 0x9E3779B97F4A7C15L);
            x ^= x >> 27;
            x *= 0x3C79AC492BA7B653L;
            x ^= x >> 33;
            x *= 0x1C69B3F74AC4AE35L;
            stateC = x ^ x >> 27;
        }

        /**
         * Sets the state completely to the given three state variables.
         * This is the same as calling {@link #setStateA(long)}, {@link #setStateB(long)},
         * and {@link #setStateC(long)} as a group. You may want
         * to call {@link #nextLong()} a few times after setting the states like this, unless
         * the value for stateA (in particular) is already adequately random; the first call
         * to {@link #nextLong()}, if it is made immediately after calling this, will return {@code stateA} as-is.
         * @param stateA the first state; this will be returned as-is if the next call is to {@link #nextLong()}
         * @param stateB the second state; can be any long
         * @param stateC the third state; can be any long
         */
        public override void SetState(ulong stateA, ulong stateB, ulong stateC)
        {
            this.stateA = stateA;
            this.stateB = stateB;
            this.stateC = stateC;
        }

        public override ulong NextUlong()
        {
            ulong fa = stateA;
            stateA = 15241094284759029579u * _c;
            _c -= stateB;
            stateB -= fa;
            _b.RotateLeftInPlace(12);
            _c.RotateLeftInPlace(44);
            return fa;
        }

        public override IRandom Copy() => new RomuTrioRandom(stateA, stateB, stateC);
        public override string StringSerialize() => $"#RTrR`{stateA:X}~{stateB:X}~{stateC:X}`";
        public override IRandom StringDeserialize(string data)
        {
            int idx = data.IndexOf('`');
            stateA = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateB = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (idx = data.IndexOf('~', idx + 1))), 16);
            stateC = Convert.ToUInt64(data.Substring(idx + 1, -1 - idx + (      data.IndexOf('`', idx + 1))), 16);
            return this;
        }

        public override bool Equals(object? obj) => Equals(obj as RomuTrioRandom);
        public bool Equals(RomuTrioRandom? other) => other != null && stateA == other.stateA && stateB == other.stateB && stateC == other.stateC;
        public override int GetHashCode() => HashCode.Combine(stateA, stateB, stateC);

        public static bool operator ==(RomuTrioRandom? left, RomuTrioRandom? right) => EqualityComparer<RomuTrioRandom>.Default.Equals(left, right);
        public static bool operator !=(RomuTrioRandom? left, RomuTrioRandom? right) => !(left == right);
    }
}
