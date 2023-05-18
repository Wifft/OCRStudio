// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

using WifftOCR.Interfaces;
using WifftOCR.DataModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WifftOCR.ViewModels
{
    public partial class WelcomePageViewModel : ObservableObject, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly IFileLoggerService _fileLoggerService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private bool _readingLogFile;
        private bool _disposed;

        [ObservableProperty]
        private bool _ocrRecordingServiceRunning = false;

        [ObservableProperty]
        private bool _fileChanged;

        [ObservableProperty]
        private string _logLines;

        public WelcomePageViewModel(ISettingsService settingsService, IFileLoggerService fileLoggerService)
        {
            _settingsService = settingsService;
            _fileLoggerService = fileLoggerService;
            _fileLoggerService.FileChanged += (s, e) => _dispatcherQueue.TryEnqueue(() => FileChanged = true);
        }

        public async Task StartOcrRecorderService()
        {
            OcrRecordingServiceRunning = true;

            #nullable enable
            Settings? settings = await _settingsService.ReadFromFileAsync();
            if (settings != null && settings.CaptureAreas.Count == 0) {
                App.GetInstance()
                    .LoggerFactory
                    .CreateLogger("WifftOCR.Services.OcrRecorderService")
                    .LogError("No capture areas found! You must create at least one.");
                
                OcrRecordingServiceRunning = false;

                return;
            }

            await App.GetInstance().Host.StartAsync();
        }

        public async Task StopOcrRecorderService()
        {
            await App.GetInstance().Host.StopAsync();

            OcrRecordingServiceRunning = false;

            await Task.Delay(3000);

            await _fileLoggerService.ClearFileAsync();
        }

        public void ReadLogFile()
        {
            if (_readingLogFile) return;

            _ = _dispatcherQueue.TryEnqueue(() => {
                FileChanged = true;
            });

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
