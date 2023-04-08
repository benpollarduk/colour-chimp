using System.Windows.Media;
using System.Windows.Shapes;
using BP.ColourChimp.Extensions;

namespace BP.ColourChimp.Classes.Sorting
{
    /// <summary>
    /// Provides a sorter for sorting rectangles by HSV.
    /// </summary>
    public class HSVSorter : IRectangleSorter
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

            var colorAHSV = colorA.ToHSV();
            var colorBHSV = colorB.ToHSV();

            if (colorAHSV.Hue > colorBHSV.Hue)
                return 1;

            if (colorAHSV.Hue < colorBHSV.Hue)
                return -1;

            if (colorAHSV.Saturation > colorBHSV.Saturation)
                return 1;

            if (colorAHSV.Saturation < colorBHSV.Saturation)
                return -1;

            if (colorAHSV.Value > colorBHSV.Value)
                return 1;

            if (colorAHSV.Value < colorBHSV.Value)
                return -1;

            return 0;
        }

        #endregion
    }
}
