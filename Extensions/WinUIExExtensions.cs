// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Drawing;

using OCRStudio.Win32Interop;

using WinUIEx;

namespace OCRStudio.Extensions
{
    public static class WinUIExExtensions
    {
        public static Point GetAbsolutePosition(this WindowEx window)
        {
            Rectangle rectangle;
            bool multiMonitorSupported = System32.GetSystemMetrics(ScreenMonitor.SM_CMONITORS) != 0;
            if (!multiMonitorSupported) {
                Window.RECT rc = default;
                System32.SystemParametersInfo(48, 0, ref rc, 0);
                rectangle = new Rectangle(rc.Left, rc.Top, rc.Width, rc.Height);

                return new Point(rectangle.X, rectangle.Y);
            }

            IntPtr hmonitor = ScreenMonitor.MonitorFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(window), ScreenMonitor.MFW.MONITOR_DEFAULTTONEAREST);
            ScreenMonitor monitor = new(hmonitor);

            rectangle = new Rectangle(monitor.WorkingArea.X, monitor.WorkingArea.Y, monitor.WorkingArea.Width, monitor.WorkingArea.Height);

            return new Point(rectangle.X, rectangle.Y);
        }
    }
}
