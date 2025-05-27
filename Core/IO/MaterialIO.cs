using Core.assets;
using Core.component;
using Core.graphics.material;
using Core.models;
using Core.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.IO
{
    public class MaterialIO
    {
        public MaterialIO() { }

        public void Save(string path, MaterialBase materialBase)
        {
            var json = JsonSerializer.Serialize(materialBase, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                        new Vector3Converter(),
                        new Vector2Converter()
                    }
            });

            File.WriteAllText(path, json);
        }

        public MaterialBase Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<MaterialBase>(json, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                        new Vector3Converter(),
                        new Vector2Converter()
                    }
            })!;
        }


        public List<MaterialBase> getAll()
        {
            ProjectData projectData = DataService.Instance.ProjectData;
            List<MaterialBase> materials = new List<MaterialBase>();
            if (projectData != null)
            {
                string path = Path.Combine(projectData.Path, "assets/assets.js");

                AssetManager assetManager = new AssetManager();
                AssetCollection assetCollection = assetManager.loadAsset(path);

                List<AssetItem> assetItems = assetCollection.Assets.Where(x => x.Type == AssetType.Material).ToList();

                foreach (AssetItem assetItem in assetItems)
                {
                    MaterialBase material = Load(Path.Combine(projectData.Path, assetItem.Path));
                    materials.Add(material);
                }
            }

            return materials;
        }
    }
}
