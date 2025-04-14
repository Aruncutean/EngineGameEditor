using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.models
{
    public class ProjectData
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0";
        public string MainScene { get; set; } = string.Empty;
        public List<SceneInfo> Scenes { get; set; } = new();

    }
}
