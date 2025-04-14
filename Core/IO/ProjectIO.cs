using Core.models;
using Core.scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.IO
{
    public class ProjectIO
    {
        public ProjectIO() { }

        public void SaveProject(string path, ProjectData projectData)
        {
            var json = JsonSerializer.Serialize(projectData, new JsonSerializerOptions
            {
                WriteIndented = true,

            });

            File.WriteAllText(path, json);
        }

        public ProjectData LoadProject(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ProjectData>(json, new JsonSerializerOptions())!;
        }
    }
}
