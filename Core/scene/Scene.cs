using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.entity;

namespace Core.scene
{
    public class Scene
    {
        public List<Entity> Entities { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }

        public Scene()
        {

        }

        public void AddEntity(Entity e) {
            Entities.Add(e);
            LastUpdated = DateTime.Now;

        }
    }
}
