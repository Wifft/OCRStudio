using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.UI.Dispatching;

using Windows.Storage;

using OCRStudio.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;

namespace OCRStudio.ViewModels
{
    internal partial class LogsHistoryViewModel : ObservableObject, IDisposable
    {
        public readonly IFileLoggerService _fileLoggerService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private bool _disposed;

        [ObservableProperty]
        private ObservableCollection<string> _logFiles;

        public LogsHistoryViewModel(IFileLoggerService fileLoggerService)
        {
            _fileLoggerService = fileLoggerService;
        }

        public async Task<string> Read(string fileName)
        {
            return await _fileLoggerService.ReadFromFileAsync(fileName);
        }

        public async Task DeleteLogs()
        {
            StorageFolder logsFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync("logs");
            foreach (StorageFile file in (await logsFolder.GetFilesAsync()).Where(f => f.Name != App.GetInstance().CurrentSessionLogFileName)) 
                await file.DeleteAsync();

            GetLogFiles();
        }

        [RelayCommand]
        public void GetLogFiles()
        {
            Task.Run(async () => {
                StorageFolder logsFolder = await ApplicationData.Current.RoamingFolder.GetFolderAsync("logs");
                await _dispatcherQueue.EnqueueAsync(async () => {
                    LogFiles = new();
                    foreach (StorageFile file in await logsFolder.GetFilesAsync())
                        LogFiles.Add(file.Name);

                    LogFiles = new(LogFiles.Reverse().ToList());
                });
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
