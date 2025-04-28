using Core.component;
using Core.models;
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
            return JsonSerializer.Deserialize<MaterialBase>(json, new JsonSerializerOptions())!;
        }

    }
}
