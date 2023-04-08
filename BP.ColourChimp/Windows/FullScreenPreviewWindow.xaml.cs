using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BP.ColourChimp.Windows
{
    /// <summary>
    /// Interaction logic for FullScreenPreviewWindow.xaml
    /// </summary>
    public partial class FullScreenPreviewWindow : Window
    {
        #region StaticProperties

        /// <summary>
        /// Get or set if maxing out the alpha channel on preview.
        /// </summary>
        public static bool MaxOutAlphaOnPreview { get; set; } = true;

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Identifies the FullScreenPreviewWindow.SelectedColor property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(FullScreenPreviewWindow), new PropertyMetadata(Colors.Transparent, OnSelectedColorPropertyChanges));

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the selected color. This is a dependency property.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        #endregion

        #region PropertyCallbacks

        private static void OnSelectedColorPropertyChanges(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as FullScreenPreviewWindow;

            if (window == null)
                return;

            if (args.NewValue == null)
                return;

            var c = (Color)args.NewValue;
            window.Title = c.ToString();
            window.PreviewBorder.Background = new SolidColorBrush(c);
            window.Background = MaxOutAlphaOnPreview ? Brushes.White : Brushes.Transparent;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the FullScreenPreviewWindow class.
        /// </summary>
        public FullScreenPreviewWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize a new instance of the FullScreenPreviewWindow class.
        /// </summary>
        /// <param name="color">The color to preview.</param>
        public FullScreenPreviewWindow(Color color)
        {
            InitializeComponent();
            SelectedColor = color;
        }

        #endregion

        #region EventHandlers

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}