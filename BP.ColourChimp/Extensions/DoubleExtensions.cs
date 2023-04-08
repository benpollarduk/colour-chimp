using System;

namespace BP.ColourChimp.Extensions
{
    /// <summary>
    /// Provides extension functions for System.Double.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Determine if two values are roughly equal.
        /// </summary>
        /// <param name="value1">Value 1.</param>
        /// <param name="value2">Value 2.</param>
        /// <returns>True if the values are roughly equal, else false.</returns>
        public static bool AboutEqual(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < double.Epsilon;
        }
    }
}
