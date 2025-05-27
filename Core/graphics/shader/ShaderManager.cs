using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.shader
{
    public class ShaderManager
    {
        private static readonly Dictionary<ShaderTypes, uint> _shaders = new();
        private static GL _gl;

        public static void Init(GL gl)
        {
            _gl = gl;
        }
        public static void LoadShaders(GL gl)
        {
            Init(gl);
            Load(ShaderTypes.Basic, "shader/basic.vert.glsl", "shader/basic.frag.glsl");
            Load(ShaderTypes.gizmo, "shader/gizmo.vert.glsl", "shader/gizmo.frag.glsl");
            Load(ShaderTypes.Phong, "shader/phong.vert.glsl", "shader/phong.frag.glsl");
        }

        private static uint Load(ShaderTypes id, string vertexPath, string fragmentPath)
        {
            if (_shaders.ContainsKey(id))
                return _shaders[id];

            string vertexCode = File.ReadAllText(vertexPath);
            string fragmentCode = File.ReadAllText(fragmentPath);

            uint vertexShader = CompileShader(ShaderType.VertexShader, vertexCode);
            uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentCode);

            uint program = _gl.CreateProgram();
            _gl.AttachShader(program, vertexShader);
            _gl.AttachShader(program, fragmentShader);
            _gl.LinkProgram(program);

            _gl.GetProgram(program, GLEnum.LinkStatus, out var success);
            if (success == 0)
            {
                string infoLog = _gl.GetProgramInfoLog(program);
                throw new Exception($"Shader link error ({id}): {infoLog}");
            }

            _gl.DetachShader(program, vertexShader);
            _gl.DetachShader(program, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);

            _shaders[id] = program;
            return program;
        }

        public static uint Get(ShaderTypes id)
        {
            if (!_shaders.TryGetValue(id, out var program))
                throw new Exception($"Shader with ID '{id}' not loaded.");

            return program;
        }

        private static uint CompileShader(ShaderType type, string source)
        {
            uint shader = _gl.CreateShader(type);
            _gl.ShaderSource(shader, source);
            _gl.CompileShader(shader);

            _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
            if (status == 0)
            {
                string infoLog = _gl.GetShaderInfoLog(shader);
                throw new Exception($"{type} compile error: {infoLog}");
            }

            return shader;
        }
    }
}
