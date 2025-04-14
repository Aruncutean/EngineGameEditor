using Core.assets;
using Core.models;
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
        private string assetPath = "assets";
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
                if (StoreService.GetInstance().AssetCollection == null)
                {
                    var projectPath = StoreService.GetInstance().ProjectInfo.Path;
                    StoreService.GetInstance().AssetCollection = assetManager.loadAsset(Path.Combine(projectPath, assetPathFile));
                }
            }
            catch (FileNotFoundException)
            {
                StoreService.GetInstance().AssetCollection = new AssetCollection();
                saveAssets();
            }
        }
        public void saveAssets()
        {
            checkPath();
            var projectPath = StoreService.GetInstance().ProjectInfo.Path;
            assetManager.saveAsset(Path.Combine(projectPath, assetPathFile), StoreService.GetInstance().AssetCollection);
        }

        public void SaveMesh(MeshData meshData)
        {
            checkPath();
            var meshPath = Path.Combine(StoreService.GetInstance().ProjectInfo.Path, this.meshPath);

            assetManager.SaveMesh(Path.Combine(meshPath, meshData.Id + ".mesh"), meshData);
            AssetItem assetItem = new AssetItem
            {
                Name="Cub",
                Type= AssetType.Mesh,
                Path= Path.Combine(meshPath, meshData.Id + ".mesh")
            };
            StoreService.GetInstance().AssetCollection.Assets.Add(assetItem);
            saveAssets();
           
        }

        public MeshData LoadMesh(string path)
        {
            return assetManager.LoadMesh(path);
        }

        private void checkPath()
        {
            var projectInfo = StoreService.GetInstance().ProjectInfo;

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
