using Editor.component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.OpenGL;
using Core.system;
using Core.IO;
using Core.services;
using ImGuiNET;
using System.Numerics;
using System;
using System.Numerics;
using ImGuiNET;

using Core.process;
using System.Diagnostics;
using Core.component;
using ImGuizmoNET;


namespace Editor.windows.editorWindow
{
    public class SceneView : IView
    {
        public GL? gl = null;

        private WorldManager _worldSystem;
        private int framebufferWidth;
        private int framebufferHeight;

        public SceneView(GL? gl)
        {
            this.gl = gl;
        }
        public void Init()
        {
            var scenePath = Path.Combine(DataService.Instance.ProjectInfo.Path, "scenes", DataService.Instance.CurrentScene.Path);
            var json = File.ReadAllText(Path.Combine(DataService.Instance.ProjectInfo.Path, scenePath));
            SceneIO sceneIO = new SceneIO();
            DataService.Instance.Scene = sceneIO.LoadScene(scenePath);



            _worldSystem = new WorldManager(DataService.Instance.ProjectData);
            _worldSystem.isEditMode = true;
            _worldSystem.renderInFrameBuffer = true;
            _worldSystem.Init(gl);
            _worldSystem.LoadWorld(DataService.Instance.Scene);

        }

        public void Update(float deltaTime)
        {
            if (_worldSystem != null)
            {
                _worldSystem.Update((float)deltaTime);
            }
        }

        public void Run(float deltaTime)
        {

            if (_worldSystem != null || gl != null)
            {

                _worldSystem.Render((float)deltaTime);
            }
            bool isOpen = true;
            UI.Window("Scenes", ref isOpen, () =>
            {
                // trebuie să fie în fereastră validă



                Vector2 size = ImGui.GetContentRegionAvail();
                if ((int)size.X != framebufferWidth || (int)size.Y != framebufferHeight)
                {

                    WindowsService.Instance.Height = (int)size.Y;
                    WindowsService.Instance.Width = (int)size.X;
                    framebufferHeight = (int)size.Y;
                    framebufferWidth = (int)size.X;
                    _worldSystem.Resize((int)size.X, (int)size.Y);
                }

                bool isHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup | ImGuiHoveredFlags.AllowWhenBlockedByActiveItem);
                if (isHovered)
                {
                    Input.isActive = true;
                }
                else
                {
                    Input.isActive = false;
                }

                _worldSystem._frameBuffer.BlitToTexture();

                if (_worldSystem._frameBuffer != null)
                {
                    ImGui.Image((IntPtr)_worldSystem._frameBuffer.ColorTexture, size);
                }

                Vector2 available = ImGui.GetContentRegionAvail();
                ImGui.Image((IntPtr)_worldSystem._frameBuffer.ColorTexture, available);



                // Setează zona de desen (de obicei, dimensiunea ferestrei sau viewport-ului)
                var windowPos = ImGui.GetWindowPos();
                var windowSize = ImGui.GetWindowSize();


                //var camera = _worldSystem._cameraEntity.GetComponent<CameraComponent>();
                //var transform = _worldSystem._cameraEntity.GetComponent<TransformComponent>();

                //Matrix4x4 modelM = Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));
                //float[] model = MatrixToArray(modelM);

                //CameraSystem _cameraSystem = new CameraSystem();
                //var viewM = _cameraSystem.GetViewMatrix(transform, camera);
                //var projectionM = _cameraSystem.GetProjectionMatrix(camera, (float)WindowsService.Instance.Width / WindowsService.Instance.Height);

                //float[] view = MatrixToArray(viewM);
                //float[] projection = MatrixToArray(projectionM);

                //unsafe

                //{
                //    fixed (float* viewPtr = view)
                //    fixed (float* projPtr = projection)
                //    fixed (float* modelPtr = model)
                //    {
                //        ImGuizmo.Manipulate(
                //            ref *viewPtr,
                //            ref *projPtr,
                //            OPERATION.TRANSLATE,
                //            MODE.LOCAL,
                //            ref *modelPtr
                //        );
                //    }
                //}
            });
        }

        private float[] MatrixToArray(Matrix4x4 m) => new float[]
{
    m.M11, m.M12, m.M13, m.M14,
    m.M21, m.M22, m.M23, m.M24,
    m.M31, m.M32, m.M33, m.M34,
    m.M41, m.M42, m.M43, m.M44,
};
    }
}
