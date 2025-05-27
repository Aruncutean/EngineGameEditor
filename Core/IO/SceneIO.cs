using Core.component;
using Core.scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.IO
{
    public class SceneIO
    {
        public SceneIO() { }


        public void SaveScene(string path, World scene)
        {
            var json = JsonSerializer.Serialize(scene, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = {
                    new Vector3Converter(),
                    new Vector2Converter(),
                    new ComponentConverter()
                }
            });
            File.WriteAllText(path, json);
        }

        public World LoadScene(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<World>(json, new JsonSerializerOptions
            {
                Converters = {
                    new ComponentConverter(),
                    new Vector3Converter(),
                    new Vector2Converter()
                }
            }) ?? new();
        }

    }
}
