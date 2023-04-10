using System.Windows.Media;
using System.Windows.Shapes;

namespace BP.ColourChimp.Classes.Sorting
{
    /// <summary>
    /// Provides a sorter for sorting rectangles by ARGB.
    /// </summary>
    public class ARGBSorter : IRectangleSorter
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

            if (colorA.R > colorB.R)
                return -1;

            if (colorA.R < colorB.R)
                return 1;

            if (colorA.G > colorB.G)
                return -1;

            if (colorA.G < colorB.G)
                return 1;

            if (colorA.B > colorB.B)
                return -1;

            if (colorA.B < colorB.B)
                return 1;

            if (colorA.A > colorB.A)
                return -1;

            if (colorA.A < colorB.A)
                return 1;

            return 0;
        }

        #endregion
    }
}
