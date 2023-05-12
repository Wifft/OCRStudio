using System;
using System.Threading.Tasks;

using WifftOCR.DataModels;

namespace WifftOCR.Interfaces
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
