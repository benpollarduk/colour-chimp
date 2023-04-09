using System.Globalization;
using System.Windows.Controls;

namespace BP.ColourChimp.Validation
{
    /// <summary>
    /// Provides validation for determining if a value is a valid double expressing a percentage between 0 and 100.
    /// </summary>
    public class DoublePercentageValidationRule : ValidationRule
    {
        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="T:System.Windows.Controls.ValidationResult" /> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!double.TryParse(value?.ToString() ?? string.Empty, out var d))
                return new ValidationResult(false, "Value is not a double");

            if (d >= 0 && d <= 100)
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Value is outside on 0 - 100 range.");
        }
    }
}