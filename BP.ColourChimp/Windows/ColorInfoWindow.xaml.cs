using System;
using System.Windows;
using System.Windows.Media;
using BP.ColourChimp.Classes;
using BP.ColourChimp.Extensions;

namespace BP.ColourChimp.Windows
{
    /// <summary>
    /// Interaction logic for ColorInfoWindow.xaml
    /// </summary>
    public partial class ColorInfoWindow : Window
    {
        #region Properties

        /// <summary>
        /// Get or set the selected color.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Occurs when any modification on this controls ColorInfoWindow.SelectedColor is completed.
        /// </summary>
        public event EventHandler<Color> ColorModificationComplete;

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Identifies the ColorInfoWindow.SelectedColor property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorInfoWindow), new PropertyMetadata(Colors.Transparent, OnSelectedColorPropertyChanges));

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the ColorInfoWindow class.
        /// </summary>
        public ColorInfoWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize a new instance of the ColorInfoWindow class.
        /// </summary>
        /// <param name="color">The color to display information about.</param>
        public ColorInfoWindow(Color color)
        {
            InitializeComponent();
            SelectedColor = color;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Display info for a specified color space.
        /// </summary>
        /// <param name="colorSpace">The color space to show the info for.</param>
        public void DisplayInfo(ColorSpace colorSpace)
        {
            switch (colorSpace)
            {
                case ColorSpace.ARGB:
                    ARGBExpander.IsExpanded = true;
                    break;
                case ColorSpace.CMYK:
                    CMYKExpander.IsExpanded = true;
                    break;
                case ColorSpace.HSV:
                    HSVExpander.IsExpanded = true;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region PropertyCallbacks

        private static void OnSelectedColorPropertyChanges(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as ColorInfoWindow;

            if (args.NewValue == null)
                return;

            if (window == null)
                return;

            var c = (Color)args.NewValue;

            window.Title = $"{c} info";
            window.RGBHexLabel.Content = "#" + c.ToString().Substring(3);
            window.RGBLabel.Content = $"{c.R} {c.G} {c.B} ";
            window.RGBNormalisedLabel.Content = $"{Math.Round(1d / 255d * c.R, 4)} {Math.Round(1d / 255d * c.G, 4)} {Math.Round(1d / 255d * c.B, 4)}";
            window.RGBPercentLabel.Content = $"{Math.Round(100d / 255d * c.R, 1)} {Math.Round(100d / 255d * c.G, 1)} {Math.Round(100d / 255d * c.B, 1)}";
            window.HexLabel.Content = c.ToString();
            window.ARGBLabel.Content = $"{c.A} {c.R} {c.G} {c.B}";
            window.ARGBNormalisedLabel.Content = $"{Math.Round(1d / 255d * c.A, 4)} {Math.Round(1d / 255d * c.R, 4)} {Math.Round(1d / 255d * c.G, 4)} {Math.Round(1d / 255d * c.B, 4)}";
            window.ARGBPercentLabel.Content = $"{Math.Round(100d / 255d * c.A, 1)} {Math.Round(100d / 255d * c.R, 1)} {Math.Round(100d / 255d * c.G, 1)} {Math.Round(100d / 255d * c.B, 1)}";

            var cmyk = c.ToCMYK();
            window.CMYKLabel.Content = cmyk.ToString();
            window.CMYKPercentLabel.Content = cmyk.ToPercentageString();

            var hsv = c.ToHSV();
            window.HSVLabel.Content = hsv.ToString();
            window.HSVDegreesNormalisedByteLabel.Content = $"{hsv.HueDegrees} {hsv.Saturation} {hsv.ValueAsByte}";
            window.HSVPercentLabel.Content = hsv.ToPercentageString();

            window.RShape.Opacity = c.R > 0 ? c.R / 255d : 0.0d;
            window.GShape.Opacity = c.G > 0 ? c.G / 255d : 0.0d;
            window.BShape.Opacity = c.B > 0 ? c.B / 255d : 0.0d;

            window.CShape.Opacity = cmyk.Cyan;
            window.MShape.Opacity = cmyk.Magenta;
            window.YShape.Opacity = cmyk.Yellow;
        }

        #endregion

        #region EventHandlers

        private void CopyRGBHexButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(RGBHexLabel.Content.ToString());
        }

        private void CopyRGBButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(RGBLabel.Content.ToString());
        }

        private void CopyRGBNormalisedButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(RGBNormalisedLabel.Content.ToString());
        }

        private void CopyRGBPercentButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(RGBPercentLabel.Content.ToString());
        }

        private void CopyArgbButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ARGBLabel.Content.ToString());
        }

        private void CopyHexButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HexLabel.Content.ToString());
        }

        private void CopyARGBNormalisedButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ARGBNormalisedLabel.Content.ToString());
        }

        private void CopyCMYKButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CMYKLabel.Content.ToString());
        }

        private void CopyCMYKPercentButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CMYKPercentLabel.Content.ToString());
        }

        private void CopyArgbPercentButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ARGBPercentLabel.Content.ToString());
        }

        private void CopyHSVButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HSVLabel.Content.ToString());
        }

        private void CopyHSVDegreesNormalisedByteButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HSVDegreesNormalisedByteLabel.Content.ToString());
        }

        private void CopyHSVPercentButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HSVPercentLabel.Content.ToString());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NegativeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SelectedColor = SelectedColor.ToNegative();
        }

        private void NegativeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SelectedColor = SelectedColor.ToNegative();
        }

        private void AddModifiedColorToMainButton_Click(object sender, RoutedEventArgs e)
        {
            ColorModificationComplete?.Invoke(this, SelectedColor);
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            var preview = new FullScreenPreviewWindow(SelectedColor);
            preview.Show();
        }

        #endregion
    }
}