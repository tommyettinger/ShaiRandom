using System;

namespace ShaiRandom
{
    /// <summary>
    /// Static methods that each take a number and scramble or mix it to get a different number, often with a one-to-one relationship between inputs and outputs.
    /// </summary>
    public static class Mixers
    {
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

    }
}
