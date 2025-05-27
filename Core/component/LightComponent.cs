using Core.graphics.light;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.component
{

    public class LightComponent : IComponent
    {
        public LightType Type { get; set; } = LightType.Point;

        public LightBase LightBase { get; set; } = new LightBase();

    }
}
