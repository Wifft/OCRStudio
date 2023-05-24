// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Windows.Graphics;

namespace OCRStudio.Win32Interop
{
    // https://stackoverflow.com/questions/76299400/winui-3-open-a-new-window-in-secondary-monitor
    public sealed partial class ScreenMonitor
    {
        private ScreenMonitor(nint handle)
        {
            Handle = handle;
            var mi = new MONITORINFOEX();
            mi.cbSize = Marshal.SizeOf(mi);
            if (!GetMonitorInfo(handle, ref mi))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            DeviceName = mi.szDevice.ToString();
            Bounds = new RectInt32(mi.rcMonitor.left, mi.rcMonitor.top, mi.rcMonitor.right - mi.rcMonitor.left, mi.rcMonitor.bottom - mi.rcMonitor.top);
            WorkingArea = new RectInt32(mi.rcWork.left, mi.rcWork.top, mi.rcWork.right - mi.rcWork.left, mi.rcWork.bottom - mi.rcWork.top);
            IsPrimary = mi.dwFlags.HasFlag(MONITORINFOF.MONITORINFOF_PRIMARY);
        }

        public nint Handle { get; }
        public bool IsPrimary { get; }
        public RectInt32 WorkingArea { get; }
        public RectInt32 Bounds { get; }
        public string DeviceName { get; }

        public static IEnumerable<ScreenMonitor> All
        {
            get
            {
                var all = new List<ScreenMonitor>();
                EnumDisplayMonitors(nint.Zero, nint.Zero, (m, h, rc, p) => {
                    all.Add(new ScreenMonitor(m));
                    return true;
                }, nint.Zero);
                return all;
            }
        }

        public override string ToString() => DeviceName;
        public static nint GetNearestFromWindow(nint hwnd) => MonitorFromWindow(hwnd, MFW.MONITOR_DEFAULTTONEAREST);
        public static nint GetDesktopMonitorHandle() => GetNearestFromWindow(GetDesktopWindow());
        public static nint GetShellMonitorHandle() => GetNearestFromWindow(GetShellWindow());
        public static ScreenMonitor FromWindow(nint hwnd, MFW flags = MFW.MONITOR_DEFAULTTONULL)
        {
            var h = MonitorFromWindow(hwnd, flags);
            return h != nint.Zero ? new ScreenMonitor(h) : null;
        }

        [Flags]
        public enum MFW
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002,
        }

        [Flags]
        public enum MONITORINFOF
        {
            MONITORINFOF_NONE = 0x00000000,
            MONITORINFOF_PRIMARY = 0x00000001,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public MONITORINFOF dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public const int SM_CMONITORS = 80;

        private delegate bool MonitorEnumProc(nint monitor, nint hdc, nint lprcMonitor, nint lParam);


        [LibraryImport("user32")]
        private static partial nint GetDesktopWindow();

        [LibraryImport("user32")]
        private static partial nint GetShellWindow();

        [LibraryImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnumDisplayMonitors(nint hdc, nint lprcClip, MonitorEnumProc lpfnEnum, nint dwData);

        #pragma warning disable CA1401
        [LibraryImport("user32")]
        public static partial nint MonitorFromWindow(nint hwnd, MFW flags);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "Can't do it here.")]
        public static extern bool GetMonitorInfo(nint hmonitor, ref MONITORINFOEX info);
    }
}
