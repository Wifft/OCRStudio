// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

using WinRT;

using OCRStudio.Views;

using WinUIEx;

namespace OCRStudio.ProgramWindows
{
    public sealed partial class MainWindow : WindowEx
    {
        public const string WINDOW_TITLE = "OCR Studio";

        private WindowsSystemDispatcherQueueHelper _wsdqHelper;
        private MicaController _micaBackdropController;
        private DesktopAcrylicController _acrylicBackdropController;
        private SystemBackdropConfiguration _configurationSource;

        public MainWindow()
        {
            InitializeComponent();

            Title = WINDOW_TITLE;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            TrySetSystemBackdrop();
        }

        private bool TrySetSystemBackdrop()
        {
            _wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            _configurationSource = new SystemBackdropConfiguration();
            Activated += Window_Activated;
            Closed += Window_Closed;
            ((FrameworkElement) Content).ActualThemeChanged += Window_ThemeChanged;

            _configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            if (MicaController.IsSupported()) {
                _micaBackdropController = new MicaController();
                _micaBackdropController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                _micaBackdropController.SetSystemBackdropConfiguration(_configurationSource);
                
                return true;
            } else if (DesktopAcrylicController.IsSupported()) {
                _acrylicBackdropController = new DesktopAcrylicController();
                _acrylicBackdropController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                _acrylicBackdropController.SetSystemBackdropConfiguration(_configurationSource);

                return true;
            }

            return false;
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            if (_micaBackdropController != null) {
                _micaBackdropController.Dispose();
                _micaBackdropController = null;
            } else if (_acrylicBackdropController != null) {
                _acrylicBackdropController.Dispose();
                _acrylicBackdropController = null;
            }

            Activated -= Window_Activated;
            _configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (_configurationSource != null) SetConfigurationSourceTheme();
        }

        private void SetConfigurationSourceTheme()
        {
            _configurationSource.Theme = SystemBackdropTheme.Dark;
        }

        public static void NavigateToSection(Type type)
        {
            ShellPage.Navigate(type);
        }

        public static void EnsurePageIsSelected()
        {
            ShellPage.EnsurePageIsSelected();
        }

        private sealed class WindowsSystemDispatcherQueueHelper
        {
            [StructLayout(LayoutKind.Sequential)]
            internal struct DispatcherQueueOptions
            {
                internal int dwSize;
                internal int threadType;
                internal int apartmentType;
            }

            [DllImport("CoreMessaging.dll")]
            private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

            private const int DQTYPE_THREAD_CURRENT = 2;
            private const int DQTAT_COM_STA = 2;

            private object m_dispatcherQueueController = null;
            
            public void EnsureWindowsSystemDispatcherQueueController()
            {
                if (Windows.System.DispatcherQueue.GetForCurrentThread() != null) return;

                if (m_dispatcherQueueController == null)
                {
                    DispatcherQueueOptions options;
                    options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                    options.threadType = DQTYPE_THREAD_CURRENT; 
                    options.apartmentType = DQTAT_COM_STA;

                    _ = CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
                }
            }
        }
    }
}