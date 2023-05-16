// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;

using WifftOCR.Loggers;

namespace WifftOCR.Providers
{
    class XamlLogViewerProvider : ILoggerProvider
    {
        #nullable enable
        private readonly IDisposable? _onChangeToken = null;
        private readonly ConcurrentDictionary<string, XamlLogViewerLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
        
        private readonly TextBlock _logTextBlock;

        public XamlLogViewerProvider(TextBlock logTextBlock)
        {
            _logTextBlock = logTextBlock;
        }

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new XamlLogViewerLogger(_logTextBlock));

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
