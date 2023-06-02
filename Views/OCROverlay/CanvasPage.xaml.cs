using System;
using System.IO;

using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using Windows.UI;
using Windows.Foundation;
using Windows.Storage;

using OCRStudio.Helpers.OCROverlay;
using OCRStudio.Win32Interop;

namespace OCRStudio.Views.OCROverlay
{
    public sealed partial class CanvasPage : Page
    {
        private Point _startPoint;
        private bool _isDragging = false;

        private RectangleGeometry SelectedGeometry { get; set; }
        public ScreenMonitor Screen { get; set; }

        private const double ACTIVE_OPACITY = 0.6;

        public CanvasPage()
        {
            InitializeComponent();
        }

        private void CanvasPage_Loaded(object sender, RoutedEventArgs e)
        {
            ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Cross);

            Uri imgPath = new(Path.Join(ApplicationData.Current.TemporaryFolder.Path, $"ocr_overlay_capture_{Screen.Handle}.bmp"));

            BackgroundImage.Source = new BitmapImage(imgPath);
            BackgroundImage.Opacity = ACTIVE_OPACITY;

            RegionClickCanvas.Width = BackgroundImage.Width;
            RegionClickCanvas.Height = BackgroundImage.Height;

            RegionClickCanvas.PointerPressed += RegionClickCanvas_PointerPressed;
            RegionClickCanvas.PointerMoved += RegionClickCanvas_PointerMoved;
            RegionClickCanvas.PointerReleased += RegionClickCanvas_PointerReleased;
        }

        private void RegionClickCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _startPoint = e.GetCurrentPoint(RegionClickCanvas).Position;

            RegionClickCanvas.CapturePointer(e.Pointer);
            //CursorClipper.ClipCursor(sender as FrameworkElement, BackgroundImage);

            RegionClickCanvas.Clip = null;

            _isDragging = true;
        }

        private void RegionClickCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isDragging) return;

            Point currentPoint = e.GetCurrentPoint(RegionClickCanvas).Position;

            double x = _startPoint.X < currentPoint.X ? _startPoint.X : currentPoint.X;
            double y = _startPoint.Y < currentPoint.Y ? _startPoint.Y : currentPoint.Y;
            double width = _startPoint.X < currentPoint.X ? currentPoint.X - _startPoint.X : _startPoint.X - currentPoint.X;
            double height = _startPoint.Y < currentPoint.Y ? currentPoint.Y - _startPoint.Y : _startPoint.Y - currentPoint.Y;

            Rect selectionRect = new(x, y, width, height);

            SelectedGeometry = new()
            {
                Rect = selectionRect
            };

            byte bgAlpha = (byte) Math.Round(255 * (ACTIVE_OPACITY - 0.1));

            RegionClickCanvas.Clip = SelectedGeometry;
            RegionClickCanvas.Background = new SolidColorBrush(Color.FromArgb(bgAlpha, 255, 0, 0));
        }

        private void RegionClickCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = false;

            RegionClickCanvas.ReleasePointerCapture(e.Pointer);
            //CursorClipper.UnClipCursor();

            RegionClickCanvas.Background = new SolidColorBrush(Colors.Transparent);

            if (SelectedGeometry != null)
                App.GetInstance().OcrOverlayGivenRect = SelectedGeometry.Rect;

            WindowHelper.CloseAllOCROverlays();
        }
    }
}
