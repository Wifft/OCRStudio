// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Windows.Foundation;
using Windows.System;
using Windows.UI;

using OCRStudio.Helpers.OCROverlay;

using WinUIEx;

namespace OCRStudio.Windows
{
    public sealed partial class OCROverlay : WindowEx
    {
        private AppWindow _apw;
        private OverlappedPresenter _presenter;

        private Point _startPoint;

        private bool _isFirstActivation = false;
        private bool _isDragging = false;

        private RectangleGeometry SelectedGeometry { get; set; }

        private const double ACTIVE_OPACITY = 0.6;

        public OCROverlay()
        {
            InitializeComponent();

            GetWindowPresenter();
            _presenter.IsMaximizable = false;
            _presenter.IsMinimizable = false;
            _presenter.SetBorderAndTitleBar(false, false);
        }

        public void GetWindowPresenter()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

            _apw = AppWindow.GetFromWindowId(wndId);
            _apw.Title = "";
            _apw.IsShownInSwitchers = false;

            _presenter = _apw.Presenter as OverlappedPresenter;
        }

        private void Window_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs e)
        {
            if (!_isFirstActivation) {
                BackgroundImage.Source = CanvasImageHelper.GetWindowBoundsImage(this);
                BackgroundImage.Opacity = ACTIVE_OPACITY;

                RegionClickCanvas.Width = BackgroundImage.Width;
                RegionClickCanvas.Height = BackgroundImage.Height;

                RegionClickCanvas.PointerPressed += RegionClickCanvas_PointerPressed;
                RegionClickCanvas.PointerMoved += RegionClickCanvas_PointerMoved;
                RegionClickCanvas.PointerReleased += RegionClickCanvas_PointerReleased;
            }
         
            _isFirstActivation = true;
        }

        private void BackgroundImage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key) {
                case VirtualKey.Escape:
                    WindowHelper.CloseAllOCROverlays();

                    break;
            }
        }

        private void RegionClickCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _startPoint = e.GetCurrentPoint(RegionClickCanvas).Position;

            RegionClickCanvas.CapturePointer(e.Pointer);
            CursorClipper.ClipCursor(sender as FrameworkElement, BackgroundImage);

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
            CursorClipper.UnClipCursor();

            RegionClickCanvas.Background = new SolidColorBrush(Colors.Transparent);

            App.GetInstance().OcrOverlayGivenRect = SelectedGeometry.Rect;

            WindowHelper.CloseAllOCROverlays();
        }
    }
}
