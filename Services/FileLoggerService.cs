// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;

using WifftOCR.Interfaces;

namespace WifftOCR.Services
{
    internal partial class FileLoggerService : IFileLoggerService, IDisposable
    {
        private readonly SemaphoreSlim _asyncLock = new(1, 1);
        private readonly IFileSystem _fileSystem;
        private readonly IFileSystemWatcher _fileSystemWatcher;
        private readonly string _settingsFilePath;

        private bool _disposed;

        public string SettingsFilePath => _settingsFilePath;

        public event EventHandler FileChanged;

        #nullable enable
        public FileLoggerService(IFileSystem? fileSystem = null)
        {
            _fileSystem = fileSystem;

            _settingsFilePath = StorageFile.GetFileFromApplicationUriAsync(new Uri(App.LOG_FILE_LOCATION_URI))
                .GetAwaiter()
                .GetResult()
                .Path;

            if (fileSystem != null) {
                #nullable disable
                _fileSystemWatcher = _fileSystem.FileSystemWatcher.New();
                _fileSystemWatcher.Path = _fileSystem.Path.GetDirectoryName(SettingsFilePath);
                _fileSystemWatcher.Filter = _fileSystem.Path.GetFileName(SettingsFilePath);
                _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                _fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        public async Task<bool> WriteToFileAsync(string logLine)
        {
            try {
                await _asyncLock.WaitAsync();

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(App.LOG_FILE_LOCATION_URI));

                using StreamWriter writer = new(await file.OpenStreamForWriteAsync());
                await writer.WriteLineAsync(logLine);
                writer.Close();
            } catch {
                return false;
            } finally {
                _asyncLock.Release();
            }

            return true;
        }

        public async Task<bool> ClearFileAsync()
        {
            try {
                await _asyncLock.WaitAsync();

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(App.LOG_FILE_LOCATION_URI));

                Stream stream = await file.OpenStreamForWriteAsync();
                stream.SetLength(0);

                using StreamWriter writer = new(stream);
                await writer.WriteAsync(string.Empty);
                
                writer.Close();
            } catch {
                return false;
            } finally {
                _asyncLock.Release();
            }

            return true;
        }

        #nullable enable
        public async Task<string> ReadFromFileAsync()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(App.LOG_FILE_LOCATION_URI));
            using StreamReader reader = new(await file.OpenStreamForReadAsync());

            string lines = reader.ReadToEnd();

            reader.Close();

            return lines;
        }

        public static async Task<bool> WriteLogFileForServiceProviderAsync(string logLine)
        {
            return await (new FileLoggerService(null)).WriteToFileAsync(logLine);
        }

        public static async Task<string> ReadLogFileForServiceProviderAsync()
        {
            return await (new FileLoggerService(null)).ReadFromFileAsync();
        }

        public static async Task<bool> ClearLogFileOnStartAsync()
        {
            return await (new FileLoggerService(null)).ClearFileAsync();
        }

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
