using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.shader
{
    public class ShaderBinder
    {
        private readonly ShaderProgram _shader;
        private readonly GL _gl;
        private readonly Dictionary<string, int> _uniformCache = new();

        public ShaderBinder(GL gl, ShaderProgram shader)
        {
            _gl = gl;
            _shader = shader;
        }

        private int GetLocation(string name)
        {
            if (_uniformCache.TryGetValue(name, out var loc))
                return loc;

            int location = _gl.GetUniformLocation(_shader.ProgramId, name);
            if (location == -1)
                Console.WriteLine($"[Warning] Uniform '{name}' not found.");
            _uniformCache[name] = location;
            return location;
        }

        public void SetFloat(string name, float value)
            => _gl.Uniform1(GetLocation(name), value);

        public void SetInt(string name, int value)
            => _gl.Uniform1(GetLocation(name), value);

        public void SetVec3(string name, System.Numerics.Vector3 vec)
            => _gl.Uniform3(GetLocation(name), vec.X, vec.Y, vec.Z);

        public void SetVec4(string name, System.Numerics.Vector4 vec)
            => _gl.Uniform4(GetLocation(name), vec.X, vec.Y, vec.Z, vec.W);

        public void SetMat4(string name, System.Numerics.Matrix4x4 mat)
        {
            Span<float> data = stackalloc float[16]
            {
                 mat.M11, mat.M12, mat.M13, mat.M14,
                 mat.M21, mat.M22, mat.M23, mat.M24,
                 mat.M31, mat.M32, mat.M33, mat.M34,
                 mat.M41, mat.M42, mat.M43, mat.M44
            };


            unsafe
            {
                fixed (float* ptr = data)
                {
                    _gl.UniformMatrix4(GetLocation(name), 1, false, ptr);
                }
            }
        }
    }
}
