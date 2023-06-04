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
        
        public static bool ClipCursor(FrameworkElement element, FrameworkElement container, ScreenMonitor screen)
        {
            GeneralTransform transform = element.TransformToVisual(container);
            Point topLeft = transform.TransformPoint(new Point(screen.Bounds.X, screen.Bounds.Y));

            if (container == null) return false;

            const double DPI_X = DPI_96 * 1.0d;
            const double DPI_Y = DPI_96 * 1.0d;

            int width = (int) (element.ActualWidth * DPI_X / DPI_96);
            int height = (int) (element.ActualHeight * DPI_Y / DPI_96);

            Win32Interop.Window.RECT rect = new()
            {
                Left = (int) topLeft.X,
                Top = (int) topLeft.Y,
                Right = (int) topLeft.X + width,
                Bottom = (int) topLeft.Y + height
            };

            return Mouse.ClipCursor(ref rect);
        }

        public static bool UnClipCursor()
        {
            return Mouse.ClipCursor(nint.Zero);
        }
    }
}
