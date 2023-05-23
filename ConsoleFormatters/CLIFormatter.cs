// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

using OCRStudio.Extensions;

namespace OCRStudio.ConsoleFormatters
{
    internal sealed partial class CLIFormatter : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable _optionsReloadToken;
        private Options _formatterOptions;

        private bool ConsoleColorFormattingEnabled =>
            _formatterOptions.ColorBehavior == LoggerColorBehavior.Enabled ||
            _formatterOptions.ColorBehavior == LoggerColorBehavior.Default &&
            Console.IsOutputRedirected == false;

        public CLIFormatter(IOptionsMonitor<Options> options) : base("OCRStudioCLIFormatter") =>
            (_optionsReloadToken, _formatterOptions) = (options.OnChange(ReloadLoggerOptions), options.CurrentValue);

        private void ReloadLoggerOptions(Options options) => _formatterOptions = options;

        public override void Write<TState>(in LogEntry<TState> entry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
        {
            #nullable enable
            string? message = entry.Formatter?.Invoke(entry.State, entry.Exception);

            if (message is null) return;

            if (ConsoleColorFormattingEnabled) textWriter.WriteWithColor(_formatterOptions.CustomPrefix ?? string.Empty, ConsoleColor.Black, ConsoleColor.Red);
            else textWriter.Write(_formatterOptions.CustomPrefix);

            textWriter.WriteLine(message);
        }

        public void Dispose() => _optionsReloadToken?.Dispose();

        internal sealed class Options : SimpleConsoleFormatterOptions
        {
            public string? CustomPrefix { get; set; }
        }
    }
}
