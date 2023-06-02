// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

using OCRStudio.Helpers.OCROverlay;
using OCRStudio.Win32Interop;

using WinUIEx;

namespace OCRStudio.Windows
{
    public sealed partial class OCROverlay : WindowEx
    {
        private AppWindow _apw;
        private OverlappedPresenter _presenter;

        private bool _firstActivation = false;

        public ScreenMonitor CurrentScreen { get; set; }

        public OCROverlay()
        {
            InitializeComponent();

            GetWindowPresenter();
            _presenter.IsMaximizable = false;
            _presenter.IsAlwaysOnTop = true;
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

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (!_firstActivation) {
                CanvasImageHelper.GetWindowBoundsImage(this);
                CanvasPage.Screen = CurrentScreen;
            }

            _firstActivation = true;
        }
    }
}
