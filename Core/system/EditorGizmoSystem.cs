using Core.component;
using Core.entity;
using Core.gizmo;
using Core.graphics.light;
using Core.graphics.shader;
using Core.process;
using Core.scene;
using Core.services;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Texture = Core.graphics.material.texture.Texture;

namespace Core.system
{
    public class EditorGizmoSystem
    {
        private uint pointOfLightIcon = 0;
        private uint directLightIcon = 0;
        private uint spotLightIcon = 0;

        private CameraSystem _cameraSystem;

        public Entity cameraEntity { get; set; }

        public GL gl;
        public EditorGizmoSystem(GL gl)
        {
            this.gl = gl;

            _cameraSystem = new CameraSystem();
            var texture = new Texture();
            pointOfLightIcon = texture.LoadTexture(gl, "assets/bulb.png");
            directLightIcon = texture.LoadTexture(gl, "assets/sun.png");
            spotLightIcon = texture.LoadTexture(gl, "assets/spotlight.png");
        }
        public void Render(World scene)
        {

            if (cameraEntity == null) return;

            var camera = cameraEntity.GetComponent<CameraComponent>();
            var transform = cameraEntity.GetComponent<TransformComponent>();

            if (transform == null || camera == null)
            {
                return;
            }
            var view = _cameraSystem.GetViewMatrix(transform, camera);
            var projection = _cameraSystem.GetProjectionMatrix(camera, (float)WindowsService.Instance.Width / WindowsService.Instance.Height);

            LightProcessor lightProcessor = new LightProcessor();

            List<Entity> entities = lightProcessor.GetLights(scene);

            foreach (var entity in entities)
            {
                var transformComponent = entity.GetComponent<TransformComponent>();
                var lightComponent = entity.GetComponent<LightComponent>();

                if (lightComponent != null && transformComponent != null)
                {


                    float size = 0.5f;
                    float[] vertices = {
        -size, -size, 0f, 0f, 0f,
         size, -size, 0f, 1f, 0f,
         size,  size, 0f, 1f, 1f,
        -size,  size, 0f, 0f, 1f,
    };
                    uint[] indices = { 0, 1, 2, 2, 3, 0 };

                    uint vao = gl.GenVertexArray();
                    uint vbo = gl.GenBuffer();
                    uint ebo = gl.GenBuffer();

                    gl.BindVertexArray(vao);

                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
                    gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

                    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
                    gl.BufferData<uint>(BufferTargetARB.ElementArrayBuffer, indices, BufferUsageARB.StaticDraw);

                    gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
                    gl.EnableVertexAttribArray(0);

                    gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
                    gl.EnableVertexAttribArray(1);

                    gl.BindVertexArray(0);

                    var model = Matrix4x4.CreateBillboard(transformComponent.Position, transform.Position, camera.Up, camera.Front);

                    uint shaderProgram = ShaderManager.Get(ShaderTypes.gizmo);
                    gl.UseProgram(shaderProgram);
                    var binder = new ShaderBinder(gl, new ShaderProgram(shaderProgram));
                    binder.SetMat4("uModel", model);
                    binder.SetMat4("uView", view);
                    binder.SetMat4("uProjection", projection);


                    gl.ActiveTexture(TextureUnit.Texture0);


                    if (lightComponent.Type == LightType.Direct)
                    {
                        gl.BindTexture(TextureTarget.Texture2D, directLightIcon);
                    }
                    else if (lightComponent.Type == LightType.Spot)
                    {
                        gl.BindTexture(TextureTarget.Texture2D, spotLightIcon);
                    }
                    else
                    {
                        gl.BindTexture(TextureTarget.Texture2D, pointOfLightIcon);
                    }


                    gl.Enable(GLEnum.Blend);
                    gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

                    gl.BindVertexArray(vao);
                    unsafe
                    {
                        gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);
                    }
                    gl.BindVertexArray(0);
                }
            }
        }
    }
}
