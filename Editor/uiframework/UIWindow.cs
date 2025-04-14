using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.uiframework
{
    public class UIWindow
    {
        public string Title;
        public bool IsOpen = true;
        public List<UIComponent> Components = new();

        public ImGuiWindowFlags Flags = ImGuiWindowFlags.None;

        public UIWindow(string title)
        {
            Title = title;
        }

        public void Add(UIComponent component)
        {
            Components.Add(component);
        }

        public void Draw()
        {
            if (!IsOpen) return;

            if (ImGui.Begin(Title, ref IsOpen, Flags))
            {
                foreach (var component in Components)
                {
                    component.Draw();
                }
            }

            ImGui.End();
        }
    }
}
