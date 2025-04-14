using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.uiframework
{
    public class UIButton : UIComponent
    {
        public string Label;
        public Action? OnClick;

        public UIButton(string id, string label, Action? onClick = null) : base(id)
        {
            Label = label;
            OnClick = onClick;
        }

        public override void Draw()
        {
            if (ImGui.Button(Label))
            {
                OnClick?.Invoke();
            }
        }
    }
}
