using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using Core.component;
using Core.entity;
using Core.graphics.shader;
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
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Assimp;


namespace EditorAvalonia.viewmodels
{
    public partial class AssetsViewModel : INotifyPropertyChanged
    {
        private string _currentPath;
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
            }
        }

        [RelayCommand]
        public void NavigateTo(BreadcrumbItem breadcrumbItem)
        {
            if (breadcrumbItem != null)
            {
                CurrentPath = breadcrumbItem.Name;

                var NewBreadcrumbParts = new List<BreadcrumbItem>();
                NewBreadcrumbParts.Add(new BreadcrumbItem
                {
                    Index = 0,
                    Name = "assets",
                });

                CurrentFolder = "assets";

                for (int i = 1; i <= breadcrumbItem.Index; i++)
                {

                    CurrentFolder = Path.Combine(CurrentFolder, BreadcrumbParts[i].Name);
                    NewBreadcrumbParts.Add(new BreadcrumbItem
                    {
                        Index = i,
                        Name = BreadcrumbParts[i].Name,
                    });
                }

                BreadcrumbParts.Clear();
                foreach (var item in NewBreadcrumbParts)
                {
                    BreadcrumbParts.Add(item);
                }

                var storeService = StoreService.GetInstance();
                if (storeService.AssetCollection?.Assets != null)
                {
                    var filtered = GetItemsInFolder(storeService.AssetCollection.Assets);
                    Assets.Clear();
                    foreach (var item in filtered)
                        Assets.Add(new AssetItemView(item));
                }
            }

        }


        public ObservableCollection<BreadcrumbItem> BreadcrumbParts { get; set; } = new();

        private string CurrentFolder = "assets";

        public ObservableCollection<AssetItemView> Assets { get; set; } = new ObservableCollection<AssetItemView>();

        private AssetItemView? _selectedAsset;
        public AssetItemView? SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                _selectedAsset = value;
                OnPropertyChanged(nameof(SelectedAsset));
                //    OnAssetSelected();
            }
        }

        private string GetUniqueMaterialPath(string baseDirectory, string baseName)
        {

            string name = baseName;
            string path = Path.Combine(baseDirectory, $"{name}.material.json");

            int counter = 1;
            while (File.Exists(Path.Combine(StoreService.GetInstance().ProjectInfo.Path, path)))
            {
                name = $"{baseName} ({counter})";
                path = Path.Combine(baseDirectory, $"{name}.material.json");
                counter++;
            }

            return name;
        }

        public void AddNewMaterial()
        {
            string name = GetUniqueMaterialPath(CurrentFolder, "NewMaterial");
            var assetItem = new AssetItem
            {
                Name = name,
                Type = AssetType.Material,
                Path = Path.Combine(CurrentFolder, $"{name}.material.json"),
                BaseDirector = CurrentFolder
            };

            var assetCollection = StoreService.GetInstance().AssetCollection;
            if (assetCollection?.Assets != null)
            {
                assetCollection.Assets.Add(assetItem);
                var assetItemView = new AssetItemView(assetItem);
                Assets.Add(assetItemView);
            }


            MaterialBase material = new PhongMaterial
            {
                DiffuseColor = new(1, 0, 0),
                Shininess = 64.0f
            };


            MaterialIO materialIO = new MaterialIO();
            materialIO.Save(Path.Combine(StoreService.GetInstance().ProjectInfo.Path, assetItem.Path), material);


            AssetsService assetsService = new AssetsService();
            assetsService.SaveAssets();
        }

        public void OnAssetSelected()
        {
            if (SelectedAsset != null)
            {

                var shaderComponent = new ShaderComponent();
                shaderComponent.shaderType = ShaderTypes.Basic;

                var transformComponent = new TransformComponent();
                transformComponent.Position = new Vector3(0, 0, 0);
                transformComponent.Scale = new Vector3(1, 1, 1);

                var meshComponent = new MeshComponent();
                meshComponent.MeshPath = SelectedAsset.Model.Path;

                var materialComponent = new MaterialComponent();

                Entity entity = new Entity();

                entity.AddComponent(shaderComponent);
                entity.AddComponent(meshComponent);
                entity.AddComponent(transformComponent);
                entity.AddComponent(materialComponent);

                StoreService.GetInstance().AddEntity(entity);
                //    await Task.Delay(50);
                SelectedAsset = null;
            }
            //SceneIO sceneIO = new SceneIO();

            //var scenePath = Path.Combine(StoreService.GetInstance().ProjectInfo.Path, "scenes", StoreService.GetInstance().CurentScene.Path);
            //sceneIO.SaveScene(scenePath, StoreService.GetInstance().Scene);

        }

        public void AddFoldern()
        {

            var folderName = GetUniqueFolderName(CurrentFolder, "New Folder");

            var folder = new AssetItem
            {
                Name = folderName,
                Type = AssetType.Folder,
                Path = Path.Combine(CurrentFolder, folderName),
                BaseDirector = CurrentFolder

            };

            var projectInfo = StoreService.GetInstance().ProjectInfo;
            if (projectInfo != null)
            {
                Directory.CreateDirectory(Path.Combine(projectInfo.Path, folder.Path));
            }


            var assetCollection = StoreService.GetInstance().AssetCollection;
            if (assetCollection?.Assets != null)
            {
                assetCollection.Assets.Add(folder);

                var assetItemView = new AssetItemView(folder);
                Assets.Add(assetItemView);
            }

            AssetsService assetsService = new AssetsService();
            assetsService.SaveAssets();
        }


        private string GetUniqueFolderName(string basePath, string baseName)
        {
            var projectInfo = StoreService.GetInstance().ProjectInfo;
            string candidate = baseName;
            if (projectInfo != null)
            {
                int counter = 1;
                candidate = baseName;
                string candidatePath = Path.Combine(projectInfo.Path, basePath, candidate);

                while (Directory.Exists(candidatePath))
                {
                    candidate = $"{baseName} ({counter})";
                    candidatePath = Path.Combine(projectInfo.Path, basePath, candidate);
                    counter++;
                }
            }
            return candidate;
        }

        public AssetsViewModel()
        {
            SelectedAsset = null;

            var assetsService = new AssetsService();
            var storeService = StoreService.GetInstance();
            if (storeService.AssetCollection?.Assets != null)
            {
                List<AssetItem> assetItems = GetItemsInFolder(storeService.AssetCollection.Assets);
                Assets.Clear();
                foreach (var assetItem in assetItems)
                {
                    var assetItemView = new AssetItemView(assetItem);
                    Assets.Add(assetItemView);
                }
            }

            BreadcrumbParts.Add(new BreadcrumbItem
            {
                Index = BreadcrumbParts.Count,
                Name = "assets",
            });
        }
        public void OpenFolder()
        {
            var storeService = StoreService.GetInstance();
            if (storeService.AssetCollection?.Assets != null && SelectedAsset != null && SelectedAsset.Model.Type == AssetType.Folder)
            {
                BreadcrumbParts.Add(new BreadcrumbItem
                {
                    Index = BreadcrumbParts.Count,
                    Name = SelectedAsset.Model.Name,
                });
                CurrentFolder = Path.Combine(CurrentFolder, SelectedAsset.Model.Name);
                var filtered = GetItemsInFolder(storeService.AssetCollection.Assets);
                Assets.Clear();
                foreach (var item in filtered)
                    Assets.Add(new AssetItemView(item));
            }
        }

        public List<AssetItem> GetItemsInFolder(List<AssetItem> allAssets)
        {
            return allAssets
                .Where(a => a.BaseDirector == CurrentFolder)
                .OrderByDescending(a => a.Type == AssetType.Folder)
                .ThenBy(a => a.Name)
                .ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void OnMeshSelected()
        {
            if (SelectedAsset != null)
            {
                MaterialIO materialIO = new MaterialIO();
                var material = materialIO.Load(Path.Combine(StoreService.GetInstance().ProjectInfo.Path, SelectedAsset.Model.Path));
                StoreService.GetInstance().EditMaterial(material);
            }

        }
    }
}
