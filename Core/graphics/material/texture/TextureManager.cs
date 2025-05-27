using Core.assets;
using Core.IO;
using Core.models;
using Core.services;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.material.texture
{
    public class TextureManager
    {
        private static readonly Dictionary<string, uint> _texture = new();


        public static uint Get(string id, GL gl)
        {
            if (id == null)
            {
                return 0;
            }

            if (_texture.ContainsKey(id))
            {
                return _texture[id];
            }
            else
            {
                MaterialIO materialIO = new MaterialIO();
                ProjectData projectData = DataService.Instance.ProjectData;
                if (projectData != null)
                {
                    string path = Path.Combine(projectData.Path, "assets/assets.json");
                    AssetManager assetManager = new AssetManager();
                    AssetCollection assetCollection = assetManager.loadAsset(path);
                    AssetItem? assetItem = assetCollection.Assets.FirstOrDefault(x => x.Id == id);
                    if (assetItem != null)
                    {
                        string texturePath = Path.Combine(projectData.Path, assetItem.Path);
                        Texture texture = new Texture();
                        uint textureID = texture.LoadTexture(gl, texturePath);
                        _texture.Add(id, textureID);
                        return textureID;
                    }
                }
            }
            return 0;
        }

    }
}
