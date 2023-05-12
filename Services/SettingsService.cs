using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;

using WifftOCR.Interfaces;
using WifftOCR.DataModels;

namespace WifftOCR.Services
{
    internal class SettingsService : ISettingsService, IDisposable
    {
        private readonly SemaphoreSlim _asyncLock = new(1, 1);
        private readonly string _settingsFilePath;

        private bool _disposed;

        public string SettingsFilePath => _settingsFilePath;

        public event EventHandler FileChanged;

        public SettingsService()
        {
            _settingsFilePath = App.SETTINGS_LOCATION_URI;
        }

        public async Task<bool> WriteToFileAsync(Settings settings)
        {
            try {
                await _asyncLock.WaitAsync();

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
                _asyncLock.Release();
            }
            
            return true;
        }

        #nullable enable
        public async Task<Settings?> ReadFromFileAsync()
        {
            try {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(App.SETTINGS_LOCATION_URI));
                StreamReader reader = new(await file.OpenStreamForReadAsync());

                return JsonSerializer.Deserialize<Settings>(await reader.ReadToEndAsync());
            } catch (IOException e) {
                Console.WriteLine(e.Message);

                return null;
            }
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
