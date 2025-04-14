using Editor.data;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Editor.ui
{
    public class ProjectList
    {

        public ProjectList() { }


        private void init()
        {

        }

        public void draw()
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);

            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoResize |
                                     ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.NoCollapse |
                                     ImGuiWindowFlags.NoBringToFrontOnFocus |
                                     ImGuiWindowFlags.NoNavFocus;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f)); // culoare solidă

            ImGui.Begin("MainFullWindow", flags);
            ImGui.PopStyleVar(3);

            ImGui.Text("Enter name:");
            ImGui.SameLine();
            string name = "Default";
            byte[] buffer = new byte[256];
            Encoding.UTF8.GetBytes(name, 0, name.Length, buffer, 0);

            if (ImGui.InputText("##NameInput", buffer, (uint)buffer.Length))
            {
                name = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            }
            ImGui.SameLine();
           
            if (ImGui.Button("Submit"))
            {
                Console.WriteLine($"Submitted: {name}");
            }

            int selectedId = -1;
            ImGui.Separator();
            ImGui.Text("Items:");
            ImGui.BeginChild("List", new Vector2(0, -50)); 
            for (int i = 0; i < 10; i++)
            {
                bool selected = selectedId == i;

                if (ImGui.Selectable($"Object {i}", selected))
                {
                    selectedId = i;
                }
                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    Console.WriteLine($"CCCC {i}");
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup($"popup_{i}");
                }

                if (ImGui.BeginPopup($"popup_{i}"))
                {
                    ImGui.Text($"Object {i}");
                    ImGui.Separator();
                    if (ImGui.MenuItem("Rename"))
                    {
                        Console.WriteLine($"Rename object {i}");
                    }
                    if (ImGui.MenuItem("Delete"))
                    {
                        Console.WriteLine($"Delete object {i}");
                    }
                    ImGui.EndPopup();
                }
            }

            ImGui.EndChild();

            float windowWidth = ImGui.GetWindowSize().X;
            float buttonWidth = 200;

            ImGui.SetCursorPosX(windowWidth - buttonWidth - 10);
        
            if (ImGui.Button("➕ Create", new Vector2(200, 40)))
            {
                Console.WriteLine("Create clicked!");
            }

            ImGui.End();
            ImGui.PopStyleColor();
        }


        public static void SaveProject(Project project, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true 
            };

            string json = JsonSerializer.Serialize(project, options);
            File.WriteAllText(filePath, json);
        }

        public static Project LoadProject(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Project>(json)!;
        }

    }
}
