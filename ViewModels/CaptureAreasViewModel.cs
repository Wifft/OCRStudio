using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.UI.Dispatching;

using WifftOCR.DataModels;
using WifftOCR.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Windowing;

namespace WifftOCR.ViewModels
{
    public partial class CaptureAreasViewModel : ObservableObject, IDisposable
    {
        private readonly ISettingsService _settingsService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private bool _readingCaptureAreas;
        private bool _disposed;
        private CancellationTokenSource _tokenSource;

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

        private bool _activeFilter;

        public bool ActiveFilter 
        { 
            get => _activeFilter;
            set {
                SetProperty(ref _activeFilter, value);
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
            captureArea.PropertyChanged += CaptureArea_PropertyChanged;
            _captureAreas.Add(captureArea);

            SaveCaptureAreas();
        }

        public void Update(int index,  CaptureArea captureArea)
        {
            CaptureArea existingCaptureArea = CaptureAreas[index] as CaptureArea;
            existingCaptureArea.Name = captureArea.Name;
            existingCaptureArea.VectorA = captureArea.VectorA;
            existingCaptureArea.VectorB = captureArea.VectorB;
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

                Settings? settings = await _settingsService.ReadFromFileAsync();
                
                #nullable disable
                if (settings != null) {
                    await _dispatcherQueue.EnqueueAsync(() => {
                        _captureAreas = new ObservableCollection<CaptureArea>(settings.CaptureAreas);
                        foreach (CaptureArea captureArea in _captureAreas)
                            captureArea.PropertyChanged += CaptureArea_PropertyChanged;

                        CaptureAreas = new AdvancedCollectionView(_captureAreas, true);
                        OnPropertyChanged(nameof(CaptureAreas));

                        IsLoading = false;
                    });
                }
            });
            _readingCaptureAreas = false;

            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
        }

        [RelayCommand]
        public void ApplyFilters()
        {
            List<Expression<Func<object, bool>>> expressions = new(4);
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
            if (settings != null)
            {
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
