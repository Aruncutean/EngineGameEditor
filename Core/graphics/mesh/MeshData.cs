using Core.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.graphics.mesh
{
    public class MeshData
    {
        public string Id { get; set; } = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

        public List<Vector3> Positions { get; set; } = new();

        public List<Vector3> Normals { get; set; } = new();

        public List<Vector2> UVs { get; set; } = new();
        public List<uint> Indices { get; set; } = new();
    }
}
