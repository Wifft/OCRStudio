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
        [ObservableProperty]
        private string? _name;

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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
