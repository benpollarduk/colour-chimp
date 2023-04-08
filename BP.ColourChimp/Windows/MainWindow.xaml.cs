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
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BP.ColourChimp.Classes;
using BP.ColourChimp.Converters;
using BP.ColourChimp.Properties;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Windows.Size;
using SystemColors = System.Windows.SystemColors;

namespace BP.ColourChimp.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region StaticProperties

        /// <summary>
        /// Get the modifier key for color selection modes
        /// </summary>
        private const System.Windows.Forms.Keys colorSelectionModesModifierKey = System.Windows.Forms.Keys.Shift;

        /// <summary>
        /// Get the modifier key for color selction modes
        /// </summary>
        private const Key colorSelectionModesModifierKeyWPF = Key.LeftShift;

        /// <summary>
        /// Get the key for increment alpha
        /// </summary>
        private const Key incrementAlphaKeyWPF = Key.Up;

        /// <summary>
        /// Get the key for deincrement alpha
        /// </summary>
        private const Key deincrementAlphaKeyWPF = Key.Down;

        /// <summary>
        /// Get the key for increment brightness
        /// </summary>
        private const Key incrementBrightnessKeyWPF = Key.Right;

        /// <summary>
        /// Get the key for deincrement brightness
        /// </summary>
        private const Key deincrementBrightnessKeyWPF = Key.Left;

        /// <summary>
        /// Get the fixed grid columns
        /// </summary>
        private const double fixedGridColoumns = 3d;

        /// <summary>
        /// Get the random generator
        /// </summary>
        private static readonly Random randomGenerator = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the alpha. This is a dependency property
        /// </summary>
        public byte Alpha
        {
            get { return (byte)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }

        /// <summary>
        /// Get or set the red. This is a dependency property
        /// </summary>
        public byte Red
        {
            get { return (byte)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }

        /// <summary>
        /// Get or set the green. This is a dependency property
        /// </summary>
        public byte Green
        {
            get { return (byte)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }

        /// <summary>
        /// Get or set the blue. This is a dependency property
        /// </summary>
        public byte Blue
        {
            get { return (byte)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }

        /// <summary>
        /// Get or set the cyan. This is specified as a percentage. This is a dependency property
        /// </summary>
        public double Cyan
        {
            get { return (double)GetValue(CyanProperty); }
            set { SetValue(CyanProperty, value); }
        }

        /// <summary>
        /// Get or set the magenta. This is specified as a percentage. This is a dependency property
        /// </summary>
        public double Magenta
        {
            get { return (double)GetValue(MagentaProperty); }
            set { SetValue(MagentaProperty, value); }
        }

        /// <summary>
        /// Get or set the yellow. This is specified as a percentage. This is a dependency property
        /// </summary>
        public double Yellow
        {
            get { return (double)GetValue(YellowProperty); }
            set { SetValue(YellowProperty, value); }
        }

        /// <summary>
        /// Get or set the key. This is specified as a percentage. This is a dependency property
        /// </summary>
        public double K
        {
            get { return (double)GetValue(KProperty); }
            set { SetValue(KProperty, value); }
        }

        /// <summary>
        /// Get or set the hue. This is specified as a percentage. This is a dependency property
        /// </summary>
        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        /// <summary>
        /// Get or set the saturation. This is specified as a percentage. This is a dependency property
        /// </summary>
        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        /// <summary>
        /// Get or set the value. This is specified as a percentage. This is a dependency property
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Get the current status. This is a dependency property
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            private set { SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// Get or set the mode. This is a dependency property
        /// </summary>
        public Mode Mode
        {
            get { return (Mode)GetValue(ModeProperty); }
            private set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Get if this control has some children colors. This is a dependency property
        /// </summary>
        public bool HasChildrenColors
        {
            get { return (bool)GetValue(HasChildrenColorsProperty); }
            private set { SetValue(HasChildrenColorsProperty, value); }
        }

        /// <summary>
        /// Get if all background operations are idle. This is a dependency property
        /// </summary>
        public bool AllBackgroundOperationsIdle
        {
            get { return (bool)GetValue(AllBackgroundOperationsIdleProperty); }
            private set { SetValue(AllBackgroundOperationsIdleProperty, value); }
        }

        /// <summary>
        /// Get or set the value used to determine channel dominance as a normalised value (0.0-1.0). This is a dependency property
        /// </summary>
        public double ChannelDominance
        {
            get { return (double)GetValue(ChannelDominanceProperty); }
            set { SetValue(ChannelDominanceProperty, value); }
        }

        /// <summary>
        /// Get the amount of colors currently displayed. This is a dependency property
        /// </summary>
        public double ColorCount
        {
            get { return (double)GetValue(ColorCountProperty); }
            private set { SetValue(ColorCountProperty, value); }
        }

        /// <summary>
        /// Get of set if alpha is maxed out on previews. This is a dependency property
        /// </summary>
        public bool MaxOutAlphaOnPreview
        {
            get { return (bool)GetValue(MaxOutAlphaOnPreviewProperty); }
            set { SetValue(MaxOutAlphaOnPreviewProperty, value); }
        }

        /// <summary>
        /// Get of set the grid mode. This is a dependency property
        /// </summary>
        public GridMode GridMode
        {
            get { return (GridMode)GetValue(GridModeProperty); }
            set { SetValue(GridModeProperty, value); }
        }

        /// <summary>
        /// Get of set the number of fixed columns. This is only applicable when the GridMode property is set to MaintainSize. This is a dependency property
        /// </summary>
        public double FixedColumns
        {
            get { return (double)GetValue(FixedColumnsProperty); }
            set { SetValue(FixedColumnsProperty, value); }
        }

        /// <summary>
        /// Get of set the color space. This is only applicable when the GridMode property is set to MaintainSize. This is a dependency property
        /// </summary>
        public ColorSpace ColorSpace
        {
            get { return (ColorSpace)GetValue(ColorSpaceProperty); }
            private set { SetValue(ColorSpaceProperty, value); }
        }

        /// <summary>
        /// Get or set the ROI top left
        /// </summary>
        private Point? roiSelectionTopLeft;

        /// <summary>
        /// Get or set the ROI bottom right
        /// </summary>
        private Point? roiSelectionBottomRight;

        /// <summary>
        /// Get or set the ROI gathering worker
        /// </summary>
        private BackgroundWorker roiGatherWorker;

        /// <summary>
        /// Get or set the color arangement worker
        /// </summary>
        private BackgroundWorker colorArangmentWorker;

        /// <summary>
        /// Get or set the RGB to CMYK converter
        /// </summary>
        private readonly IValueConverter rgb2cmykConverter = new RGBToCMYKConverter();

        /// <summary>
        /// Get or set the RGB to HSV converter
        /// </summary>
        private readonly IValueConverter rgb2hsvConverter = new RGBToHSVConverter();

        /// <summary>
        /// Get or set the default mode
        /// </summary>
        private readonly Mode defaultMode = Mode.Add;

        /// <summary>
        /// Get or set the default path
        /// </summary>
        private string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Indetifies the MainWindow.Alpha property
        /// </summary>
        public static readonly DependencyProperty AlphaProperty = DependencyProperty.Register("Alpha", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Red property
        /// </summary>
        public static readonly DependencyProperty RedProperty = DependencyProperty.Register("Red", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Green property
        /// </summary>
        public static readonly DependencyProperty GreenProperty = DependencyProperty.Register("Green", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Blue property
        /// </summary>
        public static readonly DependencyProperty BlueProperty = DependencyProperty.Register("Blue", typeof(byte), typeof(MainWindow), new PropertyMetadata((byte)255, OnSharedARGBPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Cyan property
        /// </summary>
        public static readonly DependencyProperty CyanProperty = DependencyProperty.Register("Cyan", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Magenta property
        /// </summary>
        public static readonly DependencyProperty MagentaProperty = DependencyProperty.Register("Magenta", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Yellow property
        /// </summary>
        public static readonly DependencyProperty YellowProperty = DependencyProperty.Register("Yellow", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.K property
        /// </summary>
        public static readonly DependencyProperty KProperty = DependencyProperty.Register("K", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedCMYKPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Hue property
        /// </summary>
        public static readonly DependencyProperty HueProperty = DependencyProperty.Register("Hue", typeof(double), typeof(MainWindow), new PropertyMetadata(100d, OnSharedHSVPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Saturation property
        /// </summary>
        public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register("Saturation", typeof(double), typeof(MainWindow), new PropertyMetadata(0d, OnSharedHSVPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Value property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(MainWindow), new PropertyMetadata(100d, OnSharedHSVPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.Status property
        /// </summary>
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(string), typeof(MainWindow), new PropertyMetadata("Ready"));

        /// <summary>
        /// Indetifies the MainWindow.HasChildrenColors property
        /// </summary>
        public static readonly DependencyProperty HasChildrenColorsProperty = DependencyProperty.Register("HasChildrenColors", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        /// <summary>
        /// Indetifies the MainWindow.AllBackgroundOperationsIdle property
        /// </summary>
        public static readonly DependencyProperty AllBackgroundOperationsIdleProperty = DependencyProperty.Register("AllBackgroundOperationsIdle", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        /// <summary>
        /// Indetifies the MainWindow.ChannelDominance property
        /// </summary>
        public static readonly DependencyProperty ChannelDominanceProperty = DependencyProperty.Register("ChannelDominance", typeof(double), typeof(MainWindow), new PropertyMetadata(0.4d));

        /// <summary>
        /// Indetifies the MainWindow.ColorCount property
        /// </summary>
        public static readonly DependencyProperty ColorCountProperty = DependencyProperty.Register("ColorCount", typeof(double), typeof(MainWindow), new PropertyMetadata(0d));

        /// <summary>
        /// Indetifies the MainWindow.Mode property
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(Mode), typeof(MainWindow), new PropertyMetadata(Mode.Add, OnModePropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.MaxOutAlphaOnPreview property
        /// </summary>
        public static readonly DependencyProperty MaxOutAlphaOnPreviewProperty = DependencyProperty.Register("MaxOutAlphaOnPreview", typeof(bool), typeof(MainWindow), new PropertyMetadata(FullScreenPreviewWindow.MaxOutAlphaOnPreview, OnMaxOutAlphaOnPreviewPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.GridMode property
        /// </summary>
        public static readonly DependencyProperty GridModeProperty = DependencyProperty.Register("GridMode", typeof(GridMode), typeof(MainWindow), new PropertyMetadata(GridMode.FitToArea, OnGridModePropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.FixedColumns property
        /// </summary>
        public static readonly DependencyProperty FixedColumnsProperty = DependencyProperty.Register("FixedColumns", typeof(double), typeof(MainWindow), new PropertyMetadata(fixedGridColoumns, OnFixedColumnsPropertyChanged));

        /// <summary>
        /// Indetifies the MainWindow.ColorSpace property
        /// </summary>
        public static readonly DependencyProperty ColorSpaceProperty = DependencyProperty.Register("ColorSpace", typeof(ColorSpace), typeof(MainWindow), new PropertyMetadata(ColorSpace.ARGB));

        #endregion

        #region Methods

        /// <summary>
        /// Initialize a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // load settings
            loadSettingsFromManifest();
        }

        /// <summary>
        /// Load all settings from the manifest
        /// </summary>
        private void loadSettingsFromManifest()
        {
            try
            {
                // set topmost
                Topmost = Settings.Default.KeepApplicationInForeground;

                // set fixed columns
                FixedColumns = Settings.Default.Columns;

                // set if maxing out
                MaxOutAlphaOnPreview = Settings.Default.UseMaximumAlphaOnPreview;

                // set grid mode
                GridMode = (GridMode)Enum.Parse(typeof(GridMode), Settings.Default.GridMode.ToString());

                // set color space
                ColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), Settings.Default.ColorSpace.ToString());
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("It appears that you have no configuration file, a new one will be generated when Colour Chimp exits", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidCastException)
            {
                // display
                var result = MessageBox.Show("It appears that your configuration file is corrupted. Would you like to create a new configuration file now?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

                // handle result
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            // reset all settings
                            updateManifestProperties();

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Update all manifest properties
        /// </summary>
        private void updateManifestProperties()
        {
            // set keep in foreground
            Settings.Default.KeepApplicationInForeground = Topmost;

            // set columns
            Settings.Default.Columns = (int)FixedColumns;

            // set if maxing out
            Settings.Default.UseMaximumAlphaOnPreview = MaxOutAlphaOnPreview;

            // set grid mode property
            Settings.Default.GridMode = GridMode.ToString();

            // set repetition pause property
            Settings.Default.ColorSpace = ColorSpace.ToString();

            // save settings
            Settings.Default.Save();
        }

        /// <summary>
        /// Clear all colors from the grid
        /// </summary>
        public void Clear()
        {
            // if some children
            if (coloursGrid.Children.Count > 0)
            {
                // get rectangles
                var rectangles = new List<Rectangle>(coloursGrid.Children.Cast<Rectangle>());

                // itterate all children
                foreach (var r in rectangles)
                    // remove
                    removeRectangle(r);

                // set status
                Status = "All colours cleared";
            }
        }

        /// <summary>
        /// Clear the last color from the grid
        /// </summary>
        public void ClearLast()
        {
            // check for children
            if (coloursGrid.Children.Count > 0)
                // remove last
                removeRectangle(coloursGrid.Children[coloursGrid.Children.Count - 1] as Rectangle);
        }

        /// <summary>
        /// Show the export dialog
        /// </summary>
        private void showExportDialog()
        {
            // create dialog
            var sFD = new SaveFileDialog();

            // set title
            sFD.Title = "Export image as...";

            // show overwrite prompts
            sFD.OverwritePrompt = true;

            // default extension
            sFD.DefaultExt = "*.bmp";

            // view extensions
            sFD.Filter = "24-bit Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|TIFF (*.tif)|*.tif";

            // set location
            sFD.InitialDirectory = defaultPath;

            // ok accepted
            sFD.FileOk += sFD_FileOk;

            // show
            sFD.ShowDialog();
        }

        /// <summary>
        /// Show the import dialog
        /// </summary>
        private void showImportDialog()
        {
            // create dialog
            var oFD = new OpenFileDialog();

            // set title
            oFD.Title = "Import a image...";

            // default extension
            oFD.DefaultExt = "*.png";

            // view extensions
            oFD.Filter = "Image files (*.bmp, *.gif, *.jpg, *.png, *.tif)|*.bmp;*.gif;*.jpg;*.png;*.tif;";

            // set location
            oFD.InitialDirectory = defaultPath;

            // ok accepted
            oFD.FileOk += oFD_FileOk;

            // show
            oFD.ShowDialog();
        }

        /// <summary>
        /// Export a patchwork of the grid as a png
        /// </summary>
        /// <param name="path">The path of the new file</param>
        /// <returns>The status of the export</returns>
        public bool ExportPatchworkAsPNG(string path)
        {
            // hold background
            var background = coloursGrid.Background;

            try
            {
                // set status
                Status = $"Exporting to {path}...";

                // hold colour count
                var count = (int)ColorCount;

                // get square root of colours
                var sqr = (int)Math.Ceiling(Math.Sqrt(count));

                // create a bitmap
                var opBMP = new Bitmap(sqr, sqr);

                // get all rectangles in the grid
                var rectangles = coloursGrid.Children.OfType<Rectangle>().ToArray();

                // hold index
                var rectangleIndex = 0;

                // itterate rows
                for (var row = 0; row < sqr; row++)
                    // itterate columns
                for (var column = 0; column < sqr; column++)
                {
                    // if a colour
                    if (rectangleIndex < count)
                    {
                        // get rectangle at index
                        var r = rectangles[rectangleIndex];

                        // if a solid fil
                        if (r.Fill is SolidColorBrush)
                        {
                            // get brush
                            var brush = r.Fill as SolidColorBrush;

                            // set pixel
                            opBMP.SetPixel(column, row, System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B));
                        }
                    }
                    else
                    {
                        // set pixel to transparent
                        opBMP.SetPixel(column, row, System.Drawing.Color.Transparent);
                    }

                    // increment index
                    rectangleIndex++;
                }

                // hold format
                ImageFormat format;

                // get ext
                var ext = path.Substring(path.Length - 3);

                // select extension
                switch (ext.ToUpper())
                {
                    case "BMP":
                        {
                            // set format
                            format = ImageFormat.Bmp;

                            break;
                        }
                    case "GIF":
                        {
                            // set format
                            format = ImageFormat.Gif;

                            break;
                        }
                    case "JPG":
                        {
                            // set format
                            format = ImageFormat.Jpeg;

                            break;
                        }
                    case "PNG":
                        {
                            // set format
                            format = ImageFormat.Png;

                            break;
                        }
                    case "TIF":
                        {
                            // set format
                            format = ImageFormat.Tiff;

                            break;
                        }
                    default:
                        {
                            // not implemented yet
                            throw new NotImplementedException();
                        }
                }

                // save bmp
                opBMP.Save(path, format);

                #region RenderedBitmap

                /*
                // save current canvas transform
                Transform transform = this.coloursGrid.LayoutTransform;

                // reset current transform incase of scalling or rotating
                this.coloursGrid.LayoutTransform = null;

                // set background so that we have a solid, non transparent base
                this.coloursGrid.Background = Brushes.White;

                // get size of canvas
                Size size = new Size(this.coloursGrid.ActualWidth, this.coloursGrid.ActualHeight);

                // measure and arrange the canvas
                this.coloursGrid.Measure(size);

                // arrange the surface - this line is what causes the transition jitter...
                this.coloursGrid.Arrange(new Rect(size));

                // craete and render surface and push bitmap to it
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((Int32)size.Width, (Int32)size.Height, 96d, 96d, PixelFormats.Pbgra32);

                // now render surface to bitmap
                renderBitmap.Render(this.coloursGrid);

                // hold encoder
                BitmapEncoder encoder = null;

                // get ext
                String ext = path.Substring(path.Length - 3);

                // select extension
                switch (ext.ToUpper())
                {
                    case ("BMP"):
                        {
                            // create encoder
                            encoder = new BmpBitmapEncoder();

                            break;
                        }
                    case ("GIF"):
                        {
                            // create encoder
                            encoder = new GifBitmapEncoder();

                            break;
                        }
                    case ("JPG"):
                        {
                            // create encoder
                            encoder = new JpegBitmapEncoder();

                            break;
                        }
                    case ("PNG"):
                        {
                            // create encoder
                            encoder = new PngBitmapEncoder();

                            break;
                        }
                    case ("TIF"):
                        {
                            // create encoder
                            encoder = new TiffBitmapEncoder();

                            break;
                        }
                    default:
                        {
                            // not implemented yet
                            throw new NotImplementedException();
                        }
                }

                // puch rendered bitmap into it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                // set source of image as frame
                BitmapFrame image = encoder.Frames[0];

                // restore previously saved layout
                this.coloursGrid.LayoutTransform = transform;

                // create stream
                FileStream stream = new FileStream(path, FileMode.Create);

                // save the file
                encoder.Save(stream);

                // ensure closed
                stream.Close();

                // release stream
                stream = null;

                // release encoder
                encoder = null;

                */

                #endregion

                // set status
                Status = $"Exported to: {path}";

                // pass
                return true;
            }
            catch (Exception e)
            {
                // display
                MessageBox.Show($"An error occured while saving the file: {e.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // set status
                Status = "Export failed";

                // fail
                return false;
            }
            finally
            {
                // reset background
                coloursGrid.Background = background;
            }
        }

        /// <summary>
        /// Import an image
        /// </summary>
        /// <param name="path">The path of the image to open</param>
        public void ImportImage(string path)
        {
            // open file

            try
            {
                // get ext
                var ext = path.Substring(path.Length - 3);

                // hold decoder
                BitmapDecoder decoder = null;

                // create encoder for re-encoding of image
                BitmapEncoder encoder;

                // set status
                Status = $"Importing {path}...";

                // select extension
                switch (ext.ToUpper())
                {
                    case "BMP":
                        {
                            // create decoder
                            decoder = new BmpBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

                            // create reencoder
                            encoder = new BmpBitmapEncoder();

                            break;
                        }
                    case "GIF":
                        {
                            // create decoder
                            decoder = new GifBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

                            // create reencoder
                            encoder = new GifBitmapEncoder();

                            break;
                        }
                    case "JPG":
                        {
                            // create decoder
                            decoder = new JpegBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

                            // create reencoder
                            encoder = new JpegBitmapEncoder();

                            break;
                        }
                    case "PNG":
                        {
                            // create decoder
                            decoder = new PngBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

                            // create reencoder
                            encoder = new PngBitmapEncoder();

                            break;
                        }
                    case "TIF":
                        {
                            // create decoder
                            decoder = new TiffBitmapDecoder(new Uri(path, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

                            // create reencoder
                            encoder = new TiffBitmapEncoder();

                            break;
                        }
                    default:
                        {
                            // not implemented yet
                            throw new NotImplementedException();
                        }
                }

                // get frame
                var frame = decoder.Frames[0];

                // add the image as a frame
                encoder.Frames.Add(frame);

                // holds the image in a stream
                var imageInStream = new MemoryStream();

                // save to stream
                encoder.Save(imageInStream);

                // create the old scool win forms bitmap
                var winFormsBitmap = new Bitmap(imageInStream);

                // open file
                gatherAllPixelsInROI(0, 0, winFormsBitmap.Height, winFormsBitmap.Width, winFormsBitmap);

                // close the stream
                imageInStream.Close();

                // release stream
                imageInStream = null;

                // release encoder
                encoder = null;

                // set status
                Status = "Import complete";
            }
            catch (Exception e)
            {
                // display
                MessageBox.Show($"There was an error importing the image: {e.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // set status
                Status = "Import failed";
            }
        }

        /// <summary>
        /// Begin color picking
        /// </summary>
        private void beginColorPicking()
        {
            // color picking
            Mode = Mode.Pipette;

            // get the desktop
            var bmp = GetVirtualScreenBitmap();

            // hold if there was an error
            var wasError = false;

            // queue callback
            ThreadPool.QueueUserWorkItem(state =>
            {
                do
                {
                    // keep going while mouse down or modifier key down and no error

                    // update picking
                    wasError = !(bool)Dispatcher.Invoke(new BitmapCallback(updateColorToPixelAtCurrentPoint), bmp);

                    // if no error
                    if (!wasError)
                        // wait to keep sampling down
                        Thread.Sleep(10);
                } while ((System.Windows.Forms.Control.MouseButtons == System.Windows.Forms.MouseButtons.Left || System.Windows.Forms.Control.ModifierKeys == colorSelectionModesModifierKey) &&
                         !wasError);

                // if errored out
                if (wasError)
                    // dispatch update
                    Dispatcher.BeginInvoke(new LambdaCallback(() =>
                    {
                        // display error to user only
                        MessageBox.Show("There was an error picking the colour", "Pick Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));

                // handle end of picking
                Dispatcher.BeginInvoke(new LambdaCallback(() => Mode = Mode.Add));
            }, bmp);
        }

        /// <summary>
        /// Get the mouse position over the virtual screen
        /// </summary>
        /// <returns></returns>
        private Point getCurrentMousePositionOverVirtualScreen()
        {
            // get point
            Point p = System.Windows.Forms.Control.MousePosition;

            // translate
            p.X += Math.Abs(SystemInformation.VirtualScreen.Left);
            p.Y += Math.Abs(SystemInformation.VirtualScreen.Top);

            // return translated pos
            return p;
        }

        /// <summary>
        /// Get a bitmap of the entire virtual sceen area
        /// </summary>
        /// <returns></returns>
        public Bitmap GetVirtualScreenBitmap()
        {
            // determine the size of the "virtual screen", which includes all monitor
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            // create a bitmap of the appropriate size to receive the screenshot
            var bmp = new Bitmap(screenWidth, screenHeight);

            // draw the screenshot into our bitmap
            using (var g = Graphics.FromImage(bmp))
            {
                // copy graphics
                g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
            }

            // return bitmap
            return bmp;
        }

        /// <summary>
        /// Get a bitmap of a region in the virtual sceen area
        /// </summary>
        /// <param name="region">The region over the screen to gather</param>
        /// <returns></returns>
        public Bitmap GetVirtualScreenBitmap(Int32Rect region)
        {
            // create a bitmap of the appropriate size to receive the screenshot
            var bmp = new Bitmap(region.Width, region.Height);

            // draw the screenshot into our bitmap
            using (var g = Graphics.FromImage(bmp))
            {
                // copy graphics
                g.CopyFromScreen(region.X, region.Y, 0, 0, bmp.Size);
            }

            // return bitmap
            return bmp;
        }

        /// <summary>
        /// Update the color to the pixel at the current point
        /// </summary>
        /// <param name="bmp">The bitmap to interrogate</param>
        /// <returns>True if the operation passes, else false</returns>
        private bool updateColorToPixelAtCurrentPoint(Bitmap bmp)
        {
            try
            {
                // if a bmp
                if (bmp != null)
                {
                    // get cursor position
                    var cursorPosition = getCurrentMousePositionOverVirtualScreen();

                    // if a cursor position
                    if (cursorPosition != null)
                    {
                        // get color
                        var c = bmp.GetPixel(cursorPosition.X, cursorPosition.Y);

                        // if a color
                        if (c != null)
                        {
                            // depends on color space
                            switch (ColorSpace)
                            {
                                case ColorSpace.ARGB:
                                    {
                                        // set channels
                                        Alpha = c.A;
                                        Red = c.R;
                                        Green = c.G;
                                        Blue = c.B;

                                        break;
                                    }
                                case ColorSpace.CMYK:
                                    {
                                        // get cmyk 
                                        var cymkColor = rgb2cmykConverter.Convert(Color.FromArgb(c.A, c.R, c.G, c.B), typeof(CMYKColor), null, null) as CMYKColor;

                                        // update cmyk
                                        Cyan = cymkColor.Cyan * 100;
                                        Magenta = cymkColor.Magenta * 100;
                                        Yellow = cymkColor.Yellow * 100;
                                        K = cymkColor.Key * 100;

                                        break;
                                    }
                                case ColorSpace.HSV:
                                    {
                                        // get hsv
                                        var hsvColor = rgb2hsvConverter.Convert(Color.FromArgb(c.A, c.R, c.G, c.B), typeof(HSVColor), null, null) as HSVColor;

                                        // update hsv
                                        Hue = hsvColor.Hue * 100;
                                        Saturation = hsvColor.Saturation * 100;
                                        Value = hsvColor.Value * 100;

                                        break;
                                    }
                                default:
                                    {
                                        throw new NotImplementedException();
                                    }
                            }

                            // pass
                            return true;
                        }
                    }
                }

                // failure
                return false;
            }
            catch (Exception)
            {
                // fail
                return false;
            }
        }

        /// <summary>
        /// Increment a byte by 16
        /// </summary>
        /// <param name="b">The byte to increment</param>
        /// <returns></returns>
        private byte incrementByte(byte b)
        {
            // if less than 239
            if (b < 239)
                // increase
                b += 10;
            else
                // max out
                b = 255;

            // return
            return b;
        }

        /// <summary>
        /// Deincrement a byte by 16
        /// </summary>
        /// <param name="b">The byte to deincrement</param>
        /// <returns></returns>
        private byte deincrementByte(byte b)
        {
            // if above 15
            if (b > 15)
                // decrease
                b -= 16;
            else
                // min out
                b = 0;

            // return
            return b;
        }

        /// <summary>
        /// Increment a double by 10
        /// </summary>
        /// <param name="d">The double to increment</param>
        /// <returns></returns>
        private double incrementDouble(double d)
        {
            // if less than 90
            if (d < 90)
                // increase
                d += 10;
            else
                // max out
                d = 100;

            // return
            return d;
        }

        /// <summary>
        /// Deincrement a double by 10
        /// </summary>
        /// <param name="d">The double to deincrement</param>
        /// <returns></returns>
        private double deincrementDouble(double d)
        {
            // if above 10
            if (d > 10)
                // decrease
                d -= 10;
            else
                // min out
                d = 0;

            // return
            return d;
        }

        /// <summary>
        /// Capture all pixels in a selected region of interest
        /// </summary>
        /// <param name="top">The top coordinate</param>
        /// <param name="left">
        /// <The left coordinate
        /// </param>
        /// <param name="bottom">The bottom coordinate</param>
        /// <param name="right">The right coordinate</param>
        private void gatherAllPixelsInROI(int top, int left, int bottom, int right)
        {
            try
            {
                // get the desktop
                var bmp = GetVirtualScreenBitmap();

                // gather
                gatherAllPixelsInROI(top, left, bottom, right, bmp);
            }
            catch (Exception e)
            {
                // display error to user only
                MessageBox.Show($"There was an error gathering screen region pixels {e.Message}", "Gather Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Capture all pixels in a selected region of interest
        /// </summary>
        /// <param name="top">The top coordinate</param>
        /// <param name="left">The left coordinate</param>
        /// <param name="bottom">The bottom coordinate</param>
        /// <param name="right">The right coordinate</param>
        /// <param name="bitmap">The bitmap to gather from</param>
        private void gatherAllPixelsInROI(int top, int left, int bottom, int right, Bitmap bitmap)
        {
            try
            {
                // set staus
                Status = "Beginning screen region gather...";

                // if gatherer running
                if (roiGatherWorker != null)
                    // cancel
                    roiGatherWorker.CancelAsync();

                // create new worker
                roiGatherWorker = new BackgroundWorker();

                // support cancel
                roiGatherWorker.WorkerSupportsCancellation = true;

                // not idle
                AllBackgroundOperationsIdle = false;

                // set worker work handler
                roiGatherWorker.DoWork += (sender, args) =>
                {
                    // get the bmp from the argument property of the event args
                    var capture = args.Argument as Bitmap;

                    // holds list of pixels found
                    var pixels = new List<string>();

                    // hold percentage complete
                    double percentageComplete = 0;

                    try
                    {
                        // if incorrectly entered
                        if (right < left)
                        {
                            var temp = left;
                            left = right;
                            right = temp;
                        }

                        // if incorrectly entered
                        if (bottom < top)
                        {
                            var temp = top;
                            top = bottom;
                            bottom = temp;
                        }

                        // create new size
                        var size = new Size(right - left, bottom - top);

                        // itterate all rows
                        for (var row = top; row < bottom; row++)
                        {
                            // itterate all columns
                            for (var column = left; column < right; column++)
                                // if not cancelled
                                if (!roiGatherWorker.CancellationPending)
                                {
                                    // get pixel
                                    var pixel = bitmap.GetPixel(column, row);

                                    // hold id of pixel
                                    var id = $"{pixel.A}{pixel.R}{pixel.G}{pixel.B}";

                                    // if color not already used
                                    if (!pixels.Contains(id))
                                        // invoke adding of colour on UI thread
                                        Dispatcher.Invoke(new LambdaCallback(() =>
                                        {
                                            // add color
                                            addColor(Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));

                                            // add id
                                            pixels.Add(id);
                                        }), DispatcherPriority.Background);
                                }
                                else
                                {
                                    // return thread
                                    return;
                                }

                            // update percentage
                            percentageComplete = 100d / size.Height * (row - top);

                            // update status
                            Dispatcher.Invoke(new LambdaCallback(() => Status = $"Gathering colors in ROI {Math.Round(percentageComplete, 1)}% complete, row {row + 1 - top} of {size.Height}"), DispatcherPriority.Normal);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // just display
                        Dispatcher.BeginInvoke(new LambdaCallback(() =>
                        {
                            // display error to user only, probably window is minimized
                            MessageBox.Show("Screen region is not valid. Check that window is not minimized", "Gather Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                    }
                    catch (Exception e)
                    {
                        // just display
                        Dispatcher.BeginInvoke(new LambdaCallback(() =>
                        {
                            // display error to user only
                            MessageBox.Show($"There was an error gathering screen region: {e.Message}", "Gather Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }));
                    }
                    finally
                    {
                        // update status
                        Dispatcher.Invoke(new LambdaCallback(() => Status = $"Screen region gather finished, {pixels.Count} colours were found"));

                        // release worker
                        roiGatherWorker = null;

                        // set if all operations are idle
                        Dispatcher.Invoke(new LambdaCallback(() => AllBackgroundOperationsIdle = getIfAllBackgroundOperationsAreComplete()));
                    }
                };

                // do work to do ^
                roiGatherWorker.RunWorkerAsync(bitmap);
            }
            catch (Exception e)
            {
                // display error to user only
                MessageBox.Show($"There was an error gathering screen reigion: {e.Message}", "Gather Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Get if all background operations are complete
        /// </summary>
        /// <returns></returns>
        private bool getIfAllBackgroundOperationsAreComplete()
        {
            return colorArangmentWorker == null &&
                   roiGatherWorker == null;
        }

        /// <summary>
        /// Add a color to the display
        /// </summary>
        /// <param name="color">The color to add</param>
        private void addColor(Color color)
        {
            try
            {
                // create shape
                var r = new Rectangle();

                // set colour from argb bytes
                r.Fill = new SolidColorBrush(color);

                // select mode
                switch (GridMode)
                {
                    case GridMode.FitToArea:
                        {
                            // pull style from resources
                            //r.Style = this.coloursGrid.FindResource("sizeableRectangleStyle") as Style;

                            break;
                        }
                    case GridMode.MaintainSize:
                        {
                            // pull style from resources
                            r.Style = coloursGrid.FindResource("squareRectangleStyle") as Style;

                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }

                // add the child
                coloursGrid.Children.Add(r);

                // increment count
                ColorCount++;

                // has atleast a child color
                HasChildrenColors = true;
            }
            catch (Exception)
            {
                // display
                MessageBox.Show("Cannot add colour, check values are entered in a correct hex format", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Remove a rectangle from the display
        /// </summary>
        /// <param name="rectangle">The rectangle to remove</param>
        private void removeRectangle(Rectangle rectangle)
        {
            try
            {
                // remove the child
                coloursGrid.Children.Remove(rectangle);

                // increment count
                ColorCount--;

                // check it atleast a child color
                HasChildrenColors = coloursGrid.Children.Count > 0;
            }
            catch (Exception)
            {
                // display
                MessageBox.Show("Cannot remove rectangle, check rectangle is part of the grid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Sort all colors
        /// </summary>
        /// <param name="sortMethod">The sort method to use</param>
        private void sortColors(Comparison<Rectangle> sortMethod)
        {
            // sorting colors
            Mode = Mode.Sort;

            // if sorter running
            if (colorArangmentWorker != null)
                // cancel
                colorArangmentWorker.CancelAsync();

            // create new worker
            colorArangmentWorker = new BackgroundWorker();

            // support cancel
            colorArangmentWorker.WorkerSupportsCancellation = true;

            // not idle
            AllBackgroundOperationsIdle = false;

            // get the children
            var rectangleCollection = coloursGrid.Children;

            // set worker work handler
            colorArangmentWorker.DoWork += (sender, args) =>
            {
                try
                {
                    // here we need to sort the colors as best we can. this is never going to be perfect

                    // if no collection of rectangles
                    if (!(args.Argument is UIElementCollection)) throw new ArgumentException("No colours were provided for the sort");

                    // get collection
                    var collection = args.Argument as UIElementCollection;

                    // hold sort list
                    var sortList = new List<Rectangle>();

                    // invoke on dispatcher
                    Dispatcher.Invoke(new LambdaCallback(() =>
                    {
                        // the sort is going to be overall dark to light
                        foreach (var o in collection)
                            // if a rectangle
                            if (o is Rectangle)
                                // add rectangle to list
                                sortList.Add(o as Rectangle);
                    }), DispatcherPriority.Background);

                    // if not cancelling
                    if (!colorArangmentWorker.CancellationPending)
                        // invoke sort
                        Dispatcher.Invoke(new LambdaCallback(() =>
                        {
                            try
                            {
                                // sort
                                sortList.Sort(sortMethod);
                            }
                            catch (ArgumentException)
                            {
                                // this is probally the sort algorhythm
                                Console.WriteLine("Exception caught sorting rectangles. If the randomize sort method was used this could be because a 0 was never returned and should be ignored");
                            }
                        }), DispatcherPriority.Background);
                    else
                        // return thread
                        return;

                    // itterate all rectangles
                    for (var index = 0; index < sortList.Count; index++)
                        // if not cancelling
                        if (!colorArangmentWorker.CancellationPending)
                            // invoke update
                            Dispatcher.Invoke(new LambdaCallback(() =>
                            {
                                // remove rectangle
                                coloursGrid.Children.Remove(sortList[index]);

                                // add at colour index
                                coloursGrid.Children.Insert(index, sortList[index]);
                            }), DispatcherPriority.Background);
                        else
                            // return thread
                            return;
                }
                catch (Exception e)
                {
                    // just display
                    Dispatcher.BeginInvoke(new LambdaCallback(() =>
                    {
                        // display error to user only
                        MessageBox.Show($"There was an error sorting colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
                finally
                {
                    // begin invoke
                    Dispatcher.Invoke(new LambdaCallback(() =>
                    {
                        // update status
                        Status = "Colour sort finished";

                        // release worker
                        colorArangmentWorker = null;

                        // not sorting
                        Mode = defaultMode;

                        // set if all operations are idle
                        AllBackgroundOperationsIdle = getIfAllBackgroundOperationsAreComplete();
                    }));
                }
            };

            // do work to do ^
            colorArangmentWorker.RunWorkerAsync(rectangleCollection);
        }

        /// <summary>
        /// Populate the windows sub menu with all open windows
        /// </summary>
        private void populateWindowsSubMenuWithOpenWindows()
        {
            // gather all processes so we can obtain windows...

            // clear all window names
            gatherFromWindowMenuItem.Items.Clear();

            // get all processes
            var procs = Process.GetProcesses();

            // itterate all processes
            foreach (var proc in procs)
                // if handle
                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    // dynamicaly build menu

                    // add
                    var windowNameItem = new MenuItem();

                    // set header
                    windowNameItem.Header = proc.ProcessName;

                    // allow click
                    windowNameItem.Click += windowNameItem_Click;

                    // add item to menu
                    gatherFromWindowMenuItem.Items.Add(windowNameItem);
                }

            // set if enabled
            gatherFromWindowMenuItem.IsEnabled = gatherFromWindowMenuItem.Items.Count > 0;
        }

        /// <summary>
        /// Remove all duplicated colors
        /// </summary>
        public void RemoveDuplicateColors()
        {
            // filtering colors
            Mode = Mode.Filter;

            // if filterer running
            if (colorArangmentWorker != null)
                // cancel
                colorArangmentWorker.CancelAsync();

            // create new worker
            colorArangmentWorker = new BackgroundWorker();

            // support cancel
            colorArangmentWorker.WorkerSupportsCancellation = true;

            // not idle
            AllBackgroundOperationsIdle = false;

            // get the children
            var rectangleCollection = coloursGrid.Children;

            // set worker work handler
            colorArangmentWorker.DoWork += (sender, args) =>
            {
                try
                {
                    // get collection
                    var collection = args.Argument as UIElementCollection;

                    // hold rectangle dictionary
                    var colorDictionary = new Dictionary<string, short>();

                    // hold all rectangles
                    var allRecatngles = new List<Rectangle>();

                    // invoke on dispatcher
                    Dispatcher.Invoke(new LambdaCallback(() =>
                    {
                        // itterate all objects in collesction
                        foreach (var o in collection)
                        {
                            // if a rectangle
                            if (o is Rectangle)
                            {
                                // get rectanagle
                                var r = o as Rectangle;

                                // if fill
                                if (!(r.Fill is SolidColorBrush))
                                    // throw exception
                                    throw new Exception("Only rectangles with fills that are of type SolidColorBrush can be filtered");

                                // get brush
                                var brush = r.Fill as SolidColorBrush;

                                // create key
                                var key = $"{brush.Color.A}{brush.Color.R}{brush.Color.G}{brush.Color.B}";

                                // if key already exists
                                if (colorDictionary.Keys.Contains(key))
                                    // increment
                                    colorDictionary[key]++;
                                else
                                    // create new entry
                                    colorDictionary.Add(key, 1);
                            }

                            // add rectangle
                            allRecatngles.Add(o as Rectangle);
                        }
                    }), DispatcherPriority.Background);

                    // while still some keys
                    while (colorDictionary.Keys.Count > 0)
                        // if not cancelling
                        if (!colorArangmentWorker.CancellationPending)
                        {
                            // get key
                            var key = colorDictionary.Keys.ElementAt(colorDictionary.Keys.Count - 1);

                            // look up 
                            if (colorDictionary[key] > 1)
                            {
                                // hold how many to remove
                                var toRemove = colorDictionary[key] - 1;

                                // invoke callback
                                Dispatcher.Invoke(new LambdaCallback(() =>
                                {
                                    // itterate all rectangles again
                                    foreach (var r in allRecatngles)
                                    {
                                        // if a solid brush
                                        if (r.Fill is SolidColorBrush)
                                        {
                                            // get brush
                                            var brush = r.Fill as SolidColorBrush;

                                            // if key matches
                                            if ($"{brush.Color.A}{brush.Color.R}{brush.Color.G}{brush.Color.B}" == key)
                                            {
                                                // remove from GUI
                                                removeRectangle(r);

                                                // reduce left to remove
                                                toRemove--;
                                            }
                                        }

                                        // if a cancel pending, or just one left
                                        if (toRemove == 0 ||
                                            colorArangmentWorker.CancellationPending)
                                            // break itteration
                                            break;
                                    }
                                }), DispatcherPriority.Background);
                            }

                            // remove key from dictionary
                            colorDictionary.Remove(key);
                        }
                        else
                        {
                            // return thread
                            return;
                        }
                }
                catch (Exception e)
                {
                    // just display
                    Dispatcher.BeginInvoke(new LambdaCallback(() =>
                    {
                        // display error to user only
                        MessageBox.Show($"There was an error removing duplicate colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
                finally
                {
                    // begin invoke
                    Dispatcher.Invoke(new LambdaCallback(() =>
                    {
                        // update status
                        Status = "Colour filter finished";

                        // release worker
                        colorArangmentWorker = null;

                        // not sorting
                        Mode = defaultMode;

                        // set if all operations are idle
                        AllBackgroundOperationsIdle = getIfAllBackgroundOperationsAreComplete();
                    }));
                }
            };

            // do work to do ^
            colorArangmentWorker.RunWorkerAsync(rectangleCollection);
        }

        /// <summary>
        /// Find the maximum difference between 3 colors
        /// </summary>
        /// <param name="colorA">Color a</param>
        /// <param name="colorB">Color B</param>
        /// <param name="colorC">Color c</param>
        /// <returns>The maximum difference</returns>
        private double findMaximumDifference(double colorA, double colorB, double colorC)
        {
            // calculate max minus min
            return Math.Max(colorA, Math.Max(colorB, colorC)) - Math.Min(colorA, Math.Max(colorB, colorC));
        }

        /// <summary>
        /// Filter the colors using a specified mode
        /// </summary>
        /// <param name="mode">The mode to use</param>
        /// <param name="dominance">The dominance value to use</param>
        public void Filter(ChannelDominanceModes mode, double dominance)
        {
            // filtering colors
            Mode = Mode.Filter;

            // if filterer running
            if (colorArangmentWorker != null)
                // cancel
                colorArangmentWorker.CancelAsync();

            // create new worker
            colorArangmentWorker = new BackgroundWorker();

            // support cancel
            colorArangmentWorker.WorkerSupportsCancellation = true;

            // not idle
            AllBackgroundOperationsIdle = false;

            // get the children
            var rectangleCollection = coloursGrid.Children;

            // set worker work handler
            colorArangmentWorker.DoWork += (sender, args) =>
            {
                try
                {
                    // here we need to sort the colors as best we can. this is never going to be perfect

                    // if no collection of rectangles
                    if (!(args.Argument is UIElementCollection)) throw new ArgumentException("No colours were provided for the filter");

                    // check dominance is normalised
                    if (dominance > 1.0d || dominance < 0.0d) throw new ArgumentException("Dominance is outside of normalised range (0.0 - 1.0)");

                    // get collection
                    var collection = args.Argument as UIElementCollection;

                    // hold sort list
                    var sortList = new List<Rectangle>();

                    // hold all rectangle list
                    var allRectangleList = new List<Rectangle>();

                    // invoke on dispatcher
                    Dispatcher.Invoke(new LambdaCallback(() =>
                    {
                        // select mode
                        switch (mode)
                        {
                            case ChannelDominanceModes.DominantRGB:
                            case ChannelDominanceModes.NonDominantRGB:
                                {
                                    // get byte value of dominance
                                    dominance = 255d / 100d * (dominance * 100d);

                                    break;
                                }
                            case ChannelDominanceModes.DominantCMY:
                            case ChannelDominanceModes.NonDominantCMY:
                            case ChannelDominanceModes.Gray:
                            case ChannelDominanceModes.NonGray:
                                {
                                    // dominance remains

                                    break;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }

                        // the sort is going to be overall dark to light
                        foreach (var o in collection)
                            // if a rectangle
                            if (o is Rectangle)
                            {
                                // get rectanagle
                                var r = o as Rectangle;

                                // if fill
                                if (!(r.Fill is SolidColorBrush))
                                    // throw exception
                                    throw new Exception("Only rectangles with fills that are of type SolidColorBrush can be filtered");

                                // select mode
                                switch (mode)
                                {
                                    case ChannelDominanceModes.DominantCMY:
                                        {
                                            // create converter
                                            IValueConverter rgbToCMYKConverter = new RGBToCMYKConverter();

                                            // get color a
                                            var colorA = rgbToCMYKConverter.Convert(((SolidColorBrush)r.Fill).Color, typeof(CMYKColor), null, null) as CMYKColor;

                                            // check for dominant channel
                                            if (!(findMaximumDifference(colorA.Cyan, colorA.Magenta, colorA.Yellow) >= dominance))
                                                // add rectangle to list
                                                sortList.Add(r);

                                            break;
                                        }
                                    case ChannelDominanceModes.DominantRGB:
                                        {
                                            // get color a
                                            var colorA = ((SolidColorBrush)r.Fill).Color;

                                            // check for dominant channel
                                            if (!(findMaximumDifference(colorA.R, colorA.G, colorA.B) >= dominance))
                                                // add rectangle to list
                                                sortList.Add(r);

                                            break;
                                        }
                                    case ChannelDominanceModes.Gray:
                                        {
                                            // get color a
                                            var colorA = ((SolidColorBrush)r.Fill).Color;

                                            // check for a non-gray color
                                            if (colorA.R != colorA.G || colorA.G != colorA.B || colorA.R != colorA.B)
                                                // add rectangle to list
                                                sortList.Add(r);

                                            break;
                                        }
                                    case ChannelDominanceModes.NonDominantCMY:
                                        {
                                            // create converter
                                            IValueConverter rgbToCMYKConverter = new RGBToCMYKConverter();

                                            // get color a
                                            var colorA = rgbToCMYKConverter.Convert(((SolidColorBrush)r.Fill).Color, typeof(CMYKColor), null, null) as CMYKColor;

                                            // check for dominant channel
                                            if (findMaximumDifference(colorA.Cyan, colorA.Magenta, colorA.Yellow) >= dominance)
                                                // add rectangle to list
                                                sortList.Add(r);

                                            break;
                                        }
                                    case ChannelDominanceModes.NonDominantRGB:
                                        {
                                            // get color a
                                            var colorA = ((SolidColorBrush)r.Fill).Color;

                                            // check for dominant channel
                                            if (findMaximumDifference(colorA.R, colorA.G, colorA.B) >= dominance)
                                                // add rectangle to list
                                                sortList.Add(r);

                                            break;
                                        }

                                    case ChannelDominanceModes.NonGray:
                                        {
                                            // get color a
                                            var colorA = ((SolidColorBrush)r.Fill).Color;

                                            // check for a gray color
                                            if (colorA.R == colorA.G && colorA.G == colorA.B)
                                                // add rectangle to list
                                                sortList.Add(r);

                                            break;
                                        }

                                    default:
                                        {
                                            throw new NotImplementedException();
                                        }
                                }

                                // add to all rectangle list
                                allRectangleList.Add(o as Rectangle);
                            }
                    }), DispatcherPriority.Background);

                    // itterate all rectangles
                    for (var index = 0; index < allRectangleList.Count; index++)
                        // if not cancelling
                        if (!colorArangmentWorker.CancellationPending)
                            // invoke update
                            Dispatcher.Invoke(new LambdaCallback(() =>
                            {
                                // if if sorted list does not contain the rectangle
                                if (!sortList.Contains(allRectangleList[index]))
                                    // remove from GUI
                                    removeRectangle(allRectangleList[index]);
                            }), DispatcherPriority.Background);
                        else
                            // return thread
                            return;
                }
                catch (Exception e)
                {
                    // just display
                    Dispatcher.BeginInvoke(new LambdaCallback(() =>
                    {
                        // display error to user only
                        MessageBox.Show($"There was an error filtering colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
                finally
                {
                    // begin invoke
                    Dispatcher.Invoke(new LambdaCallback(() =>
                    {
                        // update status
                        Status = "Colour filter finished";

                        // release worker
                        colorArangmentWorker = null;

                        // not sorting
                        Mode = defaultMode;

                        // set if all operations are idle
                        AllBackgroundOperationsIdle = getIfAllBackgroundOperationsAreComplete();
                    }));
                }
            };

            // do work to do ^
            colorArangmentWorker.RunWorkerAsync(rectangleCollection);
        }

        /// <summary>
        /// Populate the colors using a specified mode
        /// </summary>
        /// <param name="mode">The mode to use</param>
        public void Populate(PopulationModes mode)
        {
            // populating colors
            Mode = Mode.Populate;

            // if filterer running
            if (colorArangmentWorker != null)
                // cancel
                colorArangmentWorker.CancelAsync();

            // create new worker
            colorArangmentWorker = new BackgroundWorker();

            // support cancel
            colorArangmentWorker.WorkerSupportsCancellation = true;

            // not idle
            AllBackgroundOperationsIdle = false;

            // set worker work handler
            colorArangmentWorker.DoWork += (sender, args) =>
            {
                try
                {
                    // select mode
                    switch (mode)
                    {
                        case PopulationModes.Reds:
                        case PopulationModes.Greens:
                        case PopulationModes.Blues:
                            {
                                // hold red byte
                                byte r = 0;

                                // hold green byte
                                byte g = 0;

                                // hold blue byte
                                byte b = 0;

                                // add itterate index
                                for (byte index = 0; index < 255; index++)
                                {
                                    // select mode
                                    switch (mode)
                                    {
                                        case PopulationModes.Blues:
                                            {
                                                // increment blue
                                                b = index;

                                                break;
                                            }
                                        case PopulationModes.Greens:
                                            {
                                                // increment green
                                                g = index;

                                                break;
                                            }
                                        case PopulationModes.Reds:
                                            {
                                                // increment red
                                                r = index;

                                                break;
                                            }
                                        default:
                                            {
                                                throw new NotImplementedException();
                                            }
                                    }


                                    // add a new rectangle
                                    Dispatcher.Invoke(new LambdaCallback(() =>
                                    {
                                        // add the color
                                        addColor(Color.FromRgb(r, g, b));
                                    }), DispatcherPriority.Background);
                                }

                                break;
                            }
                        case PopulationModes.Cyans:
                        case PopulationModes.Magentas:
                        case PopulationModes.Yellows:
                            {
                                // hold cyan byte
                                byte c = 0;

                                // hold magenta byte
                                byte m = 0;

                                // hold yellow byte
                                byte y = 0;

                                // create converter for converting CMYK to RGB
                                IValueConverter converter = new RGBToCMYKConverter();

                                // add itterate index
                                for (byte index = 0; index < 255; index++)
                                {
                                    // select mode
                                    switch (mode)
                                    {
                                        case PopulationModes.Cyans:
                                            {
                                                // increment cyan
                                                c = index;

                                                break;
                                            }
                                        case PopulationModes.Magentas:
                                            {
                                                // increment magenta
                                                m = index;

                                                break;
                                            }
                                        case PopulationModes.Yellows:
                                            {
                                                // increment yellow
                                                y = index;

                                                break;
                                            }
                                        default:
                                            {
                                                throw new NotImplementedException();
                                            }
                                    }


                                    // add a new rectangle
                                    Dispatcher.Invoke(new LambdaCallback(() =>
                                    {
                                        // add the color
                                        addColor((Color)converter.ConvertBack(new CMYKColor(1.0d / 255 * c, 1.0d / 255 * m, 1.0d / 255 * y, 0.0), typeof(Color), null, null));
                                    }), DispatcherPriority.Background);
                                }

                                break;
                            }
                        case PopulationModes.Grayscale:
                            {
                                // add itterate index
                                for (byte index = 0; index < 255; index++)
                                    // add a new rectangle
                                    Dispatcher.Invoke(new LambdaCallback(() =>
                                    {
                                        // add the color
                                        addColor(Color.FromArgb(255, index, index, index));
                                    }), DispatcherPriority.Background);

                                break;
                            }
                        case PopulationModes.PresentationFramework:
                            {
                                // hold properties
                                var infos = typeof(Colors).GetProperties();

                                // itterate all properties
                                for (var index = 0; index < infos.Length; index++)
                                    // add a new rectangle
                                    Dispatcher.Invoke(new LambdaCallback(() =>
                                    {
                                        // add the color
                                        addColor((Color)ColorConverter.ConvertFromString(infos[index].Name));
                                    }), DispatcherPriority.Background);

                                break;
                            }
                        case PopulationModes.SystemColors:
                            {
                                // hold properties
                                var infos = typeof(SystemColors).GetProperties();

                                // itterate all properties
                                for (var index = 0; index < infos.Length; index++)
                                    if (infos[index].GetValue(infos[index], null) is Color)
                                        // add a new rectangle
                                        Dispatcher.Invoke(new LambdaCallback(() =>
                                        {
                                            // add the color
                                            addColor((Color)infos[index].GetValue(infos[index], null));
                                        }), DispatcherPriority.Background);

                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                }
                catch (Exception e)
                {
                    // just display
                    Dispatcher.BeginInvoke(new LambdaCallback(() =>
                    {
                        // display error to user only
                        MessageBox.Show($"There was an error populating colours: {e.Message}", "Sort Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
                finally
                {
                    // begin invoke
                    Dispatcher.Invoke(new LambdaCallback(() =>
                    {
                        // update status
                        Status = "Colour population finished";

                        // release worker
                        colorArangmentWorker = null;

                        // not sorting
                        Mode = defaultMode;

                        // set if all operations are idle
                        AllBackgroundOperationsIdle = getIfAllBackgroundOperationsAreComplete();
                    }));
                }
            };

            // do work to do ^
            colorArangmentWorker.RunWorkerAsync();
        }

        #endregion

        #region StaticMethods

        /// <summary>
        /// Get a propotion of 2 colors
        /// </summary>
        /// <param name="colorA">Color A</param>
        /// <param name="colorB">Color B</param>
        /// <returns>The proportion fo the combined colors</returns>
        private static double getTwoColorProportion(double colorA, double colorB)
        {
            // calculate
            return (colorA + colorB) / 2;
        }

        /// <summary>
        /// Sort rectangles by their colour, cyan primarily, then yellows, then blues
        /// </summary>
        /// <param name="a">Rectangle a</param>
        /// <param name="b">Rectangle b</param>
        /// <returns>The result (as an integer) of the equation. More cyan, then magenta, then yellow colors return negative numbers</returns>
        private static int sortRectanglesByCMYKColor(Rectangle a, Rectangle b)
        {
            // if incorrect fills
            if (!(a.Fill is SolidColorBrush) || !(b.Fill is SolidColorBrush))
                // throw exception
                throw new Exception("Only rectangles with fills that are of type SolidColorBrush can be sorted");

            // create converter
            IValueConverter rgbToCMYKConverter = new RGBToCMYKConverter();

            // get color a
            var colorA = rgbToCMYKConverter.Convert(((SolidColorBrush)a.Fill).Color, typeof(CMYKColor), null, null) as CMYKColor;

            // get color b
            var colorB = rgbToCMYKConverter.Convert(((SolidColorBrush)b.Fill).Color, typeof(CMYKColor), null, null) as CMYKColor;

            // check cyan first
            if (colorA.Cyan > colorB.Cyan)
                // less - more cyan
                return -1;

            if (colorA.Cyan < colorB.Cyan)
                // more - less cyan
                return 1;

            // check magenta
            if (colorA.Magenta > colorB.Magenta)
                // less - more magenta
                return -1;

            if (colorA.Magenta < colorB.Magenta)
                // more - less magenta
                return 1;

            // compare yellow
            if (colorA.Yellow > colorB.Yellow)
                // less - more yellow
                return -1;

            if (colorA.Yellow < colorB.Yellow)
                // more - less yellow
                return 1;

            // check key
            if (colorA.Key > colorB.Key)
                // more - darker
                return -1;
            if (colorA.Key < colorB.Key)
                // less - lighter
                return 1;
            return 0;
        }

        /// <summary>
        /// Sort rectangles by their colour, red primarily, then blues, then greens
        /// </summary>
        /// <param name="a">Rectangle a</param>
        /// <param name="b">Rectangle b</param>
        /// <returns>The result (as an integer) of the equation. Redder, then bluer, then greener, then alpha values return negative numbers</returns>
        private static int sortRectanglesByARGBColor(Rectangle a, Rectangle b)
        {
            // if incorrect fills
            if (!(a.Fill is SolidColorBrush) || !(b.Fill is SolidColorBrush))
                // throw exception
                throw new Exception("Only rectangles with fills that are of type SolidColorBrush can be sorted");

            // get color a
            var colorA = ((SolidColorBrush)a.Fill).Color;

            // get color b
            var colorB = ((SolidColorBrush)b.Fill).Color;

            // check reds first
            if (colorA.R > colorB.R)
                // less - more red
                return -1;

            if (colorA.R < colorB.R)
                // more - less red
                return 1;

            // check greens
            if (colorA.G > colorB.G)
                // less - more green
                return -1;

            if (colorA.G < colorB.G)
                // more - less green
                return 1;

            // compare blues
            if (colorA.B > colorB.B)
                // less - more blue
                return -1;

            if (colorA.B < colorB.B)
                // more - less blue
                return 1;

            // ccheck aplha
            if (colorA.A > colorB.A)
                // less - darker
                return -1;
            if (colorA.A < colorB.A)
                // more - lighter
                return 1;
            return 0;
        }

        /// <summary>
        /// Sort rectangles by their relative greyscale
        /// </summary>
        /// <param name="a">Rectangle a</param>
        /// <param name="b">Rectangle b</param>
        /// <returns>The result (as an integer) of the equation. Darker colors return negative numbers, where as lighter colors return positive numbers</returns>
        private static int sortRectanglesByRelativeGrayscale(Rectangle a, Rectangle b)
        {
            // if incorrect fills
            if (!(a.Fill is SolidColorBrush) || !(b.Fill is SolidColorBrush))
                // throw exception
                throw new Exception("Only rectangles with fills that are of type SolidColorBrush can be sorted");

            // get color a
            var colorA = ((SolidColorBrush)a.Fill).Color;

            // get color b
            var colorB = ((SolidColorBrush)b.Fill).Color;

            // get average * alpha
            var aAve = (byte)((colorA.R + colorA.G + colorA.B) / 3d / 255d * colorA.A);

            // get average * alpha
            var bAve = (byte)((colorB.R + colorB.G + colorB.B) / 3d / 255d * colorB.A);

            // check alpha first
            if (aAve > bAve)
                // lighter
                return 1;
            if (aAve < bAve)
                // darker
                return -1;
            return 0;
        }


        /// <summary>
        /// Sort rectangles by HSV
        /// </summary>
        /// <param name="a">Rectangle a</param>
        /// <param name="b">Rectangle b</param>
        /// <returns>The result (as an integer) of the equation. Darker colors return negative numbers, where as lighter colors return positive numbers</returns>
        private static int sortRectanglesByHSV(Rectangle a, Rectangle b)
        {
            // if incorrect fills
            if (!(a.Fill is SolidColorBrush) || !(b.Fill is SolidColorBrush))
                // throw exception
                throw new Exception("Only rectangles with fills that are of type SolidColorBrush can be sorted");

            // get color a
            var colorA = ((SolidColorBrush)a.Fill).Color;

            // get color b
            var colorB = ((SolidColorBrush)b.Fill).Color;

            // create converter
            IValueConverter converter = new RGBToHSVConverter();

            // color a as HSV
            var colorAHSV = converter.Convert(colorA, typeof(HSVColor), null, null) as HSVColor;

            // color b as HSV
            var colorBHSV = converter.Convert(colorB, typeof(HSVColor), null, null) as HSVColor;

            // now we compare colors h first, then saturation, then value
            if (colorAHSV.Hue > colorBHSV.Hue)
                // greater
                return 1;

            if (colorAHSV.Hue < colorBHSV.Hue)
                // less
                return -1;

            // compare saturation
            if (colorAHSV.Saturation > colorBHSV.Saturation)
                // greater
                return 1;

            if (colorAHSV.Saturation < colorBHSV.Saturation)
                // less
                return -1;

            // compare value
            if (colorAHSV.Value > colorBHSV.Value)
                // greater
                return 1;
            if (colorAHSV.Value < colorBHSV.Value)
                // less
                return -1;
            return 0;
        }

        /// <summary>
        /// Sort rectangles by a ranomizing them
        /// </summary>
        /// <param name="a">Rectangle a</param>
        /// <param name="b">Rectangle b</param>
        /// <returns>The result (as an integer) of the equation</returns>
        private static int sortRectanglesByRandom(Rectangle a, Rectangle b)
        {
            // generate random
            return randomGenerator.Next(-1, 2);
        }

        #endregion

        #region PropertyCallbacks

        /// <summary>
        /// Shared handleing for Alpha, Red, Green and Blue property changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSharedARGBPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // get object
            var window = obj as MainWindow;

            // update swatch
            window.swatchBorder.Background = new SolidColorBrush(Color.FromArgb(window.Alpha, window.Red, window.Green, window.Blue));

            // if updating by argb or using pipette
            if (window.ColorSpace == ColorSpace.ARGB)
            {
                // get cmyk 
                var cymkColor = window.rgb2cmykConverter.Convert(Color.FromArgb(window.Alpha, window.Red, window.Green, window.Blue), typeof(CMYKColor), null, null) as CMYKColor;

                // update cmyk
                window.Cyan = cymkColor.Cyan * 100;
                window.Magenta = cymkColor.Magenta * 100;
                window.Yellow = cymkColor.Yellow * 100;
                window.K = cymkColor.Key * 100;

                // get hsv
                var hsvColor = window.rgb2hsvConverter.Convert(Color.FromArgb(window.Alpha, window.Red, window.Green, window.Blue), typeof(HSVColor), null, null) as HSVColor;

                // update hsv
                window.Hue = hsvColor.Hue * 100;
                window.Saturation = hsvColor.Saturation * 100;
                window.Value = hsvColor.Value * 100;
            }
        }

        /// <summary>
        /// Shared handleing for Cyan, Magenta, Yellow and Key property changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSharedCMYKPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // get object
            var window = obj as MainWindow;

            // if updating by cmyk
            if (window.ColorSpace == ColorSpace.CMYK)
            {
                // get color 
                var color = (Color)window.rgb2cmykConverter.ConvertBack(new CMYKColor(window.Cyan / 100, window.Magenta / 100, window.Yellow / 100, window.K / 100), typeof(Color), null, null);

                // update argb
                window.Red = color.R;
                window.Green = color.G;
                window.Blue = color.B;

                // get hsv
                var hsvColor = window.rgb2hsvConverter.Convert(Color.FromArgb(window.Alpha, window.Red, window.Green, window.Blue), typeof(HSVColor), null, null) as HSVColor;

                // update hsv
                window.Hue = hsvColor.Hue * 100;
                window.Saturation = hsvColor.Saturation * 100;
                window.Value = hsvColor.Value * 100;
            }
        }

        /// <summary>
        /// Shared handleing for Hue, Saturation and Value property changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSharedHSVPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // get object
            var window = obj as MainWindow;

            // if updating by HSV
            if (window.ColorSpace == ColorSpace.HSV)
            {
                // get color 
                var color = (Color)window.rgb2hsvConverter.ConvertBack(new HSVColor(window.Hue / 100, window.Saturation / 100, window.Value / 100), typeof(Color), null, null);

                // update argb
                window.Red = color.R;
                window.Green = color.G;
                window.Blue = color.B;

                // get cmyk 
                var cymkColor = window.rgb2cmykConverter.Convert(color, typeof(CMYKColor), null, null) as CMYKColor;

                // update cmyk
                window.Cyan = cymkColor.Cyan * 100;
                window.Magenta = cymkColor.Magenta * 100;
                window.Yellow = cymkColor.Yellow * 100;
                window.K = cymkColor.Key * 100;
            }
        }

        /// <summary>
        /// Handle Mode property changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // get object
            var window = obj as MainWindow;

            // get the new mode
            var newMode = (Mode)args.NewValue;

            // select mode
            switch (newMode)
            {
                case Mode.Add:
                    {
                        // set cursor
                        window.Cursor = Cursors.Arrow;

                        // set status
                        window.Status = "Ready";

                        // set cursor
                        window.coloursGrid.Cursor = Cursors.Hand;

                        break;
                    }
                case Mode.Delete:
                    {
                        // set staus
                        window.Status = "Click a color to remove it, or press escape to cancel...";

                        // set cursor
                        window.coloursGrid.Cursor = Cursors.Arrow;

                        break;
                    }
                case Mode.Filter:
                    {
                        // set cursor
                        window.Cursor = Cursors.Wait;

                        // set staus
                        window.Status = "Colours are being filtered, press escape to cancel...";

                        break;
                    }
                case Mode.Pipette:
                    {
                        // set cursor
                        window.Cursor = Cursors.Pen;

                        // set staus
                        window.Status = "Move the mouse to the desired pixel, and release the mouse button to select as a colour...";

                        break;
                    }
                case Mode.ROIGather:
                    {
                        // set cursor
                        window.Cursor = Cursors.Cross;

                        // set staus
                        window.Status = "Move to the top left of the region and press and hold Shift to specify the first point...";

                        break;
                    }
                case Mode.Sort:
                    {
                        // set cursor
                        window.Cursor = Cursors.Wait;

                        // set staus
                        window.Status = "Colours are being sorted, press escape to cancel...";

                        break;
                    }
                case Mode.Populate:
                    {
                        // set cursor
                        window.Cursor = Cursors.Wait;

                        // set staus
                        window.Status = "Colours are being populated, press escape to cancel...";

                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        /// <summary>
        /// Handle MaxOutAlphaOnPreview property changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaxOutAlphaOnPreviewPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // get object
            var window = obj as MainWindow;

            // set if maxing out colors
            FullScreenPreviewWindow.MaxOutAlphaOnPreview = (bool)args.NewValue;
        }

        /// <summary>
        /// Handle GridMode property changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnGridModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // get object
            var window = obj as MainWindow;

            // select mode
            switch ((GridMode)args.NewValue)
            {
                case GridMode.FitToArea:
                    {
                        // set grid columns
                        window.coloursGrid.Columns = 0;

                        // set first column
                        window.coloursGrid.FirstColumn = 0;

                        // itterate all children
                        foreach (Rectangle r in window.coloursGrid.Children)
                            // set style
                            r.Style = window.coloursGrid.FindResource("sizeableRectangleStyle") as Style;

                        break;
                    }
                case GridMode.MaintainSize:
                    {
                        // set grid columns
                        window.coloursGrid.Columns = (int)window.FixedColumns;

                        // itterate all children
                        foreach (Rectangle r in window.coloursGrid.Children)
                            // set style
                            r.Style = window.coloursGrid.FindResource("squareRectangleStyle") as Style;

                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        /// <summary>
        /// Handle FixedColumns property changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnFixedColumnsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // get object
            var window = obj as MainWindow;

            // if in maintain size mode
            if (window.GridMode == GridMode.MaintainSize)
                // set grid columns
                window.coloursGrid.Columns = (int)(double)args.NewValue;
        }

        #endregion

        #region EventHandlers

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // add color
            addColor(Color.FromArgb(Alpha, Red, Green, Blue));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // update all manifest properties
            updateManifestProperties();
        }

        private void window_Closed(object sender, EventArgs e)
        {
            // remove subscription from ColorModificationComplete event
            ((ColorInfoWindow)sender).ColorModificationComplete -= window_ColorModificationComplete;

            // remove subscription from Closed event
            ((ColorInfoWindow)sender).ColorModificationComplete -= window_Closed;
        }

        private void window_ColorModificationComplete(object sender, ColorEventArgs args)
        {
            // add color
            addColor(args.Color);
        }

        private void pickColorButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // begin colour picking
            beginColorPicking();
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            // set alpha
            Alpha = 255;

            // set red
            Red = 255;

            // set green
            Green = 255;

            // set blue
            Blue = 255;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // handle window keys
            switch (e.Key)
            {
                case colorSelectionModesModifierKeyWPF:
                    {
                        // if not roi selection going on...
                        if (Mode != Mode.ROIGather &&
                            getIfAllBackgroundOperationsAreComplete())
                        {
                            // begin color picking
                            beginColorPicking();
                        }
                        else
                        {
                            // set status
                            Status = "Drag to the bottom right of the region and release Shift to complete selection...";

                            // if no top left already set
                            if (roiSelectionTopLeft == null)
                                // set top left
                                roiSelectionTopLeft = getCurrentMousePositionOverVirtualScreen();
                        }

                        break;
                    }
                case deincrementAlphaKeyWPF:
                    {
                        // deincrement
                        Alpha = deincrementByte(Alpha);

                        break;
                    }
                case deincrementBrightnessKeyWPF:
                    {
                        // deincrement
                        Red = deincrementByte(Red);
                        Green = deincrementByte(Green);
                        Blue = deincrementByte(Blue);

                        break;
                    }
                case incrementAlphaKeyWPF:
                    {
                        // increment
                        Alpha = incrementByte(Alpha);

                        break;
                    }
                case incrementBrightnessKeyWPF:
                    {
                        // increment
                        Red = incrementByte(Red);
                        Green = incrementByte(Green);
                        Blue = incrementByte(Blue);

                        break;
                    }
                case Key.Escape:
                    {
                        // if roi selecting
                        if (Mode == Mode.ROIGather)
                        {
                            // reset coordinates
                            roiSelectionTopLeft = null;
                            roiSelectionBottomRight = null;
                        }

                        // if gathering working
                        if (roiGatherWorker != null &&
                            roiGatherWorker.IsBusy)
                            // set cancelled
                            roiGatherWorker.CancelAsync();

                        // if sorting working
                        if (colorArangmentWorker != null &&
                            colorArangmentWorker.IsBusy)
                            // set cancelled
                            colorArangmentWorker.CancelAsync();

                        // reset mode
                        Mode = defaultMode;

                        break;
                    }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // check the modifier key
            switch (e.Key)
            {
                case colorSelectionModesModifierKeyWPF:
                    {
                        // if color picking
                        if (Mode == Mode.Pipette)
                        {
                            // end color picking
                            Mode = defaultMode;
                        }
                        else if (Mode == Mode.ROIGather)
                        {
                            // not roi selecting
                            Mode = defaultMode;

                            // set bottom right
                            roiSelectionBottomRight = getCurrentMousePositionOverVirtualScreen();

                            // if we have two values
                            if (roiSelectionTopLeft.HasValue)
                            {
                                // hold roi points
                                int top, left, bottom, right;

                                // set points
                                left = Math.Min(roiSelectionBottomRight.Value.X, roiSelectionTopLeft.Value.X);
                                right = Math.Max(roiSelectionBottomRight.Value.X, roiSelectionTopLeft.Value.X);
                                top = Math.Min(roiSelectionBottomRight.Value.Y, roiSelectionTopLeft.Value.Y);
                                bottom = Math.Max(roiSelectionBottomRight.Value.Y, roiSelectionTopLeft.Value.Y);

                                // begin gather of ROI - with actual top left, bottom right coordinates
                                gatherAllPixelsInROI(top, left, bottom, right);
                            }

                            // release coordinates
                            roiSelectionTopLeft = null;
                            roiSelectionBottomRight = null;
                        }

                        break;
                    }
            }
        }

        private void coloursGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // hit test to find rectangle we are over
            var result = VisualTreeHelper.HitTest(sender as Visual, Mouse.GetPosition(sender as FrameworkElement));

            // if a hit, and a rectangle
            if (result != null &&
                result.VisualHit is Rectangle)
            {
                // if left button pressed
                if (e.ChangedButton == MouseButton.Left)
                {
                    // if deleting colors
                    if (Mode == Mode.Delete)
                    {
                        // remove the rectangle
                        removeRectangle(result.VisualHit as Rectangle);
                    }
                    else
                    {
                        // get sending border
                        var hitElement = result.VisualHit as Rectangle;

                        // show message
                        var sCB = hitElement.Fill as SolidColorBrush;

                        // if brush found
                        if (sCB != null)
                            // select color space
                            switch (ColorSpace)
                            {
                                case ColorSpace.ARGB:
                                    {
                                        // set as selected color
                                        Alpha = sCB.Color.A;
                                        Red = sCB.Color.R;
                                        Green = sCB.Color.G;
                                        Blue = sCB.Color.B;

                                        break;
                                    }
                                case ColorSpace.CMYK:
                                    {
                                        // get cmyk 
                                        var cymkColor = rgb2cmykConverter.Convert(sCB.Color, typeof(CMYKColor), null, null) as CMYKColor;

                                        // update cmyk
                                        Cyan = cymkColor.Cyan * 100;
                                        Magenta = cymkColor.Magenta * 100;
                                        Yellow = cymkColor.Yellow * 100;
                                        K = cymkColor.Key * 100;

                                        break;
                                    }
                                case ColorSpace.HSV:
                                    {
                                        // get hsv
                                        var hsvColor = rgb2hsvConverter.Convert(sCB.Color, typeof(HSVColor), null, null) as HSVColor;

                                        // update hsv
                                        Hue = hsvColor.Hue * 100;
                                        Saturation = hsvColor.Saturation * 100;
                                        Value = hsvColor.Value * 100;

                                        break;
                                    }
                                default:
                                    {
                                        throw new NotImplementedException();
                                    }
                            }
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    // get sending border
                    var hitElement = result.VisualHit as Rectangle;

                    // show message
                    var sCB = hitElement.Fill as SolidColorBrush;

                    // if brush found
                    if (sCB != null)
                        // show info
                        showColorInfoWindow(sCB.Color);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // focus the staus bar - a safe focus element so key downs have some relevance
            Keyboard.Focus(statusBar);
        }

        private void incrementAlphaButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Alpha = incrementByte(Alpha);
        }

        private void incrementRedButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Red = incrementByte(Red);
        }

        private void incrementGreenButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Green = incrementByte(Green);
        }

        private void incrementBlueButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Blue = incrementByte(Blue);
        }

        private void deincrementBlueButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Blue = deincrementByte(Blue);
        }

        private void deincrementGreenButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Green = deincrementByte(Green);
        }

        private void deincrementRedButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Red = deincrementByte(Red);
        }

        private void deincrementAlphaButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Alpha = deincrementByte(Alpha);
        }

        private void incrementCyanButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Cyan = incrementDouble(Cyan);
        }

        private void incrementYellowButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Yellow = incrementDouble(Yellow);
        }

        private void incrementKeyButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            K = incrementDouble(K);
        }

        private void incrementMagentaButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Magenta = incrementDouble(Magenta);
        }

        private void deincrementCyanButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Cyan = deincrementDouble(Cyan);
        }

        private void deincrementMagentaButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Magenta = deincrementDouble(Magenta);
        }

        private void deincrementYellowButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Yellow = deincrementDouble(Yellow);
        }

        private void deincrementKeyButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            K = deincrementDouble(K);
        }

        private void incrementHueButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Hue = incrementDouble(Hue);
        }

        private void incrementSaturationButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Saturation = incrementDouble(Saturation);
        }

        private void incrementValueButton_Click(object sender, RoutedEventArgs e)
        {
            // increment
            Value = incrementDouble(Value);
        }

        private void deincrementHueButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Hue = deincrementDouble(Hue);
        }

        private void deincrementSaturationButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Saturation = deincrementDouble(Saturation);
        }

        private void deincrementValueButton_Click(object sender, RoutedEventArgs e)
        {
            // deincrement
            Value = deincrementDouble(Value);
        }

        private void sFD_FileOk(object sender, CancelEventArgs e)
        {
            // get dialog
            var sfd = sender as SaveFileDialog;

            try
            {
                // set path
                defaultPath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("\\") - 1);

                // export
                ExportPatchworkAsPNG(sfd.FileName);
            }
            catch (Exception)
            {
                // show status
                Status = "Error: Invalid file name";
            }
        }

        private void oFD_FileOk(object sender, CancelEventArgs e)
        {
            // get dialog
            var ofd = sender as OpenFileDialog;

            try
            {
                // set path
                defaultPath = ofd.FileName.Substring(0, ofd.FileName.LastIndexOf("\\") - 1);

                // import
                ImportImage(ofd.FileName);
            }
            catch (Exception)
            {
                // show status
                Status = "Error: Invalid file name";
            }
        }

        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            // show info
            showColorInfoWindow(Color.FromArgb(Alpha, Red, Green, Blue));
        }

        /// <summary>
        /// Show a color informaition window for a specified color
        /// </summary>
        /// <param name="c">The color to view the info of</param>
        private void showColorInfoWindow(Color c)
        {
            // create new window
            var window = new ColorInfoWindow(c);

            // set the owner
            window.Owner = Application.Current.MainWindow;

            // modification event
            window.ColorModificationComplete += window_ColorModificationComplete;

            // close event
            window.Closed += window_Closed;

            // display the info
            window.DisplayInfo(ColorSpace);

            // show the window
            window.Show();
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
                        gatherAllPixelsInROI(rectangle.Top, rectangle.Left, rectangle.Bottom, rectangle.Right);

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

        private void imortComandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // show dialog
            showImportDialog();
        }

        private void exportCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // show dialog
            showExportDialog();
        }

        private void exitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // exit with 0 code
            Application.Current.Shutdown(0);
        }

        private void populateBalancedRedsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.Reds);
        }

        private void populateBalancedGreensCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.Greens);
        }

        private void populateBalancedBuesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.Blues);
        }

        private void populateBalancedCyansCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.Cyans);
        }

        private void populateBalancedMagentasCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.Magentas);
        }

        private void populateBalancedYellowsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.Yellows);
        }

        private void populateGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.Grayscale);
        }

        private void populateSystemColoursCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.SystemColors);
        }

        private void sortRGBCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // sort colors
            sortColors(sortRectanglesByARGBColor);
        }

        private void sortCMYCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // sort colors
            sortColors(sortRectanglesByCMYKColor);
        }

        private void sortHSVCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // sort colors
            sortColors(sortRectanglesByHSV);
        }

        private void sortGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // sort colors
            sortColors(sortRectanglesByRelativeGrayscale);
        }

        private void randomizeCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // sort colors
            sortColors(sortRectanglesByRandom);
        }

        private void discardNonDominantRGBCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // filter
            Filter(ChannelDominanceModes.NonDominantRGB, ChannelDominance);
        }

        private void discardDominantRGBCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // filter
            Filter(ChannelDominanceModes.DominantRGB, ChannelDominance);
        }

        private void discardNonDominantCMYCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // filter
            Filter(ChannelDominanceModes.NonDominantCMY, ChannelDominance);
        }

        private void discardDominantCMYCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // filter
            Filter(ChannelDominanceModes.DominantCMY, ChannelDominance);
        }

        private void discardNonDominantGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // filter
            Filter(ChannelDominanceModes.NonGray, 0.0d);
        }

        private void discardDominantGrayscaleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // filter
            Filter(ChannelDominanceModes.Gray, 0.0d);
        }

        private void discardDuplicatesCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // remove duplicates
            RemoveDuplicateColors();
        }

        private void removeLastCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // clear last
            ClearLast();
        }

        private void removalAllCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // clear all
            Clear();
        }

        private void aboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // show modal
            var about = new AboutWindow();

            // set owner
            about.Owner = this;

            // if can't be displayed ok
            if (Top + ActualHeight >= SystemParameters.PrimaryScreenHeight)
                // modify the start up location
                about.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // show and wait for close
            about.ShowDialog();
        }

        private void adCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // add color
            addColor(Color.FromArgb(Alpha, Red, Green, Blue));
        }

        private void gatherROICommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // start roi
            Mode = Mode.ROIGather;
        }

        private void gatherFullScreenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // capture full
            gatherAllPixelsInROI(0, 0, SystemInformation.VirtualScreen.Height, SystemInformation.VirtualScreen.Width);
        }

        private void populartePresentationFrameworkCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // populate
            Populate(PopulationModes.PresentationFramework);
        }

        private void selectiveRemovalCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // put into mode
            Mode = Mode.Delete;
        }

        #endregion
    }

    #region ValidationRules

    /// <summary>
    /// Validation rule for bytes
    /// </summary>
    public class ByteValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // if some data
            if (value == null ||
                string.IsNullOrEmpty(value.ToString()))
                // return result
                return new ValidationResult(false, "No data");

            // hold byte
            byte b;

            // try parse
            if (byte.TryParse(value.ToString(), out b))
                // valid
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "Data is not a byte");
        }
    }

    /// <summary>
    /// Validation rule for double percentages
    /// </summary>
    public class DoublePercentageValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // if some data
            if (value == null ||
                string.IsNullOrEmpty(value.ToString()))
                // return result
                return new ValidationResult(false, "No data");

            // hold double
            double d;

            // try parse
            if (double.TryParse(value.ToString(), out d))
            {
                // if in range
                if (d >= 0 && d <= 100)
                    // valid
                    return ValidationResult.ValidResult;
                return new ValidationResult(false, "Value is outside on 0-100 range. Whilst this is still a percentage, it is not valid for this input");
            }

            // return result
            return new ValidationResult(false, "Data is not a percentage");
        }
    }

    /// <summary>
    /// Validation rule for normalised doubles
    /// </summary>
    public class NormalisedDoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // if some data
            if (value == null ||
                string.IsNullOrEmpty(value.ToString()))
                // return result
                return new ValidationResult(false, "No data");

            // hold double
            double d;

            // try parse
            if (double.TryParse(value.ToString(), out d))
            {
                // if in range
                if (d >= 0.0d && d <= 1.0d)
                    // valid
                    return ValidationResult.ValidResult;
                return new ValidationResult(false, "Value is outside on 0.0-1.0 range. Whilst this is still a double, it is not normalised");
            }

            // return result
            return new ValidationResult(false, "Data is not a normlalised double");
        }
    }

    /// <summary>
    /// Validation rule for doubles representing degrees
    /// </summary>
    public class DegreeDoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // if some data
            if (value == null ||
                string.IsNullOrEmpty(value.ToString()))
                // return result
                return new ValidationResult(false, "No data");

            // hold double
            double d;

            // try parse
            if (double.TryParse(value.ToString(), out d))
            {
                // if in range
                if (d >= 0.0d && d <= 360.0d)
                    // valid
                    return ValidationResult.ValidResult;
                return new ValidationResult(false, "Value is outside on 0.0-360.0 range. Whilst this is still a degree, it is not within range");
            }

            // return result
            return new ValidationResult(false, "Data is not a normlalised double");
        }
    }

    /// <summary>
    /// Validation rule for hex bytes
    /// </summary>
    public class HexByteValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // if some data
            if (value == null ||
                string.IsNullOrEmpty(value.ToString()))
                // return result
                return new ValidationResult(false, "No data");

            // itterate all aceptable characters
            foreach (var c in value.ToString().ToUpper())
                // if not hex
                if (!"0123456789ABCDEF".Contains(c))
                    // return result
                    return new ValidationResult(false, "Value is not hex");

            // hold Int32
            var data = Convert.ToInt32(value.ToString(), 16);

            // if in range
            if (data >= 0 && data <= 255)
                // valid
                return ValidationResult.ValidResult;
            return new ValidationResult(false, "Value is outside on 0-255 range");
        }
    }

    /// <summary>
    /// Validation rule to ensure a hex string is a byte
    /// </summary>
    [ValueConversion(typeof(byte), typeof(bool))]
    public class HexStringIsByteValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // hold conversion byte
            byte b;

            // get if valid
            var isValid = byte.TryParse(value.ToString(), NumberStyles.AllowHexSpecifier, null, out b);

            // return result
            return new ValidationResult(isValid, isValid ? string.Empty : "The string spoecified was not a hex number between 0 and 255");
        }
    }

    #endregion

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

    /// <summary>
    /// Converts a double specified as a percentage to a normalised double
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class DoublePercentageToNormalisedDoubleConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold output
            double d;

            // if parese
            if (value != null &&
                double.TryParse(value.ToString(), out d))
                // normalise
                return Math.Round(d /= 100, 3);
            return 0.0d;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold output
            double d;

            // if parese
            if (value != null &&
                double.TryParse(value.ToString(), out d))
                // change to percentage
                return Math.Round(d *= 100, 3);
            return 0.0d;
        }

        #endregion
    }

    /// <summary>
    /// Converts between a double percentage and a double degrees value
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class DoublePercentageToDoubleDegreesConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value
            double d;

            // try get d
            if (value != null &&
                double.TryParse(value.ToString(), out d))
                // return
                return Math.Round(360 * (d / 100), 1);
            return 0.0d;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value
            double d;

            // try get d
            if (value != null &&
                double.TryParse(value.ToString(), out d))
                // return
                return Math.Round(100d / 360d * d, 1);
            return 0.0d;
        }

        #endregion
    }

    /// <summary>
    /// Converts a double percentage to a byte (percentage of 255)
    /// </summary>
    [ValueConversion(typeof(double), typeof(byte))]
    public class DoublePercentageToByteConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value as double
            double valueAsDouble;

            // if a value that parses
            if (value != null &&
                double.TryParse(value.ToString(), out valueAsDouble))
                // convert to byte
                return (byte)Math.Round(255d / 100d * valueAsDouble, 0);
            return (byte)0;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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

        #endregion
    }

    /// <summary>
    /// Represents a class for converting between Boolean and Visibility values. A boolean value can be specified as the paramater - this will deifne the state that Visibility.Visible is returned, if the boolean provided as the value parameter doesn't match this value then Visibilty.Hidden is returned
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibiltyConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding source</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // hold value
            bool v;

            // if parses
            if (value != null && bool.TryParse(value.ToString(), out v))
            {
                // if a parameter
                if (parameter != null)
                {
                    // hold paramater
                    bool p;

                    // if parses
                    if (bool.TryParse(parameter.ToString(), out p))
                        // return converted value
                        return v == p ? Visibility.Visible : Visibility.Hidden;
                    throw new ArgumentException();
                }

                // return converted value
                return v ? Visibility.Visible : Visibility.Hidden;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">The value produced by the binding target</param>
        /// <param name="targetType">The type to convert to</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if a value
            if (value != null)
            {
                // try and parse value
                var v = (Visibility)Enum.Parse(typeof(Visibility), value.ToString());

                // if a parameter
                if (parameter != null)
                {
                    // hold paramater
                    bool p;

                    // if parses
                    if (bool.TryParse(parameter.ToString(), out p))
                        // return converted value
                        return v == Visibility.Visible ? p : !p;
                    throw new ArgumentException();
                }

                // return converted value
                return v == Visibility.Visible ? true : false;
            }

            throw new ArgumentException();
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// Delegate for handling bitmap callback
    /// </summary>
    /// <param name="bmp">The bitmap to pass in the callback</param>
    /// <returns></returns>
    public delegate bool BitmapCallback(Bitmap bmp);

    /// <summary>
    /// Delegate for handling lambda callback
    /// </summary>
    public delegate void LambdaCallback();
}