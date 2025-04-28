using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.component
{
    public class CameraComponent : IComponent
    {
        public float FieldOfView { get; set; } = 45f;
        public float NearClip { get; set; } = 0.1f;
        public float FarClip { get; set; } = 1000f;
        public Vector3 Front { get; set; } = new(0f, 0f, -1f);
        public Vector3 Up { get; set; } = new(0f, 1f, 0f);

        public bool IsMainCamera { get; set; } = false;
    }
}
