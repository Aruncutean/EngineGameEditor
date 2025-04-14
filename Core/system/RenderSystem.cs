using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Core.component;
using Core.entity;
using Core.graphics.shader;
using Core.scene;
using Silk.NET.OpenGL;

namespace Core.system
{
    public class RenderSystem
    {

        private readonly CameraSystem _cameraSystem;

        public Entity cameraEntity { get; set; }
        public int screenWidth { get; set; } = 800;
        public int screenHeight { get; set; } = 600;

        public RenderSystem()
        {
            _cameraSystem = new CameraSystem();
        }
        public void Render(Scene scene, GL gl)
        {

            if (cameraEntity == null) return;

            var camera = cameraEntity.GetComponent<CameraComponent>();
            var transform = cameraEntity.GetComponent<TransformComponent>();

            if (transform == null || camera == null)
            {
                return;
            }
            var view = _cameraSystem.GetViewMatrix(transform);
            var projection = _cameraSystem.GetProjectionMatrix(camera, (float)screenWidth / screenHeight);


            foreach (var entity in scene.Entities)
            {
                var transformComponent = entity.GetComponent<TransformComponent>();
                var meshComponent = entity.GetComponent<MeshComponent>();
                var shaderComponent = entity.GetComponent<ShaderComponent>();



                if (meshComponent != null && transformComponent != null && shaderComponent != null)
                {
                    uint shaderProgram = ShaderManager.Get(shaderComponent.shaderType);
                    gl.UseProgram(shaderProgram);

                    Matrix4x4 model = Matrix4x4.Identity;
                    model = Matrix4x4.CreateTranslation(transformComponent.Position);

                    var binder = new ShaderBinder(gl, new ShaderProgram(shaderProgram));
                    binder.SetMat4("model", model);
                    binder.SetMat4("view", view);
                    binder.SetMat4("projection", projection);


                    if (meshComponent.gLMesh == null)
                    {
                        meshComponent.LoadMesh(gl);
                    }
                    else
                    {
                        meshComponent.gLMesh.Render();
                    }

                }

            }
        }
    }
}
