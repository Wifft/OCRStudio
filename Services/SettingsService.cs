// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;

using WifftOCR.Interfaces;
using WifftOCR.DataModels;

namespace WifftOCR.Services
{
    internal partial class SettingsService : ISettingsService, IDisposable
    {
        private readonly SemaphoreSlim _asyncLock = new(1, 1);
        private readonly IFileSystem _fileSystem;
        private readonly IFileSystemWatcher _fileSystemWatcher;
        private readonly string _settingsFilePath;

        private bool _disposed;

        public string SettingsFilePath => _settingsFilePath;

        public event EventHandler FileChanged;

        #nullable enable
        public SettingsService(IFileSystem? fileSystem = null)
        {
            _fileSystem = fileSystem;

            _settingsFilePath = StorageFile.GetFileFromApplicationUriAsync(new Uri(App.SETTINGS_LOCATION_URI))
                .GetAwaiter()
                .GetResult()
                .Path;

            if (_fileSystem != null) {
                #nullable disable
                _fileSystemWatcher = _fileSystem.FileSystemWatcher.New();
                _fileSystemWatcher.Path = _fileSystem.Path.GetDirectoryName(SettingsFilePath);
                _fileSystemWatcher.Filter = _fileSystem.Path.GetFileName(SettingsFilePath);
                _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                _fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        public async Task<bool> WriteToFileAsync(Settings settings)
        {
            try {
                await _asyncLock.WaitAsync();
                _fileSystemWatcher.EnableRaisingEvents = false;

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(App.SETTINGS_LOCATION_URI));

                using Stream stream = await file.OpenStreamForWriteAsync();
                stream.SetLength(0);

                JsonSerializerOptions options = new() {
                    WriteIndented = true
                };

                await JsonSerializer.SerializeAsync(stream, settings, options);
            } catch {
                return false;
            } finally {
                _fileSystemWatcher.EnableRaisingEvents = true;
                _asyncLock.Release();
            }
            
            return true;
        }

        #nullable enable
        public async Task<Settings?> ReadFromFileAsync()
        {
            try {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(App.SETTINGS_LOCATION_URI));
                using StreamReader reader = new(await file.OpenStreamForReadAsync());

                return JsonSerializer.Deserialize<Settings>(await reader.ReadToEndAsync());
            } catch (IOException e) {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public static async Task<Settings?> ReadSettingsForOcrServiceAsync()
        {
            return await (new SettingsService(null)).ReadFromFileAsync();
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
