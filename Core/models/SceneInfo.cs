using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.models
{
    public class SceneInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
