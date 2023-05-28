// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Dispatching;
using Windows.Graphics.Display;

namespace OCRStudio.Helpers.OCROverlay
{
    public class DpiHelper
    {
        public static (double RawDpiX, double RawDpiY) GetRawDpi()
        {
            float rawDpiX = 0, rawDpiY = 0;

            DispatcherQueue.GetForCurrentThread().TryEnqueue(() => {
                DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
                float logicalDpi = displayInformation.LogicalDpi;

                rawDpiX = logicalDpi;
                rawDpiY = logicalDpi;
            });

            return (rawDpiX, rawDpiY);
        }
    }
}
