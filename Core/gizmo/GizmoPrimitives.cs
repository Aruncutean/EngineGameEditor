using Core.graphics.shader;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.gizmo
{
    public class GizmoPrimitives
    {
        public static void DrawLine(GL gl, Vector3 from, Vector3 to, Vector3 color)
        {
            float[] vertices = new float[]
            {
        from.X, from.Y, from.Z,
        to.X,   to.Y,   to.Z
            };

            uint vbo = gl.GenBuffer();
            uint vao = gl.GenVertexArray();

            gl.BindVertexArray(vao);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            gl.BufferData<float>(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), vertices, BufferUsageARB.StaticDraw);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            int colorLoc = gl.GetUniformLocation(ShaderManager.Get(ShaderTypes.gizmo), "uColor");
            gl.Uniform3(colorLoc, color.X, color.Y, color.Z);

            gl.DrawArrays(PrimitiveType.Lines, 0, 2);

            gl.DeleteBuffer(vbo);
            gl.DeleteVertexArray(vao);
        }

        public static void DrawCircle(GL gl, Vector3 center, float radius, Vector3 color, int segments = 32)
        {
            float[] vertices = new float[segments * 3];
            for (int i = 0; i < segments; i++)
            {
                float angle = 2.0f * MathF.PI * i / segments;
                float x = MathF.Cos(angle) * radius;
                float z = MathF.Sin(angle) * radius;

                vertices[i * 3 + 0] = center.X + x;
                vertices[i * 3 + 1] = center.Y;
                vertices[i * 3 + 2] = center.Z + z;
            }

            uint vbo = gl.GenBuffer();
            uint vao = gl.GenVertexArray();

            gl.BindVertexArray(vao);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            gl.BufferData<float>(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), vertices, BufferUsageARB.StaticDraw);

            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            int colorLoc = gl.GetUniformLocation(ShaderManager.Get(ShaderTypes.gizmo), "uColor");
            gl.Uniform3(colorLoc, color.X, color.Y, color.Z);

            gl.DrawArrays(PrimitiveType.LineLoop, 0, (uint)segments);

            gl.DeleteBuffer(vbo);
            gl.DeleteVertexArray(vao);
        }
    }
}
