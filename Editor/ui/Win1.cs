using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.ui
{
    public class Win1
    {
        public string Name = "Cube";

        public void HandleNameChange(string newName)
        {
            Name = newName;
            Console.WriteLine($"[Inspector] Name changed: {newName}");
        }

        public void HandleApply()
        {
            Console.WriteLine($"[Inspector] Apply clicked — current name: {Name}");
        }

    }
}
