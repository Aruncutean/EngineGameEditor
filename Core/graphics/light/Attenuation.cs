using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics.light
{
    public static class Attenuation
    {
        public static void FromRange(float range, out float constant, out float linear, out float quadratic)
        {
            constant = 1.0f;
            linear = 4.5f / range;
            quadratic = 75.0f / (range * range);
        }
    }
}
