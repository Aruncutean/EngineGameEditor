using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.ui
{
    internal class Scena
    {
        public Scena() { }

        public void init()
        {


        }

        public void draw()
        {
           
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
            ImGui.Begin("Scena");

            ImGui.End();
        }
    }
}
