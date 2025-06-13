using Core.component;
using Core.entity;
using Core.process;
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
        private Vector2 _lastMousePos;
        private bool _firstMove = true;
        private float _yaw = -90f;
        private float _pitch = 0f;

        private bool mousePresssBool = false;
        public float MoveSpeed { get; set; } = 5f;
        public float LookSensitivity { get; set; } = 0.002f;

        private CameraComponent cameraComponent;
        private TransformComponent transform;

        public CameraControllerSystem(Entity cameraEntity)
        {
            transform = cameraEntity.GetComponent<TransformComponent>();
            cameraComponent = cameraEntity.GetComponent<CameraComponent>();


            Input.MouseClicked += (MouseButton1) =>
            {
                if (MouseButton1 == MouseButton.Left)
                {
                    mousePresssBool = true;
                }

            };

            Input.MouseReleased += (MouseButton1) =>
            {
                if (MouseButton1 == MouseButton.Left)
                {
                    mousePresssBool = false;
                }
            };

            Input.MouseMove += (Vector2 newPos) =>
            {
                OnMouseMove(newPos);
            };
        }


        public void mousePresss(bool mouseButtonPressed)
        {
            mousePresssBool = mouseButtonPressed;
        }

        public void OnMouseMove(Vector2 newPos)
        {
            if (_firstMove)
            {
                _lastMousePos = newPos;
                _firstMove = false;
            }

            float xoffset = newPos.X - _lastMousePos.X;
            float yoffset = _lastMousePos.Y - newPos.Y;
            _lastMousePos = newPos;

            if (mousePresssBool)
            {
                float sensitivity = 0.1f;
                xoffset *= sensitivity;
                yoffset *= sensitivity;

                _yaw += xoffset;
                _pitch += yoffset;

                if (_pitch > 89.0f)
                    _pitch = 89.0f;
                if (_pitch < -89.0f)
                    _pitch = -89.0f;

                float yawRad = MathF.PI / 180f * _yaw;
                float pitchRad = MathF.PI / 180f * _pitch;

                Vector3 front = new Vector3
                {
                    X = MathF.Cos(yawRad) * MathF.Cos(pitchRad),
                    Y = MathF.Sin(pitchRad),
                    Z = MathF.Sin(yawRad) * MathF.Cos(pitchRad)
                };

                cameraComponent.Front = Vector3.Normalize(front);
            }
        }

        public void Update(float deltaTime)
        {
            var cameraSeed = MoveSpeed * deltaTime;
            if (Input.KeyW == true)
            {
                transform.Position += cameraSeed * cameraComponent.Front;
            }
            if (Input.KeyS == true)
            {
                transform.Position -= cameraSeed * cameraComponent.Front;
            }
            if (Input.KeyA == true)
            {
                transform.Position -= Vector3.Normalize(Vector3.Cross(cameraComponent.Front, cameraComponent.Up)) * cameraSeed;
            }
            if (Input.KeyD == true)
            {
                transform.Position += Vector3.Normalize(Vector3.Cross(cameraComponent.Front, cameraComponent.Up)) * cameraSeed;
            }
        }
    }
}
