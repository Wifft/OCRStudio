// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

using OCRStudio.DataModels;
using OCRStudio.Interfaces;
using OCRStudio.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using OCRStudio.Enums;

namespace OCRStudio.ViewModels
{
    internal partial class WelcomePageViewModel : ObservableObject, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly IFileLoggerService _fileLoggerService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private bool _readingLogFile;
        private bool _disposed;        

        [ObservableProperty]
        private bool _fileChanged;

        [ObservableProperty]
        private string _logLines;

        [ObservableProperty]
        private ExecutionMode _executionMode;

        public WelcomePageViewModel(ISettingsService settingsService, IFileLoggerService fileLoggerService)
        {
            _settingsService = settingsService;

            _fileLoggerService = fileLoggerService;
            _fileLoggerService.FileChanged += (s, e) => _dispatcherQueue.TryEnqueue(() => FileChanged = true);
        }

        public async Task TakeScreenshoot()
        {

        }

        public async Task StartOcrRecorderService()
        {
            App appInstance = App.GetInstance();

            try {
                appInstance.OcrRecorderServiceRunning = true;

                #nullable enable
                Settings? settings = await _settingsService.ReadFromFileAsync();
                if (settings != null && settings.CaptureAreas.Where(ca => ca.Active).ToList().Count == 0) {
                    #nullable disable
                    appInstance.LoggerFactory
                        .CreateLogger(typeof(OcrRecorderService).FullName)
                        .LogError("No capture areas found! You must create at least one.");

                    await Task.Delay(500);

                    appInstance.OcrRecorderServiceRunning = false;
                    
                    return;
                }

                await appInstance.OcrRecorderServiceHost.StartAsync();
            } catch (OperationCanceledException) {
                appInstance.OcrRecorderServiceHost = Host.CreateDefaultBuilder()
                    .UseContentRoot(AppContext.BaseDirectory)
                    .ConfigureLogging(App.ConfigureLoggingForOcrRecorderServiceHostCallback)
                    .ConfigureServices(App.ConfigureServicesForOcrRecorderServiceHostCallback)
                    .Build();

                appInstance.BuildOcrRecorderServiceLoggerFactory();

                await appInstance.OcrRecorderServiceHost.StartAsync();
            } catch (Exception) {
                appInstance.OcrRecorderServiceRunning = false;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Can't be an static member.")]
        public async Task StopOcrRecorderService()
        {
            App appInstance = App.GetInstance();

            await appInstance.OcrRecorderServiceHost.StopAsync();

            await Task.Delay(1000);

            appInstance.OcrRecorderServiceRunning = false;
        }

        public void ReadLogFile()
        {
            if (_readingLogFile) return;

            Task.Run(async () => {
                _readingLogFile = true;
                LogLines = await _fileLoggerService.ReadFromFileAsync();
                _readingLogFile = false;
            });
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) _disposed = true;
        }
    }
}
