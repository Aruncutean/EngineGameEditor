using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.graphics
{
    public static class ColorUtils
    {
        public static Vector3 HexToVector3(string hex)
        {
            hex = hex.TrimStart('#');

            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return new Vector3(r / 255f, g / 255f, b / 255f);
        }
    }
}
