using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BP.ColourChimp.Controls
{
    /// <summary>
    /// A text box that selects all text when it is clicked.
    /// </summary>
    public class ClickSelectTextBox : TextBox
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of the ClickSelectTextBox class.
        /// </summary>
        public ClickSelectTextBox()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText), true);
        }

        #endregion

        #region StaticMethods

        private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            DependencyObject parent = e.OriginalSource as UIElement;

            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent == null)
                return;

            var textBox = parent as TextBox;

            if (textBox.IsKeyboardFocusWithin)
                return;

            textBox.Focus();
            e.Handled = true;
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            textBox?.SelectAll();
        }

        #endregion
    }
}