using Editor.windows.editorWindow;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.windows
{
    public enum AppWindowType
    {
        Project,
        Scene,
        Editor
    }
    public class WindowManager
    {
        private static Dictionary<AppWindowType, object> _windows = new();

        public static void Open(AppWindowType type)
        {
            if (_windows.ContainsKey(type))
                return;

            Thread thread = new(() =>
            {
                object? window = type switch
                {
                    AppWindowType.Project => new ProjectWindow(),
                    AppWindowType.Editor => new EditorWindow(),
                    AppWindowType.Scene => new SceneWindow(),
                    _ => null
                };

                if (window != null)
                {
                    _windows[type] = window;
                    var runMethod = window.GetType().GetMethod("Run");
                    runMethod?.Invoke(window, null);
                }

                _windows.Remove(type);
            });

            thread.IsBackground = false;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static void Close(AppWindowType type)
        {
            if (_windows.TryGetValue(type, out var window))
            {
                var closeMethod = window.GetType().GetMethod("Close");
                closeMethod?.Invoke(window, null);
                _windows.Remove(type);
            }
        }

        public static void Toggle(AppWindowType type)
        {
            if (_windows.ContainsKey(type))
                Close(type);
            else
                Open(type);
        }
    }
}
