using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.scene
{
    public class SceneSerializer
    {
        public static void SaveScene(string path, World scene)
        {
            var json = JsonSerializer.Serialize(scene, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new ComponentConverter() }
            });

            File.WriteAllText(path, json);
        }

        public static World LoadScene(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<World>(json, new JsonSerializerOptions
            {
                Converters = { new ComponentConverter() }
            })!;
        }
    }
}
