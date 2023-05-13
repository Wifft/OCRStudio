using System.Threading.Tasks;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using WifftOCR.DataModels;
using WifftOCR.ViewModels;

namespace WifftOCR.Views
{
    public sealed partial class SettingsPage : Page
    {
        internal SettingsViewModel ViewModel { get; private set;  }

        public SettingsPage()
        {
            InitializeComponent();

            ViewModel = App.GetService<SettingsViewModel>();
            DataContext = ViewModel;
            
            Loaded += SettingsPage_Loaded; 
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(async () => {
                while (ViewModel.SettingsModel == null) await Task.Delay(100);

                SetObserverValue(ViewModel.SettingsModel.Observer);
                SetServerEndpointValue(ViewModel.SettingsModel.ServerEndpoint);
                SetServerKeyValue(ViewModel.SettingsModel.ServerKey);
            });
        }

        #nullable enable
        private void SetObserverValue(uint? value)
        {
            #nullable disable
            Observer.Value = (double) value;
        }

        #nullable enable
        private void SetServerEndpointValue(string? value)
        {
            ServerEndpoint.Text = value;
        }

        private void SetServerKeyValue(string? value)
        {
            ServerKey.Text = value;
        }

        private async void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            Settings? settings = ViewModel.SettingsModel;
            if (settings != null) {
                settings.Observer = (uint)Observer.Value;
                settings.ServerEndpoint = ServerEndpoint.Text;
                settings.ServerKey = ServerKey.Text;

                await ViewModel.SettingsService.WriteToFileAsync(settings);
            }
        }
    }
}
