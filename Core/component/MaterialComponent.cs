using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.component
{
    public class MaterialComponent : IComponent
    {
        public Vector3 Albedo { get; set; } = new Vector3(1f, 1f, 1f);
        public float Metallic { get; set; } = 0f;
        public float Roughness { get; set; } = 0.5f;
        public float Specular { get; set; } = 0.5f;
        public string? AlbedoTexture { get; set; }
        public string? NormalTexture { get; set; }
        public string? SpecularTexture { get; set; }
    }
}
