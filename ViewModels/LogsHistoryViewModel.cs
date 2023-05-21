using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Microsoft.UI.Dispatching;

using Windows.Storage;

using OCRStudio.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI;
using System.Linq.Expressions;
using System.Linq;

namespace OCRStudio.ViewModels
{
    public partial class LogsHistoryViewModel : ObservableObject, IDisposable
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
