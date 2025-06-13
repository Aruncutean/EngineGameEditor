using Core.component;
using Core.entity;
using Core.graphics.shader;
using Core.IO;
using Core.models;
using Core.scene;
using Core.services;
using Core.system;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.IO;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Core.process;

namespace RunTime
{
    internal class Program
    {
        private static IWindow window;
        private static GL gl;
        private static bool isEditMode = false;
        private static string curentScene = string.Empty;
        private static RenderSystem renderSystem;
        private static World scene;
        private static IInputContext _input;
        private static CameraControllerSystemRunTime cameraControllerSystem;
        private static Entity _cameraEntity;

        private static WorldManager _worldSystem;
        private static ProjectData projectData;
        private static string? path = null;
        static void Main(string[] args)
        {

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--edit")
                {
                    isEditMode = true;
                }
                else
                if (args[i] == "--scene" && i + 1 < args.Length)
                {
                    curentScene = args[i + 1];
                    i++;
                }
                else if (!args[i].StartsWith("--") && Directory.Exists(args[i]))
                {
                    path = args[i];
                }
            }

            //  path ??= Directory.GetCurrentDirectory();

            Console.WriteLine($"Project path: {path}");
            Console.WriteLine($"Edit mode: {isEditMode}");
            Console.WriteLine($"Scene path: {curentScene}");

            ProjectIO projectIO = new ProjectIO();

            ProjectData projectData = projectIO.LoadProject(Path.Combine(path, "project.json"));
            DataService.Instance.ProjectData = projectData;
            if (projectData == null)
            {
                Console.WriteLine("Project data is null");
                return;
            }

            if (curentScene == string.Empty)
            {
                curentScene = projectData.MainScene;
            }

            if (curentScene == string.Empty)
            {
                Console.WriteLine("No scene selected");
                return;
            }

            SceneIO sceneIO = new SceneIO();
            scene = sceneIO.LoadScene(Path.Combine(path, "scenes", curentScene));

            if (scene == null)
            {
                Console.WriteLine("Scene data is null");
                return;
            }

            if (scene != null)
            {
                Console.WriteLine($"Scene name: {scene.Name}");
                Console.WriteLine($"Scene path: {scene.Path}");
                Console.WriteLine($"Scene created at: {scene.CreatedAt}");
                Console.WriteLine($"Scene last updated at: {scene.LastUpdated}");
            }

            var options = WindowOptions.Default with
            {
                Size = new Silk.NET.Maths.Vector2D<int>(800, 600),
                Title = "Silk.NET Window"
            };

            window = Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Closing += OnClose;
            window.Resize += OnResize;

            window.Run();
        }

        private static void OnLoad()
        {
            gl = GL.GetApi(window);
            _input = window.CreateInput();
            Input.Init(_input);
            WindowsService.Instance.Width = 800;
            WindowsService.Instance.Height = 600;

            _worldSystem = new WorldManager(projectData);
            _worldSystem.isEditMode = false;
            _worldSystem.renderInFrameBuffer = false;
            _worldSystem.Init(gl);

            if (path != null)
            {
                _worldSystem.LoadWorld(Path.Combine(path, "scenes", curentScene));
            }


        }

        private static void OnRender(double delta)
        {

            _worldSystem.Render((float)delta);
        }

        private static void OnUpdate(double delta)
        {
            if (_worldSystem != null)
            {
                _worldSystem.Update((float)delta);
            }
        }


        private static void OnResize(Vector2D<int> newSize)
        {
            Console.WriteLine($"New window size: {newSize.X} x {newSize.Y}");
            WindowsService.Instance.Width = newSize.X;
            WindowsService.Instance.Height = newSize.Y;

            _worldSystem.Resize(newSize.X, newSize.Y);

        }


        private static void OnClose()
        {
            Console.WriteLine("Window is closing.");
        }
    }
}
