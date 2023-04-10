using System.Windows.Shapes;

namespace BP.ColourChimp.Classes.Sorting
{
    /// <summary>
    /// Represents any object that can sort rectangles.
    /// </summary>
    public interface IRectangleSorter
    {
        /// <summary>
        /// Sort the rectangles.
        /// </summary>
        /// <param name="a">Rectangle a.</param>
        /// <param name="b">Rectangle b.</param>
        /// <returns>1 if a is greater than b, -1 if it is less, 0 if they are equal.</returns>
        int Sort(Rectangle a, Rectangle b);
    }
}
