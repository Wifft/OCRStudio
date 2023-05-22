// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Windows.Storage;

using OCRStudio.Loggers;


namespace OCRStudio.Providers
{
    class FileLoggerProvider : ILoggerProvider
    {
        #nullable enable
        private readonly IDisposable? _onChangeToken = null;
        private readonly ConcurrentDictionary<string, FileLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

        public ILogger CreateLogger(string categoryName) {
            FileLogger fileLogger = _loggers.GetOrAdd(categoryName, name => new FileLogger(name));

            Task.Run(RestoreLogFileState);

            return fileLogger;
        }

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }

        private static async Task RestoreLogFileState()
        {
            Uri logFileUri = new($"ms-appdata:///roaming/logs/{App.GetInstance().CurrentSessionLogFileName}");
            StorageFile logFile = await StorageFile.GetFileFromApplicationUriAsync(logFileUri);

            string fileContents = "";
            using (StreamReader reader = new(await logFile.OpenStreamForReadAsync())) {
                fileContents = await reader.ReadToEndAsync();
                reader.Close();
            }

            using StreamWriter writer = new(await logFile.OpenStreamForWriteAsync());
            writer.Write(fileContents);
            writer.Close();
        }
    }
}
