// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

using OCRStudio.DataModels;
using OCRStudio.ViewModels;

using CommunityToolkit.Mvvm.Input;

namespace OCRStudio.Views
{
    public sealed partial class CaptureAreasPage : Page
    {
        public CaptureAreasViewModel ViewModel { get; private set; }

        public ICommand AddAreaCommand => new AsyncRelayCommand(AddAreaDialogAsync);
        public ICommand AddCommand => new AsyncRelayCommand(Add);
        public ICommand UpdateCommand => new RelayCommand(Update);
        public ICommand DeleteCommand => new AsyncRelayCommand(Delete);

        public CaptureAreasPage()
        {
            InitializeComponent();

            ViewModel = App.GetService<CaptureAreasViewModel>();
            DataContext = ViewModel;
        }

        private async Task AddAreaDialogAsync()
        {
            CaptureAreaDialog.Title = "Add new capture area";
            CaptureAreaDialog.PrimaryButtonText = "Add";
            CaptureAreaDialog.PrimaryButtonCommand = AddCommand;
            CaptureAreaDialog.DataContext = new CaptureArea(string.Empty, new Point(), new Size(), false);

            await CaptureAreaDialog.ShowAsync();
        }

        private async Task Add() => await ViewModel.Add(CaptureAreaDialog.DataContext as CaptureArea);
        private void Update() => ViewModel.Update(CaptureAreasList.SelectedIndex, CaptureAreaDialog.DataContext as CaptureArea);
        private async Task Delete() => await ViewModel.DeleteSelected();

        private async void CaptureAreas_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.Selected = e.ClickedItem as CaptureArea;
            
            CaptureAreaDialog.Title = $"Editing \"{ViewModel.Selected.Name}\"";
            CaptureAreaDialog.PrimaryButtonText = "Update";
            CaptureAreaDialog.PrimaryButtonCommand = UpdateCommand;

            CaptureArea clone = ViewModel.Selected.Clone();
            CaptureAreaDialog.DataContext = clone;

            await CaptureAreaDialog.ShowAsync();
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement owner = sender as FrameworkElement;
            if (owner != null) {
                FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(owner);
                FlyoutShowOptions options = new() {
                    Position = e.GetPosition(owner),
                    ShowMode = FlyoutShowMode.Transient
                };
                
                flyoutBase.ShowAt(owner, options);
            }
        }

        private void ReorderButtonUp_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null) {
                CaptureArea captureArea = menuFlyoutItem.DataContext as CaptureArea;
                
                int index = ViewModel.CaptureAreas.IndexOf(captureArea);
                if (index > 0) ViewModel.Move(index, index - 1);
            }
        }

        private void ReorderButtonDown_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null) {
                CaptureArea captureArea = menuFlyoutItem.DataContext as CaptureArea;

                int index = ViewModel.CaptureAreas.IndexOf(captureArea);
                if (index < ViewModel.CaptureAreas.Count - 1) 
                    ViewModel.Move(index, index + 1);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem != null) { 
                CaptureArea selectedCaptureArea = menuFlyoutItem.DataContext as CaptureArea;
                ViewModel.Selected = selectedCaptureArea;
                DeleteDialog.Title = $"Deleting \"{selectedCaptureArea.Name}\"";

                await DeleteDialog.ShowAsync();
            }
        }
    }
}
