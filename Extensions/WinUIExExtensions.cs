// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

using OCRStudio.Helpers.OCROverlay;
using OCRStudio.Win32Interop;

using WinUIEx;

namespace OCRStudio.Extensions
{
    public static class WinUIExExtensions
    {
        public static Point GetAbsolutePosition(this WindowEx window)
        {
            if (!WindowHelper.IsWindowMaximized())
                return new Point((int) window.Bounds.Left, (int) window.Bounds.Top);

            Rectangle r;
            bool multiMonitorSupported = System32.GetSystemMetrics(ScreenMonitor.SM_CMONITORS) != 0;
            if (!multiMonitorSupported) {
                Window.RECT rc = default;
                System32.SystemParametersInfo(48, 0, ref rc, 0);
                r = new Rectangle(rc.Left, rc.Top, rc.Width, rc.Height);
            } else {
                IntPtr hmonitor = ScreenMonitor.MonitorFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(window), ScreenMonitor.MFW.MONITOR_DEFAULTTONEAREST);
                ScreenMonitor.MONITORINFOEX info = new();
                ScreenMonitor.GetMonitorInfo(new HandleRef(null, hmonitor).Handle, ref info);
                r = new Rectangle(info.rcMonitor.right, info.rcMonitor.top, info.rcMonitor.bottom, info.rcMonitor.left);
            }

            return new Point(r.X, r.Y);
        }
    }
}
