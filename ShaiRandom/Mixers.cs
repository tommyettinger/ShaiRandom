namespace ShaiRandom
{
    /// <summary>
    /// Static methods that each take a number and scramble or mix it to get a different number, often with a one-to-one relationship between inputs and outputs.
    /// </summary>
    public static class Mixers
    {
        /// <summary>
        /// A fast, medium-quality mixing method that takes its state as a parameter; state is expected to change between calls to this.
        /// </summary>
        /// <remarks>
        /// It is suggested that you use <code>Mixers.MixFast(++state)</code> to produce a sequence of different numbers, but any increments are allowed
        /// (even-number increments are discouraged because they won't be able to produce all outputs, but sometimes their quality will be decent for the numbers they
        /// can produce). All longs are accepted by this method, and all longs can be produced. Passing 0 here does not cause this to produce 0.
        /// <br />
        /// This is a relatively simple unary hash function; it multiplies its input by a large odd-number constant, runs an XLCG step on the result (XORing with one constant,
        /// then multiplying by another), then xorshifts, multiplies, and xorshifts again before returning. The function is bijective, but not especially strong when the inputs
        /// don't change by an odd number.
        /// </remarks>
        /// <param name="state">Any ulong; subsequent calls should change by an odd number, such as with <code>Mixers.MixFast(++state)</code>.</param>
        /// <returns>Any ulong.</returns>
        public static ulong MixFast(ulong state)
        {
            return (state = ((state = (((state * 0x632BE59BD9B4E019UL) ^ 0x9E3779B97F4A7C15UL) * 0xC6BC279692B5CC83UL)) ^ state >> 27) * 0xAEF17502108EF2D9UL) ^ state >> 25;
        }

        /// <summary>
        /// A high-quality mixing method that takes its state as a parameter; state is expected to change between calls to this.
        /// </summary>
        /// <remarks>It is suggested that you use <code>Mixers.MixStrong(++state)</code> to produce a sequence of different numbers, but any increments are allowed
        /// (even-number increments won't be able to produce all outputs, but their quality will be fine for the numbers they
        /// can produce). All longs are accepted by this method, and all longs can be produced. Passing 0 here does not cause this to produce 0.
        /// <br/>
        /// This uses Pelle Evensen's <a href="https://mostlymangling.blogspot.com/2020/01/nasam-not-another-strange-acronym-mixer.html">xNASAM</a>.
        /// It has excellent qualities regardless of patterns in input, though issues could be detected by running specific tests on many petabytes (or more) of generated numbers.
        /// </remarks>
        /// <param name="state">Any ulong; subsequent calls should change by an odd number, such as with <code>Mixers.MixStrong(++state)</code>.</param>
        /// <returns>Any ulong.</returns>
        public static ulong MixStrong(ulong state)
        {
            state ^= 0xD1B54A32D192ED03UL;
            return (state = ((state = (state ^ state.RotateLeft(39) ^ state.RotateLeft(17)) * 0x9E6C63D0676A9A99L) ^ state >> 23 ^ state >> 51) * 0x9E6D62D06F6A9A9BUL) ^ state >> 23 ^ state >> 51;
        }

        /// <summary>
        /// A very-high-quality mixing method that takes its state as a parameter; state is expected to change between calls to this.
        /// </summary>
        /// <remarks>
        /// It is suggested that you use <code>Mixers.MixMX(++state)</code> to produce a sequence of different numbers. You can instead use any odd increment,
        /// but smaller ones are preferred because there is probably a very large constant that is close to the modular multiplicative inverse of the one constant
        /// multiplier this uses, and using that constant as an increment might cause problems.
        /// </remarks>
        /// <br/>
        /// This uses Jon Kagstrom's <a href="http://jonkagstrom.com/mx3/mx3_rev2.html">Revised MX3 Mixer</a>, which seems exceedingly robust even after long runs of
        /// the brutally-difficult remortality test.
        /// <param name="state">Any ulong; subsequent calls should change by an odd number, such as with <code>Mixers.MixMX(++state)</code>.</param>
        /// <returns>Any ulong.</returns>
        public static ulong MixMX(ulong state)
        {
            state ^= state >> 32;
            state *= 0xBEA225F9EB34556DUL;
            state ^= state >> 29;
            state *= 0xBEA225F9EB34556DUL;
            state ^= state >> 32;
            state *= 0xBEA225F9EB34556DUL;
            state ^= state >> 29;
            return state;
        }
    }
}
