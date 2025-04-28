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
            var view = _cameraSystem.GetViewMatrix(transform, camera);
            var projection = _cameraSystem.GetProjectionMatrix(camera, (float)screenWidth / screenHeight);
            var lights = scene.Entities
         .Where(e => e.HasComponent<LightComponent>())
         .ToList();

            var renderables = scene.Entities
      .Where(e => e.HasComponent<MeshComponent>() && e.HasComponent<ShaderComponent>())
      .Where(e => !e.HasComponent<LightComponent>()) // ← foarte important
      .ToList();

            foreach (var entity in renderables)
            {
                var transformComponent = entity.GetComponent<TransformComponent>();
                var meshComponent = entity.GetComponent<MeshComponent>();
                var shaderComponent = entity.GetComponent<ShaderComponent>();

                ShaderProgram shaderProgram = null;
                ShaderBinder binder = null;

                if (shaderComponent != null)
                {
                    shaderProgram = new ShaderProgram(ShaderManager.Get(shaderComponent.shaderType));
                    shaderProgram.Use(gl);
                    binder = new ShaderBinder(gl, shaderProgram);
                }

                if (shaderProgram != null
                    && transformComponent != null
                    && shaderComponent != null
                    && shaderComponent.shaderType != ShaderTypes.Basic)
                {
                    foreach (var light in lights)
                    {
                        var lightComp = light.GetComponent<LightComponent>();
                        var lightTransform = light.GetComponent<TransformComponent>();

                        binder.SetVec3("lightPos", lightTransform.Position);
                        binder.SetVec3("viewPos", transform.Position);
                        binder.SetVec3("lightColor", new Vector3(1f, 1f, 1f));
                        binder.SetVec3("objectColor", new Vector3(1f, 0.5f, 0.3f));
                    }
                }

                if (transformComponent != null && shaderComponent != null && shaderProgram != null)

                    if (meshComponent != null && transformComponent != null && shaderComponent != null)
                    {
                        Matrix4x4 model = Matrix4x4.Identity;
                        model = Matrix4x4.CreateTranslation(transformComponent.Position);

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
