using System.Numerics;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;
using NativeFileDialogs;
using NativeFileDialogs.Net;
using Editor.UI;
using Editor.ui;
using Silk.NET.Maths;
using System.Drawing;

namespace Editor
{
    class Program
    {
        private static IWindow _window = null!;
        private static GL _gl = null!;
        private static ImGuiController _imguiController = null!;
        private static MainWindow mainWindow;
        private static ProjectList projectList;
        private static uint _vao, _vbo, _shaderProgram;

        static void Main()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(1280, 720);
            options.Title = "ImGui.NET + Silk.NET Demo";
            
            _window = Window.Create(options);
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.FramebufferResize += OnFramebufferResize;

            _window.Run();
        }

        private static void OnLoad()
        {
            _gl = GL.GetApi(_window);
            var input = _window.CreateInput();
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
     
            _imguiController = new ImGuiController(_gl, _window, input);
            ImGui.LoadIniSettingsFromDisk("imgui_layout.ini");
            mainWindow = new MainWindow();
            projectList = new ProjectList();

            float[] vertices =
            {
            // X, Y
             0.0f,  0.5f,
            -0.5f, -0.5f,
             0.5f, -0.5f
        };

            // VAO
            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            // VBO
            _vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            unsafe
            {
                fixed (float* v = vertices)
                {
                    _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
                }
            }

            // Shaders
            string vertexShaderSource = @"
#version 330 core
layout(location = 0) in vec2 aPos;
void main()
{
    gl_Position = vec4(aPos, 0.0, 1.0);
}";
            string fragmentShaderSource = @"
#version 330 core
out vec4 FragColor;
void main()
{
    FragColor = vec4(1.0, 0.2, 0.2, 1.0);
}";

            uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vertexShader, vertexShaderSource);
            _gl.CompileShader(vertexShader);

            uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fragmentShader, fragmentShaderSource);
            _gl.CompileShader(fragmentShader);

            _shaderProgram = _gl.CreateProgram();
            _gl.AttachShader(_shaderProgram, vertexShader);
            _gl.AttachShader(_shaderProgram, fragmentShader);
            _gl.LinkProgram(_shaderProgram);

            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);

            _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            _gl.EnableVertexAttribArray(0);
        }

        private static void OnUpdate(double deltaTime)
        {
            _imguiController.Update((float)deltaTime);
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            _gl.Viewport(0, 0, (uint)newSize.X, (uint)newSize.Y);
        }

        private static void OnRender(double deltaTime)
        {

          

            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
         
            _gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            mainWindow.draw();
          //  projectList.draw();
            _gl.UseProgram(_shaderProgram);
            _gl.BindVertexArray(_vao);
            _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);


            // === ImGui render ===
            _imguiController.Render();
            ImGui.SaveIniSettingsToDisk("imgui_layout.ini");
        }
    }

    }


