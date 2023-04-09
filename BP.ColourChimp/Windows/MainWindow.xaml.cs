using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BP.ColourChimp.Classes;
using BP.ColourChimp.Classes.Sorting;
using BP.ColourChimp.Extensions;
using BP.ColourChimp.Properties;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Size = System.Windows.Size;
using SystemColors = System.Windows.SystemColors;

namespace BP.ColourChimp.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants

        private const Keys ColorSelectionModesModifierKey = Keys.Shift;
        private const Key ColorSelectionModesModifierKeyWPF = Key.LeftShift;
        private const Key IncrementAlphaKeyWPF = Key.Up;
        private const Key DecrementAlphaKeyWPF = Key.Down;
        private const Key IncrementBrightnessKeyWPF = Key.Right;
        private const Key DeccrementBrightnessKeyWPF = Key.Left;
        private const double FixedGridColoumns = 3d;

        #endregion

        #region StaticProperties

        private static Random RandomGenerator { get; } = new Random();

        #endregion

        #region Fields

        private Point? roiSelectionTopLeft;
        private Point? roiSelectionBottomRight;
        private BackgroundWorker roiGatherWorker;
        private BackgroundWorker colorSorterWorker;
        private readonly Mode defaultMode = Mode.Add;
        private string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the alpha. This is a dependency property.
        /// </summary>
        public byte Alpha
        {
            get { return (byte)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }

        /// <summary>
        /// Get or set the red. This is a dependency property.
        /// </summary>
        public byte Red
        {
            get { return (byte)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }

        /// <summary>
        /// Get or set the green. This is a dependency property.
        /// </summary>
        public byte Green
        {
            get { return (byte)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }

        /// <summary>
        /// Get or set the blue. This is a dependency property.
        /// </summary>
        public byte Blue
        {
            get { return (byte)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }

        /// <summary>
        /// Get or set the cyan. This is specified as a percentage. This is a dependency property.
        /// </summary>
        public double Cyan
        {
            get { return (double)GetValue(CyanProperty); }
            set { SetValue(CyanProperty, value); }
        }

        /// <summary>
        /// Get or set the magenta. This is specified as a percentage. This is a dependency property.
        /// </summary>
        public double Magenta
        {
            get { return (double)GetValue(MagentaProperty); }
            set { SetValue(MagentaProperty, value); }
        }

        /// <summary>
        /// Get or set the yellow. This is specified as a percentage. This is a dependency property.
        /// </summary>
        public double Yellow
        {
            get { return (double)GetValue(YellowProperty); }
            set { SetValue(YellowProperty, value); }
        }

        /// <summary>
        /// Get or set the key. This is specified as a percentage. This is a dependency property.
        /// </summary>
        public double K
        {
            get { return (double)GetValue(KProperty); }
            set { SetValue(KProperty, value); }
        }

        /// <summary>
        /// Get or set the hue. This is specified as a percentage. This is a dependency property.
        /// </summary>
        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        /// <summary>
        /// Get or set the saturation. This is specified as a percentage. This is a dependency property.
        /// </summary>
        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        /// <summary>
        /// Get or set the value. This is specified as a percentage. This is a dependency property.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Get the current status. This is a dependency property.
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            private set { SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// Get or set the mode. This is a dependency property.
        /// </summary>
        public Mode Mode
        {
            get { return (Mode)GetValue(ModeProperty); }
            private set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Get if this control has some children colors. This is a dependency property.
        /// </summary>
        public bool HasChildrenColors
        {
            get { return (bool)GetValue(HasChildrenColorsProperty); }
            private set { SetValue(HasChildrenColorsProperty, value); }
        }

        /// <summary>
        /// Get if all background operations are idle. This is a dependency property.
        /// </summary>
        public bool AllBackgroundOperationsIdle
        {
            get { return (bool)GetValue(AllBackgroundOperationsIdleProperty); }
            private set { SetValue(AllBackgroundOperationsIdleProperty, value); }
        }

        /// <summary>
        /// Get or set the value used to determine channel dominance as a normalised value (0.0-1.0). This is a dependency property.
        /// </summary>
        public double ChannelDominance
        {
            get { return (double)GetValue(ChannelDominanceProperty); }
            set { SetValue(ChannelDominanceProperty, value); }
        }

        /// <summary>
        /// Get the amount of colors currently displayed. This is a dependency property.
        /// </summary>
        public double ColorCount
        {
            get { return (double)GetValue(ColorCountProperty); }
            private set { SetValue(ColorCountProperty, value); }
        }

        /// <summary>
        /// Get of set if alpha is maxed out on previews. This is a dependency property.
        /// </summary>
        public bool MaxOutAlphaOnPreview
        {
            get { return (bool)GetValue(MaxOutAlphaOnPreviewProperty); }
            set { SetValue(MaxOutAlphaOnPreviewProperty, value); }
        }

        /// <summary>
        /// Get of set the grid mode. This is a dependency property.
        /// </summary>
        public GridMode GridMode
        {
            get { return (GridMode)GetValue(GridModeProperty); }
            set { SetValue(GridModeProperty, value); }
        }

        /// <summary>
        /// Get of set the number of fixed columns. This is only applicable when the GridMode property is set to MaintainSize. This is a dependency property.
        /// </summary>
        public double FixedColumns
        {
            get { return (double)GetValue(FixedColumnsProperty); }
            set { SetValue(FixedColumnsProperty, value); }
        }

        /// <summary>
        /// Get of set the color space. This is only applicable when the GridMode property is set to MaintainSize. This is a dependency property.
        /// </summary>
        public ColorSpace ColorSpace
        {
            get { return (ColorSpace)GetValue(ColorSpaceProperty); }
            private set { SetValue(ColorSpaceProperty, value); }
        }

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Identifies the MainWindow.Alpha property.
        /// </summary>
        public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register("Alpha", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Red property.
        /// </summary>
        public static readonly DependencyProperty RedProperty = DependencyProperty.Register("Red", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Green property.
        /// </summary>
        public static readonly DependencyProperty GreenProperty = DependencyProperty.Register("Green", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Blue property.
        /// </summary>
        public static readonly DependencyProperty BlueProperty = DependencyProperty.Register("Blue", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Cyan property.
        /// </summary>
        public static readonly DependencyProperty CyanProperty = DependencyProperty.Register("Cyan", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Magenta property.
        /// </summary>
        public static readonly DependencyProperty MagentaProperty = DependencyProperty.Register("Magenta", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Yellow property.
        /// </summary>
        public static readonly DependencyProperty YellowProperty = DependencyProperty.Register("Yellow", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.K property.
        /// </summary>
        public static readonly DependencyProperty KProperty = DependencyProperty.Register("K", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Hue property.
        /// </summary>
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(MainWindow), new PropertyMetadata(100d, OnSharedHSVPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Saturation property.
        /// </summary>
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedHSVPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Value property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(MainWindow), new PropertyMetadata(100d, OnSharedHSVPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.Status property.
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(MainWindow), new PropertyMetadata("Ready"));

        /// <summary>
        /// Identifies the MainWindow.HasChildrenColors property.
        /// </summary>
        public static readonly DependencyProperty HasChildrenColorsProperty = DependencyProperty.Register("HasChildrenColors", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the MainWindow.AllBackgroundOperationsIdle property.
        /// </summary>
        public static readonly DependencyProperty AllBackgroundOperationsIdleProperty = DependencyProperty.Register("AllBackgroundOperationsIdle", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the MainWindow.ChannelDominance property.
        /// </summary>
        public static readonly DependencyProperty ChannelDominanceProperty = DependencyProperty.Register("ChannelDominance", typeof(double), typeof(MainWindow), new PropertyMetadata(0.4d));

        /// <summary>
        /// Identifies the MainWindow.ColorCount property.
        /// </summary>
        public static readonly DependencyProperty ColorCountProperty = DependencyProperty.Register("ColorCount", typeof(double), typeof(MainWindow), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MainWindow.Mode property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(Mode), typeof(MainWindow), new PropertyMetadata(Mode.Add, OnModePropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.MaxOutAlphaOnPreview property.
        /// </summary>
        public static readonly DependencyProperty MaxOutAlphaOnPreviewProperty = DependencyProperty.Register("MaxOutAlphaOnPreview", typeof(bool), typeof(MainWindow), new PropertyMetadata(FullScreenPreviewWindow.MaxOutAlphaOnPreview, OnMaxOutAlphaOnPreviewPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.GridMode property.
        /// </summary>
        public static readonly DependencyProperty GridModeProperty = DependencyProperty.Register("GridMode", typeof(GridMode), typeof(MainWindow), new PropertyMetadata(GridMode.FitToArea, OnGridModePropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.FixedColumns property.
        /// </summary>
        public static readonly DependencyProperty FixedColumnsProperty = DependencyProperty.Register("FixedColumns", typeof(double), typeof(MainWindow), new PropertyMetadata(FixedGridColoumns, OnFixedColumnsPropertyChanged));

        /// <summary>
        /// Identifies the MainWindow.ColorSpace property.
        /// </summary>
        public static readonly DependencyProperty ColorSpaceProperty = DependencyProperty.Register("ColorSpace", typeof(ColorSpace), typeof(MainWindow), new PropertyMetadata(ColorSpace.ARGB));

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            LoadSettingsFromManifest();
        }

        #endregion

        #region Methods

        private void ShowColorInfoWindow(Color c)
        {
            var window = new ColorInfoWindow(c) { Owner = Application.Current.MainWindow };
            window.ColorModificationComplete += ColorInfoWindow_ColorModificationComplete;
            window.Closed += Window_Closed;
            window.DisplayInfo(ColorSpace);
            window.Show();
        }

        private void LoadSettingsFromManifest()
        {
            try
            {
                Topmost = Settings.Default.KeepApplicationInForeground;
                FixedColumns = Settings.Default.Columns;
                MaxOutAlphaOnPreview = Settings.Default.UseMaximumAlphaOnPreview;
                GridMode = (GridMode)Enum.Parse(typeof(GridMode), Settings.Default.GridMode);
                ColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), Settings.Default.ColorSpace);
            }
            catch (Exception e)
            {
                Status = $"Exception caught loading settings from manifest: {e.Message}";
                UpdateManifestProperties();
            }
        }

        private void UpdateManifestProperties()
        {
            Settings.Default.KeepApplicationInForeground = Topmost;
            Settings.Default.Columns = (int)FixedColumns;
            Settings.Default.UseMaximumAlphaOnPreview = MaxOutAlphaOnPreview;
            Settings.Default.GridMode = GridMode.ToString();
            Settings.Default.ColorSpace = ColorSpace.ToString();
            Settings.Default.Save();
        }

        private void Clear()
        {
            if (coloursGrid.Children.Count == 0)
                return;

            var rectangles = new List<Rectangle>(coloursGrid.Children.Cast<Rectangle>());

            foreach (var r in rectangles)
                RemoveRectangle(r);

            Status = "All colours cleared";
        }

        private void ClearLast()
        {
            if (coloursGrid.Children.Count == 0)
                return;

            RemoveRectangle(coloursGrid.Children[coloursGrid.Children.Count - 1] as Rectangle);
        }

        private void ShowExportDialog()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Export image as...",
                OverwritePrompt = true,
                DefaultExt = "*.bmp",
                Filter = "24-bit Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif",
                InitialDirectory = defaultPath
            };

            saveFileDialog.FileOk += (s, e) =>
            {
                try
                {
                    defaultPath = saveFileDialog.FileName.Substring(0, saveFileDialog.FileName.LastIndexOf("\\", StringComparison.Ordinal) - 1);
                    ExportPatchwork(saveFileDialog.FileName);
                }
                catch (Exception e)
                {
                    Status = $"Exception caught saving file: {e.Message}";
                }
            };

            saveFileDialog.ShowDialog();
        }

        private void ShowImportDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Import an image...", 
                DefaultExt = "*.png", 
                Filter = "Image files (*.bmp, *.gif, *.jpg, *.png, *.tif)|*.bmp;*.gif;*.jpg;*.png;*.tif;", 
                InitialDirectory = defaultPath
            };

            openFileDialog.FileOk += (s, e) =>
            {
                try
                {
                    defaultPath = openFileDialog.FileName.Substring(0, openFileDialog.FileName.LastIndexOf("\\", StringComparison.Ordinal) - 1);
                    ImportImage(openFileDialog.FileName);
                }
                catch (Exception e)
                {
                    Status = $"Exception caught importing file: {e.Message}";
                }
            };
            openFileDialog.ShowDialog();
        }

        private bool ExportPatchwork(string path)
        {
            var background = coloursGrid.Background;

            try
            {
                Status = $"Exporting to {path}...";
                var count = (int)ColorCount;
                var sqr = (int)Math.Ceiling(Math.Sqrt(count));
                var image = new Bitmap(sqr, sqr);
                var rectangles = coloursGrid.Children.OfType<Rectangle>().ToArray();
                var rectangleIndex = 0;

                for (var row = 0; row < sqr; row++)
                {
                    for (var column = 0; column < sqr; column++)
                    {
                        if (rectangleIndex < count)
                        {
                            var r = rectangles[rectangleIndex];

                            if (r.Fill is SolidColorBrush)
                            {
                                var brush = r.Fill as SolidColorBrush;

                                if (brush == null)
                                    continue;

                                image.SetPixel(column, row, System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B));
                            }
                        }
                        else
                        {
                            image.SetPixel(column, row, System.Drawing.Color.Transparent);
                        }
                        rectangleIndex++;
                    }
                }
                
                ImageFormat format;

                if (path.EndsWith(".png", true, CultureInfo.InvariantCulture))
                    format = ImageFormat.Png;
                else if (path.EndsWith(".tif", true, CultureInfo.InvariantCulture) || path.EndsWith(".tiff", true, CultureInfo.InvariantCulture))
                    format = ImageFormat.Tiff;
                else if (path.EndsWith(".jpg", true, CultureInfo.InvariantCulture) || path.EndsWith(".jpeg", true, CultureInfo.InvariantCulture))
                    format = ImageFormat.Jpeg;
                else if (path.EndsWith(".gif", true, CultureInfo.InvariantCulture) || path.EndsWith(".giff", true, CultureInfo.InvariantCulture))
                    format = ImageFormat.Gif;
                else
                    format = ImageFormat.Bmp;

                image.Save(path, format);
                Status = $"Exported to: {path}";
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred while saving the file: {e.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Status = "Export failed";
                return false;
            }
            finally
            {
                coloursGrid.Background = background;
            }
        }

        private void ImportImage(string path)
        {
            try
            {
                var ext = path.Substring(path.Length - 3);
                BitmapDecoder decoder;
                BitmapEncoder encoder;
                Status = $"Importing {path}...";

                switch (ext.ToUpper())
                {
                    case "BMP":
                        decoder = new BmpBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                        encoder = new BmpBitmapEncoder();
                        break;
                    case "GIF":
                        decoder = new GifBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                        encoder = new GifBitmapEncoder();
                        break;
                    case "JPG":
                        decoder = new JpegBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                        encoder = new JpegBitmapEncoder();
                        break;
                    case "PNG":
                        decoder = new PngBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                        encoder = new PngBitmapEncoder();
                        break;
                    case "TIF":
                        decoder = new TiffBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                        encoder = new TiffBitmapEncoder();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var frame = decoder.Frames[0];
                encoder.Frames.Add(frame);

                var imageInStream = new MemoryStream();
                encoder.Save(imageInStream);

                var winFormsBitmap = new Bitmap(imageInStream);
                GatherAllPixelsInROI(0, 0, winFormsBitmap.Height, winFormsBitmap.Width, winFormsBitmap);
                imageInStream.Close();

                Status = "Import complete";
            }
            catch (Exception e)
            {
                MessageBox.Show($"There was an error importing the image: {e.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Status = "Import failed";
            }
        }

        private void BeginColorPicking()
        {
            Mode = Mode.Pipette;
            var bmp = GetVirtualScreenBitmap();
            bool wasError;

            ThreadPool.QueueUserWorkItem(state =>
            {
                do
                {
                    // keep going while mouse down or modifier key down and no error

                    wasError = !(bool)Dispatcher.Invoke(new UpdatePixelCallback(UpdateColorToPixelAtCurrentPoint), bmp);

                    if (!wasError)
                        Thread.Sleep(10);

                } while ((System.Windows.Forms.Control.MouseButtons == MouseButtons.Left || System.Windows.Forms.Control.ModifierKeys == ColorSelectionModesModifierKey) && !wasError);

                if (wasError)
                    Dispatcher.Invoke(() => MessageBox.Show("There was an error picking the colour", "Pick Error", MessageBoxButton.OK, MessageBoxImage.Error));

                Dispatcher.Invoke(() => Mode = Mode.Add);
            }, bmp);
        }

        private Point GetCurrentMousePositionOverVirtualScreen()
        {
            var p = System.Windows.Forms.Control.MousePosition;
            p.X += Math.Abs(SystemInformation.VirtualScreen.Left);
            p.Y += Math.Abs(SystemInformation.VirtualScreen.Top);

            return p;
        }

        private Bitmap GetVirtualScreenBitmap()
        {
            // determine the size of the "virtual screen", which includes all monitor
            var screenLeft = SystemInformation.VirtualScreen.Left;
            var screenTop = SystemInformation.VirtualScreen.Top;
            var screenWidth = SystemInformation.VirtualScreen.Width;
            var screenHeight = SystemInformation.VirtualScreen.Height;

            // create a bitmap of the appropriate size to receive the screen shot
            var bmp = new Bitmap(screenWidth, screenHeight);

            // draw the screen shot into the bitmap
            using (var g = Graphics.FromImage(bmp))
                g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);

            return bmp;
        }

        private Bitmap GetVirtualScreenBitmap(Int32Rect region)
        {
            var bmp = new Bitmap(region.Width, region.Height);

            using (var g = Graphics.FromImage(bmp))
                g.CopyFromScreen(region.X, region.Y, 0, 0, bmp.Size);

            return bmp;
        }

        private bool UpdateColorToPixelAtCurrentPoint(Bitmap bmp)
        {
            try
            {
                if (bmp == null) 
                    return false;

                var cursorPosition = GetCurrentMousePositionOverVirtualScreen();
                var c = bmp.GetPixel(cursorPosition.X, cursorPosition.Y);

                switch (ColorSpace)
                {
                    case ColorSpace.ARGB:

                        Alpha = c.A;
                        Red = c.R;
                        Green = c.G;
                        Blue = c.B;

                        break;

                    case ColorSpace.CMYK:

                        var cymkColor = Color.FromArgb(c.A, c.R, c.G, c.B).ToCMYK();
                        Cyan = cymkColor.Cyan * 100;
                        Magenta = cymkColor.Magenta * 100;
                        Yellow = cymkColor.Yellow * 100;
                        K = cymkColor.Key * 100;

                        break;

                    case ColorSpace.HSV:

                        var hsvColor = Color.FromArgb(c.A, c.R, c.G, c.B).ToHSV();
                        Hue = hsvColor.Hue * 100;
                        Saturation = hsvColor.Saturation * 100;
                        Value = hsvColor.Value * 100;

                        break;

                    default:
                        throw new NotImplementedException();
                }

                return true;
            }
            catch (Exception e)
            {
                Status = $"Failed to update to colour: {e.Message}";
                return false;
            }
        }

        private byte IncrementByte(byte b)
        {
            if (b < 239)
                b += 16;
            else
                b = 255;

            return b;
        }

        private byte DeincrementByte(byte b)
        {
            if (b > 15)
                b -= 16;
            else
                b = 0;

            return b;
        }

        private double IncrementDouble(double d)
        {
            if (d < 90)
                d += 10;
            else
                d = 100;

            return d;
        }

        private double DecrementDouble(double d)
        {
            if (d > 9)
                d -= 10;
            else
                d = 0;

            return d;
        }

        private void GatherAllPixelsInROI(int top, int left, int bottom, int right)
        {
            try
            {
                var bmp = GetVirtualScreenBitmap();
                GatherAllPixelsInROI(top, left, bottom, right, bmp);
            }
            catch (Exception e)
            {
                Status = $"There was an error gathering screen region pixels {e.Message}";
            }
        }

        private void GatherAllPixelsInROI(int top, int left, int bottom, int right, Bitmap bitmap)
        {
            try
            {
                Status = "Beginning screen region gather...";

                roiGatherWorker?.CancelAsync();

                roiGatherWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                AllBackgroundOperationsIdle = false;

                roiGatherWorker.DoWork += (sender, args) =>
                {
                    var capture = args.Argument as Bitmap;
                    var pixels = new List<string>();
                    double percentageComplete;

                    try
                    {
                        if (right < left)
                        {
                            var temp = left;
                            left = right;
                            right = temp;
                        }

                        if (bottom < top)
                        {
                            var temp = top;
                            top = bottom;
                            bottom = temp;
                        }

                        var size = new Size(right - left, bottom - top);

                        for (var row = top; row < bottom; row++)
                        {
                            for (var column = left; column < right; column++)
                            {
                                if (roiGatherWorker.CancellationPending)
                                    return;

                                var pixel = bitmap.GetPixel(column, row);
                                var id = $"{pixel.A}{pixel.R}{pixel.G}{pixel.B}";

                                if (!pixels.Contains(id))
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        AddColor(Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                                        pixels.Add(id);
                                    }, DispatcherPriority.Background);
                                }

                                percentageComplete = 100d / size.Height * (row - top);
                                Dispatcher.Invoke(() => Status = $"Gathering colors in ROI {Math.Round(percentageComplete, 1)}% complete, row {row + 1 - top} of {size.Height}", DispatcherPriority.Normal);
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show("Screen region is not valid. Check that window is not minimized", "Gather Error", MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                    catch (Exception e)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show($"There was an error gathering screen region: {e.Message}", "Gather Error", MessageBoxButton.OK, MessageBoxImage.Error));
                    }
                    finally
                    {
                        roiGatherWorker = null;
                        Dispatcher.Invoke(() =>
                        {
                            Status = $"Screen region gather finished, {pixels.Count} colours were found";
                            AllBackgroundOperationsIdle = GetIfAllBackgroundOperationsAreComplete();
                        });
                    }
                };

                roiGatherWorker.RunWorkerAsync(bitmap);
            }
            catch (Exception e)
            {
                MessageBox.Show($"There was an error gathering screen region: {e.Message}", "Gather Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool GetIfAllBackgroundOperationsAreComplete()
        {
            return colorSorterWorker == null && roiGatherWorker == null;
        }

        private void AddColor(Color color)
        {
            try
            {
                var r = new Rectangle { Fill = new SolidColorBrush(color) };

                switch (GridMode)
                {
                    case GridMode.FitToArea:
                        break;
                    case GridMode.MaintainSize:
                        r.Style = coloursGrid.FindResource("squareRectangleStyle") as Style;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                coloursGrid.Children.Add(r);
                ColorCount++;
                HasChildrenColors = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot add colour, check values are entered in a correct hex format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveRectangle(Rectangle rectangle)
        {
            try
            {
                coloursGrid.Children.Remove(rectangle);
                ColorCount--;
                HasChildrenColors = coloursGrid.Children.Count > 0;
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot remove rectangle, check rectangle is part of the grid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortColors(Comparison<Rectangle> sortMethod)
        {
            Mode = Mode.Sort;
            AllBackgroundOperationsIdle = false;
            var rectangleCollection = coloursGrid.Children;

            colorSorterWorker?.CancelAsync();
            colorSorterWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            colorSorterWorker.DoWork += (sender, args) =>
            {
                try
                {
                    var collection = args.Argument as UIElementCollection;

                    if (collection == null)
                        return;

                    var sortList = new List<Rectangle>();

                    // add all rectangles
                    Dispatcher.Invoke(() => sortList.AddRange(collection.OfType<Rectangle>().Select(o => o)));

                    if (!colorSorterWorker.CancellationPending)
                        Dispatcher.Invoke(() => sortList.Sort(sortMethod));
                    else
                        return;

                    for (var index = 0; index < sortList.Count; index++)
                    {
                        if (colorSorterWorker.CancellationPending)
                            return;

                        Dispatcher.Invoke(() =>
                        {
                            coloursGrid.Children.Remove(sortList[index]);
                            coloursGrid.Children.Insert(index, sortList[index]);
                        });
                    }
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(() => MessageBox.Show($"There was an error sorting colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        Status = "Colour sort finished";
                        colorSorterWorker = null;
                        Mode = defaultMode;
                        AllBackgroundOperationsIdle = GetIfAllBackgroundOperationsAreComplete();
                    });
                }
            };

            colorSorterWorker.RunWorkerAsync(rectangleCollection);
        }

        private void PopulateWindowsSubMenuWithOpenWindows()
        {
            gatherFromWindowMenuItem.Items.Clear();

            var procs = Process.GetProcesses();

            foreach (var proc in procs)
            {
                if (proc.MainWindowHandle == IntPtr.Zero)
                    continue;

                var windowNameItem = new MenuItem { Header = proc.ProcessName };
                windowNameItem.Click += windowNameItem_Click;
                gatherFromWindowMenuItem.Items.Add(windowNameItem);
            }

            gatherFromWindowMenuItem.IsEnabled = gatherFromWindowMenuItem.Items.Count > 0;
        }

        private void RemoveDuplicateColors()
        {
            Mode = Mode.Filter;
            AllBackgroundOperationsIdle = false;
            var rectangleCollection = coloursGrid.Children;

            colorSorterWorker?.CancelAsync();
            colorSorterWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

            colorSorterWorker.DoWork += (sender, args) =>
            {
                try
                {
                    var collection = args.Argument as UIElementCollection;

                    if (collection == null)
                        return;

                    var colorDictionary = new Dictionary<string, short>();
                    var allRecatngles = new List<Rectangle>();

                    Dispatcher.Invoke(() =>
                    {
                        foreach (var o in collection)
                        {
                            if (o is Rectangle r)
                            {
                                var brush = r.Fill as SolidColorBrush;

                                if (brush == null)
                                    continue;

                                var key = $"{brush.Color.A}{brush.Color.R}{brush.Color.G}{brush.Color.B}";

                                if (colorDictionary.Keys.Contains(key))
                                    colorDictionary[key]++;
                                else
                                    colorDictionary.Add(key, 1);
                            }

                            allRecatngles.Add(o as Rectangle);
                        }
                    });

                    while (colorDictionary.Keys.Any())
                    {
                        if (colorSorterWorker.CancellationPending)
                            break;

                        var key = colorDictionary.Keys.Last();

                        if (colorDictionary[key] > 1)
                        {
                            var toRemove = colorDictionary[key] - 1;

                            Dispatcher.Invoke(() =>
                            {
                                foreach (var r in allRecatngles)
                                {
                                    if (r.Fill is SolidColorBrush brush)
                                    {
                                        if ($"{brush.Color.A}{brush.Color.R}{brush.Color.G}{brush.Color.B}" == key)
                                        {
                                            RemoveRectangle(r);
                                            toRemove--;
                                        }
                                    }

                                    if (toRemove == 0 || colorSorterWorker.CancellationPending)
                                        break;
                                }
                            });
                        }

                        colorDictionary.Remove(key);
                    }
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(() => MessageBox.Show($"There was an error removing duplicate colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
                finally
                {
                    // begin invoke
                    Dispatcher.Invoke(() =>
                    {
                        Status = "Colour filter finished";
                        colorSorterWorker = null;
                        Mode = defaultMode;
                        AllBackgroundOperationsIdle = GetIfAllBackgroundOperationsAreComplete();
                    });
                }
            };

            colorSorterWorker.RunWorkerAsync(rectangleCollection);
        }

        private double FindMaximumDifference(double colorA, double colorB, double colorC)
        {
            return Math.Max(colorA, Math.Max(colorB, colorC)) - Math.Min(colorA, Math.Max(colorB, colorC));
        }

        private void Filter(ChannelDominanceModes mode, double dominance)
        {
            Mode = Mode.Filter;
            AllBackgroundOperationsIdle = false;
            var rectangleCollection = coloursGrid.Children;

            colorSorterWorker?.CancelAsync();
            colorSorterWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            colorSorterWorker.DoWork += (sender, args) =>
            {
                try
                {
                    if (dominance > 1.0d || dominance < 0.0d) 
                        throw new ArgumentException("Dominance is outside of normalised range (0.0 - 1.0)");

                    if (!(args.Argument is UIElementCollection collection))
                        return;

                    var sortList = new List<Rectangle>();
                    var allRectangleList = new List<Rectangle>();

                    Dispatcher.Invoke(() =>
                    {
                        switch (mode)
                        {
                            case ChannelDominanceModes.DominantRGB:
                            case ChannelDominanceModes.NonDominantRGB:
                                dominance = 255d / 100d * (dominance * 100d);
                                break;
                            case ChannelDominanceModes.DominantCMY:
                            case ChannelDominanceModes.NonDominantCMY:
                            case ChannelDominanceModes.Gray:
                            case ChannelDominanceModes.NonGray:
                                // dominance remains
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        foreach (var o in collection)
                        {
                            if (!(o is Rectangle))
                                continue;

                            var r = o as Rectangle;

                            if (!(r.Fill is SolidColorBrush brush))
                                return;
                            switch (mode)
                            {
                                case ChannelDominanceModes.DominantCMY:

                                    var cmyk = brush.Color.ToCMYK();

                                    if (FindMaximumDifference(cmyk.Cyan, cmyk.Magenta, cmyk.Yellow) < dominance)
                                        sortList.Add(r);

                                    break;

                                case ChannelDominanceModes.DominantRGB:

                                    if (FindMaximumDifference(brush.Color.R, brush.Color.G, brush.Color.B) >= dominance)
                                        sortList.Add(r);

                                    break;

                                case ChannelDominanceModes.Gray:

                                    if (brush.Color.R != brush.Color.G || brush.Color.G != brush.Color.B)
                                        sortList.Add(r);

                                    break;

                                case ChannelDominanceModes.NonDominantCMY:

                                    var cmyk2 = brush.Color.ToCMYK();

                                    if (FindMaximumDifference(cmyk2.Cyan, cmyk2.Magenta, cmyk2.Yellow) >= dominance)
                                        sortList.Add(r);

                                    break;

                                case ChannelDominanceModes.NonDominantRGB:

                                    if (FindMaximumDifference(brush.Color.R, brush.Color.G, brush.Color.B) >= dominance)
                                        sortList.Add(r);

                                    break;

                                case ChannelDominanceModes.NonGray:

                                    if (brush.Color.R == brush.Color.G && brush.Color.G == brush.Color.B)
                                        sortList.Add(r);

                                    break;

                                default:
                                    throw new NotImplementedException();
                            }

                            allRectangleList.Add(o as Rectangle);
                        }
                    });

                    foreach (var t in allRectangleList)
                    {
                        if (colorSorterWorker.CancellationPending)
                            return;

                        Dispatcher.Invoke(() =>
                        {
                            if (!sortList.Contains(t))
                                RemoveRectangle(t);
                        });
                    }
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(() => MessageBox.Show($"There was an error filtering colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        Status = "Colour filter finished";
                        colorSorterWorker = null;
                        Mode = defaultMode;
                        AllBackgroundOperationsIdle = GetIfAllBackgroundOperationsAreComplete();
                    });
                }
            };

            colorSorterWorker.RunWorkerAsync(rectangleCollection);
        }

        private void Populate(PopulationModes mode)
        {
            Mode = Mode.Populate;
            AllBackgroundOperationsIdle = false;

            colorSorterWorker?.CancelAsync();
            colorSorterWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            colorSorterWorker.DoWork += (sender, args) =>
            {
                try
                {
                    switch (mode)
                    {
                        case PopulationModes.Reds:
                        case PopulationModes.Greens:
                        case PopulationModes.Blues:
                            
                            byte r = 0;
                            byte g = 0;
                            byte b = 0;

                            for (byte index = 0; index < 255; index++)
                            {
                                switch (mode)
                                {
                                    case PopulationModes.Blues:
                                        b = index;
                                        break;
                                    case PopulationModes.Greens:
                                        g = index;
                                        break;
                                    case PopulationModes.Reds:
                                        r = index;
                                        break;
                                    case PopulationModes.Cyans:
                                        break;
                                    case PopulationModes.Yellows:
                                        break;
                                    case PopulationModes.Magentas:
                                        break;
                                    case PopulationModes.Grayscale:
                                        break;
                                    case PopulationModes.PresentationFramework:
                                        break;
                                    case PopulationModes.SystemColors:
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }

                                Dispatcher.Invoke(() => AddColor(Color.FromRgb(r, g, b)));
                            }

                            break;

                        case PopulationModes.Cyans:
                        case PopulationModes.Magentas:
                        case PopulationModes.Yellows:
                            
                            byte c = 0;
                            byte m = 0;
                            byte y = 0;

                            for (byte index = 0; index < 255; index++)
                            {
                                switch (mode)
                                {
                                    case PopulationModes.Cyans:
                                        c = index;
                                        break;
                                    case PopulationModes.Magentas:
                                        m = index;
                                        break;
                                    case PopulationModes.Yellows:
                                        y = index;
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }

                                Dispatcher.Invoke(() =>
                                {
                                    var cmyk = new CMYKColor(1.0d / 255 * c, 1.0d / 255 * m, 1.0d / 255 * y, 0.0);
                                    AddColor(cmyk.ToColor());
                                });
                            }

                            break;

                        case PopulationModes.Grayscale:
                            
                            for (byte index = 0; index < 255; index++) 
                                Dispatcher.Invoke(() => AddColor(Color.FromArgb(255, index, index, index)));

                            break;

                        case PopulationModes.PresentationFramework:
                            
                            foreach (var color in typeof(Colors).GetProperties())
                                Dispatcher.Invoke(() => AddColor((Color)ColorConverter.ConvertFromString(color.Name));

                            break;

                        case PopulationModes.SystemColors:
                            
                            foreach (var color in typeof(SystemColors).GetProperties())
                            {
                                if (color.GetValue(color, null) is Color col)
                                    Dispatcher.Invoke(() => AddColor((Color)color.GetValue(col, null)));
                            }

                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(() => MessageBox.Show($"There was an error populating colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        Status = "Colour population finished";
                        colorSorterWorker = null;
                        Mode = defaultMode;
                        AllBackgroundOperationsIdle = GetIfAllBackgroundOperationsAreComplete();
                    });
                }
            };

            colorSorterWorker.RunWorkerAsync();
        }

        #endregion

        #region StaticMethods

        private static double GetTwoColorProportion(double colorA, double colorB)
        {
            return (colorA + colorB) / 2;
        }

        #endregion

        #region PropertyCallbacks

        private static void OnSharedARGBPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as MainWindow;

            if (window == null)
                return;

            window.swatchBorder.Background = new SolidColorBrush(Color.FromArgb(window.Alpha, window.Red, window.Green, window.Blue));

            if (window.ColorSpace != ColorSpace.ARGB)
                return;

            var argb = Color.FromArgb(window.Alpha, window.Red, window.Green, window.Blue);
            var cymkColor = argb.ToCMYK();
            var hsvColor = argb.ToHSV();

            window.Cyan = cymkColor.Cyan * 100;
            window.Magenta = cymkColor.Magenta * 100;
            window.Yellow = cymkColor.Yellow * 100;
            window.K = cymkColor.Key * 100;
            window.Hue = hsvColor.Hue * 100;
            window.Saturation = hsvColor.Saturation * 100;
            window.Value = hsvColor.Value * 100;
        }

        private static void OnSharedCMYKPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as MainWindow;

            if (window == null)
                return;

            if (window.ColorSpace != ColorSpace.CMYK)
                return;

            var cmyk = new CMYKColor(window.Cyan / 100, window.Magenta / 100, window.Yellow / 100, window.K / 100);
            var argb = cmyk.ToColor();
            var hsv = argb.ToHSV();

            window.Red = argb.R;
            window.Green = argb.G;
            window.Blue = argb.B;
            window.Hue = hsv.Hue * 100;
            window.Saturation = hsv.Saturation * 100;
            window.Value = hsv.Value * 100;
        }

        private static void OnSharedHSVPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as MainWindow;

            if (window == null)
                return;

            if (window.ColorSpace != ColorSpace.HSV)
                return;

            var hsv = new HSVColor(window.Hue / 100, window.Saturation / 100, window.Value / 100);
            var argb = hsv.ToColor();
            var cmyk = argb.ToCMYK();

            window.Red = argb.R;
            window.Green = argb.G;
            window.Blue = argb.B;
            window.Cyan = cmyk.Cyan * 100;
            window.Magenta = cmyk.Magenta * 100;
            window.Yellow = cmyk.Yellow * 100;
            window.K = cmyk.Key * 100;
        }

        private static void OnModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as MainWindow;

            if (window == null)
                return;

            var newMode = (Mode)args.NewValue;

            switch (newMode)
            {
                case Mode.Add:
                    window.Cursor = Cursors.Arrow;
                    window.Status = "Ready";
                    window.coloursGrid.Cursor = Cursors.Hand;
                    break;
                case Mode.Delete:
                    window.Status = "Click a color to remove it, or press escape to cancel...";
                    window.coloursGrid.Cursor = Cursors.Arrow;
                    break;
                case Mode.Filter:
                    window.Cursor = Cursors.Wait;
                    window.Status = "Colours are being filtered, press escape to cancel...";
                    break;
                case Mode.Pipette:
                    window.Cursor = Cursors.Pen;
                    window.Status = "Move the mouse to the desired pixel, and release the mouse button to select as a colour...";
                    break;
                case Mode.ROIGather:
                    window.Cursor = Cursors.Cross;
                    window.Status = "Move to the top left of the region and press and hold Shift to specify the first point...";
                    break;
                case Mode.Sort:
                    window.Cursor = Cursors.Wait;
                    window.Status = "Colours are being sorted, press escape to cancel...";
                    break;
                case Mode.Populate:
                    window.Cursor = Cursors.Wait;
                    window.Status = "Colours are being populated, press escape to cancel...";
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void OnMaxOutAlphaOnPreviewPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            FullScreenPreviewWindow.MaxOutAlphaOnPreview = (bool)args.NewValue;
        }

        private static void OnGridModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as MainWindow;

            if (window == null)
                return;

            switch ((GridMode)args.NewValue)
            {
                case GridMode.FitToArea:
                    window.coloursGrid.Columns = 0;
                    window.coloursGrid.FirstColumn = 0;

                    foreach (Rectangle r in window.coloursGrid.Children)
                        r.Style = window.coloursGrid.FindResource("SizeableRectangleStyle") as Style;

                    break;
                case GridMode.MaintainSize:
                    window.coloursGrid.Columns = (int)window.FixedColumns;

                    foreach (Rectangle r in window.coloursGrid.Children)
                        r.Style = window.coloursGrid.FindResource("squareRectangleStyle") as Style;

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void OnFixedColumnsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var window = obj as MainWindow;

            if (window == null)
                return;

            if (window.GridMode == GridMode.MaintainSize)
                window.coloursGrid.Columns = (int)args.NewValue;
        }

        #endregion

        #region EventHandlers

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddColor(Color.FromArgb(Alpha, Red, Green, Blue));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UpdateManifestProperties();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var colorInfoWindow = sender as ColorInfoWindow;

            if (colorInfoWindow == null)
                return;

            colorInfoWindow.ColorModificationComplete -= ColorInfoWindow_ColorModificationComplete;
        }

        private void ColorInfoWindow_ColorModificationComplete(object sender, Color color)
        {
            AddColor(color);
        }

        private void PickColorButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BeginColorPicking();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Alpha = 255;
            Red = 255;
            Green = 255;
            Blue = 255;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case ColorSelectionModesModifierKeyWPF:

                    if (Mode != Mode.ROIGather && GetIfAllBackgroundOperationsAreComplete())
                    {
                        BeginColorPicking();
                    }
                    else
                    {
                        Status = "Drag to the bottom right of the region and release Shift to complete selection...";

                        if (roiSelectionTopLeft == null)
                            roiSelectionTopLeft = GetCurrentMousePositionOverVirtualScreen();
                    }

                    break;

                case DecrementAlphaKeyWPF:
                    
                    Alpha = DeincrementByte(Alpha);

                    break;

                case DeccrementBrightnessKeyWPF:
                    
                    Red = DeincrementByte(Red);
                    Green = DeincrementByte(Green);
                    Blue = DeincrementByte(Blue);

                    break;

                case IncrementAlphaKeyWPF:
                    
                    Alpha = IncrementByte(Alpha);

                    break;

                case IncrementBrightnessKeyWPF:
                    
                    Red = IncrementByte(Red);
                    Green = IncrementByte(Green);
                    Blue = IncrementByte(Blue);

                    break;

                case Key.Escape:
                    
                    if (Mode == Mode.ROIGather)
                    {
                        roiSelectionTopLeft = null;
                        roiSelectionBottomRight = null;
                    }

                    if (roiGatherWorker != null && roiGatherWorker.IsBusy)
                        roiGatherWorker.CancelAsync();

                    if (colorSorterWorker != null && colorSorterWorker.IsBusy)
                        colorSorterWorker.CancelAsync();

                    Mode = defaultMode;

                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case ColorSelectionModesModifierKeyWPF:
                    
                    switch (Mode)
                    {
                        case Mode.Pipette:

                            Mode = defaultMode;
                            break;

                        case Mode.ROIGather:
                            {
                                Mode = defaultMode;
                                roiSelectionBottomRight = GetCurrentMousePositionOverVirtualScreen();

                                if (roiSelectionTopLeft.HasValue)
                                {
                                    var left = Math.Min(roiSelectionBottomRight.Value.X, roiSelectionTopLeft.Value.X);
                                    var right = Math.Max(roiSelectionBottomRight.Value.X, roiSelectionTopLeft.Value.X);
                                    var top = Math.Min(roiSelectionBottomRight.Value.Y, roiSelectionTopLeft.Value.Y);
                                    var bottom = Math.Max(roiSelectionBottomRight.Value.Y, roiSelectionTopLeft.Value.Y);

                                    GatherAllPixelsInROI(top, left, bottom, right);
                                }

                                roiSelectionTopLeft = null;
                                roiSelectionBottomRight = null;

                                break;
                            }
                    }

                    break;
            }
        }

        private void ColoursGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var visual = sender as Visual;

            if (visual == null)
                return;

            var hitTestResult = VisualTreeHelper.HitTest(visual, Mouse.GetPosition(sender as FrameworkElement));

            if (!(hitTestResult?.VisualHit is Rectangle rectangle))
                return;

            var brush = rectangle.Fill as SolidColorBrush;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (Mode == Mode.Delete)
                {
                    RemoveRectangle(rectangle);
                }
                else
                {
                    if (brush != null)
                    {
                        switch (ColorSpace)
                        {
                            case ColorSpace.ARGB:
                                
                                Alpha = brush.Color.A;
                                Red = brush.Color.R;
                                Green = brush.Color.G;
                                Blue = brush.Color.B;

                                break;

                            case ColorSpace.CMYK:
                                
                                var cymkColor = brush.Color.ToCMYK();
                                Cyan = cymkColor.Cyan * 100;
                                Magenta = cymkColor.Magenta * 100;
                                Yellow = cymkColor.Yellow * 100;
                                K = cymkColor.Key * 100;

                                break;

                            case ColorSpace.HSV:

                                var hsvColor = brush.Color.ToHSV();
                                Hue = hsvColor.Hue * 100;
                                Saturation = hsvColor.Saturation * 100;
                                Value = hsvColor.Value * 100;

                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (brush != null)
                    ShowColorInfoWindow(brush.Color);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(statusBar);
        }

        private void IncrementAlphaButton_Click(object sender, RoutedEventArgs e)
        {
            Alpha = IncrementByte(Alpha);
        }

        private void IncrementRedButton_Click(object sender, RoutedEventArgs e)
        {
            Red = IncrementByte(Red);
        }

        private void IncrementGreenButton_Click(object sender, RoutedEventArgs e)
        {
            Green = IncrementByte(Green);
        }

        private void IncrementBlueButton_Click(object sender, RoutedEventArgs e)
        {
            Blue = IncrementByte(Blue);
        }

        private void DecrementBlueButton_Click(object sender, RoutedEventArgs e)
        {
            Blue = DeincrementByte(Blue);
        }

        private void DecrementGreenButton_Click(object sender, RoutedEventArgs e)
        {
            Green = DeincrementByte(Green);
        }

        private void DecrementRedButton_Click(object sender, RoutedEventArgs e)
        {
            Red = DeincrementByte(Red);
        }

        private void DecrementAlphaButton_Click(object sender, RoutedEventArgs e)
        {
            Alpha = DeincrementByte(Alpha);
        }

        private void IncrementCyanButton_Click(object sender, RoutedEventArgs e)
        {
            Cyan = IncrementDouble(Cyan);
        }

        private void IncrementYellowButton_Click(object sender, RoutedEventArgs e)
        {
            Yellow = IncrementDouble(Yellow);
        }

        private void IncrementKeyButton_Click(object sender, RoutedEventArgs e)
        {
            K = IncrementDouble(K);
        }

        private void IncrementMagentaButton_Click(object sender, RoutedEventArgs e)
        {
            Magenta = IncrementDouble(Magenta);
        }

        private void DecrementCyanButton_Click(object sender, RoutedEventArgs e)
        {
            Cyan = DecrementDouble(Cyan);
        }

        private void DecrementMagentaButton_Click(object sender, RoutedEventArgs e)
        {
            Magenta = DecrementDouble(Magenta);
        }

        private void DecrementYellowButton_Click(object sender, RoutedEventArgs e)
        {
            Yellow = DecrementDouble(Yellow);
        }

        private void DecrementKeyButton_Click(object sender, RoutedEventArgs e)
        {
            K = DecrementDouble(K);
        }

        private void IncrementHueButton_Click(object sender, RoutedEventArgs e)
        {
            Hue = IncrementDouble(Hue);
        }

        private void IncrementSaturationButton_Click(object sender, RoutedEventArgs e)
        {
            Saturation = IncrementDouble(Saturation);
        }

        private void IncrementValueButton_Click(object sender, RoutedEventArgs e)
        {
            Value = IncrementDouble(Value);
        }

        private void DecrementHueButton_Click(object sender, RoutedEventArgs e)
        {
            Hue = DecrementDouble(Hue);
        }

        private void DecrementSaturationButton_Click(object sender, RoutedEventArgs e)
        {
            Saturation = DecrementDouble(Saturation);
        }

        private void DecrementValueButton_Click(object sender, RoutedEventArgs e)
        {
            Value = DecrementDouble(Value);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ShowColorInfoWindow(Color.FromArgb(Alpha, Red, Green, Blue));
        }

#region MenuItems

        private void windowNameItem_Click(object sender, RoutedEventArgs e)
        {
            // re-find process to get handle

            // get item
            var item = sender as MenuItem;

            // get all processes
            var procs = Process.GetProcesses();

            // itterate all processes
            foreach (var proc in procs)
                // check names
                if (proc.ProcessName == item.Header.ToString())
                {
                    // get handle
                    var handle = proc.MainWindowHandle;

                    // bring to front
                    DesktopHelper.SetForegroundWindow(handle.ToInt32());

                    // create rectangle
                    var rectangle = new DesktopHelper.RECT();

                    // get window rectangle
                    if (DesktopHelper.GetWindowRect(handle, out rectangle))
                        // capture roi
                        GatherAllPixelsInROI(rectangle.Top, rectangle.Left, rectangle.Bottom, rectangle.Right);

                    // get helper for interop
                    var helper = new WindowInteropHelper(this);

                    // bring this back to foreground
                    DesktopHelper.SetForegroundWindow(helper.Handle.ToInt32());

                    // break out
                    break;
                }
        }

        private void gatherMenuItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // if the sub menu is not already opening (could be selecting a window..)
            if (!gatherFromWindowMenuItem.IsSubmenuOpen)
                // popuplate window
                populateWindowsSubMenuWithOpenWindows();
        }

        private void gatherMenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            // if the sub menu is not already opening (could be selecting a window..)
            if (!gatherFromWindowMenuItem.IsSubmenuOpen)
                // popuplate window
                populateWindowsSubMenuWithOpenWindows();
        }

#endregion

#endregion

        #region CommandCallbacks

        private void ImportComandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowImportDialog();
        }

        private void ExportCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ShowExportDialog();
        }

        private void ExitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void PopulateBalancedRedsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            Populate(PopulationModes.Reds);
        }

        private void PopulateBalancedGreensCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.Greens);
        }

        private void PopulateBalancedBuesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.Blues);
        }

        private void PopulateBalancedCyansCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.Cyans);
        }

        private void PopulateBalancedMagentasCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.Magentas);
        }

        private void PopulateBalancedYellowsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.Yellows);
        }

        private void PopulateGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.Grayscale);
        }

        private void PopulateSystemColoursCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.SystemColors);
        }

        private void SortRGBCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SortColors(new ARGBSorter().Sort);
        }

        private void SortCMYCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SortColors(new CMYKSorter().Sort);
        }

        private void SortHSVCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SortColors(new HSVSorter().Sort);
        }

        private void SortGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SortColors(new GreyscaleSorter().Sort);
        }

        private void randomizeCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SortColors(new RandomSorter(RandomGenerator).Sort);
        }

        private void DiscardNonDominantRGBCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Filter(ChannelDominanceModes.NonDominantRGB, ChannelDominance);
        }

        private void DiscardDominantRGBCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Filter(ChannelDominanceModes.DominantRGB, ChannelDominance);
        }

        private void DiscardNonDominantCMYCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Filter(ChannelDominanceModes.NonDominantCMY, ChannelDominance);
        }

        private void DiscardDominantCMYCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Filter(ChannelDominanceModes.DominantCMY, ChannelDominance);
        }

        private void DiscardNonDominantGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Filter(ChannelDominanceModes.NonGray, 0.0d);
        }

        private void DiscardDominantGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Filter(ChannelDominanceModes.Gray, 0.0d);
        }

        private void DiscardDuplicatesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemoveDuplicateColors();
        }

        private void RemoveLastCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearLast();
        }

        private void RemovalAllCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clear();
        }

        public void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var about = new AboutWindow { Owner = this };

            if (Top + ActualHeight >= SystemParameters.PrimaryScreenHeight)
                about.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            about.ShowDialog();
        }

        private void AddCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AddColor(Color.FromArgb(Alpha, Red, Green, Blue));
        }

        private void GatherROICommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Mode = Mode.ROIGather;
        }

        private void GatherFullScreenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GatherAllPixelsInROI(0, 0, SystemInformation.VirtualScreen.Height, SystemInformation.VirtualScreen.Width);
        }

        private void PopulartePresentationFrameworkCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Populate(PopulationModes.PresentationFramework);
        }

        public void SelectiveRemovalCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Mode = Mode.Delete;
        }

        #endregion
    }

#region Converters

    /// <summary>
    /// Converts a Double to a Visibility. The Double provided as the value is compared to the Double provided as the parameter. If the value >= the parameter Visibility.Visible is returned, else Visibility.Hidden is returned
    /// </summary>
    [ValueConversion(typeof(double), typeof(Visibility))]
    public class DoubleHeightGreaterThatParameterToVisibilityConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value as a double
            double valueAsDouble;

            // hold parameter as a double
            double parameterAsDouble;

            // parse values
            if (value != null &&
                double.TryParse(value.ToString(), out valueAsDouble) &&
                parameter != null &&
                double.TryParse(parameter.ToString(), out parameterAsDouble))
                // parse
                return valueAsDouble >= parameterAsDouble ? Visibility.Visible : Visibility.Hidden;
            return Visibility.Visible;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts bewteen a background colour and a readble foreground colour
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Color))]
    public class BackgroundColorToReadableForegroundColorConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if some value
            if (value != null)
            {
                // get colour
                var background = (Color)ColorConverter.ConvertFromString(value.ToString());

                // if too white
                if (background.A < 125 ||
                    (background.R + background.G + background.B) / 3 > 122.5)
                    // return black
                    return Colors.Black;
                return Colors.White;
            }

            // just use black
            return Colors.Black;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts a Byte to a string representation in hex
    /// </summary>
    [ValueConversion(typeof(byte), typeof(string))]
    public class ByteToHexStringConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value as byte
            byte valueAsByte;

            // if a value that parses
            if (value != null &&
                byte.TryParse(value.ToString(), out valueAsByte))
                // convert to base 16
                return Convert.ToString(valueAsByte, 16).ToUpper();
            return "00";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if a value
            if (value != null)
                try
                {
                    // convert
                    var b = Convert.ToByte(value.ToString().ToUpper(), 16);

                    // return
                    return b;
                }
                catch (InvalidCastException)
                {
                    // return 0 as byte
                    return (byte)0;
                }
                catch (InvalidOperationException)
                {
                    // return 0 as byte
                    return (byte)0;
                }

            return (byte)0;
        }

#endregion
    }

    /// <summary>
    /// Converts a Byte to a percentage of it's maximum value (i.e a percentage of 256)
    /// </summary>
    [ValueConversion(typeof(byte), typeof(double))]
    public class ByteToDoublePercentConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value as byte
            byte valueAsByte;

            // if a value that parses
            if (value != null &&
                byte.TryParse(value.ToString(), out valueAsByte))
                // convert to percent
                return Math.Round(100d / 255d * valueAsByte, 1);
            return 0d;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value as double
            double valueAsDouble;

            // if a value that parses
            if (value != null &&
                double.TryParse(value.ToString(), out valueAsDouble))
                // convert to percent
                return 255d / 100d * valueAsDouble;
            return (byte)0;
        }

#endregion
    }

    /// <summary>
    /// Converts bewteen a Byte and a Boolean, returning true if the byte is different to 255
    /// </summary>
    [ValueConversion(typeof(byte), typeof(bool))]
    public class NotMaxByteToBooleanConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold byte
            byte b;

            // if a byte value
            if (value != null &&
                byte.TryParse(value.ToString(), out b))
                // return result
                return b != 255;
            return false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts bewteen a Byte and a Boolean, returning true if the byte is different to 0
    /// </summary>
    [ValueConversion(typeof(byte), typeof(bool))]
    public class NotMinByteToBooleanConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold byte
            byte b;

            // if a byte value
            if (value != null &&
                byte.TryParse(value.ToString(), out b))
                // return result
                return b != 0;
            return false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts bewteen a Double and a Boolean, returning true if the double is different to 100
    /// </summary>
    [ValueConversion(typeof(byte), typeof(bool))]
    public class NotMaxDoublePercentageToBooleanConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold double
            double d;

            // if a byte value
            if (value != null &&
                double.TryParse(value.ToString(), out d))
                // return result
                return d != 100;
            return false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts bewteen a Double and a Boolean, returning true if the double is different to 0
    /// </summary>
    [ValueConversion(typeof(byte), typeof(bool))]
    public class NotMinDoublePercentageToBooleanConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold double
            double d;

            // if a double value
            if (value != null &&
                double.TryParse(value.ToString(), out d))
                // return result
                return d != 0;
            return false;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts between multiple boolean values to a boolean value using and (&&) logic
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanAndMultiConverter : IMultiValueConverter
    {
#region IMultiValueConverter Members

        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // hold result
            var result = true;

            // itterate all values
            foreach (var o in values)
            {
                // if a boolean
                if (o is bool)
                    // add result
                    result = (bool)o;
                else
                    // as default make false
                    result = false;

                // if a false in there somewhere
                if (result == false)
                    // break - why keep checking
                    return false;
            }

            // return
            return result;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts bewteen an EGridMode and a Boolean value. An EGridMode must be provieded as the value and parameter and, if these values match, true is retruned, else false
    /// </summary>
    [ValueConversion(typeof(GridMode), typeof(bool))]
    public class EGridModeToBooleanConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // if values present
                if (value != null && parameter != null)
                {
                    // get value
                    var valueMode = (GridMode)Enum.Parse(typeof(GridMode), value.ToString());

                    // get parameter
                    var parameterMode = (GridMode)Enum.Parse(typeof(GridMode), parameter.ToString());

                    // return if the same
                    return valueMode == parameterMode;
                }

                // return fail
                return false;
            }
            catch (Exception)
            {
                // return fail
                return false;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // hold value
                bool valueAsBoolean;

                // if a value and a parameter
                if (bool.TryParse(value.ToString(), out valueAsBoolean) &&
                    parameter != null)
                {
                    // get parameter mode
                    var parameterMode = (GridMode)Enum.Parse(typeof(GridMode), parameter.ToString());

                    // if true
                    if (valueAsBoolean)
                        // return mode
                        return parameterMode;
                    return GridMode.MaintainSize;
                }

                // just default
                return GridMode.MaintainSize;
            }
            catch (Exception)
            {
                // just default
                return GridMode.MaintainSize;
            }
        }

#endregion
    }

    /// <summary>
    /// Converts bewteen an Double and a Boolean value. A Double must be provieded as the value and parameter and, if these values match, true is retruned, else false
    /// </summary>
    [ValueConversion(typeof(double), typeof(bool))]
    public class DoubleToBooleanConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // hold value as double
                double valueAsDouble;

                // hold parameter as double
                double parameterAsDouble;

                // if values present
                if (double.TryParse(value.ToString(), out valueAsDouble) &&
                    double.TryParse(parameter.ToString(), out parameterAsDouble))
                    // return if the same
                    return valueAsDouble == parameterAsDouble;
                return false;
            }
            catch (Exception)
            {
                // return fail
                return false;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // hold value
                bool valueAsBoolean;

                // hold parameter as double
                double parameterAsDouble;

                // if a value and a parameter
                if (bool.TryParse(value.ToString(), out valueAsBoolean) &&
                    double.TryParse(parameter.ToString(), out parameterAsDouble))
                {
                    // if true
                    if (valueAsBoolean)
                        // return mode
                        return parameterAsDouble;
                    return 3d;
                }

                // just default
                return 3d;
            }
            catch (Exception)
            {
                // just default
                return 3d;
            }
        }

#endregion
    }

    /// <summary>
    /// Converts between an EGridMode and a VerticalAlignment. EGridMode.MaintainSize will return VerticalAlignment.Top, else VerticalAlignment.Stretch will be returned
    /// </summary>
    [ValueConversion(typeof(GridMode), typeof(VerticalAlignment))]
    public class EGridModeToVerticalAlignmentConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // get mode
                var mode = (GridMode)Enum.Parse(typeof(GridMode), value.ToString());

                // return the correct alignment
                return mode == GridMode.MaintainSize ? VerticalAlignment.Top : VerticalAlignment.Stretch;
            }
            catch (Exception)
            {
                // just return stretch
                return VerticalAlignment.Stretch;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

#endregion
    }

    /// <summary>
    /// Converts between an EColorSpace and a Boolean. A comparative EColorSpace should be provided as the parameter, which is used to comare to the EColorSpace value to return the Boolean result
    /// </summary>
    [ValueConversion(typeof(ColorSpace), typeof(bool))]
    public class EColorSpaceToBooleanConverter : IValueConverter
    {
#region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // get value
                var valueColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), value.ToString());

                // get parameter
                var parameterColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), parameter.ToString());

                // compare and return
                return valueColorSpace == parameterColorSpace;
            }
            catch (NullReferenceException)
            {
                // return fail
                return false;
            }
            catch (InvalidCastException)
            {
                // return fail
                return false;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // get value
                var valueBoolean = bool.Parse(value.ToString());

                // get parameter
                var parameterColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), parameter.ToString());

                // compare and return parameter or standard
                return valueBoolean ? parameterColorSpace : ColorSpace.ARGB;
            }
            catch (NullReferenceException)
            {
                // return standard
                return ColorSpace.ARGB;
            }
            catch (InvalidCastException)
            {
                // return standard
                return ColorSpace.ARGB;
            }
        }

#endregion
    }
}