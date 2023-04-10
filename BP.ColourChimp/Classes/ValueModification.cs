namespace BP.ColourChimp.Classes
{
    /// <summary>
    /// Provides helper functions for modiyfing values.
    /// </summary>
    public static class ValueModification
    {
        /// <summary>
        /// Increment a byte by 16.
        /// </summary>
        /// <param name="b">The initial value.</param>
        /// <returns>The incremented byte.</returns>
        public static byte IncrementByteBy16(byte b)
        {
            if (b < 239)
                b += 16;
            else
                b = 255;

            return b;
        }

        /// <summary>
        /// Decrement a byte by 16.
        /// </summary>
        /// <param name="b">The initial value.</param>
        /// <returns>The decremented byte.</returns>
        public static byte DecrementByteBy16(byte b)
        {
            if (b > 15)
                b -= 16;
            else
                b = 0;

            return b;
        }

        /// <summary>
        /// Increment a double by 10.
        /// </summary>
        /// <param name="d">The initial value.</param>
        /// <returns>The incremented double.</returns>
        public static double IncrementDoubleBy10(double d)
        {
            if (d < 90)
                d += 10;
            else
                d = 100;

            return d;
        }

        /// <summary>
        /// Decrement a double by 10.
        /// </summary>
        /// <param name="d">The initial value.</param>
        /// <returns>The decremented double.</returns>
        public static double DecrementDoubleBy10(double d)
        {
            if (d > 9)
                d -= 10;
            else
                d = 0;

            return d;
        }
    }
}
