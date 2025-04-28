using Avalonia.Media;
using EditorAvalonia.service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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

        public static void Create(string projectFolder, string outputZipPath)
        {
            if (File.Exists(outputZipPath))
                File.Delete(outputZipPath);

            using var archive = ZipFile.Open(outputZipPath, ZipArchiveMode.Create);

            // Adaugă project.json
            string projectJsonPath = Path.Combine(projectFolder, "project.json");
            if (File.Exists(projectJsonPath))
                archive.CreateEntryFromFile(projectJsonPath, "project.json");

            // Adaugă scenes/
            string scenesFolder = Path.Combine(projectFolder, "scenes");
            if (Directory.Exists(scenesFolder))
            {
                foreach (var file in Directory.GetFiles(scenesFolder))
                {
                    string entryName = Path.Combine("scenes", Path.GetFileName(file));
                    archive.CreateEntryFromFile(file, entryName);
                }
            }

            // Adaugă assets/
            string assetsFolder = Path.Combine(projectFolder, "assets");
            if (Directory.Exists(assetsFolder))
            {
                foreach (var file in Directory.GetFiles(assetsFolder))
                {
                    string entryName = Path.Combine("assets", Path.GetFileName(file));
                    archive.CreateEntryFromFile(file, entryName);
                }
            }

            Console.WriteLine($"✅ GameData.zip creat la: {outputZipPath}");
        }

      public  void StartEmulator()
        {
            Run("adb", "start-server");
            var list = RunWithResult("adb", "devices");

            if (!list.Contains("emulator"))
            {
                Console.WriteLine("📱 Lansează un emulator în Android Studio sau folosind avdmanager.");
                // sau automatizează cu `emulator -avd <name>`
          
            }
        }

        void Run(string file, string args)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            p.WaitForExit();
        }

        string RunWithResult(string file, string args)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }


    }


}
