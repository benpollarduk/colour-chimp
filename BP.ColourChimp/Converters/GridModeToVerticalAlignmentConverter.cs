using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BP.ColourChimp.Classes;

namespace BP.ColourChimp.Converters
{
    [ValueConversion(typeof(GridMode), typeof(VerticalAlignment))]
    internal class GridModeToVerticalAlignmentConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>Converts a value. </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null"/>, the valid null value is used.</returns>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Enum.TryParse(value?.ToString() ?? string.Empty, out GridMode mode))
                return VerticalAlignment.Stretch;

            return mode == GridMode.MaintainSize ? VerticalAlignment.Top : VerticalAlignment.Stretch;
        }

        /// <summary>Converts a value. </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null"/>, the valid null value is used.</returns>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}