using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.component
{
    public class CameraControllerComponent : IComponent
    {
        public float MoveSpeed { get; set; } = 5f;
        public float LookSensitivity { get; set; } = 0.1f;
    }
}
