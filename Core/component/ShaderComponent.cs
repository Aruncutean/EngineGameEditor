using Core.graphics.shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.component
{
    public class ShaderComponent : IComponent
    {
        public ShaderTypes shaderType { get; set; } = ShaderTypes.Basic;
    }
}
