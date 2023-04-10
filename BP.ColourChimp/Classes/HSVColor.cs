using System;
using System.Windows.Media;
using BP.ColourChimp.Extensions;

namespace BP.ColourChimp.Classes
{
    /// <summary>
    /// Represents a HSV (Hue, Saturation Value) color
    /// </summary>
    public class HSVColor : IEquatable<HSVColor>
    {
        #region Properties

        /// <summary>
        /// Get or set the hue, as a normalised value.
        /// </summary>
        public double Hue { get; set; }

        /// <summary>
        /// Get the hue, specified as degrees.
        /// </summary>
        public double HueDegrees => 360 * Hue;

        /// <summary>
        /// Get or set the saturation, as a normalised value.
        /// </summary>
        public double Saturation { get; set; }

        /// <summary>
        /// Get or set the value, as a normalised value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Get the value as a byte.
        /// </summary>
        public byte ValueAsByte => (byte)Math.Round(255 * Value, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the HSVColor class.
        /// </summary>
        /// <param name="hue">Hue, as a normalised value.</param>
        /// <param name="saturation">Saturation, as a normalised value.</param>
        /// <param name="value">Value, as a normalised value.</param>
        public HSVColor(double hue, double saturation, double value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a string that represents the current object, as percentages.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToPercentageString()
        {
            return $"{Hue * 100} {Saturation * 100} {Value * 100}";
        }

        /// <summary>
        /// Get this value as a System.Windows.Media.Color.
        /// </summary>
        /// <returns>The converted color.</returns>
        public Color ToColor()
        {
            if (Saturation.AboutEqual(0))
                return Color.FromArgb(255, 255, 255, 255);

            var hueDegrees = HueDegrees;
            var saturation = Saturation;
            var value = Value;
            var hueDegreesAsSector = hueDegrees / 60d;
            var integerPartOfHue = Math.Floor(hueDegreesAsSector);
            var factorialOfHue = hueDegreesAsSector - integerPartOfHue;
            var p = value * (1d - saturation);
            var q = value * (1d - saturation * factorialOfHue);
            var t = value * (1d - saturation * (1d - factorialOfHue));

            if (integerPartOfHue.AboutEqual(0))
                return Color.FromArgb(255, (byte)(255 * value), (byte)(255 * t), (byte)(255 * p));

            if (integerPartOfHue.AboutEqual(1))
                return Color.FromArgb(255, (byte)(255 * q), (byte)(255 * value), (byte)(255 * p));

            if (integerPartOfHue.AboutEqual(2))
                return Color.FromArgb(255, (byte)(255 * p), (byte)(255 * value), (byte)(255 * t));

            if (integerPartOfHue.AboutEqual(3))
                return Color.FromArgb(255, (byte)(255 * p), (byte)(255 * q), (byte)(255 * value));
            
            if (integerPartOfHue.AboutEqual(4))
                return Color.FromArgb(255, (byte)(255 * t), (byte)(255 * p), (byte)(255 * value));

            return Color.FromArgb(255, (byte)(255 * value), (byte)(255 * p), (byte)(255 * q));
        }

        #endregion

        #region Overrides of Object

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Hue} {Saturation} {Value}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true"/> if the specified object  is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public bool Equals(HSVColor obj)
        {
            if (obj == null)
                return false;

            return Hue.AboutEqual(obj.Hue) &&
                   Saturation.AboutEqual(obj.Saturation) &&
                   Value.AboutEqual(obj.Value);
        }

        #endregion
    }
}