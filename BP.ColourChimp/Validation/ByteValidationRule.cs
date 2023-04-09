using System.Globalization;
using System.Windows.Controls;

namespace BP.ColourChimp.Validation
{
    /// <summary>
    /// Provides validation for determining if a value is a valid byte.
    /// </summary>
    public class ByteValidationRule : ValidationRule
    {
        #region Overrides of ValidationRule

        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="T:System.Windows.Controls.ValidationResult" /> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (byte.TryParse(value?.ToString() ?? string.Empty, out _))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Data is not a byte.");
        }

        #endregion
    }
}