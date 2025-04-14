using Avalonia.Media;
using EditorAvalonia.service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorAvalonia.runtime
{
    public class RuntimeLauncher
    {
        public void RunProject(string projectPath)
        {
            var runtimeExe = Path.Combine("Runtime", "C:\\Users\\arunc\\Desktop\\GameEditor\\RunTime\\bin\\Debug\\net8.0\\RunTime.exe");

            if (!File.Exists(runtimeExe))
            {
                Console.WriteLine("❌ Runtime exe not found.");
                return;
            }


            var args = $"\"{projectPath}\"";
                args += " --edit";
                args += $" --scene \"{StoreService.GetInstance().CurentScene.Path}\"";


            var startInfo = new ProcessStartInfo
            {
                FileName = runtimeExe,
                Arguments = args,
                UseShellExecute = false,
            };

            try
            {
                Process.Start(startInfo);
                Console.WriteLine($"✅ Launched runtime with project: {projectPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to start: {ex.Message}");
            }
        }
    }
}
