using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace BP.ColourChimp.Validation
{
    /// <summary>
    /// Provides validation for determining if a value is a valid byte expressed as hex.
    /// </summary>
    public class HexByteValidationRule : ValidationRule
    {
        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="T:System.Windows.Controls.ValidationResult"/> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value?.ToString().ToUpper() ?? string.Empty;

            if (str.Any(c => !"0123456789ABCDEF".Contains(c)))
                return new ValidationResult(false, "Value is not hex.");

            var data = Convert.ToInt32(str, 16);

            if (data >= 0 && data <= 255)
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Value is outside the 0 - 255 range.");
        }
    }
}