// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using WifftOCR.ViewModels;

namespace WifftOCR.Views
{
    public sealed partial class WelcomePage : Page
    {
        public WelcomePageViewModel ViewModel { get; set; }

        public WelcomePage()
        {
            InitializeComponent();

            ViewModel = App.GetService<WelcomePageViewModel>();
            DataContext = ViewModel;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            StopButton.IsEnabled = true;

            await ViewModel.StartOcrRecorderService();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            StartButton.IsEnabled = true;

            await ViewModel.StopOcrRecorderService();
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            App appInstance = App.GetInstance();

            if (appInstance.OcrRecorderServiceRunning) {
                ViewModel.ReadLogFile();
                LogViewer.Text = ViewModel.LogLines;
            }

            StartButton.IsEnabled = !appInstance.OcrRecorderServiceRunning;
            StopButton.IsEnabled = appInstance.OcrRecorderServiceRunning;
        }
    }
}
