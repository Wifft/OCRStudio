// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WifftOCR.DataModels;

namespace WifftOCR.Helpers
{
    internal static class ValidationHelper
    {
        public static bool ValidateCaptureArea(CaptureArea captureArea)
        {
            if (captureArea == null) return false;

            if (string.IsNullOrEmpty(captureArea.Name)) return false;
            if (int.IsNegative(captureArea.VectorAX)) return false;
            if (int.IsNegative(captureArea.VectorAY)) return false;
            if (int.IsNegative(captureArea.VectorBX)) return false;
            if (int.IsNegative(captureArea.VectorBY)) return false;

            return true;
        }
    }
}
