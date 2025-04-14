using Core.component;
using Core.entity;
using Core.scene;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.system
{
    public class CameraControllerSystem
    {
        private readonly HashSet<Key> _keys = new();
        private Vector2 _lastMousePos;
        private bool _firstMove = true;
        private float _yaw = 0f;
        private float _pitch = 0f;

        private bool mousePresssBool = false;
        public float MoveSpeed { get; set; } = 5f;
        public float LookSensitivity { get; set; } = 0.002f;

        public void mousePresss(bool mouseButtonPressed)
        {
            mousePresssBool = mouseButtonPressed;
        }

        public void OnKeyDown(Key key) => _keys.Add(key);
        public void OnKeyUp(Key key) => _keys.Remove(key);



        public void OnMouseMove(Vector2 newPos)
        {

            if (_firstMove)
            {
                _lastMousePos = newPos;
                _firstMove = false;
            }

            var delta = newPos - _lastMousePos;
            _lastMousePos = newPos;
            if (mousePresssBool)
            {


                _yaw -= delta.X * LookSensitivity;
                _pitch -= delta.Y * LookSensitivity;
                _pitch = Math.Clamp(_pitch, -89f, 89f);
            }
        }

        public void Update(float deltaTime, Entity cameraEntity)
        {
            var transform = cameraEntity.GetComponent<TransformComponent>();

            var forward = Vector3.Transform(-Vector3.UnitZ, transform.Rotation);
            var right = Vector3.Transform(Vector3.UnitX, transform.Rotation);
            var up = Vector3.UnitY;

            Vector3 move = Vector3.Zero;

            if (_keys.Contains(Key.W)) move += forward;
            if (_keys.Contains(Key.S)) move -= forward;
            if (_keys.Contains(Key.A)) move -= right;
            if (_keys.Contains(Key.D)) move += right;
            if (_keys.Contains(Key.Space)) move += up;
            if (_keys.Contains(Key.ShiftLeft)) move -= up;

            if (move != Vector3.Zero)
                transform.Position += Vector3.Normalize(move) * MoveSpeed * deltaTime;

            var yawRot = Quaternion.CreateFromAxisAngle(Vector3.UnitY, _yaw);
            var pitchRot = Quaternion.CreateFromAxisAngle(Vector3.UnitX, _pitch);

            transform.Rotation = Quaternion.Normalize(pitchRot * yawRot);
        }




    
    }
}
