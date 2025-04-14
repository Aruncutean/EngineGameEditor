using Core.graphics.mesh;
using Core.models;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.component
{
    public class MeshComponent : IComponent
    {
        public string MeshPath { get; set; } = string.Empty;

        [JsonIgnore]
        public MeshData? RuntimeMesh { get; set; }

        [JsonIgnore]
        public GLMesh gLMesh { get; set; } 


        public void LoadMesh(GL gL)
        {
        
            RuntimeMesh = MeshRegistry.GetMesh(MeshPath);
            gLMesh = new GLMesh(gL,RuntimeMesh);

        }
    }
}
