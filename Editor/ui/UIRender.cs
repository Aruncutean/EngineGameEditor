using Editor.ui;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UI
{
    public class UIRender
    {
        DockspaceRoot dock;
        private Dictionary<string, Delegate> bindings;
        uint dockspaceID;
        public UIRender()
        {
            init();
        }

        private void init()
        {
            dock = JsonConvert.DeserializeObject<DockspaceRoot>(File.ReadAllText("view/viewMain.json"));
            var view = new Win1();

            bindings = new Dictionary<string, Delegate>{
                { "HandleNameChange", (Action<string>)view.HandleNameChange },
                { "HandleApply", (Action)view.HandleApply }
            };

        }

        public void draw()
        {

            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

            ImGuiWindowFlags dockspaceFlags =
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoNavFocus |
                ImGuiWindowFlags.MenuBar;

            ImGui.Begin("DockSpace_Main", dockspaceFlags);
            ImGui.PopStyleVar(3);

            dockspaceID = ImGui.GetID(dock.Id);
            ImGui.DockSpace(dockspaceID, Vector2.Zero, ImGuiDockNodeFlags.None);

            // Afișează toate ferestrele din dockspace
            foreach (var view in dock.Windows)
            {
                var json = File.ReadAllText(Path.Combine("view", view.Include));
                var element = JsonConvert.DeserializeObject<UIElement>(json);
                DrawElement(element);
            }

            ImGui.End();


        }

        void DrawElement(UIElement element)
        {
            switch (element.Type)
            {
                case "window":

                    Vector2 size = element.Size is { Count: 2 }
                        ? new Vector2(element.Size[0], element.Size[1])
                        : new Vector2(300, 400);

                    
                    ImGui.SetNextWindowDockID(dockspaceID, ImGuiCond.FirstUseEver);
                    ImGui.SetNextWindowSize(size, ImGuiCond.FirstUseEver);
                    ImGui.Begin(element.Title ?? "Untitled");

                    if (element.Children != null)
                        foreach (var child in element.Children)
                            DrawElement(child);

                    ImGui.End();
                    break;

                case "label":
                    ImGui.Text(element.Text ?? "Label");
                    break;


                case "input":

                    string value = element.Value ?? "";
                    byte[] buffer = new byte[256];
                    Encoding.UTF8.GetBytes(value, buffer);

                    if (ImGui.InputText(element.Label ?? "Input", buffer, (uint)buffer.Length))
                    {
                        string newValue = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        element.Value = newValue;

                        if (!string.IsNullOrEmpty(element.OnChange) &&
                            bindings.TryGetValue(element.OnChange, out var action))
                        {
                            (action as Action<string>)?.Invoke(newValue);
                        }
                    }
                    break;



                case "button":
                    if (ImGui.Button(element.Text ?? "Button"))
                    {
                        if (!string.IsNullOrEmpty(element.OnClick) && bindings.TryGetValue(element.OnClick, out var action))
                        {
                            (action as Action)?.Invoke();
                        }
                    }
                    break;
            }
        }


    }
}
