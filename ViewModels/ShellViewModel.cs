// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using WifftOCR.Helpers;
using WifftOCR.Services;
using WifftOCR.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WifftOCR.ViewModels
{
    internal partial class ShellViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBackEnabled;

        [ObservableProperty]
        private NavigationViewItem _selected;

        private NavigationView navigationView;

        private ICommand loadedCommand;
        private ICommand itemInvokedCommand;

        public ICommand LoadedCommand => loadedCommand ??= new RelayCommand(OnLoaded);
        public ICommand ItemInvokedCommand => itemInvokedCommand ??= new RelayCommand<NavigationViewItemInvokedEventArgs>(OnItemInvoked);

        public void Initialize(Frame frame, NavigationView navigationView)
        {
            this.navigationView = navigationView;

            NavigationService.Frame = frame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            
            this.navigationView.BackRequested += OnBackRequested;
        }

        private async void OnLoaded()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private void OnItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            NavigationViewItem settingsItem = navigationView.SettingsItem as NavigationViewItem;
            if (args.InvokedItem.Equals("Settings")) {
                Selected = settingsItem;

                NavigationService.Navigate(typeof(SettingsPage));

                foreach (NavigationViewItem selectedItem in navigationView.MenuItems.OfType<NavigationViewItem>().Where(menuItem => menuItem.IsSelected == true))
                    selectedItem.IsSelected = false;

                settingsItem.IsSelected = true;

                return;
            }

            NavigationViewItem item = navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .First(menuItem => (string) menuItem.Content == (string) args.InvokedItem);
            Type pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
            NavigationService.Navigate(pageType);

            Selected = item;
            if (settingsItem.IsSelected) settingsItem.IsSelected = false;
        }

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            Selected = navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private static bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            return menuItem.GetValue(NavHelper.NavigateToProperty) as Type == sourcePageType;
        }
    }
}
