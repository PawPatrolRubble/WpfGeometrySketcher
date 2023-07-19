#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace Lan.ImageViewer
{
    public class GeometryType : INotifyPropertyChanged
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public GeometryType(string name, string description, Geometry? iconGeometry)
        {
            Name = name;
            Description = description;
            IconGeometry = iconGeometry;
        }

        public string Name { get; }
        public string Description { get; }
        public Geometry? IconGeometry { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set { SetProperty(ref _isSelected, value); }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
