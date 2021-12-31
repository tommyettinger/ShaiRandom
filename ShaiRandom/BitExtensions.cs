namespace ShaiRandom
{
    /// <summary>
    /// Specific implementations of bitwise rotations that .NET can optimize well in some cases.
    /// </summary>
    public static class BitExtensions
    {
        /// <summary>
        /// Bitwise left-rotation of a ulong by amt, in bits.
        /// </summary>
        /// <param name="ul">The ulong to rotate left.</param>
        /// <param name="amt">How many bits to rotate.</param>
        /// <returns>The rotated ul.</returns>
        public static ulong RotateLeft(this ulong ul, int amt) => (ul << amt) | (ul >> 64 - amt);
        /// <summary>
        /// Bitwise left-rotation of a ulong by amt, in bits; assigns to ul in-place.
        /// </summary>
        /// <param name="ul">The ulong to rotate left; will be assigned to.</param>
        /// <param name="amt">How many bits to rotate.</param>
        public static void RotateLeftInPlace(ref this ulong ul, int amt) => ul = (ul << amt) | (ul >> 64 - amt);

        /// <summary>
        /// Bitwise right-rotation of a ulong by amt, in bits.
        /// </summary>
        /// <param name="ul">The ulong to rotate right.</param>
        /// <param name="amt">How many bits to rotate.</param>
        /// <returns>The rotated ul.</returns>
        public static ulong RotateRight(this ulong ul, int amt) => (ul >> amt) | (ul << 64 - amt);
        /// <summary>
        /// Bitwise right-rotation of a ulong by amt, in bits; assigns to ul in-place.
        /// </summary>
        /// <param name="ul">The ulong to rotate right; will be assigned to.</param>
        /// <param name="amt">How many bits to rotate.</param>
        public static void RotateRightInPlace(ref this ulong ul, int amt) => ul = (ul >> amt) | (ul << 64 - amt);
    }
}
