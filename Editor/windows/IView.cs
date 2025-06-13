using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.windows
{
    public interface IView
    {
        public void Init();

        public void Run(float deltaTime);
    }
}
