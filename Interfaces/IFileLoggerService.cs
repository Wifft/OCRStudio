using System;
using System.Threading.Tasks;

namespace WifftOCR.Interfaces
{
    public interface IFileLoggerService
    {
        string SettingsFilePath { get; }

        event EventHandler FileChanged;

        #nullable enable
        Task<string> ReadFromFileAsync();
        Task<bool> WriteToFileAsync(string logLine);
        Task<bool> ClearFileAsync();
    }
}
