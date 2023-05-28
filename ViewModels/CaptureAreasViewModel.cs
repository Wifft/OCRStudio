// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.UI.Dispatching;

using OCRStudio.DataModels;
using OCRStudio.Extensions;
using OCRStudio.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using OCRStudio.Helpers.OCROverlay;

namespace OCRStudio.ViewModels
{
    internal partial class CaptureAreasViewModel : ObservableObject, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private bool _readingCaptureAreas;
        private bool _disposed;

        [ObservableProperty]
        private CaptureArea _selected;

        [ObservableProperty]
        private bool _error;

        [ObservableProperty]
        private bool _fileChanged;

        [ObservableProperty]
        private string _nameFilter;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _filtered;

        private bool _activesFilter;

        public bool ActivesFilter 
        { 
            get => _activesFilter;
            set {
                SetProperty(ref _activesFilter, value);
                ApplyFilters();
            }
        }

        private ObservableCollection<CaptureArea> _captureAreas;

        public AdvancedCollectionView CaptureAreas { get; set; }

        public CaptureAreasViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _settingsService.FileChanged += (s, e) => _dispatcherQueue.TryEnqueue(() => FileChanged = true);
        }

        public async Task Add(CaptureArea captureArea) {
            if (!captureArea.Valid) return;

            captureArea.PropertyChanged += CaptureArea_PropertyChanged;
            _captureAreas.Add(captureArea);

            await SaveCaptureAreas();
        }

        public void Update(int index,  CaptureArea captureArea)
        {
            CaptureArea existingCaptureArea = CaptureAreas[index] as CaptureArea;
            existingCaptureArea.Name = captureArea.Name;
            existingCaptureArea.Location = captureArea.Location;
            existingCaptureArea.Size = captureArea.Size;
            existingCaptureArea.Active = captureArea.Active;
        }

        public async Task DeleteSelected()
        {
            _captureAreas.Remove(Selected);

            await SaveCaptureAreas();
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (Filtered) return;

            CaptureArea area1 = _captureAreas[oldIndex];
            CaptureArea area2 = _captureAreas[newIndex];

            (area2.Id, area1.Id) = (area1.Id, area2.Id);

            _captureAreas.Move(oldIndex, newIndex);
        }

        [RelayCommand]
        public void ReadCaptureAreas()
        {
            if (_readingCaptureAreas) return;

            _ = _dispatcherQueue.TryEnqueue(() => {
                FileChanged = false;
                IsLoading = true;
            });

            Task.Run(async () => {
                _readingCaptureAreas = true;
                #nullable enable
                Settings? settings = await _settingsService.ReadFromFileAsync();
                _readingCaptureAreas = false;
                
                if (settings != null) {
                    await _dispatcherQueue.EnqueueAsync(() => {
                        _captureAreas = settings.CaptureAreas;
                        foreach (CaptureArea captureArea in _captureAreas)
                            #nullable disable
                            captureArea.PropertyChanged += CaptureArea_PropertyChanged;

                        CaptureAreas = new(_captureAreas, true);
                        OnPropertyChanged(nameof(CaptureAreas));

                        IsLoading = false;
                    });
                }
            });
        }

        [RelayCommand]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Nothing to justify.")]
        public void ApplyFilters()
        {
            List<Expression<Func<object, bool>>> expressions = new(2);

            if (!string.IsNullOrWhiteSpace(_nameFilter))
                expressions.Add(ca => ((CaptureArea) ca).Name.ToLower().Contains(_nameFilter.ToLower()));

            if (_activesFilter) expressions.Add(ca => ((CaptureArea) ca).Active);

            Expression<Func<object, bool>> filterExpression = null;
            foreach (Expression<Func<object, bool>> e in expressions)
                filterExpression = filterExpression == null ? e : filterExpression.And(e);

            Filtered = filterExpression != null;
            CaptureAreas.Filter = Filtered ? filterExpression.Compile().Invoke : null;
            CaptureAreas.RefreshFilter();
        }

        [RelayCommand]
        public void ClearFilters()
        {
            NameFilter = null;
            ActivesFilter = false;
        }

        [RelayCommand]
        public void LaunchOcrOverlay()
        {
            if (!WindowHelper.IsOCROverlayCreated()) WindowHelper.LaunchOCROverlayOnEveryScreen();
        }

        private void CaptureArea_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _ = Task.Run(async () => {
                #nullable enable
                Settings? settings = await _settingsService.ReadFromFileAsync();
                if (settings != null) {
                    settings.CaptureAreas = _captureAreas;

                    bool error = !await _settingsService.WriteToFileAsync(settings);

                    _ = await _dispatcherQueue.EnqueueAsync(() => Error = error);
                }
            });
        }

        private async Task SaveCaptureAreas()
        {
            Settings? settings = await _settingsService.ReadFromFileAsync();
            if (settings != null) {
                settings.CaptureAreas = _captureAreas;

                await _settingsService.WriteToFileAsync(settings);
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) _disposed = true;
        }
    }
}
