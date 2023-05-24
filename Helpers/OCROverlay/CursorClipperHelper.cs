// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Windows.Foundation;

using OCRStudio.Win32Interop;

namespace OCRStudio.Helpers.OCROverlay
{
    public static class CursorClipper
    {
        public static bool ClipCursor(FrameworkElement element)
        {
            const double dpi96 = 96.0;

            GeneralTransform transform = element.TransformToVisual(Window.Current.Content as FrameworkElement);
            Point topLeft = transform.TransformPoint(new Point(0, 0));

            if (Window.Current.Compositor == null) return false;

            double dpiX = dpi96 * DpiHelper.GetRawDpi().RawDpiX;
            double dpiY = dpi96 * DpiHelper.GetRawDpi().RawDpiY;

            int width = (int)((element.ActualWidth + 1) * dpiX / dpi96);
            int height = (int)((element.ActualHeight + 1) * dpiY / dpi96);

            System32.RECT rect = new()
            {
                Left = (int)topLeft.X,
                Top = (int)topLeft.Y,
                Right = (int)topLeft.X + width,
                Bottom = (int)topLeft.Y + height,
            };

            return Mouse.ClipCursor(ref rect);
        }

        public static bool UnClipCursor()
        {
            return Mouse.ClipCursor(nint.Zero);
        }
    }
}
