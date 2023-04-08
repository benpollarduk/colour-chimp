using System;
using System.Windows.Shapes;

namespace BP.ColourChimp.Classes.Sorting
{
    /// <summary>
    /// Provides a sorter for sorting rectangles randomly.
    /// </summary>
    public class RandomSorter : IRectangleSorter
    {
        #region Properties

        /// <summary>
        /// Get the randomiser.
        /// </summary>
        protected Random Random { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RandomSorter.
        /// </summary>
        /// <param name="random">The randomiser.</param>
        public RandomSorter(Random random)
        {
            Random = random;
        }

        #endregion

        #region Implementation of IRectangleSorter

        /// <summary>
        /// Sort the rectangles.
        /// </summary>
        /// <param name="a">Rectangle a.</param>
        /// <param name="b">Rectangle b.</param>
        /// <returns>1 if a is greater than b, -1 if it is less, 0 if they are equal.</returns>
        public int Sort(Rectangle a, Rectangle b)
        {
            return Random.Next(-1, 2);
        }

        #endregion
    }
}
