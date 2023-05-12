using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace WifftOCR.DataModels
{
    public class Settings : ObservableObject
    {
        #nullable enable
        public uint? Observer{ get; set; }
        public string? ServerEndpoint{ get; set; }
        public string? ServerKey { get; set; }
        
        #nullable disable
        public ObservableCollection<CaptureArea> CaptureAreas { get; set; }
    }
}
