using Core.assets;
using Core.graphics.mesh;
using Core.models;
using Core.services;
using EditorAvalonia.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorAvalonia.service
{
    public class AssetsService
    {
        private string assetPathFile = "assets\\assets.json";
        private string meshPath = "assets\\mesh";

        private AssetManager assetManager;
        public AssetsService()
        {
            assetManager = new AssetManager();
            loadAsset();
        }
        public void loadAsset()
        {
            try
            {
                var storeService = StoreService.GetInstance();
                if (storeService.AssetCollection == null)
                {
                    var projectPath = storeService.ProjectInfo?.Path;

                    if (string.IsNullOrEmpty(projectPath))
                    {
                        storeService.AssetCollection = new AssetCollection
                        {
                            Assets = new List<AssetItem>()
                        };
                    }
                    else
                    {
                        storeService.AssetCollection = assetManager.loadAsset(Path.Combine(projectPath, assetPathFile));
                        DataService.Instance.AssetCollection = storeService.AssetCollection;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                var storeService = StoreService.GetInstance();
                storeService.AssetCollection = new AssetCollection
                {
                    Assets = new List<AssetItem>()
                };
                SaveAssets();
            }
        }
        public void SaveAssets()
        {
            checkPath();
            var projectPath = StoreService.GetInstance().ProjectInfo?.Path;
            if (projectPath == null)
            {
                throw new InvalidOperationException("ProjectInfo or its Path is null.");
            }
            assetManager.saveAsset(Path.Combine(projectPath, assetPathFile), StoreService.GetInstance().AssetCollection);
        }

        public void SaveMesh(MeshData meshData)
        {
            checkPath();
            var storeService = StoreService.GetInstance();
            var projectPath = storeService.ProjectInfo?.Path;
            if (projectPath == null)
            {
                throw new InvalidOperationException("ProjectInfo or its Path is null.");
            }

            var meshPath = Path.Combine(projectPath, this.meshPath);
            assetManager.SaveMesh(Path.Combine(meshPath, meshData.Id + ".mesh"), meshData);

            if (storeService.AssetCollection == null)
            {
                storeService.AssetCollection = new AssetCollection
                {
                    Assets = new List<AssetItem>()
                };
            }

            AssetItem assetItem = new AssetItem
            {
                Name = "Cub",
                Type = AssetType.Mesh,
                Path = Path.Combine(meshPath, meshData.Id + ".mesh")
            };
            storeService.AssetCollection.Assets.Add(assetItem);
            SaveAssets();
        }

        public MeshData LoadMesh(string path)
        {
            return assetManager.LoadMesh(path);
        }

        private void checkPath()
        {
            var projectInfo = StoreService.GetInstance().ProjectInfo;

            //if (projectInfo == null || string.IsNullOrEmpty(projectInfo.Path))
            //{
            //    throw new InvalidOperationException("ProjectInfo or its Path is null. Ensure a project is selected or created before accessing assets.");
            //}

            var assetsPath = Path.Combine(projectInfo.Path, "assets");
            var meshPath = Path.Combine(assetsPath, "mesh");

            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }

            if (!Directory.Exists(meshPath))
            {
                Directory.CreateDirectory(meshPath);
            }
        }

    }
}
