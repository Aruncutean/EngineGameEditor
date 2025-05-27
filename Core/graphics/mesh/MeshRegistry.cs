using Core.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.graphics.mesh
{
    public static class MeshRegistry
    {
        private static Dictionary<string, MeshData> _loadedMeshes = new();

        public static MeshData GetMesh(string meshPath)
        {
            if (_loadedMeshes.TryGetValue(meshPath, out var mesh))
                return mesh;

            var json = File.ReadAllText(meshPath);
            var options = new JsonSerializerOptions
            {
                Converters = { new Vector3Converter(), new Vector2Converter() }
            };
            mesh = JsonSerializer.Deserialize<MeshData>(json, options)!;

            _loadedMeshes[meshPath] = mesh;
            return mesh;
        }
    }
}
