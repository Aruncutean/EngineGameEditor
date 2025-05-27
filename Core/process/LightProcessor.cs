using Core.component;
using Core.entity;
using Core.scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.process
{
    public class LightProcessor
    {
        public List<Entity> GetLights(World scene)
        {
            return scene.Entities
                .Where(e => e.HasComponent<LightComponent>())
                .ToList();
        }
    }
}
