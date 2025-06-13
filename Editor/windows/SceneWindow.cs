
using Silk.NET.Windowing;
using System;
using ImGuiNET;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Windowing;

using Editor.component;
using Editor.service;
using Core.models;
using Core.services;
using Core.IO;
using Core.scene;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace Editor.windows
{
    public class SceneWindow
    {
        IInputContext _input;

        IWindow window;
        int selectedIndex = -1;

        private SceneService sceneService = new SceneService();

        string ScenName = string.Empty;
        public void Run()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(1280, 720);
            options.Title = "Silk.NET + ImGui Example";

            window = Window.Create(options);
            GL? gl = null;
            ImGuiController? controller = null;

            window.Load += () =>
            {
                _input = window.CreateInput();
                gl = GL.GetApi(window);
                controller = new ImGuiController(gl, window, _input);



            };


            window.Render += delta =>
            {
                gl!.ClearColor(0.1f, 0.1f, 0.1f, 1f);
                gl!.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                controller?.Update((float)delta);

                ImGuiViewportPtr viewport = ImGui.GetMainViewport();

                ImGui.SetNextWindowPos(viewport.Pos);
                ImGui.SetNextWindowSize(viewport.Size);
                ImGui.SetNextWindowViewport(viewport.ID);

                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
                ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

                ImGuiWindowFlags flags =
                    ImGuiWindowFlags.NoTitleBar |
                    ImGuiWindowFlags.NoResize |
                    ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoBringToFrontOnFocus |
                    ImGuiWindowFlags.NoNavFocus;

                ImGui.Begin("FullScreenWindow", flags);



                ImGui.PopStyleVar(2);
                {
                    UI.Text("Name");
                    ImGui.SameLine();
                    UI.Input("ProjectName", ref ScenName, 256);

                }
                {
                    UI.ListBoxWithEvents("", sceneService.Scenes.Select(p => p.Name).ToArray(), ref selectedIndex, null,
                    (index, item) =>
                    {
                        DataService.Instance.CurrentScene = sceneService.Scenes[index];
                        WindowManager.Open(AppWindowType.Editor);
                        Close();

                    }, (i, name) =>
                    {
                        if (ImGui.MenuItem("Delete"))
                        {

                        }
                    }, 10);
                }
                {
                    UI.Button("Create", () =>
                    {
                        if (ScenName != string.Empty)
                        {

                            var now = DateTime.Now;
                            SceneInfo info = new SceneInfo
                            {
                                Name = ScenName,
                                Path = ScenName + "_scene.json",
                                CreatedAt = now,
                                LastUpdated = now,

                            };
                            var scenes = sceneService.Scenes;

                            if (scenes != null)
                            {
                                bool nameExists = scenes.Any(scene => scene.Name.Equals(info.Name, StringComparison.OrdinalIgnoreCase));


                                sceneService.addSceneInfo(info);
                                World scene = new World
                                {
                                    Name = info.Name,
                                    Path = info.Path,
                                    CreatedAt = info.CreatedAt,
                                    LastUpdated = info.CreatedAt,
                                };

                                var scenePath = Path.Combine(DataService.Instance.ProjectInfo.Path, "scenes", info.Path);
                                SceneIO sceneIO = new SceneIO();
                                sceneIO.SaveScene(scenePath, scene);

                            }
                        }
                    });
                }

                ImGui.End();
                controller?.Render();
            };

            window.Resize += (size) =>
            {
                gl!.Viewport(0, 0, (uint)size.X, (uint)size.Y);
            };

            window.Run();
        }
        public void Close()
        {
            _input.Dispose();
            window.Close();
        }
    }
}
