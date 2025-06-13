using Core.graphics.light;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.component
{

    public class LightComponent : IComponent
    {
        [JsonConverter(typeof(LightTypeConverter))]
        public LightType Type { get; set; } = LightType.Point;

        [JsonConverter(typeof(LightBaseConverter))]
        public LightBase LightBase { get; set; } = new LightBase();

    }
}
