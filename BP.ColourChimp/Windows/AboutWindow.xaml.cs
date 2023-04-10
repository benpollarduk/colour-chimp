using System;
using System.Reflection;
using System.Windows;

namespace BP.ColourChimp.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region Methods

        /// <summary>
        /// Initialize a new instance of the AboutWindow class.
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
            RefreshVersion(Assembly.GetExecutingAssembly().GetName().Version);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refresh the version display.
        /// </summary>
        /// <param name="version">The version.</param>
        private void RefreshVersion(Version version)
        {
            VersionLabel.Content = version != null ? $"Version {version.Major}.{version.Minor}.{version.Revision}.{version.Build}" : "Version (unknown)";
        }

        #endregion
    }
}