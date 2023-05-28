// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;

using OCRStudio.Views.OCROverlay;
using OCRStudio.Helpers.OCROverlay;

using WinUIEx;
using System.Threading.Tasks;

namespace OCRStudio.Windows
{
    public sealed partial class OCROverlay : WindowEx
    {
        private AppWindow _apw;
        private OverlappedPresenter _presenter;

        public ImageSource ScreenContent;

        public OCROverlay()
        {
            InitializeComponent();

            GetWindowPresenter();
            _presenter.IsMaximizable = false;
            _presenter.IsMinimizable = false;
            _presenter.SetBorderAndTitleBar(false, false);
        }

        public void GetWindowPresenter()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

            _apw = AppWindow.GetFromWindowId(wndId);
            _apw.Title = "";
            _apw.IsShownInSwitchers = false;

            _presenter = _apw.Presenter as OverlappedPresenter;
        }

        public void Window_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs e) => ContentFrame.Navigate(typeof(CanvasPage));
    }
}
