// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace OCRStudio.Win32Interop
{
    public static class Mouse
    {
        #pragma warning disable CA1401
        #pragma warning disable SYSLIB1054
        [DllImport("user32.dll")]
        public static extern bool ClipCursor(ref Window.RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool ClipCursor([In] IntPtr lpRect);
    }
}
