using System;
using System.Windows.Media;
using BP.ColourChimp.Classes;

namespace BP.ColourChimp.Extensions
{
    /// <summary>
    /// Provides extension functions System.Windows.Media.Color.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Convert to CMYK.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The CMYK color.</returns>
        public static CMYKColor ToCMYK(this Color color)
        {
            if (color.R == 255 && color.G == 255 && color.B == 255)
                return new CMYKColor(0, 0, 0, 0);

            if (color.R == 0 && color.G == 0 && color.B == 0)
                return new CMYKColor(1, 1, 1, 1);

            var normalisedR = color.R / 255d;
            var normalisedG = color.G / 255d;
            var normalisedB = color.B / 255d;
            var normalisedKey = Math.Min(1d - normalisedR, Math.Min(1d - normalisedG, 1d - normalisedB));
            var cyan = (1d - normalisedR - normalisedKey) / (1d - normalisedKey);
            var magenta = (1d - normalisedG - normalisedKey) / (1d - normalisedKey);
            var yellow = (1d - normalisedB - normalisedKey) / (1d - normalisedKey);
            return new CMYKColor(Math.Round(cyan, 4), Math.Round(magenta, 4), Math.Round(yellow, 4), Math.Round(normalisedKey, 4));
        }

        /// <summary>
        /// Convert to HSV.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The HSV color.</returns>
        public static HSVColor ToHSV(this Color color)
        {
            var min = (double)Math.Min(color.R, Math.Min(color.G, color.B));
            var max = (double)Math.Max(color.R, Math.Max(color.G, color.B));
            var delta = max - min;
            double h, s;
            var v = max;

            if (!max.AboutEqual(0))
            {
                s = delta / max;

                if (max.AboutEqual(color.R))
                    h = (color.G - color.B) / delta;
                else if (max.AboutEqual(color.G))
                    h = 2 + (color.B - color.R) / delta;
                else
                    h = 4 + (color.R - color.G) / delta;

                h *= 60;

                if (h < 0)
                    h += 360;
            }
            else
            {
                s = 0;
                h = 0;
            }

            return new HSVColor(Math.Round(h / 360f, 4), Math.Round(s, 4), Math.Round(v / 255f, 4));
        }

        /// <summary>
        /// Convert this color to a negative version of itself.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>The negative version.</returns>
        public static Color ToNegative(this Color color)
        {
            var r = (byte)(255 - color.R);
            var g = (byte)(255 - color.G);
            var b = (byte)(255 - color.B);
            return Color.FromArgb(color.A, r, g, b);
        }
    }
}
