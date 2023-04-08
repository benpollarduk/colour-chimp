using System.Drawing;

namespace BP.ColourChimp.Classes
{
    /// <summary>
    /// Delegate for updating pixels.
    /// </summary>
    /// <param name="bmp">The bitmap to pass in the callback.</param>
    /// <returns>True if the pixel was updated, else false.</returns>
    public delegate bool UpdatePixelCallback(Bitmap bmp);
}