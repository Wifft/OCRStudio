// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Microsoft.UI;
using Microsoft.UI.Windowing;

using OCRStudio.Win32Interop;

using WinUIEx;

namespace OCRStudio.Windows
{
    public sealed partial class OCROverlay : WindowEx
    {
        private AppWindow _apw;
        private OverlappedPresenter _presenter;

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

            _ = Window.SetWindowLong(hWnd, Window.GWL_EXSTYLE, Window.GetWindowLong(hWnd, Window.GWL_EXSTYLE) ^ Window.WS_EX_LAYERED);
            _ = Window.SetLayeredWindowAttributes(hWnd, 0, 128, Window.LWA_ALPHA);
        }
    }
}
