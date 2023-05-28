using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using Windows.System;

using System.Diagnostics;

using OCRStudio.Helpers.OCROverlay;
using System;

namespace OCRStudio.Views.OCROverlay
{
    public sealed partial class CanvasPage : Page
    {
        private const double ACTIVE_OPACITY = 0.4;

        public CanvasPage()
        {
            InitializeComponent();
            
            Loaded += CanvasPage_Loaded;
        }

        private void CanvasPage_Loaded(object sender, RoutedEventArgs e)
        {
            KeyDown += CanvasPage_KeyDown;

            BackgroundImage.Source = null;
            BackgroundImage.Opacity = ACTIVE_OPACITY;
        }

        private void CanvasPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine(e.Key);

            switch (e.Key) {
                case VirtualKey.Escape:
                    WindowHelper.CloseAllOCROverlays();
                    break;
            }
        }
    }
}
