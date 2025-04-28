using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.component
{
    public enum LightType
    {
        Point,
        Directional,
        Spot
    }
    public class LightComponent : IComponent
    {
        public LightType Type { get; set; } = LightType.Point;
        public Vector3 Color { get; set; } = Vector3.One;
        public float Intensity { get; set; } = 1f;
        public float Range { get; set; } = 10f;
        public float SpotAngle { get; set; } = 45f; // doar pt. Spot
    }
}
