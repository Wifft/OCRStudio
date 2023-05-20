// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Text.Json.Serialization;

using OCRStudio.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;

namespace OCRStudio.DataModels
{
    public partial class CaptureArea : ObservableObject
    {
        [ObservableProperty]
        private string _id;

        #nullable enable
        private string? _name;

        public string? Name
        { 
            get => _name;
            set {
                SetProperty(ref _name, value);
                OnPropertyChanged(nameof(Valid));
            }
        }

        [ObservableProperty]
        private Point _location;

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int LocationX
        {
            get => _location.X;
            set {
                if (_location.X != value) {
                    _location.X = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int LocationY
        {
            get => _location.Y;
            set {
                if (_location.Y != value) {
                    _location.Y = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        [ObservableProperty]
        private Size _size;

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int SizeW
        {
            get => _size.Width;
            set {
                if (_size.Width != value) {
                    _size.Width = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int SizeH
        {
            get => _size.Height;
            set {
                if (_size.Height != value) {
                    _size.Height = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        [ObservableProperty]
        private bool _active;

        [JsonIgnore]
        public bool Valid => ValidationHelper.ValidateCaptureArea(this);

        public CaptureArea() 
        { 
        }

        public CaptureArea(string? name, Point location, Size size, bool active)
        {
            Id = Guid.NewGuid().ToString();
            Name = name?.Trim();
            Location = location;
            Size = size;
            Active = active;
        }

        public CaptureArea Clone()
        {
            return new CaptureArea {
                Id = Id,
                Name = Name,
                Location = Location,
                Size = Size,
                Active = Active
            };
        }
    }
}
