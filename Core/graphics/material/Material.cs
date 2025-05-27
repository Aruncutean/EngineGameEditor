using Core.graphics.shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.graphics.material
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(MaterialDefault), "default")]
    [JsonDerivedType(typeof(MaterialPhong), "phong")]
    [JsonDerivedType(typeof(MaterialPBR), "pbr")]
    public abstract class MaterialBase
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Path { get; set; } = "";
        public abstract ShaderTypes Shader { get; }
    }
    public class MaterialPhong : MaterialBase
    {
        public override ShaderTypes Shader => ShaderTypes.Phong;
        public ColorOrTexture Ambient { get; set; } = new() { Color = "#ffffff" };
        public ColorOrTexture Diffuse { get; set; } = new() { Color = "#ffffff" };
        public ColorOrTexture Specular { get; set; } = new() { Color = "#ffffff" };
        public float Shininess { get; set; } = 32.0f;
    }

    public class MaterialPBR : MaterialBase
    {
        public override ShaderTypes Shader => ShaderTypes.PBR;

        public string AlbedoMap { get; set; } = "";
        public float Metallic { get; set; } = 0.5f;
        public float Roughness { get; set; } = 0.5f;
        public Vector3 Emissive { get; set; } = new(0, 0, 0);
    }

    public class MaterialDefault : MaterialBase
    {
        public override ShaderTypes Shader => ShaderTypes.Basic;
        public Vector3 DiffuseColor { get; set; } = new(1, 1, 1);
        public Vector3 SpecularColor { get; set; } = new(1, 1, 1);
        public float Shininess { get; set; } = 32.0f;
    }


    public class ColorOrTexture
    {
        public string? Color { get; set; }
        public string? TexturePath { get; set; }
        public bool IsTexture { get; set; } = false;

    }

}
