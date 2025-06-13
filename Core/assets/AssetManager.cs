using Core.component;
using Core.graphics.mesh;
using Core.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.assets
{
    public class AssetManager
    {
        private static Dictionary<string, MeshData> _loadedMeshes = new();

        public void saveAsset(string path, AssetCollection assetCollection)
        {
            string newJson = JsonSerializer.Serialize(assetCollection, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                        new Vector3Converter(),
                        new Vector2Converter(),
                        new UInt32ListConverter(),
                        new LightTypeConverter(),
                        new FloatConverter(),
                        new BoolConverter(),
                        new LightBaseConverter()
                    }
            });
            File.WriteAllText(path, newJson);
        }

        public AssetCollection loadAsset(string path)
        {
            if (File.Exists(path))
            {
                Console.WriteLine("Exist");
                string jsonString = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AssetCollection>(jsonString, new JsonSerializerOptions
                {
                    Converters = { new UInt32ListConverter(), new LightTypeConverter(), new AssetTypeConverter() }
                }) ?? new AssetCollection();
            }
            return new AssetCollection();
        }

        public void SaveMesh(string path, MeshData meshData)
        {
            string newJson = JsonSerializer.Serialize(meshData, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                        new Vector3Converter(),
                        new Vector2Converter(),
                        new UInt32ListConverter(),
                        new LightTypeConverter(),
                        new FloatConverter(),
                        new BoolConverter(),
                        new LightBaseConverter()
                    }
            });
            File.WriteAllText(path, newJson);
        }

        public MeshData LoadMesh(string path)
        {
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);
                return JsonSerializer.Deserialize<MeshData>(jsonString) ?? new MeshData();
            }
            return new MeshData();
        }
    }
}
