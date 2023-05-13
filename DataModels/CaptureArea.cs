// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Text.Json.Serialization;

using WifftOCR.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;

namespace WifftOCR.DataModels
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
        private Point _vectorA;

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int VectorAX
        {
            get => _vectorA.X;
            set {
                if (_vectorA.X != value) {
                    _vectorA.X = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int VectorAY
        {
            get => _vectorA.Y;
            set {
                if (_vectorA.Y != value) {
                    _vectorA.Y = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        [ObservableProperty]
        private Point _vectorB;

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int VectorBX
        {
            get => _vectorB.X;
            set {
                if (_vectorB.X != value) {
                    _vectorB.X = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "MVVMTK0034:Direct field reference to [ObservableProperty] backing field", Justification = "Because use is necessary here.")]
        public int VectorBY
        {
            get => _vectorB.Y;
            set {
                if (_vectorB.Y != value) {
                    _vectorB.Y = value;
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

        public CaptureArea(string? name, Point vectorA, Point vectorB, bool active)
        {
            Id = Guid.NewGuid().ToString();
            Name = name?.Trim();
            VectorA = vectorA;
            VectorB = vectorB;
            Active = active;
        }

        public CaptureArea Clone()
        {
            return new CaptureArea {
                Id = Id,
                Name = Name,
                VectorA = VectorA,
                VectorB = VectorB,
                Active = Active
            };
        }
    }
}
