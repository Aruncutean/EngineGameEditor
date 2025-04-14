using Core.component;
using Core.entity;
using Core.graphics.shader;
using Core.IO;
using Core.models;
using Core.scene;
using Core.system;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.IO;
using System.Numerics;

namespace RunTime
{
    internal class Program
    {
        private static IWindow window;
        private static GL gl;
        private static bool isEditMode = false;
        private static string curentScene = string.Empty;
        private static RenderSystem renderSystem;
        private static Scene scene;
        private static IInputContext _input;
        private static CameraControllerSystemRunTime cameraControllerSystem;
        private static Entity _cameraEntity;
        static void Main(string[] args)
        {
            string? path = null;

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

            path ??= Directory.GetCurrentDirectory();

            Console.WriteLine($"Project path: {path}");
            Console.WriteLine($"Edit mode: {isEditMode}");
            Console.WriteLine($"Scene path: {curentScene}");

            ProjectIO projectIO = new ProjectIO();

            ProjectData projectData = projectIO.LoadProject(Path.Combine(path, "project.json"));

            if (projectData == null)
            {
                Console.WriteLine("Project data is null");
                return;
            }

            if(curentScene == string.Empty)
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

            if(scene == null)
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

            window.Run();
        }

        private static void OnLoad()
        {
            gl = GL.GetApi(window);

            _input = window.CreateInput();
            IMouse mouse = _input.Mice[0];
          
            mouse.Cursor.CursorMode = CursorMode.Disabled;

            ShaderManager.Init(gl);
            ShaderManager.Load("basic", "shader/basic.vert.glsl", "shader/basic.frag.glsl");

            gl.ClearColor(0.1f, 0.1f, 0.3f, 1.0f);

            _cameraEntity = new Entity();
            _cameraEntity.AddComponent(new TransformComponent
            {
                Position = new Vector3(0, 0, 5),
                Rotation = System.Numerics.Quaternion.Identity
            });
            _cameraEntity.AddComponent(new CameraComponent { IsMainCamera = true });
            _cameraEntity.AddComponent(new CameraControllerComponent { MoveSpeed = 5f });

            renderSystem = new RenderSystem();
            cameraControllerSystem = new CameraControllerSystemRunTime(_input);
            renderSystem.cameraEntity = _cameraEntity;
        }

        private static void OnRender(double delta)
        {
            gl.Clear((uint)(ClearBufferMask.ColorBufferBit));

           gl.ClearColor(0.2f, 0.2f, 0.4f, 1.0f);
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            if (scene != null)
            {
                renderSystem.Render(scene, gl);
            }
        }

        private static void OnUpdate(double delta)
        {
            if (cameraControllerSystem != null)
            {
                if (scene != null)
                {
                    cameraControllerSystem.Update((float)delta, _cameraEntity);
                }
            }
        }

        private static void OnClose()
        {
            Console.WriteLine("🟡 Window is closing.");
        }
    }
}
