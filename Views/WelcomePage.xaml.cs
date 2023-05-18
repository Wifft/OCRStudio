// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


using WifftOCR.Providers;

namespace WifftOCR.Views
{
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();

            IServiceProvider serviceProvider = App.GetInstance().Host.Services;

            DispatcherQueue.GetForCurrentThread().TryEnqueue(() => {
                ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new XamlLogViewerProvider(LogViewer));
            });
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            await App.GetInstance().Host.StartAsync();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            await App.GetInstance().Host.StopAsync();
        }
    }
}
