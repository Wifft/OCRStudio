﻿// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Abstractions;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

using Windows.Foundation;
using Windows.Storage;

using OCRStudio.DataModels;
using OCRStudio.Interfaces;
using OCRStudio.Providers;
using OCRStudio.Services;
using OCRStudio.Services.Consumers;
using OCRStudio.ViewModels;
using OCRStudio.Views;
using OCRStudio.Windows;
using OCRStudio.Win32Interop;

using WinUIEx;

namespace OCRStudio
{
    public partial class App : Application
    {
        private readonly SemaphoreSlim _asyncLock = new(1, 1);

        private Mutex _mutex;
        private MainWindow _mainWindow;

        public const string SETTINGS_LOCATION_URI = "ms-appdata:///roaming/settings.json";

        public IHost Host { get; }
        public ILoggerFactory LoggerFactory { get; private set; }

        public IHost OcrRecorderServiceHost { get; set; }
        public ILoggerFactory OcrRecorderServiceLoggerFactory { get; private set; }

        public IHost OcrScreenshotServiceHost { get; set; }
        public ILoggerFactory OcrScreenshotServiceLoggerFactory { get; private set; }

        public bool OcrRecorderServiceRunning = false;
        public bool OcrScreenshotServiceRunning = false;

        public Rect? OcrOverlayGivenRect { get; set; } = null;

        public string CurrentSessionLogFileName { get; private set; }

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

                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();

                    services.AddTransient<WelcomePage>();
                    services.AddTransient<WelcomePageViewModel>();

                    services.AddTransient<CaptureAreasPage>();
                    services.AddTransient<CaptureAreasViewModel>();

                    services.AddTransient<LogsHistoryPage>();
                    services.AddTransient<LogsHistoryViewModel>();

                    services.AddTransient<SettingsPage>();
                    services.AddTransient<SettingsViewModel>();
                })
                .Build();

            OcrRecorderServiceHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging(ConfigureLoggingForOcrRecorderServiceHostCallback)
                .ConfigureServices(ConfigureServicesForOcrRecorderServiceHostCallback)
                .Build();

            OcrScreenshotServiceHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging(ConfigureLoggingForOcrScreenshotServiceHostCallback)
                .ConfigureServices(ConfigureServicesForOcrScreenshotServiceHostCallback)
                .Build();
        }

        public static void ConfigureServicesForOcrRecorderServiceHostCallback(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<OcrRecorderServiceConsumer>();
            services.AddScoped<IScopedProcessingService, OcrRecorderService>();
        }

        public static void ConfigureLoggingForOcrRecorderServiceHostCallback(HostBuilderContext context, ILoggingBuilder logging)
        {
            logging.SetMinimumLevel(LogLevel.Debug)
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
        }

        public static void ConfigureServicesForOcrScreenshotServiceHostCallback(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<OcrScreenshotServiceConsumer>();
            services.AddScoped<IScopedProcessingService, OcrScreenshotService>();
        }

        public static void ConfigureLoggingForOcrScreenshotServiceHostCallback(HostBuilderContext context, ILoggingBuilder logging)
        {
            logging.SetMinimumLevel(LogLevel.Debug)
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            _mutex = new Mutex(true, "OCRStudio_SingleInstanceMutex", out bool isNewInstance);
            if (!isNewInstance) {
                FocusAppWindow();
                Current.Exit();

                return;
            }

            await CheckIfConfigFileExists();
            await CheckIfLogsFolderExists();
            
            _mainWindow = new MainWindow();
            _mainWindow.Activate();

            Managers.WindowManager.RegisterWindow(_mainWindow);

            #if DEBUG
            CenterAppWindowOnScreen();
            #endif

            BuildOcrRecorderServiceLoggerFactory();
            BuildLoggerFactory();

            _mutex.ReleaseMutex();

            base.OnLaunched(args);

            MainWindow.EnsurePageIsSelected();
        }

        public void BuildLoggerFactory()
        {
            ILoggerFactory loggerFactory = Host.Services.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new FileLoggerProvider());

            OcrRecorderServiceLoggerFactory = loggerFactory;
        }

        public void BuildOcrRecorderServiceLoggerFactory()
        {
            ILoggerFactory loggerFactory = OcrRecorderServiceHost.Services.GetService<ILoggerFactory>();
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

        private async Task CheckIfLogsFolderExists()
        {
            StorageFolder logsFolder;

            try {
                logsFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync("logs");
                await CreateLogFile(logsFolder);
            } catch (FileNotFoundException) {
                await ApplicationData.Current.RoamingFolder.CreateFolderAsync("logs");
                
                logsFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync("logs");
                await CreateLogFile(logsFolder);
            }
        }

        private async Task<bool> CreateLogFile(StorageFolder folder)
        {
            try {
                await _asyncLock.WaitAsync();

                string todayDate = $"{DateTime.Today:yyyy-MM-dd}";

                string lastLogFileName = null;
                foreach (StorageFile logFile in await folder.GetFilesAsync()) lastLogFileName = logFile.Name;

                int fileIdx = 0;
                if (lastLogFileName != null) {
                    string[] logParts = lastLogFileName?.Split("_");
                    if (logParts[0].Equals(todayDate))
                        fileIdx = int.Parse(Path.GetFileNameWithoutExtension(logParts[1])) + 1;
                }

                string currentLogFileName = $"{todayDate}_{fileIdx}.log";
                CurrentSessionLogFileName = currentLogFileName;

                await folder.CreateFileAsync(currentLogFileName, CreationCollisionOption.OpenIfExists);
            } catch {
                return false;
            } finally {
                _asyncLock.Release();
            }

            return true;
        }

        private static void FocusAppWindow()
        {
            foreach (Process process in Process.GetProcesses()) {
                if (process.ProcessName == "OCRStudio" && process.MainWindowHandle != IntPtr.Zero) {
                    Win32Interop.Window.SetForegroundWindow(process.MainWindowHandle);

                    break;
                }
            }
        }
        
        private void CenterAppWindowOnScreen()
        {
            ScreenMonitor[] monitors = ScreenMonitor.All.ToArray();
            if (monitors.Length == 3)
            {
                ScreenMonitor targetMonitor = monitors[0];
                if (!targetMonitor.IsPrimary) _mainWindow.CenterOnScreen();
            }
        }
    }
}
