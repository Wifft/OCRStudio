// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using Windows.System;

using OCRStudio.Helpers;
using OCRStudio.Services;
using OCRStudio.ViewModels;


namespace OCRStudio.Views
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
            ViewModel.Initialize(ShellFrame, navigationView);

            foreach (NavigationViewItem item in navigationView.MenuItems.Cast<NavigationViewItem>()) 
                SetNavigateToProperty(item);

            if (Environment.OSVersion.Version.Build < 22000)
                ShellFrame.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 36, 41, 42));
        }

        public static void Navigate(Type type)
        {
            NavigationService.Navigate(type);
        }

        public void Refresh()
        {
            ShellFrame.Navigate(typeof(WelcomePage));
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
