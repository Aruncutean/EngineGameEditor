using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.uiframework
{
    public class UIList<T> : UIComponent where T : UIComponent
    {
        public List<T> Items;
        public int SelectedIndex = -1;

        public Func<T, string> LabelSelector = item => item?.ToString() ?? "";

        public Action<T>? OnSelect;
        public Action<T>? OnRightClick;
        public Action<T>? OnDoubleClick;

        public UIList(string id, List<T> items) : base(id)
        {
            Items = items;
        }

        private float _lastClickTime = 0;
        private const float doubleClickThreshold = 0.25f;


        public override void Draw()
        {
            ImGui.BeginChild(Id, new Vector2(0, -30));

            for (int i = 0; i < Items.Count; i++)
            {
                bool isSelected = (i == SelectedIndex);
                string label = LabelSelector(Items[i]);

                if (ImGui.Selectable(label, isSelected))
                {
                    SelectedIndex = i;
                    OnSelect?.Invoke(Items[i]);

                    float time = (float)ImGui.GetTime();
                    if (time - _lastClickTime < doubleClickThreshold)
                    {
                        OnDoubleClick?.Invoke(Items[i]);
                    }
                    _lastClickTime = time;
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    OnRightClick?.Invoke(Items[i]);
                }
            }

            ImGui.EndChild();
        }
    }
}