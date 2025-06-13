using Editor.windows;
using OpenTK.Windowing.Desktop;

namespace Editor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WindowManager.Open(AppWindowType.Project);
        }

    }

}
