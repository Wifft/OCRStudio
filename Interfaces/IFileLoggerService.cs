// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace OCRStudio.Interfaces
{
    public interface IFileLoggerService
    {
        string LogFilePath { get; }

        event EventHandler FileChanged;

        #nullable enable
        Task<string> ReadFromFileAsync(string? fileName = null);
        Task<bool> WriteToFileAsync(string logLine);
    }
}
