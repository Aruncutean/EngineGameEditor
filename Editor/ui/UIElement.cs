using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.UI
{
    public class DockspaceRoot
    {
        public string Type { get; set; } = "dockspace";
        public string Id { get; set; } = "DockMain";
        public List<IncludedView> Windows { get; set; } = new();
    }

    public class IncludedView
    {
        public string Include { get; set; } = "";
    }

    public class UIElement
    {
        public string Type { get; set; } = "";
        public string Id { get; set; } = "";
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public float? Min { get; set; }
        public float? Max { get; set; }
        public string? OnClick { get; set; }
        public string? OnChange { get; set; }
        public List<UIElement>? Children { get; set; }
        public List<int>? Size { get; set; }
    }
}
