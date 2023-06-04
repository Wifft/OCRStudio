// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using OCRStudio.DataModels;

namespace OCRStudio.Helpers
{
    internal static class ValidationHelper
    {
        public static bool ValidateCaptureArea(CaptureArea captureArea)
        {
            if (captureArea == null) return false;

            if (string.IsNullOrEmpty(captureArea.Name)) return false;
            if (int.IsNegative(captureArea.SizeW)) return false;
            if (int.IsNegative(captureArea.SizeH)) return false;

            return true;
        }
    }
}
