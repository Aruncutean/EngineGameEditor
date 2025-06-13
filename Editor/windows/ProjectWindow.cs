
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
using Silk.NET.OpenGL.Extensions.ImGui;



namespace Editor.windows
{
    public class ProjectWindow
    {

        IInputContext _input;


        IWindow window;
        int selectedIndex = -1;
        ProjectService projectService = new();
        List<ProjectInfo> projectInfos = new List<ProjectInfo>();
        string ProjectName = "";
        string Path = "";
        GL? gl = null;
        public void Run()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(1280, 720);
            options.Title = "Silk.NET + ImGui Example";

            window = Window.Create(options);

            ImGuiController? controller = null;

            window.Load += () =>
            {
                projectInfos = projectService.LoadProjectList();
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
                    UI.Input("ProjectName", ref ProjectName, 256);

                }
                {
                    UI.Text("Path");
                    ImGui.SameLine();
                    UI.Input("Path", ref Path, 1024, false);
                    ImGui.SameLine();
                    UI.Button("Select Path", () =>
                    {
                        UI.OpenFolderDialog("Dialog", (folder) =>
                         {
                             if (folder.Length > 0)
                             {
                                 Path = folder[0];
                             }
                         });
                    });
                }
                {
                    UI.ListBoxWithEvents("", projectInfos.Select(p => p.Name).ToArray(), ref selectedIndex, null,
                    (index, item) =>
                    {
                        DataService.Instance.ProjectInfo = projectInfos[index];
                        projectService.loadProjectData(projectInfos[index].Path);
                        WindowManager.Open(AppWindowType.Scene);
                        Close();

                    }, (i, name) =>
                    {
                        if (ImGui.MenuItem("Delete"))
                        {
                            projectService.removeProject(name);
                            projectInfos.RemoveAt(i);
                        }
                    }, 10);
                }
                {
                    UI.Button("Create", () =>
                    {
                        if (ProjectName != "" && Path != "")
                        {
                            ProjectInfo projectInfo = projectService.CreateProject(ProjectName, Path);
                            DataService.Instance.ProjectInfo = projectInfo;
                            projectService.loadProjectData(projectInfo.Path);
                            WindowManager.Open(AppWindowType.Scene);
                            Close();

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
