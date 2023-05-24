// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Graphics.Display;

namespace OCRStudio.Helpers.OCROverlay
{
    public class DpiHelper
    {
        public static (double RawDpiX, double RawDpiY) GetRawDpi()
        {
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
            float logicalDpi = displayInformation.LogicalDpi;

            float rawDpiX = logicalDpi;
            float rawDpiY = logicalDpi;

            return (rawDpiX, rawDpiY);
        }
    }
}
