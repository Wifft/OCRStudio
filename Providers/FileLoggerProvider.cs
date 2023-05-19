// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

using WifftOCR.Loggers;

namespace WifftOCR.Providers
{
    class FileLoggerProvider : ILoggerProvider
    {
        #nullable enable
        private readonly IDisposable? _onChangeToken = null;
        private readonly ConcurrentDictionary<string, FileLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
        
        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new FileLogger(name));

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
