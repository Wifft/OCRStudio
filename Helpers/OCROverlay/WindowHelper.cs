// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.Foundation;

using OCRStudio.Win32Interop;

using WinUIEx;

namespace OCRStudio.Helpers.OCROverlay
{
    public class WindowHelper
    {
        public static void LaunchOCROverlayOnEveryScreen()
        {
            if (IsOCROverlayCreated()) throw new Exception("Tried to launch the overlay, but it has been already created.");

            foreach (ScreenMonitor screen in ScreenMonitor.All.ToArray()) {
                Windows.OCROverlay overlay = new()
                {
                    CurrentScreen = screen
                };
                overlay.SetWindowSize(screen.WorkingArea.Width, screen.WorkingArea.Height);
                overlay.AppWindow.Move(new PointInt32(screen.WorkingArea.X, screen.WorkingArea.Y));
                overlay.Activate();

                Managers.WindowManager.RegisterWindow(overlay);

                ActivateWindow(overlay);
            }
        }

        internal static bool IsOCROverlayCreated()
        {
            IReadOnlyList<Microsoft.UI.Xaml.Window> allWindows = Managers.WindowManager.GetOpenWindows();

            foreach (Microsoft.UI.Xaml.Window window in allWindows) {
                if (window is Windows.OCROverlay) return true;
            }

            return false;
        }

        internal static void CloseAllOCROverlays()
        {
            IReadOnlyList<Microsoft.UI.Xaml.Window> allWindows = Managers.WindowManager.GetOpenWindows();

            foreach (Microsoft.UI.Xaml.Window window in allWindows) {
                if (window is Windows.OCROverlay overlay) { 
                    overlay.Close();
                }
            }

            for (int i = 0; i < allWindows.Count; i++) {
                if (allWindows.ToArray()[i] is Windows.OCROverlay)
                    Managers.WindowManager.UnregisterWindow(allWindows.ToArray()[i]);
            }
        }

        public static void ActivateWindow(Microsoft.UI.Xaml.Window window)
        {
            IntPtr handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            IntPtr fgHandle = Win32Interop.Window.GetForegroundWindow();

            uint threadId1 = Win32Interop.Window.GetWindowThreadProcessId(handle, System.IntPtr.Zero);
            uint threadId2 = Win32Interop.Window.GetWindowThreadProcessId(fgHandle, System.IntPtr.Zero);

            if (threadId1 != threadId2) {
                System32.AttachThreadInput(threadId1, threadId2, true);
                Win32Interop.Window.SetForegroundWindow(handle);
                System32.AttachThreadInput(threadId1, threadId2, false);
            } else Win32Interop.Window.SetForegroundWindow(handle);
        }

        public static bool IsWindowMaximized()
        {
            Microsoft.UI.Xaml.Window currentWindow = Microsoft.UI.Xaml.Window.Current;
            Rect windowBounds = currentWindow.Bounds;

            Size systemDisplayArea = GetSystemDisplayArea();

            return windowBounds.Width == systemDisplayArea.Width && windowBounds.Height == systemDisplayArea.Height;
        }

        private static Size GetSystemDisplayArea()
        {
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
            float logicalDpi = displayInformation.LogicalDpi;

            float systemDisplayAreaWidth = displayInformation.ScreenWidthInRawPixels / logicalDpi;
            float systemDisplayAreaHeight = displayInformation.ScreenHeightInRawPixels / logicalDpi;

            return new Size(systemDisplayAreaWidth, systemDisplayAreaHeight);
        }
    }
}
