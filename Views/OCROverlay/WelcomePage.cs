// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using OCRStudio.Helpers.OCROverlay;

namespace OCRStudio.Views.OCROverlay
{
    public sealed partial class WelcomePage : BasePage
    {
        protected override void RegionClickCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isDragging = false;

            GetRegionClickCanvas().ReleasePointerCapture(e.Pointer);
            //CursorClipper.UnClipCursor();

            GetRegionClickCanvas().Background = new SolidColorBrush(Colors.Transparent);

            if (SelectedGeometry != null)
                App.GetInstance().OcrOverlayGivenRect = SelectedGeometry.Rect;

            WindowHelper.CloseAllOCROverlays();
        }
    }
}
