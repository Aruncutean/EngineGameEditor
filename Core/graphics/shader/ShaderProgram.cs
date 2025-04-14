using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.shader
{
    public class ShaderProgram
    {
        public uint ProgramId { get; private set; }

        public ShaderProgram(uint programId)
        {
            ProgramId = programId;
        }

        public void Use(GL gl)
        {
            gl.UseProgram(ProgramId);
        }
    }
}
