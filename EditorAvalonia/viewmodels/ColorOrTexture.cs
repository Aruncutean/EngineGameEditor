using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorAvalonia.viewmodels
{
    public class ColorOrTexture : INotifyPropertyChanged
    {
        private string? _colorHex = "#FF0000";

        public string? ColorHex
        {
            get => _colorHex;
            set
            {
                _colorHex = value;
                OnPropertyChanged(nameof(ColorHex));
                OnPropertyChanged(nameof(Color));
            }
        }

        public Color Color
        {
            get => Color.Parse(ColorHex ?? "#FFFFFF");
            set
            {
                ColorHex = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
                OnPropertyChanged(nameof(ColorHex));
                OnPropertyChanged(nameof(Color));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
