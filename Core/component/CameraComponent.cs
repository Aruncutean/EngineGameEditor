using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.component
{
    public class CameraComponent : IComponent
    {
        public float FieldOfView { get; set; } = 45f;
        public float NearClip { get; set; } = 0.1f;
        public float FarClip { get; set; } = 1000f;

        public bool IsMainCamera { get; set; } = false;
    }
}
