using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.uiframework
{
    public abstract class UIComponent
    {
        public string Id;
        public Vector2 Position;
        public Vector2 Size;

        public UIComponent(string id)
        {
            Id = id;
        }

        public abstract void Draw();
    }
}
