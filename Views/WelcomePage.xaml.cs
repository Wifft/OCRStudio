// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using OCRStudio.Enums;
using OCRStudio.ViewModels;

using CommunityToolkit.Mvvm.Input;

namespace OCRStudio.Views
{
    public sealed partial class WelcomePage : Page
    {
        internal WelcomePageViewModel ViewModel { get; set; }

        public ICommand StartOcrRecorderServiceCommand => new AsyncRelayCommand(StartOcrRecorderService);
        public ICommand StopOcrRecorderServiceCommand => new AsyncRelayCommand(StopOcrRecorderService);

        public WelcomePage()
        {
            InitializeComponent();

            ViewModel = App.GetService<WelcomePageViewModel>();
            DataContext = ViewModel;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private async Task StartOcrRecorderService()
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            await ViewModel.StartOcrRecorderService();
        }

        private async Task StopOcrRecorderService()
        {
            StopButton.IsEnabled = true;
            StartButton.IsEnabled = true;

            await ViewModel.StopOcrRecorderService();
        }

        /*private void ScreenshootModeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            RecorderModeButton.IsEnabled = true;

            ViewModel.ExecutionMode = ExecutionMode.Screenshoot;

            RecorderModeButtons.Visibility = Visibility.Collapsed;
            ScreenshotModeButtons.Visibility = Visibility.Visible;
        }*/

        /*private void RecorderModeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            ScreenshootModeButton.IsEnabled = true;

            ViewModel.ExecutionMode = ExecutionMode.Recorder;

            ScreenshotModeButtons.Visibility = Visibility.Collapsed;
            RecorderModeButtons.Visibility = Visibility.Visible;
        }*/

        private void CompositionTarget_Rendering(object sender, object e)
        {
            App appInstance = App.GetInstance();

            if (appInstance.OcrRecorderServiceRunning) {
                ViewModel.ReadLogFile();
                LogViewer.Text = ViewModel.LogLines;
            }

            StartButton.IsEnabled = !appInstance.OcrRecorderServiceRunning;
            StopButton.IsEnabled = appInstance.OcrRecorderServiceRunning;

            if (ActualHeight > 0) LogViewerContainer.MaxHeight = ActualHeight - 240;
        }
    }
}
