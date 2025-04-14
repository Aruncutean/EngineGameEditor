using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.uiframework
{
    public class UILayoutHorizontal : UIComponent
    {
        public List<UIComponent> Children = new();
        public float Spacing = 8f; // spațiu între elemente

        public UILayoutHorizontal(string id) : base(id) { }

        public void Add(UIComponent component)
        {
            Children.Add(component);
        }

        public override void Draw()
        {
            bool first = true;
            foreach (var child in Children)
            {
                if (!first)
                    ImGui.SameLine(); 

                child.Draw();
                first = false;

                if (Spacing > 0)
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + Spacing);
            }
        }
    }
}
