using Core.assets;
using Core.graphics.mesh;
using Core.models;
using Core.services;

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
                var storeService = DataService.Instance;
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
                var storeService = DataService.Instance;
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
            var projectPath = DataService.Instance.ProjectInfo?.Path;
            if (projectPath == null)
            {
                throw new InvalidOperationException("ProjectInfo or its Path is null.");
            }
            assetManager.saveAsset(Path.Combine(projectPath, assetPathFile), DataService.Instance.AssetCollection);
        }

        public AssetItem SaveMesh(MeshData meshData, string path = "assets")
        {
            checkPath();
            var storeService = DataService.Instance;
            var projectPath = storeService.ProjectInfo?.Path;
            if (projectPath == null)
            {
                throw new InvalidOperationException("ProjectInfo or its Path is null.");
            }

            var meshPath = Path.Combine(projectPath, path);
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
                Path = Path.Combine(meshPath, meshData.Id + ".mesh"),
                BaseDirector = path,
            };
            storeService.AssetCollection.Assets.Add(assetItem);
            SaveAssets();
            return assetItem;
        }

        public MeshData LoadMesh(string path)
        {
            return assetManager.LoadMesh(path);
        }


        public void AddMaterial(string CurrentFolder)
        {
            var materialName = GetUniqueMaterialPath(CurrentFolder, "New Material");
            var assetItem = new AssetItem
            {
                Name = materialName,
                Type = AssetType.Material,
                Path = Path.Combine(CurrentFolder, $"{materialName}.material.json"),
                BaseDirector = CurrentFolder
            };
            var projectInfo = DataService.Instance.ProjectInfo;
            if (projectInfo != null)
            {
                Directory.CreateDirectory(Path.Combine(projectInfo.Path, CurrentFolder));
            }
            var assetCollection = DataService.Instance.AssetCollection;
            if (assetCollection?.Assets != null)
            {
                assetCollection.Assets.Add(assetItem);
            }
            SaveAssets();
        }

        public void AddScript(string CurrentFolder)
        {
            //ScriptWindows scriptWindow = new ScriptWindows();
            //scriptWindow.Show();

            //scriptWindow.SaveScript += () =>
            //{
            //    var scriptName = GetUniqueFolderName(CurrentFolder, scriptWindow.ScriptName);
            //    var scriptPath = Path.Combine(CurrentFolder, $"{scriptName}.cs");
            //    var assetItem = new AssetItem
            //    {
            //        Name = scriptName,
            //        Type = AssetType.Script,
            //        Path = scriptPath,
            //        BaseDirector = CurrentFolder
            //    };

            //    var assetCollection = StoreService.GetInstance().AssetCollection;
            //    if (assetCollection?.Assets != null)
            //    {
            //        assetCollection.Assets.Add(assetItem);
            //        var assetItemView = new AssetItemView(assetItem);
            //        Assets.Add(assetItemView);
            //    }

            //    AssetsService assetsService = new AssetsService();
            //    assetsService.SaveAssets();

            //    ScriptService scriptService = new ScriptService();
            //    scriptService.AddNewScrips(Path.Combine(StoreService.GetInstance().ProjectData.Path, "script"), scriptWindow.ScriptName);

            //    scriptWindow.Close();
            // };

        }

        public void AddFolder(string name, string CurrentFolder)
        {
            var folderName = GetUniqueFolderName(CurrentFolder, "New Folder");

            var folder = new AssetItem
            {
                Name = folderName,
                Type = AssetType.Folder,
                Path = Path.Combine(CurrentFolder, folderName),
                BaseDirector = CurrentFolder

            };

            var projectInfo = DataService.Instance.ProjectInfo;
            if (projectInfo != null)
            {
                Directory.CreateDirectory(Path.Combine(projectInfo.Path, folder.Path));
            }


            var assetCollection = DataService.Instance.AssetCollection;

            if (assetCollection?.Assets != null)
            {
                assetCollection.Assets.Add(folder);
            }

            SaveAssets();
        }

        public void SaveTexture(string path, string CurrentFolder)
        {
            string fileName = Path.GetFileName(path);
            string name = GetUniqueMaterialPath(CurrentFolder, fileName);
            var assetItem = new AssetItem
            {
                Name = name,
                Type = AssetType.Texture,
                Path = Path.Combine(CurrentFolder, name),
                BaseDirector = CurrentFolder
            };

            var assetCollection = DataService.Instance.AssetCollection;
            if (assetCollection?.Assets != null)
            {
                assetCollection.Assets.Add(assetItem);
            }

            string destinatieFolder = Path.Combine(DataService.Instance.ProjectInfo.Path, CurrentFolder);
            string destinatiePath = Path.Combine(destinatieFolder, fileName);

            File.Copy(path, destinatiePath, overwrite: true);

            SaveAssets();
        }

        private void checkPath()
        {
            var projectInfo = DataService.Instance.ProjectInfo;

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


        private string GetUniqueFolderName(string basePath, string baseName)
        {
            var projectInfo = DataService.Instance.ProjectInfo;
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

        private string GetUniqueMaterialPath(string baseDirectory, string baseName)
        {

            string name = baseName;
            string path = Path.Combine(baseDirectory, $"{name}.material.json");

            int counter = 1;
            while (File.Exists(Path.Combine(DataService.Instance.ProjectInfo.Path, path)))
            {
                name = $"{baseName} ({counter})";
                path = Path.Combine(baseDirectory, $"{name}.material.json");
                counter++;
            }

            return name;
        }

    }
}
