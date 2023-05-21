using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using OCRStudio.ViewModels;
using System.Threading.Tasks;

namespace OCRStudio.Views
{
    public sealed partial class LogsHistoryPage : Page
    {
        public LogsHistoryViewModel ViewModel { get; private set; }

        public LogsHistoryPage()
        {
            InitializeComponent();

            ViewModel = App.GetService<LogsHistoryViewModel>();
            DataContext = ViewModel;
        }

        private async void LogsList_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            ComboBox comboBox = sender as ComboBox;

            LogViewer.Text = await ViewModel.Read(comboBox.SelectedItem.ToString());
            LogViewer.Text = LogViewer.Text.Length > 0 ? LogViewer.Text : "Selected log is empty.";
        }

        private async void LogsList_Loaded(object sender, RoutedEventArgs args)
        {
            ComboBox comboBox = sender as ComboBox;
            while (comboBox.Items.Count == 0) await Task.Delay(250);

            comboBox.SelectedIndex = 0;
        }
    }
}
