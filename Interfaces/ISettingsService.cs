﻿// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

using OCRStudio.DataModels;

namespace OCRStudio.Interfaces
{
    public interface ISettingsService : IDisposable
    {
        string SettingsFilePath { get; }

        event EventHandler FileChanged;

        #nullable enable
        Task<Settings?> ReadFromFileAsync();
        Task<bool> WriteToFileAsync(Settings settings);
    }
}
