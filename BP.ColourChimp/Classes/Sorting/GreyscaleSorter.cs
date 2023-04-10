using System.Windows.Media;
using System.Windows.Shapes;

namespace BP.ColourChimp.Classes.Sorting
{
    /// <summary>
    /// Provides a sorter for sorting rectangles by greyscale.
    /// </summary>
    public class GreyscaleSorter : IRectangleSorter
    {
        #region Implementation of IRectangleSorter

        /// <summary>
        /// Sort the rectangles.
        /// </summary>
        /// <param name="a">Rectangle a.</param>
        /// <param name="b">Rectangle b.</param>
        /// <returns>1 if a is greater than b, -1 if it is less, 0 if they are equal.</returns>
        public int Sort(Rectangle a, Rectangle b)
        {
            if (!(a.Fill is SolidColorBrush aBrush))
                return 0;

            if (!(b.Fill is SolidColorBrush bBrush))
                return 0;

            var colorA = aBrush.Color;
            var colorB = bBrush.Color;
            var aAve = (byte)((colorA.R + colorA.G + colorA.B) / 3d / 255d * colorA.A);
            var bAve = (byte)((colorB.R + colorB.G + colorB.B) / 3d / 255d * colorB.A);

            if (aAve > bAve)
                return 1;
            if (aAve < bAve)
                return -1;

            return 0;
        }

        #endregion
    }
}
