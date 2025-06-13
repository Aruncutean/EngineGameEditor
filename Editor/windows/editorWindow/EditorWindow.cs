using Silk.NET.Windowing;
using System;
using ImGuiNET;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Editor.component;
using Editor.service;
using Core.models;
using System.Numerics;
using EditorAvalonia.service;
using Core.services;
using Core.graphics.material;
using Core.IO;
using Editor.entity;
using Core.process;
using Silk.NET.OpenGL.Extensions.ImGui;
using ImGuizmoNET;
using Assimp;

namespace Editor.windows.editorWindow
{
    public class EditorWindow
    {
        IInputContext _input;
        GL? gl = null;
        IWindow window;
        ImGuiController? controller = null;

        AssetsView assetsView = null;
        SceneView sceneView = null;
        public void Run()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(1280, 720);
            options.Title = "Silk.NET + ImGui Example";

            window = Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Resize += OnResize;
            window.Update += OnUpdate;


            window.Run();
        }

        private void OnUpdate(double delta)
        {
            sceneView.Update((float)delta);
        }

        private void OnLoad()
        {
            _input = window.CreateInput();
            Input.Init(_input);
            gl = GL.GetApi(window);
            controller = new ImGuiController(gl, window, _input, null, () =>
            {

                ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
                ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
                ImGui.GetIO().ConfigViewportsNoAutoMerge = true;
                ImGui.GetIO().ConfigViewportsNoTaskBarIcon = true;
            });

            assetsView = new AssetsView(gl);
            assetsView.Init();

            sceneView = new SceneView(gl);
            sceneView.Init();
        }

        private void OnRender(double delta)
        {
            gl!.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            gl!.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            controller?.Update((float)delta);

            ImGui.SetCurrentContext(controller.Context);
            ImGuizmo.SetImGuiContext(controller.Context);
            ImGuizmo.SetDrawlist(ImGui.GetWindowDrawList());
            ImGuizmo.BeginFrame();


            System.Numerics.Matrix4x4 identityM = System.Numerics.Matrix4x4.Identity;

            System.Numerics.Matrix4x4 viewM = System.Numerics.Matrix4x4.CreateLookAt(
                new Vector3(0, 5, 10), // camera position
                Vector3.Zero,          // target (look at origin)
                Vector3.UnitY          // up direction
            );

            float aspect = (float)window.Size.X / window.Size.Y;
            System.Numerics.Matrix4x4 projectionM = System.Numerics.Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4f, // 45 deg FOV
                aspect,
                0.1f,
                100f
            );

            float[] view = MatrixToArray(viewM);
            float[] projection = MatrixToArray(projectionM);
            float[] model = MatrixToArray(identityM);

            float gridSize = 10.0f;
            unsafe
            {
                fixed (float* viewPtr = view)
                fixed (float* projPtr = projection)
                fixed (float* modelPtr = model)
                {
                    ImGuizmo.DrawGrid(ref *viewPtr, ref *projPtr, ref *modelPtr, gridSize);
                }
            }

            RenderDockspace();
            RenderMainMenu();
            assetsView.Run((float)delta);
            sceneView.Run((float)delta);
            RenderListEntityWindow();
            RenderPropertyWindow();

            controller?.Render();
        }

        private float[] MatrixToArray(System.Numerics.Matrix4x4 m) => new float[]
{
    m.M11, m.M12, m.M13, m.M14,
    m.M21, m.M22, m.M23, m.M24,
    m.M31, m.M32, m.M33, m.M34,
    m.M41, m.M42, m.M43, m.M44,
};

        private void OnResize(Silk.NET.Maths.Vector2D<int> size)
        {
            gl!.Viewport(0, 0, (uint)size.X, (uint)size.Y);
        }

        private void RenderDockspace()
        {
            ImGui.DockSpaceOverViewport();
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

            ImGuiWindowFlags hostWindowFlags = ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

            ImGui.Begin("DockSpaceRoot", hostWindowFlags);
            ImGui.PopStyleVar(2);

            uint dockspaceId = ImGui.GetID("MyDockspace");
            ImGui.DockSpace(dockspaceId, Vector2.Zero, ImGuiDockNodeFlags.None);
            ImGui.End();
        }

        private void RenderMainMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Project"))
                {
                    if (ImGui.MenuItem("New")) Console.WriteLine("New Project");
                    if (ImGui.MenuItem("Open")) Console.WriteLine("Open Project");
                    if (ImGui.MenuItem("Exit")) Environment.Exit(0);
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo")) Console.WriteLine("Undo");
                    if (ImGui.MenuItem("Redo")) Console.WriteLine("Redo");
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Toggle Console")) Console.WriteLine("Toggled");
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }

        private void RenderListEntityWindow()
        {
            bool isOpen = true;
            UI.Window("Entities", ref isOpen, () =>
            {
                UI.Text("Entities List");
            });
        }

        private void RenderPropertyWindow()
        {
            bool isOpen = true;
            UI.Window("Properties", ref isOpen, () =>
            {
                UI.Text("Properties of selected entity");
            });
        }

        public void Close()
        {
            _input.Dispose();
            window.Close();
        }


    }
}
