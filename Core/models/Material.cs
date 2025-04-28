using Core.graphics.shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PhongMaterial), "phong")]
    [JsonDerivedType(typeof(PBRMaterial), "pbr")]
    public abstract class MaterialBase
    {
        public abstract ShaderTypes Shader { get; }
    }

    public class PhongMaterial : MaterialBase
    {
        public override ShaderTypes Shader => ShaderTypes.Phong;

        public Vector3 DiffuseColor { get; set; } = new(1, 1, 1);
        public Vector3 SpecularColor { get; set; } = new(1, 1, 1);
        public float Shininess { get; set; } = 32.0f;
    }

    public class PBRMaterial : MaterialBase
    {
        public override ShaderTypes Shader => ShaderTypes.PBR;

        public string AlbedoMap { get; set; } = "";
        public float Metallic { get; set; } = 0.5f;
        public float Roughness { get; set; } = 0.5f;
        public Vector3 Emissive { get; set; } = new(0, 0, 0);
    }
}
