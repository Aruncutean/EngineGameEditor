﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.mesh
{
    using Core.component;
    using Silk.NET.OpenGL;

    public class GLMesh
    {
        private readonly GL _gl;
        public uint VAO { get; private set; }
        private uint _vbo;
        private uint _ebo;
        public int IndexCount { get; private set; }

        public GLMesh(GL gl, MeshData data)
        {
            _gl = gl;

            VAO = _gl.GenVertexArray();
            _vbo = _gl.GenBuffer();
            _ebo = _gl.GenBuffer();

            _gl.BindVertexArray(VAO);

            // Pozitii
            float[] vertexData = new float[data.Positions.Count * 8];
            for (int i = 0; i < data.Positions.Count; i++)
            {
                // Positions
                vertexData[i * 8 + 0] = data.Positions[i].X;
                vertexData[i * 8 + 1] = data.Positions[i].Y;
                vertexData[i * 8 + 2] = data.Positions[i].Z;

                // Texture coordinates
                vertexData[i * 8 + 3] = data.UVs.Count > i ? data.UVs[i].X : 0f;
                vertexData[i * 8 + 4] = data.UVs.Count > i ? data.UVs[i].Y : 0f;

                // Normals
                vertexData[i * 8 + 5] = data.Normals[i].X;
                vertexData[i * 8 + 6] = data.Normals[i].Y;
                vertexData[i * 8 + 7] = data.Normals[i].Z;

            }

            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            _gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertexData, BufferUsageARB.StaticDraw);


            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            _gl.BufferData<uint>(BufferTargetARB.ElementArrayBuffer, data.Indices.ToArray(), BufferUsageARB.StaticDraw);

            // layout(location = 0) -> vec3 position
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            _gl.EnableVertexAttribArray(0);

            // layout(location = 1) -> vec2 uv
            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            _gl.EnableVertexAttribArray(1);

            // layout(location = 2) -> vec3 normal
            _gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            _gl.EnableVertexAttribArray(2);


            _gl.BindVertexArray(0);

            IndexCount = data.Indices.Count;
        }

        public void Render()
        {
            _gl.BindVertexArray(VAO);
            unsafe
            {
                _gl.DrawElements(GLEnum.Triangles, (uint)IndexCount, GLEnum.UnsignedInt, null);
            }
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteBuffer(_ebo);
            _gl.DeleteVertexArray(VAO);
        }
    }
}
