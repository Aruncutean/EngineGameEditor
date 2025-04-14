using Core.component;
using Core.entity;
using Core.scene;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.system
{
    public class CameraControllerSystemRunTime
    {
        private readonly IInputContext _input;
        private float _yaw = 0f;
        private float _pitch = 0f;
        private Vector2 _previousMousePosition;
        private bool _initialized = false;
        public CameraControllerSystemRunTime(IInputContext input)
        {
            _input = input;
        }
        public void Update(float deltaTime, Entity entity)
        {
            var keyboard = _input.Keyboards[0];
            var mouse = _input.Mice[0];

            if (!_initialized)
            {
                _previousMousePosition = mouse.Position;
                _initialized = true;
            }

            if (entity.HasComponent<CameraComponent>() && entity.HasComponent<CameraControllerComponent>())
            {
                var transform = entity.GetComponent<TransformComponent>();
                var controller = entity.GetComponent<CameraControllerComponent>();

                Vector3 move = Vector3.Zero;

                if (keyboard.IsKeyPressed(Key.W)) move -= Vector3.UnitZ;
                if (keyboard.IsKeyPressed(Key.S)) move += Vector3.UnitZ;
                if (keyboard.IsKeyPressed(Key.A)) move -= Vector3.UnitX;
                if (keyboard.IsKeyPressed(Key.D)) move += Vector3.UnitX;
                if (keyboard.IsKeyPressed(Key.Space)) move += Vector3.UnitY;
                if (keyboard.IsKeyPressed(Key.ShiftLeft)) move -= Vector3.UnitY;

                if (move != Vector3.Zero)
                {
                    move = Vector3.Normalize(move);
                    move = Vector3.Transform(move, transform.Rotation);
                    transform.Position += move * controller.MoveSpeed * deltaTime;
                }

                var delta = mouse.Position - _previousMousePosition;
                _previousMousePosition = mouse.Position;

                float yaw = delta.X * controller.LookSensitivity * deltaTime;
                float pitch = delta.Y * controller.LookSensitivity * deltaTime;

                var rot = transform.Rotation;
                var yawQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -yaw);
                var pitchQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -pitch);

                transform.Rotation = Quaternion.Normalize(pitchQuat * rot * yawQuat);
            }
        }
    }
}
