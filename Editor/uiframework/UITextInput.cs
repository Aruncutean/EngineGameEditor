using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.uiframework
{
   public class UITextInput : UIComponent
    {
        public string Label;
        public string Value;
        private byte[] buffer;
        public Action<string>? OnChanged;
        private string previousValue = "";

        public UITextInput(string id, string label, string defaultValue = "") : base(id)
        {
            Label = label;
            Value = defaultValue;
            previousValue = defaultValue;
            buffer = new byte[256];
            Encoding.UTF8.GetBytes(defaultValue, 0, defaultValue.Length, buffer, 0);
        }

        public override void Draw()
        {
            if (ImGui.InputText(Label, buffer, (uint)buffer.Length))
            {
                Value = Encoding.UTF8.GetString(buffer).TrimEnd('\0');

                if (Value != previousValue)
                {
                    previousValue = Value;
                    OnChanged?.Invoke(Value);
                }
            }
        }
    }
}
