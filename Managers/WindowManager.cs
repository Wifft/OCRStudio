// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Microsoft.UI.Xaml;

namespace OCRStudio.Managers
{
    public static class WindowManager
    {
        private static readonly List<Window> openWindows = new();

        public static void RegisterWindow(Window window) => openWindows.Add(window);
        public static void UnregisterWindow(Window window) => openWindows.Remove(window);
        public static IReadOnlyList<Window> GetOpenWindows() => openWindows.AsReadOnly();
    }
}
