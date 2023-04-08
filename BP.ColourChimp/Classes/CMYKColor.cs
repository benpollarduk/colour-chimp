using System;
using System.Drawing;
using BP.ColourChimp.Extensions;

namespace BP.ColourChimp.Classes
{
    /// <summary>
    /// A color expressed as CMYK.
    /// </summary>
    public class CMYKColor : IEquatable<CMYKColor>
    {
        #region Properties

        /// <summary>
        /// Get or set cyan, as a normalised value.
        /// </summary>
        public double Cyan { get; set; }

        /// <summary>
        /// Get or set magenta, as a normalised value.
        /// </summary>
        public double Magenta { get; set; }

        /// <summary>
        /// Get or set yellow, as a normalised value.
        /// </summary>
        public double Yellow { get; set; }

        /// <summary>
        /// Get or set key, as a normalised value.
        /// </summary>
        public double Key { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the CMYKColor class.
        /// </summary>
        /// <param name="cyan">Cyan, as a normalised value.</param>
        /// <param name="magenta">Magenta, as a normalised value.</param>
        /// <param name="yellow">Yellow, as a normalised value.</param>
        /// <param name="key">Key, as a normalised value.</param>
        public CMYKColor(double cyan, double magenta, double yellow, double key)
        {
            Cyan = cyan;
            Magenta = magenta;
            Yellow = yellow;
            Key = key;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a string that represents the current object, as percentages.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPercentageString()
        {
            return $"{Cyan * 100} {Magenta * 100} {Yellow * 100} {Key * 100}";
        }

        /// <summary>
        /// Get this value as a System.Windows.Media.Color.
        /// </summary>
        /// <returns>The converted color.</returns>
        public Color ToColor()
        {
            var c = Cyan * (1d - Key) + Key;
            var m = Magenta * (1d - Key) + Key;
            var y = Yellow * (1d - Key) + Key;
            var r = (byte)((1d - c) * 255);
            var g = (byte)((1d - m) * 255);
            var b = (byte)((1d - y) * 255);

            return Color.FromArgb(255, r, g, b);
        }

        #endregion

        #region Overrides of Object

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="obj" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(CMYKColor obj)
        {
            if (obj == null)
                return false;

            return Cyan.AboutEqual(obj.Cyan) &&
                   Magenta.AboutEqual(obj.Magenta) &&
                   Yellow.AboutEqual(obj.Yellow) &&
                   Key.AboutEqual(obj.Key);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Cyan} {Magenta} {Yellow} {Key}";
        }

        #endregion
    }
}