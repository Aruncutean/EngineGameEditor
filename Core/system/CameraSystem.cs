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
        public Matrix4x4 GetViewMatrix(TransformComponent transform,CameraComponent cameraComponent)
        {
            return Matrix4x4.CreateLookAt(transform.Position, transform.Position + cameraComponent.Front, cameraComponent.Up);
        }

        public Matrix4x4 GetProjectionMatrix(CameraComponent cam, float aspectRatio)
        {
            float fovRadians = cam.FieldOfView * (MathF.PI / 180f);
            return Matrix4x4.CreatePerspectiveFieldOfView(fovRadians, aspectRatio, cam.NearClip, cam.FarClip);
        }
    }
}
