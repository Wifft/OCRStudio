// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;

using OCRStudio.Interfaces;

namespace OCRStudio.Services
{
    internal partial class FileLoggerService : IFileLoggerService, IDisposable
    {
        private readonly SemaphoreSlim _asyncLock = new(1, 1);
        private readonly IFileSystem _fileSystem;
        private readonly IFileSystemWatcher _fileSystemWatcher;
        private readonly string _logFilePath;

        private bool _disposed;

        public string LogFilePath => _logFilePath;

        public event EventHandler FileChanged;

        #nullable enable
        public FileLoggerService(IFileSystem? fileSystem = null)
        {
            _fileSystem = fileSystem;

            Uri logFileUri = new($"ms-appdata:///roaming/logs/{App.GetInstance().CurrentSessionLogFileName}");

            _logFilePath = StorageFile.GetFileFromApplicationUriAsync(logFileUri)
                .GetAwaiter()
                .GetResult()
                .Path;

            if (fileSystem != null) {
                #nullable disable
                _fileSystemWatcher = _fileSystem.FileSystemWatcher.New();
                _fileSystemWatcher.Path = _fileSystem.Path.GetDirectoryName(LogFilePath);
                _fileSystemWatcher.Filter = _fileSystem.Path.GetFileName(LogFilePath);
                _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                _fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        public async Task<bool> WriteToFileAsync(string logLine)
        {
            try {
                await _asyncLock.WaitAsync();

                Uri logFileUri = new($"ms-appdata:///roaming/logs/{App.GetInstance().CurrentSessionLogFileName}");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(logFileUri);

                using FileStream fileStream = new(file.Path, FileMode.Append, FileAccess.Write);

                using StreamWriter writer = new(fileStream);
                await writer.WriteLineAsync(logLine);
                writer.Close();
            } catch {
                return false;
            } finally {
                _asyncLock.Release();
            }

            return true;
        }

        #nullable enable
        public async Task<string> ReadFromFileAsync(string? fileName = null)
        {
            try {
                string lines = string.Empty;

                await _asyncLock.WaitAsync();

                Uri logFileUri = new($"ms-appdata:///roaming/logs/{App.GetInstance().CurrentSessionLogFileName}");
                if (fileName != null ) logFileUri = new Uri($"ms-appdata:///roaming/logs/{fileName}");

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(logFileUri);
                using (StreamReader reader = new(await file.OpenStreamForReadAsync())) {
                    lines = reader.ReadToEnd();

                    reader.Close();
                }

                return lines;
            } catch {
                return "";
            } finally {
                _asyncLock.Release();
            }
        }

        public static async Task<bool> WriteLogFileForServiceProviderAsync(string logLine) => await (new FileLoggerService(null)).WriteToFileAsync(logLine);
        public static async Task<string> ReadLogFileForServiceProviderAsync() => await (new FileLoggerService(null)).ReadFromFileAsync();

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            FileChanged?.Invoke(this, EventArgs.Empty);
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) {
                _asyncLock.Dispose();
                _disposed = true;
            }
        }
    }
}
