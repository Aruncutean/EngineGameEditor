using Core.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.system
{
    public class CameraSystem
    {
        public Matrix4x4 GetViewMatrix(TransformComponent transform)
        {
            var forward = Vector3.Transform(-Vector3.UnitZ, transform.Rotation);
            var target = transform.Position + forward;
            return Matrix4x4.CreateLookAt(transform.Position, target, Vector3.UnitY);
        }

        public Matrix4x4 GetProjectionMatrix(CameraComponent cam, float aspectRatio)
        {
            float fovRadians = cam.FieldOfView * (MathF.PI / 180f);
            return Matrix4x4.CreatePerspectiveFieldOfView(fovRadians, aspectRatio, cam.NearClip, cam.FarClip);
        }
    }
}
