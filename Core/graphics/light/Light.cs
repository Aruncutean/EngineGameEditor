using Core.graphics.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.graphics.light
{

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(LightPoint), "LightPoint")]
    [JsonDerivedType(typeof(LightDirectional), "LightDirectional")]
    [JsonDerivedType(typeof(LightSpot), "LightSpot")]
    public class LightBase
    {

    }

    public class LightPoint : LightBase
    {
        public float Intensity { get; set; } = 1.0f;
        public float Range { get; set; } = 10.0f;
        public Vector3 Color { get; set; } = new Vector3(1, 1, 1);
    }

    public class LightDirectional : LightBase
    {
        public Vector3 Direction { get; set; } = new Vector3(0, -1, 0);
        public float Intensity { get; set; } = 1.0f;
        public Vector3 Color { get; set; } = new Vector3(1, 1, 1);
    }

    public class LightSpot : LightBase
    {
        public Vector3 Direction { get; set; } = new Vector3(0, -1, 0);
        public float Intensity { get; set; } = 1.0f;
        public float Range { get; set; } = 10.0f;
        public float Cutoff { get; set; } = 30.0f;
        public Vector3 Color { get; set; } = new Vector3(1, 1, 1);
    }


}
