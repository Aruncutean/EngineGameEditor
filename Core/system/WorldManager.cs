using Core.component;
using Core.entity;
using Core.graphics.framebuffer;
using Core.graphics.mesh;
using Core.graphics.shader;
using Core.IO;
using Core.models;
using Core.scene;
using Core.services;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.system
{
    public class WorldManager
    {
        private World _currentWorld;
        private RenderSystem _renderSystem = new();
        private ProjectData _projectData;
        public Entity _cameraEntity;

        private EditorGizmoSystem _editorGizmoSystem;
        public CameraControllerSystem _cameraControllerSystem;
        public bool isEditMode = false;
        public bool renderInFrameBuffer = false;

        private GL gl;
        public FrameBuffer _frameBuffer;

        public WorldManager(ProjectData projectData)
        {
            this._projectData = projectData;
        }

        public void Init(GL gl)
        {
            this.gl = gl;

            ShaderManager.LoadShaders(gl);
            SceneIO sceneIO = new SceneIO();

            _cameraEntity = new Entity();
            _cameraEntity.AddComponent(new TransformComponent
            {
                Position = new Vector3(0, 0, 5),

            });
            _cameraEntity.AddComponent(new CameraComponent { IsMainCamera = true });
            _cameraEntity.AddComponent(new CameraControllerComponent { MoveSpeed = 5f });

            _renderSystem.CameraEntity = _cameraEntity;

            _editorGizmoSystem = new EditorGizmoSystem(gl);
            _editorGizmoSystem.cameraEntity = _cameraEntity;

            _cameraControllerSystem = new CameraControllerSystem(_cameraEntity);

            _frameBuffer = new FrameBuffer(gl);
            _frameBuffer.resize(WindowsService.Instance.Width, WindowsService.Instance.Height);

            gl.Enable(GLEnum.DepthTest);
            gl.Enable(GLEnum.Multisample);
            gl.Enable(GLEnum.Blend);
            gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

        }

        public void Resize(int width, int height)
        {
            _frameBuffer.resize(WindowsService.Instance.Width, WindowsService.Instance.Height);
            gl.Viewport(0, 0, (uint)width, (uint)height);
        }

        public void LoadWorld(SceneInfo sceneInfo)
        {
            var scenePath = Path.Combine(_projectData.Path, "scenes", sceneInfo.Path);
            _currentWorld = SceneSerializer.LoadScene(scenePath);
        }


        public void LoadWorld(string path)
        {
            SceneIO sceneIO = new SceneIO();
            _currentWorld = sceneIO.LoadScene(path);
        }

        public void LoadWorld(World world)
        {
            _currentWorld = world;
        }

        public void Update(float deltaTime)
        {
            if (_cameraControllerSystem != null)
            {
                if (_currentWorld != null)
                {
                    _cameraControllerSystem.Update(deltaTime);
                }
            }
        }

        public void Render(float deltaTime)
        {
            if (renderInFrameBuffer == true)
            {
                _frameBuffer.render(() =>
               {

                   gl?.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                   gl?.ClearColor(0.247f, 0.247f, 0.247f, 1.0f);

                   if (_currentWorld != null)
                   {
                       if (isEditMode == true)
                       {
                           _editorGizmoSystem.Render(_currentWorld);
                       }
                       _renderSystem.Render(_currentWorld, gl);
                   }
               });
            }
            else
            {
                gl?.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                gl?.ClearColor(0.247f, 0.247f, 0.247f, 1.0f);

                if (_currentWorld != null)
                {

                    _editorGizmoSystem.Render(_currentWorld);

                    _renderSystem.Render(_currentWorld, gl);
                }
            }
        }

        public void dispone()
        {

        }
    }
}
