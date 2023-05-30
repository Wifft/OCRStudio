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
        public const double DPI_96 = 96.0;
        
        public static bool ClipCursor(FrameworkElement element, FrameworkElement container)
        {
            GeneralTransform transform = element.TransformToVisual(container);
            Point topLeft = transform.TransformPoint(new Point(0, 0));

            if (container == null) return false;

            double dpiX = DPI_96;
            double dpiY = DPI_96;

            int width = (int) (element.ActualWidth * dpiX / DPI_96);
            int height = (int) (element.ActualHeight * dpiY / DPI_96);

            Win32Interop.Window.RECT rect = new()
            {
                Left = (int) topLeft.X,
                Top = (int) topLeft.Y,
                Right = (int) topLeft.X + width,
                Bottom = (int) topLeft.Y + height,
            };

            return Mouse.ClipCursor(ref rect);
        }

        public static bool UnClipCursor()
        {
            return Mouse.ClipCursor(nint.Zero);
        }
    }
}
