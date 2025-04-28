using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.shader
{
    public enum ShaderTypes
    {
        Basic,
        Phong,
        BlinnPhong,
        PBR,
        [Browsable(false)]
        gizmo
    }
}
