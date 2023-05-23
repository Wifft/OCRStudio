// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

using OCRStudio.Interfaces;
using OCRStudio.DataModels;

namespace OCRStudio.ViewModels
{
    internal partial class SettingsViewModel
    {
        public readonly ISettingsService SettingsService;

        #nullable enable
        public Settings? SettingsModel;
        
        #nullable disable
        public SettingsViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;

            Task.Run(async () => SettingsModel = await SettingsService.ReadFromFileAsync());
        }
    }
}
