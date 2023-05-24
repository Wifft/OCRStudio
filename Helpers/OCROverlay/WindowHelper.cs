// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.UI.Xaml;

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
                Windows.OCROverlay overlay = new();
                overlay.SetWindowSize(200, 200);

                int windowLeft = screen.WorkingArea.X + (screen.WorkingArea.Width / 2);
                if (screen.WorkingArea.X >= 0) windowLeft = screen.WorkingArea.X;

                int windowTop = screen.WorkingArea.Y + (screen.WorkingArea.Height / 2);                
                if (screen.WorkingArea.Y >= 0) windowTop = screen.WorkingArea.Y;
                
                overlay.AppWindow.Move(new PointInt32(windowLeft, windowTop));
                overlay.Activate();

                ActivateWindow(overlay);
            }
        }

        internal static bool IsOCROverlayCreated()
        {
            IReadOnlyList<Window> allWindows = Managers.WindowManager.GetOpenWindows();

            foreach (Window window in allWindows) {
                if (window is Windows.OCROverlay) return true;
            }

            return false;
        }

        internal static void CloseAllOCROverlays()
        {
            IReadOnlyList<Window> allWindows = Managers.WindowManager.GetOpenWindows();

            foreach (Window window in allWindows) {
                if (window is Windows.OCROverlay overlay) { 
                    overlay.Close();

                    Managers.WindowManager.UnregisterWindow(overlay);
                }
            }
        }

        public static void ActivateWindow(Window window)
        {
            IntPtr handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            IntPtr fgHandle = System32.GetForegroundWindow();

            uint threadId1 = System32.GetWindowThreadProcessId(handle, System.IntPtr.Zero);
            uint threadId2 = System32.GetWindowThreadProcessId(fgHandle, System.IntPtr.Zero);

            if (threadId1 != threadId2) {
                System32.AttachThreadInput(threadId1, threadId2, true);
                System32.SetForegroundWindow(handle);
                System32.AttachThreadInput(threadId1, threadId2, false);
            } else System32.SetForegroundWindow(handle);
        }

        public static bool IsWindowMaximized()
        {
            Window currentWindow = Window.Current;
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
