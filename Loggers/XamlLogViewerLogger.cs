// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;

namespace WifftOCR.Loggers
{
    class XamlLogViewerLogger : ILogger
    {
        private readonly StringBuilder _logBuilder;
        private readonly TextBlock _logTextBlock;

        public XamlLogViewerLogger(TextBlock logTextBlock)
        {
            _logBuilder = new StringBuilder();
            _logTextBlock = logTextBlock;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            string logEntry = $"{DateTime.Now} - [{logLevel}]: {message}";

            _logBuilder.AppendLine(logEntry);

            DispatcherQueue.GetForCurrentThread()?.TryEnqueue(() => {
                if (_logTextBlock is not null)
                    _logTextBlock.Text = _logBuilder.ToString();
            });
        }
    }
}
