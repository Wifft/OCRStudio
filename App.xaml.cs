// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Abstractions;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

using Windows.Storage;

using WifftOCR.Services.Consumers;
using WifftOCR.Loggers;
using WifftOCR.DataModels;
using WifftOCR.Interfaces;
using WifftOCR.Providers;
using WifftOCR.Services;
using WifftOCR.ViewModels;
using WifftOCR.Views;

namespace WifftOCR
{
    public partial class App : Application
    {
        private Mutex _mutex;

        private MainWindow m_window;

        public const string SETTINGS_LOCATION_URI = "ms-appdata:///roaming/settings.json";
        public const string LOG_FILE_LOCATION_URI = "ms-appdata:///roaming/system.log";

        public IHost Host { get; }
        public ILoggerFactory LoggerFactory { get; private set;  }

        public static T GetService<T>() where T : class
        {
            if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
                throw new ArgumentException($"{typeof(T)} needs to be a registered in ConfigureServices within App.xaml.cs");

            return service;
        }

        public static App GetInstance()
        {
            App app = Current as App;

            return app;
        }

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging((context, logging) => {
                    logging.SetMinimumLevel(LogLevel.Debug)
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
                })
                .ConfigureServices((context, services) => {
                    services.AddSingleton<IFileSystem, FileSystem>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IFileLoggerService, FileLoggerService>();
;
                    services.AddHostedService<OcrRecorderServiceConsumer>();
                    services.AddScoped<IScopedProcessingService, OcrRecorderService>();

                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();

                    services.AddTransient<WelcomePage>();
                    services.AddTransient<WelcomePageViewModel>();

                    services.AddTransient<CaptureAreasPage>();
                    services.AddTransient<CaptureAreasViewModel>();

                    services.AddTransient<SettingsPage>();
                    services.AddTransient<SettingsViewModel>();
                })
                .Build();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            _mutex = new Mutex(true, "WifftOCR_SingleInstanceMutex", out bool isNewInstance);
            if (!isNewInstance) {
                m_window = Window.Current as MainWindow;
                m_window?.Activate();

                Current.Exit();

                return;
            }

            base.OnLaunched(args);

            m_window = new MainWindow();
            m_window.Activate();
            MainWindow.EnsurePageIsSelected();

            await CheckIfConfigFileExists();
            await CheckIfLogFileExists();

            BuildLoggerFactory();

            _mutex.ReleaseMutex();
        }

        private void BuildLoggerFactory()
        {
            ILoggerFactory loggerFactory = Host.Services.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new FileLoggerProvider());

            LoggerFactory = loggerFactory;
        }

        private static async Task CheckIfConfigFileExists()
        {
            StorageFile configFile;

            try {
                configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(SETTINGS_LOCATION_URI));
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

        private static async Task CheckIfLogFileExists()
        {
            StorageFile logFile;

            try {
                logFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(LOG_FILE_LOCATION_URI));
            } catch (FileNotFoundException) {
                StorageFolder folder = ApplicationData.Current.RoamingFolder;

                await CreateLogFile(folder);
            }
        }

        private static async Task CreateLogFile(StorageFolder folder)
        {
            await folder.CreateFileAsync("system.log", CreationCollisionOption.OpenIfExists);
        }
    }
}
