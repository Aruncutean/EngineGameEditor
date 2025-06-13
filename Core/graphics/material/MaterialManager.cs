using Core.assets;
using Core.graphics.shader;
using Core.IO;
using Core.models;
using Core.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.material
{
    public class MaterialManager
    {
        private static readonly Dictionary<string, MaterialBase> _material = new();

        public static MaterialBase? Get(string id)
        {
            if (id == "default")
            {
                return new MaterialDefault()
                {
                    Id = "default",
                    Name = "default",
                    DiffuseColor = new System.Numerics.Vector3(1, 1, 1),
                    SpecularColor = new System.Numerics.Vector3(1, 1, 1),
                    Shininess = 32.0f,
                };
            }

            if (_material.ContainsKey(id))
            {
                return _material[id];
            }
            else
            {
                MaterialIO materialIO = new MaterialIO();
                ProjectData projectData = DataService.Instance.ProjectData;
                if (projectData != null)
                {
                    string path = Path.Combine(projectData.Path, "Assets/assets.json");
                    AssetManager assetManager = new AssetManager();
                    AssetCollection assetCollection = assetManager.loadAsset(path);


                    AssetItem? assetItem = assetCollection.Assets.FirstOrDefault(x => x.Id == id);
                    if (assetItem != null)
                    {
                        MaterialBase material = materialIO.Load(Path.Combine(projectData.Path, assetItem.Path));
                        _material.Add(id, material);
                        return material;
                    }
                }
            }
            return null;
        }

    }
}
