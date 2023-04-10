using System.Windows;
using System.Windows.Controls;

namespace BP.ColourChimp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler((s, a) => (s as TextBox)?.SelectAll()));
            base.OnStartup(e);
        }
    }
}