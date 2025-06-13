using Assimp;
using Core.graphics.mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.entity
{
    public class LoadEntity
    {
        public MeshData LoadMesh(string filePath)
        {
            var context = new AssimpContext();
            var scene = context.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.JoinIdenticalVertices);

            if (scene.MeshCount == 0)
                throw new Exception("No mesh found in file.");

            var mesh = scene.Meshes[0];
            var data = new MeshData();

            foreach (var v in mesh.Vertices)
                data.Positions.Add(new Vector3(v.X, v.Y, v.Z));

            foreach (var n in mesh.Normals)
                data.Normals.Add(new Vector3(n.X, n.Y, n.Z));

            if (mesh.TextureCoordinateChannelCount > 0)
            {
                foreach (var uv in mesh.TextureCoordinateChannels[0])
                    data.UVs.Add(new Vector2(uv.X, uv.Y));
            }

            foreach (var face in mesh.Faces)
            {
                foreach (var i in face.Indices)
                    data.Indices.Add((uint)i);
            }

            return data;
        }
    }
}

