using System;
using System.Linq;

using Microsoft.UI.Xaml.Controls;

using Windows.System;

using WifftOCR.Helpers;
using WifftOCR.Services;
using WifftOCR.ViewModels;

namespace WifftOCR.Views
{
    public sealed partial class ShellPage : UserControl
    {
        public static ShellPage ShellHandler { get; set; }

        internal ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();

            DataContext = ViewModel;
            ShellHandler = this;
            ViewModel.Initialize(shellFrame, navigationView);

            foreach (NavigationViewItem item in navigationView.MenuItems.Cast<NavigationViewItem>()) 
                SetNavigateToProperty(item);
        }

        public static void Navigate(Type type)
        {
            NavigationService.Navigate(type);
        }

        public void Refresh()
        {
            shellFrame.Navigate(typeof(WelcomePage));
        }

        internal static void EnsurePageIsSelected()
        {
            NavigationService.EnsurePageIsSelected(typeof(WelcomePage));
        }

        private async void FeedbackItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://twitter.com/wifft_"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Really can't be an static member.")]
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem selectedItem = args.SelectedItem as NavigationViewItem;
            if (selectedItem != null) {
                Type pageType = selectedItem.GetValue(NavHelper.NavigateToProperty) as Type;
                NavigationService.Navigate(pageType);
            }
        }

        private static void SetNavigateToProperty(NavigationViewItem item)
        {
            switch (item.Name) {
                case "Shell_Welcome":
                    NavHelper.SetNavigateTo(item, typeof(WelcomePage));

                    break;
                case "Shell_CaptureAreas":
                    NavHelper.SetNavigateTo(item, typeof(CaptureAreasPage));
               
                    break;
            }
        }
    }
}
