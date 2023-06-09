﻿// Copyright (c) Wifft 2023
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
        public IntPtr Handle { get; }
        public bool IsPrimary { get; }
        public RectInt32 WorkingArea { get; }
        public RectInt32 Bounds { get; }
        public string DeviceName { get; }

        public static IEnumerable<ScreenMonitor> All
        {
            get
            {
                List<ScreenMonitor> all = new();
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (m, h, rc, p) => {
                    all.Add(new ScreenMonitor(m));
                    return true;
                }, IntPtr.Zero);
                return all;
            }
        }

        public ScreenMonitor(IntPtr handle)
        {
            Handle = handle;
            MONITORINFOEX mi = new();
            mi.cbSize = Marshal.SizeOf(mi);

            if (!GetMonitorInfo(handle, ref mi)) 
                throw new Win32Exception(Marshal.GetLastWin32Error());
                
            DeviceName = mi.szDevice.ToString();
            Bounds = new RectInt32(mi.rcMonitor.left, mi.rcMonitor.top, mi.rcMonitor.right - mi.rcMonitor.left, mi.rcMonitor.bottom - mi.rcMonitor.top);
            WorkingArea = new RectInt32(mi.rcWork.left, mi.rcWork.top, mi.rcWork.right - mi.rcWork.left, mi.rcWork.bottom - mi.rcWork.top);
            IsPrimary = mi.dwFlags.HasFlag(MONITORINFOF.MONITORINFOF_PRIMARY);
        }

        public override string ToString() => DeviceName;
        public static IntPtr GetNearestFromWindow(IntPtr hwnd) => MonitorFromWindow(hwnd, MFW.MONITOR_DEFAULTTONEAREST);
        public static IntPtr GetDesktopMonitorHandle() => GetNearestFromWindow(GetDesktopWindow());
        public static IntPtr GetShellMonitorHandle() => GetNearestFromWindow(GetShellWindow());
        public static ScreenMonitor FromWindow(IntPtr hwnd, MFW flags = MFW.MONITOR_DEFAULTTONULL)
        {
            IntPtr h = MonitorFromWindow(hwnd, flags);
            return h != IntPtr.Zero ? new ScreenMonitor(h) : null;
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
            MONITORINFOF_PRIMARY = 0x00000001
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

        private delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

        [LibraryImport("user32.dll")]
        private static partial IntPtr GetDesktopWindow();

        [LibraryImport("user32.dll")]
        private static partial IntPtr GetShellWindow();

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        #pragma warning disable CA1401
        [LibraryImport("user32.dll")]
        public static partial IntPtr MonitorFromWindow(IntPtr hwnd, MFW flags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "Can't do it here.")]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, ref MONITORINFOEX info);
    }
}
