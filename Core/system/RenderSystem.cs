using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Core.component;
using Core.entity;
using Core.graphics.mesh;
using Core.graphics.shader;
using Core.process;
using Core.scene;
using Core.services;
using Silk.NET.OpenGL;

namespace Core.system
{
    public class RenderSystem
    {
        private readonly CameraSystem _cameraSystem;
        private readonly LightProcessor _lightProcessor;
        private readonly RenderableProcessor _renderableProcessor;

        public Entity CameraEntity { get; set; }

        public RenderSystem()
        {
            _cameraSystem = new CameraSystem();
            _lightProcessor = new LightProcessor();
            _renderableProcessor = new RenderableProcessor();
        }

        public void Render(World scene, GL gl)
        {
            if (CameraEntity == null) return;

            var camera = CameraEntity.GetComponent<CameraComponent>();
            var transform = CameraEntity.GetComponent<TransformComponent>();

            if (camera == null || transform == null) return;

            var view = _cameraSystem.GetViewMatrix(transform, camera);
            var projection = _cameraSystem.GetProjectionMatrix(camera,
                (float)WindowsService.Instance.Width / WindowsService.Instance.Height);

            var lights = _lightProcessor.GetLights(scene);
            var renderables = _renderableProcessor.GetRenderables(scene);
            Console.WriteLine(renderables.Count);
            foreach (var entity in renderables)
            {
                _renderableProcessor.RenderEntity(entity, lights, view, projection, gl, transform);
            }

        }
    }
}
