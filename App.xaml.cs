// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Abstractions;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

using Windows.Storage;

using WifftOCR.Services.Consumers;
using WifftOCR.ConsoleFormatters;
using WifftOCR.DataModels;
using WifftOCR.Interfaces;
using WifftOCR.Services;
using WifftOCR.ViewModels;
using WifftOCR.Views;

namespace WifftOCR
{
    public partial class App : Application
    {
        private MainWindow m_window;

        public const string SETTINGS_LOCATION_URI = "ms-appdata:///roaming/settings.json";

        public IHost Host { get; }

        public static T GetService<T>() where T : class
        {
            if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
                throw new ArgumentException($"{typeof(T)} needs to be a registered in ConfigureServices within App.xaml.cs");

            return service;
        }

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging((context, logging) => {
                    logging.SetMinimumLevel(LogLevel.Debug)
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning)
                        .AddConsole(options => options.FormatterName = "wifftFormatter")
                        .AddConsoleFormatter<CustomFormatter, CustomFormatter.Options>(options => options.CustomPrefix = "[WifftOCR] ");
                })
                .ConfigureServices((context, services) => {
                    services.AddSingleton<IFileSystem, FileSystem>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IScopedProcessingService, OcrService>();

                    services.AddHostedService<OcrServiceConsumer>();

                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();

                    services.AddTransient<WelcomePage>();

                    services.AddTransient<CaptureAreasPage>();
                    services.AddTransient<CaptureAreasViewModel>();

                    services.AddTransient<SettingsPage>();
                    services.AddTransient<SettingsViewModel>();

                    services.AddScoped<IScopedProcessingService, OcrService>();
                })
                .Build();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            MainWindow.EnsurePageIsSelected();

            await CheckIfConfigFileExists();
        }

        private static async Task CheckIfConfigFileExists()
        {
            StorageFile configFile;

            try {
                configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(SETTINGS_LOCATION_URI));
                Console.WriteLine(configFile.Path);
            } catch (FileNotFoundException) {
                StorageFolder folder = ApplicationData.Current.RoamingFolder;

                await CreateConfigFile(folder);
            }
        }

        private static async Task CreateConfigFile(StorageFolder folder)
        {
            StorageFile file = await folder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);

            Settings settings = new()
            {
                Observer = null,
                ServerEndpoint = null,
                ServerKey = null,
                CaptureAreas = new ObservableCollection<CaptureArea>()
            };

            using Stream stream = await file.OpenStreamForWriteAsync();
            await JsonSerializer.SerializeAsync(stream, settings);
        }
    }
}
