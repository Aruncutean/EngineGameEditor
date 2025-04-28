using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;
using Core.models;
using EditorAvalonia.utils;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace EditorAvalonia.models
{
    public partial class AssetItemView : INotifyPropertyChanged
    {
        private readonly AssetItem _model;
        public AssetItemView(AssetItem model)
        {
            _model = model;
        }

        public string Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name != value)
                {
                    _model.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set
            {

                _isRenaming = value;
                OnPropertyChanged(nameof(IsRenaming));

            }
        }

        [RelayCommand]
        public void StartRename()
        {
            IsRenaming = true;

        }

        private bool CanOpenProject()
        {
            return true;
        }

        public Bitmap Icon => LoadIcon(Model.Type);

        private Bitmap LoadIcon(AssetType type)
        {
            var uri = type switch
            {
                AssetType.Folder => new Uri("avares://EditorAvalonia/assets/folder.png"),
                AssetType.Mesh => new Uri("avares://EditorAvalonia/assets/geometry.png"),
                AssetType.Material => new Uri("avares://EditorAvalonia/assets/material.png"),
                _ => new Uri("avares://EditorAvalonia/assets/block.png")
            };
            return new Bitmap(AssetLoader.Open(uri));
        }

        public AssetItem Model => _model;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

}
