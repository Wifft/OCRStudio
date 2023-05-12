using System.Threading.Tasks;

using WifftOCR.Interfaces;
using WifftOCR.DataModels;

namespace WifftOCR.ViewModels
{
    internal class SettingsViewModel
    {
        public readonly ISettingsService SettingsService;

        #nullable enable
        public Settings? SettingsModel;
        
        #nullable disable
        public SettingsViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;

            Task.Run(async () => {
                SettingsModel = await SettingsService.ReadFromFileAsync();
            });
        }
    }
}
