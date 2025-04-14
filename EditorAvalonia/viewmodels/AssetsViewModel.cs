using Core.component;
using Core.entity;
using Core.IO;
using Core.models;
using EditorAvalonia.models;
using EditorAvalonia.service;
using EditorAvalonia.views.project;
using MsBox.Avalonia.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows.Input;

namespace EditorAvalonia.viewmodels
{
    public class AssetsViewModel : INotifyPropertyChanged
    {
        private AssetsService _assetsService = new();


        public ObservableCollection<AssetItem> Assets { get; } = new();

        private AssetItem? _selectedAsset;
        public AssetItem? SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                _selectedAsset = value;
                OnPropertyChanged(nameof(SelectedAsset));
                OnAssetSelected();
            }
        }

        private void OnAssetSelected()
        {
            var scene = StoreService.GetInstance().Scene;

            if(scene == null)
            {
                Debug.WriteLine("Scene is null");
                return;
            }

            var shaderComponent = new ShaderComponent();
            shaderComponent.shaderType = "basic";

            var transformComponent = new TransformComponent();
            transformComponent.Position = new Vector3(0, 0, 0);
            transformComponent.Scale = new Vector3(1, 1, 1);

            var meshComponent = new MeshComponent();
            meshComponent.MeshPath = SelectedAsset.Path;
           

            Entity entity = new Entity();

            entity.AddComponent(shaderComponent);
            entity.AddComponent(meshComponent);
            entity.AddComponent(transformComponent);

            scene.AddEntity(entity);

            SceneIO sceneIO = new SceneIO();

            var scenePath = Path.Combine(StoreService.GetInstance().ProjectInfo.Path, "scenes", StoreService.GetInstance().CurentScene.Path);
            sceneIO.SaveScene(scenePath,scene);

        }

        public AssetsViewModel()
        {
            List<AssetItem> assetItems = StoreService.GetInstance().AssetCollection.Assets;
            Assets.Clear();
            foreach (var assetItem in assetItems)
            {
                Assets.Add(assetItem);
            }


        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
