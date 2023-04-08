using System.Windows.Media;
using System.Windows.Shapes;
using BP.ColourChimp.Extensions;

namespace BP.ColourChimp.Classes.Sorting
{
    /// <summary>
    /// Provides a sorter for sorting rectangles by CMYK.
    /// </summary>
    public class CYMKSorter : IRectangleSorter
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

            var colorA = aBrush.Color.ToCMYK();
            var colorB = bBrush.Color.ToCMYK();

            if (colorA.Cyan > colorB.Cyan)
                return -1;

            if (colorA.Cyan < colorB.Cyan)
                return 1;

            if (colorA.Magenta > colorB.Magenta)
                return -1;

            if (colorA.Magenta < colorB.Magenta)
                return 1;

            if (colorA.Yellow > colorB.Yellow)
                return -1;

            if (colorA.Yellow < colorB.Yellow)
                return 1;

            if (colorA.Key > colorB.Key)
                return -1;

            if (colorA.Key < colorB.Key)
                return 1;

            return 0;
        }

        #endregion
    }
}
