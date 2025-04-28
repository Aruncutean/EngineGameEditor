using Core.component;
using Core.entity;
using Core.gizmo;
using Core.graphics.shader;
using Core.graphics.texture;
using Core.scene;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Texture = Core.graphics.texture.Texture;

namespace Core.system
{
    public class EditorGizmoSystem
    {

        private uint _gizmoShader;

        private uint pointOfLightIcon = 0;
        private CameraSystem _cameraSystem;
        public int screenWidth { get; set; } = 800;
        public int screenHeight { get; set; } = 600;
        public Entity cameraEntity { get; set; }

        public GL gl;
        public EditorGizmoSystem(GL gl)
        {

            this.gl = gl;
            _gizmoShader = ShaderManager.Get(ShaderTypes.gizmo);
            _cameraSystem = new CameraSystem();
            var texture = new Texture();
            pointOfLightIcon = texture.LoadTexture(gl, "assets/bulb.png");

        }
        public void Render(Scene scene)
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


            foreach (var entity in scene.Entities.ToList())
            {
                var transformComponent = entity.GetComponent<TransformComponent>();
                var shaderComponent = entity.GetComponent<ShaderComponent>();
                var lightComponent = entity.GetComponent<LightComponent>();

                if (lightComponent != null && transformComponent != null && shaderComponent != null)
                {


                    float size = 0.5f;
                    float[] vertices = {
        -size, -size, 0f, 0f, 0f,
         size, -size, 0f, 1f, 0f,
         size,  size, 0f, 1f, 1f,
        -size,  size, 0f, 0f, 1f,
    };
                    uint[] indices = { 0, 1, 2, 2, 3, 0 };

                    // 🔧 Gen VAO + VBO + EBO (poți să îl reții static)
                    uint vao = gl.GenVertexArray();
                    uint vbo = gl.GenBuffer();
                    uint ebo = gl.GenBuffer();

                    gl.BindVertexArray(vao);

                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
                    gl.BufferData<float>(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

                    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
                    gl.BufferData<uint>(BufferTargetARB.ElementArrayBuffer, indices, BufferUsageARB.StaticDraw);

                    // Vertex layout
                    gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
                    gl.EnableVertexAttribArray(0);

                    gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
                    gl.EnableVertexAttribArray(1);

                    gl.BindVertexArray(0);

                    // 📐 Creează billboard matrix
                    var model = Matrix4x4.CreateBillboard(transformComponent.Position, transform.Position, camera.Up, camera.Front);

                    // 🧠 Setează shaderul și matricile
                    uint shaderProgram = ShaderManager.Get(shaderComponent.shaderType);
                    gl.UseProgram(shaderProgram);
                    var binder = new ShaderBinder(gl, new ShaderProgram(shaderProgram));
                    binder.SetMat4("uModel", model);
                    binder.SetMat4("uView", view);
                    binder.SetMat4("uProjection", projection);

                    // 💡 Poți seta și textură aici, dacă ai
                    gl.BindTexture(TextureTarget.Texture2D, pointOfLightIcon);

                    gl.Enable(GLEnum.Blend);
                    gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

                    gl.BindVertexArray(vao);
                    unsafe
                    {
                        gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, null);
                    }
                    gl.BindVertexArray(0);

                    //float size = 0.25f;
                    //GizmoPrimitives.DrawLine(gl, new Vector3(transformComponent.Position.X, 0f, transformComponent.Position.Z), transformComponent.Position, lightComponent.Color);
                    //GizmoPrimitives.DrawCircle(gl, transformComponent.Position, size, new Vector3(1, 1, 0));
                }
            }
        }
    }
}
