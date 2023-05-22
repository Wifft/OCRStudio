using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using OCRStudio.ViewModels;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OCRStudio.Views
{
    public sealed partial class LogsHistoryPage : Page
    {
        public LogsHistoryViewModel ViewModel { get; private set; }

        public ICommand DeleteLogsCommand => new AsyncRelayCommand(DeleteLogs);

        public LogsHistoryPage()
        {
            InitializeComponent();

            ViewModel = App.GetService<LogsHistoryViewModel>();
            DataContext = ViewModel;
        }

        public async Task DeleteLogs() => await ViewModel.DeleteLogs();

        private async void LogsList_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox.SelectedItem != null) {
                LogViewer.Text = await ViewModel.Read(comboBox.SelectedItem.ToString());
                LogViewer.Text = LogViewer.Text.Length > 0 ? LogViewer.Text : "Selected log is empty.";
            } else comboBox.SelectedIndex = 0;
        }

        private async void LogsList_Loaded(object sender, RoutedEventArgs args)
        {
            ComboBox comboBox = sender as ComboBox;
            while (comboBox.Items.Count == 0) await Task.Delay(250);

            comboBox.SelectedIndex = 0;
        }
    }
}
